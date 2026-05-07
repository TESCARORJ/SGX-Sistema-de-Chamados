using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public sealed class DevelopmentSeedService(
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions,
    ILogger<DevelopmentSeedService> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var departamento = await dbContext.Departamentos
            .FirstOrDefaultAsync(x => x.Ativo && x.Sigla == "TI", cancellationToken);

        if (departamento is null)
        {
            departamento = new Departamento(
                "Tecnologia da Informacao",
                "TI",
                "Departamento de suporte tecnico interno.",
                "seed.development");

            await dbContext.Departamentos.AddAsync(departamento, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var categoria = await dbContext.CategoriasChamado
            .FirstOrDefaultAsync(x => x.Ativo && x.Nome == "Suporte Tecnico", cancellationToken);

        if (categoria is null)
        {
            categoria = new CategoriaChamado(
                "Suporte Tecnico",
                "Categoria inicial para atendimento tecnico interno.",
                departamento.Id,
                "seed.development");

            await dbContext.CategoriasChamado.AddAsync(categoria, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (!authOptions.Value.ModoLocalHabilitado)
        {
            return;
        }

        var email = authOptions.Value.AdminLocalEmail.Trim().ToLowerInvariant();
        var nome = string.IsNullOrWhiteSpace(authOptions.Value.AdminLocalNome)
            ? "Administrador Local"
            : authOptions.Value.AdminLocalNome.Trim();
        var login = email;
        var loginLegado = email.Contains('@', StringComparison.Ordinal)
            ? email[..email.IndexOf('@')]
            : email;

        var usuario = await dbContext.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(
                x => x.Email == email || x.Login == login || x.Login == loginLegado,
                cancellationToken);

        if (usuario is null)
        {
            usuario = new Usuario(nome, email, login, "seed.development", departamento.Id);
            await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var atualizouDadosBasicos = false;

            if (!string.Equals(usuario.Nome, nome, StringComparison.Ordinal))
            {
                usuario.DefinirNome(nome);
                atualizouDadosBasicos = true;
            }

            if (!string.Equals(usuario.Email, email, StringComparison.OrdinalIgnoreCase))
            {
                usuario.DefinirEmail(email);
                atualizouDadosBasicos = true;
            }

            if (!string.Equals(usuario.Login, login, StringComparison.OrdinalIgnoreCase))
            {
                usuario.DefinirLogin(login);
                atualizouDadosBasicos = true;
            }

            if (atualizouDadosBasicos)
            {
                usuario.AtualizarAuditoria("seed.development");
            }

            if (!usuario.Ativo)
            {
                usuario.Ativar("seed.development");
            }

            if (usuario.Situacao != SituacaoUsuario.Ativo)
            {
                usuario.AlterarSituacao(SituacaoUsuario.Ativo, "seed.development");
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var perfilAdministrador = await dbContext.PerfisAcesso
            .FirstOrDefaultAsync(x => x.Ativo && x.TipoPerfil == TipoPerfil.Administrador, cancellationToken);

        if (perfilAdministrador is null)
        {
            logger.LogWarning("Perfil Administrador nao encontrado. Usuario tecnico local foi criado sem perfil.");
            return;
        }

        var possuiPerfilAdministrador = await dbContext.UsuariosPerfisAcesso
            .AnyAsync(
                x => x.UsuarioId == usuario.Id && x.PerfilAcessoId == perfilAdministrador.Id,
                cancellationToken);

        if (!possuiPerfilAdministrador)
        {
            await dbContext.UsuariosPerfisAcesso.AddAsync(
                new UsuarioPerfilAcesso(usuario.Id, perfilAdministrador.Id, "seed.development"),
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        logger.LogWarning(
            "Modo local de autenticacao habilitado em Development com usuario tecnico {Email}.",
            email);
    }
}
