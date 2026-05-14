using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class DevelopmentSeedServiceTests
{
    private const string UsuarioTecnicoTeste = "teste.seed";

    private static readonly Dictionary<string, TipoPerfil> UsuariosOficiaisPorEmail = new(StringComparer.OrdinalIgnoreCase)
    {
        ["admin@sgxdigital.com"] = TipoPerfil.Administrador,
        ["admin2@sgxdigital.com"] = TipoPerfil.Administrador,
        ["atendente.demo@sgxdigital.com"] = TipoPerfil.Atendente,
        ["atendente2.demo@sgxdigital.com"] = TipoPerfil.Atendente,
        ["solicitante.demo@sgxdigital.com"] = TipoPerfil.Solicitante,
        ["solicitante2.demo@sgxdigital.com"] = TipoPerfil.Solicitante
    };

    [Fact]
    public async Task SeedCriaApenasDoisUsuariosAtivosPorPerfilEmBaseNova()
    {
        await using var dbContext = CriarContexto();
        var service = CriarService(dbContext);

        await service.SeedAsync();

        var usuariosAtivos = await dbContext.Usuarios
            .AsNoTracking()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .Where(x => x.Ativo && x.Situacao == SituacaoUsuario.Ativo)
            .ToListAsync();

        Assert.Equal(6, usuariosAtivos.Count);
        Assert.All(usuariosAtivos, usuario => Assert.Contains(usuario.Email, UsuariosOficiaisPorEmail.Keys, StringComparer.OrdinalIgnoreCase));
        Assert.Equal(2, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador)));
        Assert.Equal(2, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Atendente)));
        Assert.Equal(2, usuariosAtivos.Count(x => x.UsuarioPerfis.Any(p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Solicitante)));

        foreach (var usuario in usuariosAtivos)
        {
            Assert.True(UsuariosOficiaisPorEmail.TryGetValue(usuario.Email, out var perfilEsperado));
            Assert.Single(usuario.UsuarioPerfis);
            Assert.Equal(perfilEsperado, usuario.UsuarioPerfis.Single().PerfilAcesso.TipoPerfil);
        }
    }

    [Fact]
    public async Task SeedNaoDuplicaUsuariosNemRecriaDemoInativado()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);
        await CriarUsuarioAsync(
            dbContext,
            "Solicitante Local",
            "solicitante.local@sgx.local",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();
        await service.SeedAsync();

        var oficiais = await dbContext.Usuarios
            .AsNoTracking()
            .Where(x => UsuariosOficiaisPorEmail.Keys.Contains(x.Email))
            .ToListAsync();
        Assert.Equal(6, oficiais.Count);
        Assert.Equal(6, oficiais.Select(x => x.Email).Distinct(StringComparer.OrdinalIgnoreCase).Count());

        var demoAntigo = await dbContext.Usuarios
            .AsNoTracking()
            .SingleAsync(x => x.Email == "solicitante.local@sgx.local");
        Assert.False(demoAntigo.Ativo);
        Assert.Equal(SituacaoUsuario.Inativo, demoAntigo.Situacao);
    }

    [Fact]
    public async Task SeedInativaUsuariosDemonstrativosAntigos()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);

        var emailsAntigos = new[]
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
            "solicitante.sla.local@sgx.local",
            "usuario.homol.demo@sgx.local"
        };

        foreach (var email in emailsAntigos)
        {
            var tipoPerfil = email.Contains("atendente", StringComparison.OrdinalIgnoreCase)
                ? TipoPerfil.Atendente
                : email.Contains("solicitante", StringComparison.OrdinalIgnoreCase)
                    ? TipoPerfil.Solicitante
                    : TipoPerfil.Administrador;

            await CriarUsuarioAsync(dbContext, $"Usuário demo {email}", email, tipoPerfil, perfis);
        }

        await CriarUsuarioAsync(
            dbContext,
            "Usuário Homol Especial",
            "qualquer.homologacao@empresa.com",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var antigos = await dbContext.Usuarios
            .AsNoTracking()
            .Where(x => emailsAntigos.Contains(x.Email) || x.Nome.Contains("Homol"))
            .ToListAsync();

        Assert.NotEmpty(antigos);
        Assert.All(antigos, usuario =>
        {
            Assert.False(usuario.Ativo);
            Assert.Equal(SituacaoUsuario.Inativo, usuario.Situacao);
        });
    }

    [Fact]
    public async Task SeedInativaUsuarioDemonstrativoGenericoDoDominioLegado()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);

        _ = await CriarUsuarioAsync(
            dbContext,
            "Atendente Legado Generico",
            "at1@sgx.local",
            TipoPerfil.Atendente,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var usuario = await dbContext.Usuarios
            .AsNoTracking()
            .SingleAsync(x => x.Email == "at1@sgx.local");

        Assert.False(usuario.Ativo);
        Assert.Equal(SituacaoUsuario.Inativo, usuario.Situacao);
    }

    [Fact]
    public async Task SeedNaoInativaUsuarioRealForaDosPadroesDemonstrativos()
    {
        await using var dbContext = CriarContexto();
        var perfis = await CarregarPerfisAsync(dbContext);
        var usuarioReal = await CriarUsuarioAsync(
            dbContext,
            "Maria da Silva",
            "maria.silva@empresa.com",
            TipoPerfil.Solicitante,
            perfis);

        var service = CriarService(dbContext);
        await service.SeedAsync();

        var usuarioAtualizado = await dbContext.Usuarios.AsNoTracking().SingleAsync(x => x.Id == usuarioReal.Id);
        Assert.True(usuarioAtualizado.Ativo);
        Assert.Equal(SituacaoUsuario.Ativo, usuarioAtualizado.Situacao);
    }

    [Fact]
    public async Task SeedNaoInativaAdministradorInicialReal()
    {
        const string emailAdminInicial = "admin.real@sgxdigital.com";
        var emailOriginal = Environment.GetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL");

        try
        {
            Environment.SetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL", emailAdminInicial);

            await using var dbContext = CriarContexto();
            var perfis = await CarregarPerfisAsync(dbContext);
            var adminInicial = await CriarUsuarioAsync(
                dbContext,
                "Administrador Inicial",
                emailAdminInicial,
                TipoPerfil.Administrador,
                perfis);

            var service = CriarService(dbContext);
            await service.SeedAsync();

            var usuarioAtualizado = await dbContext.Usuarios.AsNoTracking().SingleAsync(x => x.Id == adminInicial.Id);
            Assert.True(usuarioAtualizado.Ativo);
            Assert.Equal(SituacaoUsuario.Ativo, usuarioAtualizado.Situacao);
        }
        finally
        {
            Environment.SetEnvironmentVariable("SGX_ADMIN_INICIAL_EMAIL", emailOriginal);
        }
    }

    private static async Task<Dictionary<TipoPerfil, PerfilAcesso>> CarregarPerfisAsync(SGXSistemaChamadoDbContext dbContext)
    {
        return await dbContext.PerfisAcesso
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => x.TipoPerfil == TipoPerfil.Administrador
                || x.TipoPerfil == TipoPerfil.Atendente
                || x.TipoPerfil == TipoPerfil.Solicitante)
            .ToDictionaryAsync(x => x.TipoPerfil, x => x);
    }

    private static async Task<Usuario> CriarUsuarioAsync(
        SGXSistemaChamadoDbContext dbContext,
        string nome,
        string email,
        TipoPerfil tipoPerfil,
        IReadOnlyDictionary<TipoPerfil, PerfilAcesso> perfis)
    {
        var usuario = new Usuario(nome, email, email, UsuarioTecnicoTeste);
        await dbContext.Usuarios.AddAsync(usuario);
        await dbContext.SaveChangesAsync();

        var vinculoPerfil = new UsuarioPerfilAcesso(usuario.Id, perfis[tipoPerfil].Id, UsuarioTecnicoTeste);
        await dbContext.UsuariosPerfisAcesso.AddAsync(vinculoPerfil);
        await dbContext.SaveChangesAsync();

        return usuario;
    }

    private static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase($"seed-tests-{Guid.NewGuid():N}")
            .Options;

        var context = new SGXSistemaChamadoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static DevelopmentSeedService CriarService(SGXSistemaChamadoDbContext dbContext)
    {
        var authOptions = Options.Create(new AuthOptions
        {
            LoginLocalHabilitado = false
        });

        return new DevelopmentSeedService(
            dbContext,
            new FakeEnvironment(),
            authOptions,
            new PasswordHasher<Usuario>(),
            NullLogger<DevelopmentSeedService>.Instance);
    }

    private sealed class FakeEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
