using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class NotificacaoPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public NotificacaoPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DevePersistirNotificacaoComSomenteUsuario()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var chave = _fixture.NovaChaveIdempotencia();
        var dataProcessamento = new DateTime(2026, 6, 21, 12, 10, 0, DateTimeKind.Utc);

        var notificacao = CriarNotificacao(destinatarioUsuarioId: usuarioId, chaveIdempotencia: chave);
        notificacao.IniciarProcessamento(dataProcessamento, CriadoPorTeste, usuarioId);

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);

        Assert.Equal(usuarioId, persistida.DestinatarioUsuarioId);
        Assert.Null(persistida.DestinatarioEndereco);
        Assert.Equal(StatusNotificacao.EmProcessamento, persistida.Status);
        Assert.Equal(TipoEventoNotificacao.EventoChamado, persistida.TipoEvento);
        Assert.Equal(CanalNotificacao.Email, persistida.Canal);
        Assert.Equal(dataProcessamento, persistida.ProcessadaEm);
        Assert.Equal(1, persistida.QuantidadeTentativas);
        Assert.Equal(chave, persistida.ChaveIdempotencia);
    }

    [Fact]
    public async Task DevePersistirNotificacaoComSomenteEndereco()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var notificacao = CriarNotificacao(
            destinatarioEndereco: "externo.persistencia@cliente.com",
            chaveIdempotencia: _fixture.NovaChaveIdempotencia());

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);

        Assert.Null(persistida.DestinatarioUsuarioId);
        Assert.Equal("externo.persistencia@cliente.com", persistida.DestinatarioEndereco);
        Assert.Equal(StatusNotificacao.Pendente, persistida.Status);
    }

    [Fact]
    public async Task DevePersistirNotificacaoComUsuarioEnderecoEChamadoExistente()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var chamado = await _fixture.CriarChamadoTemporarioAsync(context);
        var dataAgendamento = new DateTime(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc);

        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: usuarioId,
            destinatarioEndereco: "duplo.destinatario@cliente.com",
            chamadoId: chamado.Id,
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            chaveCorrelacao: "corr-persistencia");
        notificacao.Agendar(dataAgendamento, CriadoPorTeste, usuarioId);

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);

        Assert.Equal(chamado.Id, persistida.ChamadoId);
        Assert.Equal(usuarioId, persistida.DestinatarioUsuarioId);
        Assert.Equal("duplo.destinatario@cliente.com", persistida.DestinatarioEndereco);
        Assert.Equal(StatusNotificacao.Agendada, persistida.Status);
        Assert.Equal(dataAgendamento, persistida.AgendadaEm);
        Assert.Equal("corr-persistencia", persistida.ChaveCorrelacao);
        Assert.Equal("Conteudo materializado da notificacao", persistida.Conteudo);
    }

    [Fact]
    public async Task DeveRejeitarNotificacaoSemUsuarioESemEndereco()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        var ex = await Assert.ThrowsAsync<PostgresException>(() =>
            InserirNotificacaoViaSqlAsync(
                context,
                id: Guid.NewGuid(),
                chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
                destinatarioUsuarioId: null,
                destinatarioEndereco: null,
                quantidadeTentativas: 0));

        Assert.Equal(PostgresErrorCodes.CheckViolation, ex.SqlState);
        Assert.Equal("ck_notificacoes_destinatario", ex.ConstraintName);
    }

    [Fact]
    public async Task DeveRejeitarQuantidadeTentativasNegativa()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var id = Guid.NewGuid();

        await InserirNotificacaoViaSqlAsync(
            context,
            id: id,
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            destinatarioUsuarioId: usuarioId,
            destinatarioEndereco: null,
            quantidadeTentativas: 0);

        var ex = await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlInterpolatedAsync($"UPDATE notificacoes SET quantidade_tentativas = {-1} WHERE id = {id}"));

        Assert.Equal(PostgresErrorCodes.CheckViolation, ex.SqlState);
        Assert.Equal("ck_notificacoes_quantidade_tentativas_nao_negativa", ex.ConstraintName);
    }

    [Fact]
    public async Task DeveRejeitarChaveIdempotenciaDuplicada()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var chave = _fixture.NovaChaveIdempotencia();

        context.Notificacoes.Add(CriarNotificacao(destinatarioUsuarioId: usuarioId, chaveIdempotencia: chave));
        context.Notificacoes.Add(CriarNotificacao(destinatarioUsuarioId: usuarioId, chaveIdempotencia: chave));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, postgres.SqlState);
        Assert.Equal("ux_notificacoes_chave_idempotencia", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveRejeitarFkDeChamadoInexistente()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        context.Notificacoes.Add(CriarNotificacao(
            destinatarioUsuarioId: usuarioId,
            chamadoId: Guid.NewGuid(),
            chaveIdempotencia: _fixture.NovaChaveIdempotencia()));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Contains(postgres.SqlState, new[]
        {
            PostgresErrorCodes.ForeignKeyViolation,
            PostgresErrorCodes.RestrictViolation
        });
        Assert.Equal("FK_notificacoes_chamados_chamado_id", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveRejeitarFkDeUsuarioInexistente()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        context.Notificacoes.Add(CriarNotificacao(
            destinatarioUsuarioId: Guid.NewGuid(),
            chaveIdempotencia: _fixture.NovaChaveIdempotencia()));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Equal(PostgresErrorCodes.ForeignKeyViolation, postgres.SqlState);
        Assert.Equal("FK_notificacoes_usuarios_destinatario_usuario_id", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveRespeitarTamanhoMaximoNoBanco()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var enderecoLongo = new string('a', 64) + "@" + new string('b', 252) + ".com";

        var ex = await Assert.ThrowsAsync<PostgresException>(() =>
            InserirNotificacaoViaSqlAsync(
                context,
                id: Guid.NewGuid(),
                chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
                destinatarioUsuarioId: usuarioId,
                destinatarioEndereco: enderecoLongo,
                quantidadeTentativas: 0));

        Assert.Equal(PostgresErrorCodes.StringDataRightTruncation, ex.SqlState);
    }

    [Fact]
    public async Task DeveImpedirExclusaoDeChamadoComNotificacaoVinculada()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var chamado = await _fixture.CriarChamadoTemporarioAsync(context);
        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: usuarioId,
            chamadoId: chamado.Id,
            chaveIdempotencia: _fixture.NovaChaveIdempotencia());

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var exclusaoContexto = _fixture.CreateContext();
        var chamadoPersistido = await exclusaoContexto.Chamados.SingleAsync(x => x.Id == chamado.Id);
        exclusaoContexto.Chamados.Remove(chamadoPersistido);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => exclusaoContexto.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Contains(postgres.SqlState, new[]
        {
            PostgresErrorCodes.ForeignKeyViolation,
            PostgresErrorCodes.RestrictViolation
        });
        Assert.Equal("FK_notificacoes_chamados_chamado_id", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveImpedirExclusaoDeUsuarioDestinatarioOuAuditoriaComNotificacaoVinculada()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuario = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacao(
            destinatarioUsuarioId: usuario,
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            criadoPorUsuarioId: usuario);

        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var exclusaoContexto = _fixture.CreateContext();
        var entidadeUsuario = await exclusaoContexto.Usuarios.SingleAsync(x => x.Id == usuario);
        exclusaoContexto.Usuarios.Remove(entidadeUsuario);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => exclusaoContexto.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Contains(postgres.SqlState, new[]
        {
            PostgresErrorCodes.ForeignKeyViolation,
            PostgresErrorCodes.RestrictViolation
        });
        Assert.Contains("FK_notificacoes_usuarios_", postgres.ConstraintName, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeveValidarMigrationEstruturalSemEstruturasFuturasIndevidas()
    {
        var migrationPath = Path.Combine(
            NotificacaoPersistenceDatabaseFixture.ObterRaizDaSolucao(),
            "src",
            "SGX.SistemaChamado.Infrastructure",
            "Persistence",
            "Migrations",
            "20260620025821_CriarEstruturaNotificacaoSprint6.cs");

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"notificacoes\"", migration, StringComparison.Ordinal);
        Assert.Contains("table.PrimaryKey(\"PK_notificacoes\"", migration, StringComparison.Ordinal);
        Assert.Contains("FK_notificacoes_chamados_chamado_id", migration, StringComparison.Ordinal);
        Assert.Contains("ux_notificacoes_chave_idempotencia", migration, StringComparison.Ordinal);
        Assert.Contains("ck_notificacoes_destinatario", migration, StringComparison.Ordinal);
        Assert.Contains("ck_notificacoes_quantidade_tentativas_nao_negativa", migration, StringComparison.Ordinal);
        Assert.Contains("DropTable(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("templates_notificacao", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("preferencias_notificacao", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fila", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("InsertData(", migration, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeveConfirmarProviderPostgreSqlETabelaSemSeedDeNotificacao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        Assert.Equal("Npgsql.EntityFrameworkCore.PostgreSQL", context.Database.ProviderName);
        Assert.Equal(0, await context.Notificacoes.CountAsync());

        var tabelasIndevidas = await LerListaAsync(context, @"
select table_name
from information_schema.tables
where table_schema = 'public'
  and table_name in ('preferencias_notificacao', 'fila_notificacoes', 'tentativas_envio_notificacao')
order by table_name;");

        Assert.Empty(tabelasIndevidas);
        Assert.Equal(0, await context.Set<TemplateNotificacao>().CountAsync());
    }

    private static Notificacao CriarNotificacao(
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = null,
        Guid? chamadoId = null,
        string? chaveIdempotencia = null,
        string? chaveCorrelacao = "corr-persistencia",
        Guid? criadoPorUsuarioId = null)
    {
        return new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo materializado da notificacao",
            chaveIdempotencia ?? $"notif:{Guid.NewGuid():N}",
            CriadoPorTeste,
            destinatarioUsuarioId,
            destinatarioEndereco,
            chamadoId,
            "Assunto persistido",
            chaveCorrelacao,
            criadoPorUsuarioId);
    }

    private static async Task InserirNotificacaoViaSqlAsync(
        SGXSistemaChamadoDbContext context,
        Guid id,
        string chaveIdempotencia,
        Guid? destinatarioUsuarioId,
        string? destinatarioEndereco,
        int quantidadeTentativas)
    {
        await context.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO notificacoes
(
    id,
    chamado_id,
    tipo_evento,
    canal,
    status,
    destinatario_usuario_id,
    destinatario_endereco,
    assunto,
    conteudo,
    chave_correlacao,
    chave_idempotencia,
    agendada_em,
    processada_em,
    enviada_em,
    falhou_em,
    cancelada_em,
    quantidade_tentativas,
    ultimo_erro,
    motivo_cancelamento,
    criado_por_usuario_id,
    atualizado_por_usuario_id,
    criado_em,
    criado_por,
    atualizado_em,
    atualizado_por,
    ativo
)
VALUES
(
    {id},
    {(Guid?)null},
    {(int)TipoEventoNotificacao.EventoChamado},
    {(int)CanalNotificacao.Email},
    {(int)StatusNotificacao.Pendente},
    {destinatarioUsuarioId},
    {destinatarioEndereco},
    {"Assunto SQL"},
    {"Conteudo SQL"},
    {"corr-sql"},
    {chaveIdempotencia},
    {(DateTime?)null},
    {(DateTime?)null},
    {(DateTime?)null},
    {(DateTime?)null},
    {(DateTime?)null},
    {quantidadeTentativas},
    {(string?)null},
    {(string?)null},
    {(Guid?)null},
    {(Guid?)null},
    {DateTime.UtcNow},
    {CriadoPorTeste},
    {(DateTime?)null},
    {(string?)null},
    {true}
);");
    }

    private static async Task<List<string>> LerListaAsync(SGXSistemaChamadoDbContext context, string sql)
    {
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = sql;

        if (command.Connection!.State != System.Data.ConnectionState.Open)
        {
            await command.Connection.OpenAsync();
        }

        var resultado = new List<string>();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            resultado.Add(reader.GetString(0));
        }

        return resultado;
    }

    private const string CriadoPorTeste = "test.notificacao.persistence";
}

public sealed class NotificacaoPersistenceDatabaseFixture : IAsyncLifetime
{
    private const string DatabaseName = "sgx_notificacoes_tests";

    private string _connectionString = string.Empty;

    public string ConnectionString => _connectionString;

    public async Task InitializeAsync()
    {
        var configuracao = JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(
            ObterRaizDaSolucao(),
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
        await context.Chamados.Where(x => x.CriadoPor == "test.notificacao.persistence").ExecuteDeleteAsync();
        await context.StatusChamado.Where(x => x.CriadoPor == "test.notificacao.persistence").ExecuteDeleteAsync();
        await context.PrioridadesChamado.Where(x => x.CriadoPor == "test.notificacao.persistence").ExecuteDeleteAsync();
        await context.CategoriasChamado.Where(x => x.CriadoPor == "test.notificacao.persistence").ExecuteDeleteAsync();
        await context.Usuarios.Where(x => x.CriadoPor == "test.notificacao.persistence").ExecuteDeleteAsync();
    }

    public async Task<Guid> CriarUsuarioTemporarioAsync(SGXSistemaChamadoDbContext context)
    {
        var usuario = new Usuario(
            $"Usuario Teste {Guid.NewGuid():N}"[..30],
            $"usuario.{Guid.NewGuid():N}@teste.local",
            $"notif.{Guid.NewGuid():N}"[..20],
            "test.notificacao.persistence");

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario.Id;
    }

    public async Task<Chamado> CriarChamadoTemporarioAsync(SGXSistemaChamadoDbContext context)
    {
        var solicitanteId = await CriarUsuarioTemporarioAsync(context);
        var categoria = new CategoriaChamado(
            $"Categoria Teste {Guid.NewGuid():N}"[..25],
            "Categoria criada para testes relacionais de notificacao.",
            null,
            "test.notificacao.persistence");
        var prioridade = await context.PrioridadesChamado
            .OrderBy(x => x.CriadoEm)
            .FirstAsync();
        var status = await context.StatusChamado
            .OrderBy(x => x.CriadoEm)
            .FirstAsync();

        context.CategoriasChamado.Add(categoria);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            $"NTF-{Guid.NewGuid():N}"[..12],
            "Chamado temporario para notificacao",
            "Chamado criado para validar relacionamento da notificacao.",
            solicitanteId,
            categoria.Id,
            prioridade.Id,
            status.Id,
            OrigemChamado.Portal,
            "test.notificacao.persistence");

        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();
        return chamado;
    }

    public string NovaChaveIdempotencia() => $"notif:{Guid.NewGuid():N}";

    public static string ObterRaizDaSolucao()
    {
        var diretorio = new DirectoryInfo(AppContext.BaseDirectory);
        while (diretorio is not null)
        {
            if (File.Exists(Path.Combine(diretorio.FullName, "SGX.SistemaChamado.sln")))
            {
                return diretorio.FullName;
            }

            diretorio = diretorio.Parent;
        }

        throw new InvalidOperationException("Nao foi possivel localizar a raiz da solucao.");
    }

}
