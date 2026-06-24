using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class TemplateNotificacaoPersistenceTests : IClassFixture<TemplateNotificacaoPersistenceDatabaseFixture>
{
    private readonly TemplateNotificacaoPersistenceDatabaseFixture _fixture;

    public TemplateNotificacaoPersistenceTests(TemplateNotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DevePersistirTemplateERecuperarEnumsEVariaveis()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var template = CriarTemplate(usuarioId, versao: 2, variaveisPermitidas: ["chamado.codigo", "solicitante.nome"]);

        context.Set<TemplateNotificacao>().Add(template);
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistido = await consulta.Set<TemplateNotificacao>().SingleAsync(x => x.Id == template.Id);

        Assert.Equal(TipoEventoNotificacao.EventoChamado, persistido.TipoEvento);
        Assert.Equal(CanalNotificacao.Email, persistido.Canal);
        Assert.Equal(2, persistido.Versao);
        Assert.Equal(["chamado.codigo", "solicitante.nome"], persistido.VariaveisPermitidas);
    }

    [Fact]
    public async Task DeveValidarUnicidadePorNomeEVersao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        context.Set<TemplateNotificacao>().Add(CriarTemplate(usuarioId, nome: "Aviso", versao: 1));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(usuarioId, nome: "Aviso", versao: 1));

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Equal(PostgresErrorCodes.UniqueViolation, postgres.SqlState);
        Assert.Equal("ux_templates_notificacao_nome_versao", postgres.ConstraintName);
    }

    [Fact]
    public async Task DeveValidarVersaoPositivaEVigenciaNoBanco()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        var exVersao = await Assert.ThrowsAsync<PostgresException>(() =>
            InserirTemplateViaSqlAsync(
                context,
                id: Guid.NewGuid(),
                usuarioId: usuarioId,
                nome: "Template sql 1",
                versao: 0,
                vigenteDe: null,
                vigenteAte: null));

        Assert.Equal(PostgresErrorCodes.CheckViolation, exVersao.SqlState);
        Assert.Equal("ck_templates_notificacao_versao_positiva", exVersao.ConstraintName);

        var exVigencia = await Assert.ThrowsAsync<PostgresException>(() =>
            InserirTemplateViaSqlAsync(
                context,
                id: Guid.NewGuid(),
                usuarioId: usuarioId,
                nome: "Template sql 2",
                versao: 1,
                vigenteDe: new DateTime(2026, 6, 22, 0, 0, 0, DateTimeKind.Utc),
                vigenteAte: new DateTime(2026, 6, 21, 0, 0, 0, DateTimeKind.Utc)));

        Assert.Equal(PostgresErrorCodes.CheckViolation, exVigencia.SqlState);
        Assert.Equal("ck_templates_notificacao_vigencia", exVigencia.ConstraintName);
    }

    [Fact]
    public async Task DeveValidarFkEAusenciaDeCascata()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var template = CriarTemplate(usuarioId);

        context.Set<TemplateNotificacao>().Add(template);
        await context.SaveChangesAsync();

        await using var exclusaoContexto = _fixture.CreateContext();
        var usuario = await exclusaoContexto.Usuarios.SingleAsync(x => x.Id == usuarioId);
        exclusaoContexto.Usuarios.Remove(usuario);

        var ex = await Assert.ThrowsAsync<DbUpdateException>(() => exclusaoContexto.SaveChangesAsync());
        var postgres = Assert.IsType<PostgresException>(ex.InnerException);

        Assert.Contains(postgres.SqlState, new[] { PostgresErrorCodes.ForeignKeyViolation, PostgresErrorCodes.RestrictViolation });
        Assert.Contains("FK_templates_notificacao_usuarios_", postgres.ConstraintName, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DevePermitirSelecionarPorEventoCanalEOrdenarPorVersao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        context.Set<TemplateNotificacao>().Add(CriarTemplate(usuarioId, nome: "Template A", versao: 1));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(usuarioId, nome: "Template B", versao: 3));
        context.Set<TemplateNotificacao>().Add(CriarTemplate(usuarioId, nome: "Template C", versao: 2));
        await context.SaveChangesAsync();

        var selecionados = await context.Set<TemplateNotificacao>()
            .Where(x => x.TipoEvento == TipoEventoNotificacao.EventoChamado && x.Canal == CanalNotificacao.Email && x.Ativo)
            .OrderByDescending(x => x.Versao)
            .ThenBy(x => x.Nome)
            .Select(x => x.Versao)
            .ToArrayAsync();

        Assert.Equal([3, 2, 1], selecionados);
    }

    [Fact]
    public async Task DeveConfirmarTabelaSemSeedFuncionalDeTemplate()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();

        Assert.Equal(0, await context.Set<TemplateNotificacao>().CountAsync());

        var tabelasIndevidas = await LerListaAsync(context, @"
select table_name
from information_schema.tables
where table_schema = 'public'
  and table_name in ('preferencias_notificacao', 'fila_notificacoes', 'tentativas_envio_notificacao')
order by table_name;");

        Assert.Empty(tabelasIndevidas);
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
            "20260621170511_CriarEstruturaTemplateNotificacaoSprint6.cs");

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"templates_notificacao\"", migration, StringComparison.Ordinal);
        Assert.Contains("table.PrimaryKey(\"PK_templates_notificacao\"", migration, StringComparison.Ordinal);
        Assert.Contains("ux_templates_notificacao_nome_versao", migration, StringComparison.Ordinal);
        Assert.Contains("ix_templates_notificacao_tipo_evento_canal_ativo", migration, StringComparison.Ordinal);
        Assert.Contains("ck_templates_notificacao_versao_positiva", migration, StringComparison.Ordinal);
        Assert.Contains("ck_templates_notificacao_vigencia", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("preferencias_notificacao", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("fila_notificacoes", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("tentativas_envio_notificacao", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("InsertData(", migration, StringComparison.Ordinal);
        Assert.Contains("DropTable(", migration, StringComparison.Ordinal);
    }

    private static TemplateNotificacao CriarTemplate(
        Guid usuarioId,
        string nome = "Template persistencia",
        int versao = 1,
        IReadOnlyCollection<string>? variaveisPermitidas = null)
    {
        return new TemplateNotificacao(
            nome,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            versao,
            "Conteudo {{chamado.codigo}}",
            usuarioId,
            "test.template.persistence",
            variaveisPermitidas ?? ["chamado.codigo"],
            "Assunto {{chamado.codigo}}",
            "Descricao");
    }

    private static async Task InserirTemplateViaSqlAsync(
        SGXSistemaChamadoDbContext context,
        Guid id,
        Guid usuarioId,
        string nome,
        int versao,
        DateTime? vigenteDe,
        DateTime? vigenteAte)
    {
        await context.Database.ExecuteSqlInterpolatedAsync($@"
INSERT INTO templates_notificacao
(
    id,
    nome,
    descricao,
    tipo_evento,
    canal,
    versao,
    assunto_template,
    conteudo_template,
    variaveis_permitidas,
    vigente_de,
    vigente_ate,
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
    {nome},
    {"Descricao SQL"},
    {(int)TipoEventoNotificacao.EventoChamado},
    {(int)CanalNotificacao.Email},
    {versao},
    {"Assunto {{chamado.codigo}}"},
    {"Conteudo {{chamado.codigo}}"},
    {"[\"chamado.codigo\"]"},
    {vigenteDe},
    {vigenteAte},
    {usuarioId},
    {(Guid?)null},
    {DateTime.UtcNow},
    {"test.template.persistence"},
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
}

public sealed class TemplateNotificacaoPersistenceDatabaseFixture : IAsyncLifetime
{
    private string _connectionString = string.Empty;

    public async Task InitializeAsync()
    {
        var configuracao = System.Text.Json.JsonDocument.Parse(await File.ReadAllTextAsync(Path.Combine(
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
            Database = $"sgx_template_tests_{Guid.NewGuid():N}"
        };

        _connectionString = builder.ConnectionString;

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = CreateContext();
        await context.Database.EnsureDeletedAsync();
    }

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
        await context.Set<TemplateNotificacao>().ExecuteDeleteAsync();
        await context.Usuarios.Where(x => x.CriadoPor == "test.template.persistence").ExecuteDeleteAsync();
    }

    public async Task<Guid> CriarUsuarioTemporarioAsync(SGXSistemaChamadoDbContext context)
    {
        var usuario = new Usuario(
            $"Usuario Template {Guid.NewGuid():N}"[..30],
            $"template.{Guid.NewGuid():N}@teste.local",
            $"tmpl.{Guid.NewGuid():N}"[..20],
            "test.template.persistence");

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario.Id;
    }
}
