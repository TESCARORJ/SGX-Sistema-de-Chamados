using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class PreferenciaNotificacaoUsuarioPersistenceTests : IClassFixture<PreferenciaNotificacaoUsuarioPersistenceDatabaseFixture>
{
    private readonly PreferenciaNotificacaoUsuarioPersistenceDatabaseFixture _fixture;

    public PreferenciaNotificacaoUsuarioPersistenceTests(PreferenciaNotificacaoUsuarioPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DevePersistirPreferenciaERecuperarEventoCanalEHabilitacao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        context.Add(new PreferenciaNotificacaoUsuario(
            usuarioId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            false,
            usuarioId,
            "teste"));
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Set<PreferenciaNotificacaoUsuario>().SingleAsync();

        Assert.Equal(usuarioId, persistida.UsuarioId);
        Assert.Equal(TipoEventoNotificacao.EventoChamado, persistida.TipoEvento);
        Assert.Equal(CanalNotificacao.Email, persistida.Canal);
        Assert.False(persistida.Habilitada);
    }

    [Fact]
    public async Task DeveRejeitarDuplicidadeDaChaveComposta()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        context.Add(new PreferenciaNotificacaoUsuario(usuarioId, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email, true, usuarioId, "teste"));
        context.Add(new PreferenciaNotificacaoUsuario(usuarioId, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email, false, usuarioId, "teste"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, postgres.SqlState);
        Assert.Equal("ux_preferencias_notificacao_usuario_chave", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveValidarFkDeUsuario()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        context.Add(new PreferenciaNotificacaoUsuario(Guid.NewGuid(), TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema, true, Guid.NewGuid(), "teste"));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, postgres.SqlState);
    }

    [Fact]
    public async Task DeveImpedirExclusaoRestritaDeUsuario()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        context.Add(new PreferenciaNotificacaoUsuario(usuarioId, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Sistema, true, usuarioId, "teste"));
        await context.SaveChangesAsync();

        await using var exclusao = _fixture.CreateContext();
        var usuario = await exclusao.Usuarios.SingleAsync(x => x.Id == usuarioId);
        exclusao.Usuarios.Remove(usuario);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => exclusao.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Contains(postgres.SqlState, new[] { PostgresErrorCodes.ForeignKeyViolation, PostgresErrorCodes.RestrictViolation });
    }

    [Fact]
    public async Task DeveAtualizarPreferenciaExistenteSemCriarSegundaLinha()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var preferencia = new PreferenciaNotificacaoUsuario(usuarioId, TipoEventoNotificacao.EventoChamado, CanalNotificacao.Email, false, usuarioId, "teste");

        context.Add(preferencia);
        await context.SaveChangesAsync();

        preferencia.Habilitar(usuarioId, "teste");
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        Assert.Single(consulta.Set<PreferenciaNotificacaoUsuario>());
        Assert.True((await consulta.Set<PreferenciaNotificacaoUsuario>().SingleAsync()).Habilitada);
    }

    [Fact]
    public async Task DeveConfirmarAusenciaDeSeedFuncionalDePreferenciasENotificacoes()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        Assert.Equal(0, await context.Set<PreferenciaNotificacaoUsuario>().CountAsync());
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveExecutarFluxoIntegradoDeDefinicaoEAvaliacaoSemCriarNotificacao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var usuarioDestinoId = await _fixture.CriarUsuarioTemporarioAsync(context);

        var definir = new DefinirPreferenciaNotificacaoUsuarioUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<PreferenciaNotificacaoUsuario>(context),
            PortalUseCasesTestFactory.Uow(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioAtualId,
                "Admin",
                "admin@sgx.local",
                "admin",
                ["Administrador"])));

        var avaliar = new AvaliarPreferenciaNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            PortalUseCasesTestFactory.Repo<PreferenciaNotificacaoUsuario>(context));

        await definir.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
            usuarioDestinoId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            false));

        var bloqueada = await avaliar.ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(
            usuarioDestinoId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email));

        await definir.ExecutarAsync(new DefinirPreferenciaNotificacaoUsuarioRequest(
            usuarioDestinoId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            true));

        var permitida = await avaliar.ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(
            usuarioDestinoId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email));

        var fallback = await avaliar.ExecutarAsync(new AvaliarPreferenciaNotificacaoRequest(
            usuarioDestinoId,
            TipoEventoNotificacao.EventoSla,
            CanalNotificacao.Sistema));

        Assert.False(bloqueada.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.PreferenciaDesabilitada, bloqueada.Motivo);
        Assert.True(permitida.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.PreferenciaHabilitada, permitida.Motivo);
        Assert.True(fallback.Permitida);
        Assert.Equal(MotivoDecisaoPreferenciaNotificacao.FallbackPermitido, fallback.Motivo);
        Assert.Single(context.Set<PreferenciaNotificacaoUsuario>());
        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveValidarMigrationEstruturalSemAlterarNotificacoesOuTemplates()
    {
        var migrationPath = Path.Combine(
            NotificacaoPersistenceDatabaseFixture.ObterRaizDaSolucao(),
            "src",
            "SGX.SistemaChamado.Infrastructure",
            "Persistence",
            "Migrations",
            "20260621175818_CriarEstruturaPreferenciaNotificacaoUsuarioSprint6.cs");

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"preferencias_notificacao_usuario\"", migration, StringComparison.Ordinal);
        Assert.Contains("ux_preferencias_notificacao_usuario_chave", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("notificacoes", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("templates_notificacao", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("outbox", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fila", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("InsertData(", migration, StringComparison.Ordinal);
        Assert.Contains("DropTable(", migration, StringComparison.Ordinal);
    }
}

public sealed class PreferenciaNotificacaoUsuarioPersistenceDatabaseFixture : IAsyncLifetime
{
    private const string DatabaseName = "sgx_notificacoes_tests";
    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
    {
        var configuracao = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(
            NotificacaoPersistenceDatabaseFixture.ObterRaizDaSolucao(),
            "src",
            "SGX.SistemaChamado.Api",
            "appsettings.Development.json")));

        var baseConnectionString = configuracao.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString()!;

        var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
        {
            Database = DatabaseName
        };

        _connectionString = builder.ConnectionString;

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    public SGXSistemaChamadoDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseNpgsql(_connectionString)
            .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        return new SGXSistemaChamadoDbContext(options);
    }

    public async Task ResetAsync()
    {
        await using var context = CreateContext();
        await context.Set<PreferenciaNotificacaoUsuario>().ExecuteDeleteAsync();
        await context.Notificacoes.ExecuteDeleteAsync();
        await context.Set<TemplateNotificacao>().ExecuteDeleteAsync();
        await context.Usuarios.Where(x => x.CriadoPor == "test.preferencia.persistence").ExecuteDeleteAsync();
    }

    public async Task<Guid> CriarUsuarioTemporarioAsync(SGXSistemaChamadoDbContext context)
    {
        var usuario = new Usuario(
            $"Usuario Teste {Guid.NewGuid():N}"[..30],
            $"usuario.{Guid.NewGuid():N}@teste.local",
            $"pref.{Guid.NewGuid():N}"[..20],
            "test.preferencia.persistence");

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario.Id;
    }
}
