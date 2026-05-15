using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public sealed class DevelopmentSeedService(
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions,
    IPasswordHasher<Usuario> passwordHasher,
    ILogger<DevelopmentSeedService> logger)
{
    private const string UsuarioTecnico = "seed.development";
    private const string DominioDemoOficial = "@sgxdigital.com";
    private const string DominioDemoLegado = "@sgx.local";

    private static readonly UsuarioDemoDef[] UsuariosDemo =
    [
        new("Administrador Demo 1", "admin@sgxdigital.com", TipoPerfil.Administrador),
        new("Administrador Demo 2", "admin2@sgxdigital.com", TipoPerfil.Administrador),
        new("Atendente Demo 1", "atendente.demo@sgxdigital.com", TipoPerfil.Atendente),
        new("Atendente Demo 2", "atendente2.demo@sgxdigital.com", TipoPerfil.Atendente),
        new("Solicitante Demo 1", "solicitante.demo@sgxdigital.com", TipoPerfil.Solicitante),
        new("Solicitante Demo 2", "solicitante2.demo@sgxdigital.com", TipoPerfil.Solicitante)
    ];

    private static readonly HashSet<string> EmailsDemoLegados = new(StringComparer.OrdinalIgnoreCase)
    {
        "administrador.admin@sgx.local",
        "admin.local@sgx.local",
        "atendente.admin@sgx.local",
        "atendente.local@sgx.local",
        "atendente.sla.local@sgx.local",
        "solicitante.a.local@sgx.local",
        "solicitante.a@sgx.local",
        "solicitante.admin@sgx.local",
        "solicitante.b@sgx.local",
        "solicitante.b.local@sgx.local",
        "solicitante.local@sgx.local",
        "solicitante.portal@sgx.local",
        "solicitante.sla.local@sgx.local"
    };

    private static readonly string[] PrefixosDemoLegados =
    [
        "administrador.",
        "admin.",
        "atendente.",
        "solicitante.",
        "usuario.homol."
    ];

    private static readonly string[] MarcadoresParteLocalDemoLegados =
    [
        ".local",
        ".homol",
        ".demo",
        ".sla",
        ".portal",
        ".teste",
        ".seed"
    ];

    private static readonly string[] MarcadoresDemoLegados =
    [
        ".local@",
        ".homol.",
        ".demo@",
        ".sla.",
        ".portal@"
    ];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var departamento = await GarantirDepartamentoAsync(cancellationToken);
        await GarantirCategoriaAsync(departamento.Id, cancellationToken);

        var perfis = await dbContext.PerfisAcesso
            .Where(x => x.Ativo)
            .ToDictionaryAsync(x => x.TipoPerfil, x => x, cancellationToken);

        if (!perfis.ContainsKey(TipoPerfil.Administrador)
            || !perfis.ContainsKey(TipoPerfil.Atendente)
            || !perfis.ContainsKey(TipoPerfil.Solicitante))
        {
            logger.LogWarning("Perfis base nao encontrados para seed Development.");
            return;
        }

        var emailsPermitidos = new HashSet<string>(
            UsuariosDemo.Select(x => x.Email),
            StringComparer.OrdinalIgnoreCase);

        var usuariosRelacionados = await dbContext.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .ToListAsync(cancellationToken);

        var emailAdminInicialProtegido = ObterEmailAdminInicialProtegido();

        foreach (var usuarioExistente in usuariosRelacionados)
        {
            var email = NormalizarEmail(usuarioExistente.Email);
            if (emailsPermitidos.Contains(email))
            {
                continue;
            }

            if (EhAdministradorInicialProtegido(usuarioExistente, email, emailAdminInicialProtegido))
            {
                continue;
            }

            if (!EhUsuarioDemonstrativoAntigo(usuarioExistente, email))
            {
                continue;
            }

            if (usuarioExistente.Ativo)
            {
                usuarioExistente.Desativar(UsuarioTecnico);
            }

            if (usuarioExistente.Situacao != SituacaoUsuario.Inativo)
            {
                usuarioExistente.AlterarSituacao(SituacaoUsuario.Inativo, UsuarioTecnico);
            }
        }

        foreach (var usuarioDemo in UsuariosDemo)
        {
            var usuario = usuariosRelacionados.FirstOrDefault(x =>
                string.Equals(x.Email, usuarioDemo.Email, StringComparison.OrdinalIgnoreCase)
                || string.Equals(x.Login, usuarioDemo.Email, StringComparison.OrdinalIgnoreCase));

            if (usuario is null)
            {
                usuario = new Usuario(
                    usuarioDemo.Nome,
                    usuarioDemo.Email,
                    usuarioDemo.Email,
                    UsuarioTecnico,
                    departamento.Id);

                await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
                usuariosRelacionados.Add(usuario);
            }
            else
            {
                usuario.DefinirNome(usuarioDemo.Nome);
                usuario.DefinirEmail(usuarioDemo.Email);
                usuario.DefinirLogin(usuarioDemo.Email);
                usuario.DefinirDepartamento(departamento.Id, UsuarioTecnico);
                if (!usuario.Ativo)
                {
                    usuario.Ativar(UsuarioTecnico);
                }

                if (usuario.Situacao != SituacaoUsuario.Ativo)
                {
                    usuario.AlterarSituacao(SituacaoUsuario.Ativo, UsuarioTecnico);
                }
            }

            var perfilEsperado = perfis[usuarioDemo.Perfil];
            var jaPossuiPerfil = usuario.UsuarioPerfis.Any(x => x.PerfilAcessoId == perfilEsperado.Id);
            if (!jaPossuiPerfil)
            {
                var vinculo = new UsuarioPerfilAcesso(usuario.Id, perfilEsperado.Id, UsuarioTecnico);
                usuario.UsuarioPerfis.Add(vinculo);
                await dbContext.UsuariosPerfisAcesso.AddAsync(vinculo, cancellationToken);
            }

            var vinculosParaRemover = usuario.UsuarioPerfis
                .Where(x => x.PerfilAcessoId != perfilEsperado.Id)
                .ToList();
            foreach (var vinculo in vinculosParaRemover)
            {
                usuario.UsuarioPerfis.Remove(vinculo);
                dbContext.UsuariosPerfisAcesso.Remove(vinculo);
            }

            AplicarSenhaLocalSeConfigurada(usuario);
            usuario.DefinirDeveAlterarSenha(false, UsuarioTecnico);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seed Development aplicado com 2 usuarios por perfil (Administrador, Atendente, Solicitante).");
    }

    private static bool EhAdministradorInicialProtegido(Usuario usuario, string email, string? emailAdminInicialProtegido)
    {
        if (string.IsNullOrWhiteSpace(emailAdminInicialProtegido))
        {
            return false;
        }

        return string.Equals(email, emailAdminInicialProtegido, StringComparison.OrdinalIgnoreCase);
    }

    private static bool EhUsuarioDemonstrativoAntigo(Usuario usuario, string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        if (EmailsDemoLegados.Contains(email))
        {
            return true;
        }

        if ((usuario.Nome ?? string.Empty).Contains("homol", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (email.EndsWith(DominioDemoLegado, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!email.EndsWith(DominioDemoOficial, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var parteLocal = ObterParteLocalEmail(email);
        if (string.IsNullOrWhiteSpace(parteLocal))
        {
            return false;
        }

        if (parteLocal.StartsWith("usuario.homol.", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (PrefixosDemoLegados.Any(prefixo => parteLocal.StartsWith(prefixo, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (MarcadoresParteLocalDemoLegados.Any(marcador => parteLocal.Contains(marcador, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return MarcadoresDemoLegados.Any(marcador => email.Contains(marcador, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizarEmail(string? email)
        => (email ?? string.Empty).Trim().ToLowerInvariant();

    private static string ObterParteLocalEmail(string email)
    {
        var arroba = email.IndexOf('@');
        if (arroba <= 0)
        {
            return string.Empty;
        }

        return email[..arroba];
    }

    private static string? ObterEmailAdminInicialProtegido()
    {
        var email = Environment.GetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL");
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return NormalizarEmail(email);
    }

    private async Task<Departamento> GarantirDepartamentoAsync(CancellationToken cancellationToken)
    {
        var departamento = await dbContext.Departamentos
            .FirstOrDefaultAsync(x => x.Ativo && x.Sigla == "TI", cancellationToken);

        if (departamento is not null)
        {
            return departamento;
        }

        departamento = new Departamento(
            "Tecnologia da Informacao",
            "TI",
            "Departamento de suporte tecnico interno.",
            UsuarioTecnico);

        await dbContext.Departamentos.AddAsync(departamento, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return departamento;
    }

    private async Task GarantirCategoriaAsync(Guid departamentoId, CancellationToken cancellationToken)
    {
        var categoria = await dbContext.CategoriasChamado
            .FirstOrDefaultAsync(x => x.Ativo && x.Nome == "Suporte Tecnico", cancellationToken);

        if (categoria is not null)
        {
            return;
        }

        categoria = new CategoriaChamado(
            "Suporte Tecnico",
            "Categoria inicial para atendimento tecnico interno.",
            departamentoId,
            UsuarioTecnico);

        await dbContext.CategoriasChamado.AddAsync(categoria, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void AplicarSenhaLocalSeConfigurada(Usuario usuario)
    {
        if (!authOptions.Value.LoginLocalHabilitado)
        {
            return;
        }

        var senhaAdminLocal = authOptions.Value.AdminLocalSenha?.Trim();
        if (string.IsNullOrWhiteSpace(senhaAdminLocal))
        {
            return;
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, senhaAdminLocal), UsuarioTecnico);
    }

    private sealed record UsuarioDemoDef(string Nome, string Email, TipoPerfil Perfil);
}
