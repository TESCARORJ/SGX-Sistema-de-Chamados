using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class CampoFormularioServicoPersistenceTests
{
    [Fact]
    public async Task EfDevePersistirCampoVinculadoAVersaoDoFormulario()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var formulario = await CriarFormularioAsync(context, "persistencia-campo");
        var versao = new FormularioServicoVersao(formulario.Id, 1, false, null, "teste");
        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();

        var campo = new CampoFormularioServico(
            versao.Id,
            "matriculaServidor",
            "Matricula do servidor",
            TipoCampoFormularioServico.TextoCurto,
            true,
            1,
            "Informe a matricula institucional.",
            true,
            "teste");

        context.CamposFormularioServico.Add(campo);
        await context.SaveChangesAsync();

        var salvo = await context.CamposFormularioServico
            .Include(x => x.FormularioServicoVersao)
            .SingleAsync(x => x.Id == campo.Id);

        Assert.Equal(versao.Id, salvo.FormularioServicoVersaoId);
        Assert.NotNull(salvo.FormularioServicoVersao);
        Assert.Equal(1, salvo.FormularioServicoVersao.Numero);
        Assert.Equal(TipoCampoFormularioServico.TextoCurto, salvo.Tipo);
        Assert.True(salvo.Obrigatorio);
        Assert.Equal(1, salvo.Ordem);
        Assert.Equal("Informe a matricula institucional.", salvo.TextoAjuda);
        Assert.True(salvo.Visivel);
    }

    [Fact]
    public async Task DeveConfigurarNomeTecnicoUnicoPorVersaoNoModelo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(CampoFormularioServico));
        Assert.NotNull(entityType);

        var indice = Assert.Single(
            entityType!.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(CampoFormularioServico.FormularioServicoVersaoId), nameof(CampoFormularioServico.Nome) }));

        Assert.Equal("ux_campo_form_serv_nome", indice.GetDatabaseName());
    }

    [Fact]
    public async Task DeveConfigurarIndiceDeOrdenacaoUnicoPorVersaoNoModelo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(CampoFormularioServico));
        Assert.NotNull(entityType);

        var indice = Assert.Single(
            entityType!.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(CampoFormularioServico.FormularioServicoVersaoId), nameof(CampoFormularioServico.Ordem) }));

        Assert.Equal("ux_campo_form_serv_ordem", indice.GetDatabaseName());
    }

    [Fact]
    public async Task DeveConfigurarMapeamentoEfDoTipoDoCampo()
    {
        var configurationPath = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Configurations", "CampoFormularioServicoConfiguration.cs");

        var configuration = await File.ReadAllTextAsync(configurationPath);

        Assert.Contains("Property(x => x.Tipo)", configuration, StringComparison.Ordinal);
        Assert.Contains("HasColumnName(\"tipo\")", configuration, StringComparison.Ordinal);
        Assert.Contains("HasConversion<int>()", configuration, StringComparison.Ordinal);
        Assert.Contains("Property(x => x.Obrigatorio)", configuration, StringComparison.Ordinal);
        Assert.Contains("Property(x => x.Ordem)", configuration, StringComparison.Ordinal);
        Assert.Contains("Property(x => x.TextoAjuda)", configuration, StringComparison.Ordinal);
        Assert.Contains("Property(x => x.Visivel)", configuration, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MigrationEstruturalDeveCriarSomenteTabelaBaseDosCampos()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarCamposFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"campos_formulario_servico\"", migration, StringComparison.Ordinal);
        Assert.Contains("formulario_servico_id", migration, StringComparison.Ordinal);
        Assert.Contains("ux_campos_formulario_servico_formulario_servico_id_nome", migration, StringComparison.Ordinal);
        Assert.Contains("ForeignKey(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("opcoes", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("respostas", migration, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task MigrationEstruturalDeveAdicionarSomenteColunaTipoNoCampo()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarTipoCampoFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("AddColumn<int>(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"tipo\"", migration, StringComparison.Ordinal);
        Assert.Contains("table: \"campos_formulario_servico\"", migration, StringComparison.Ordinal);
        Assert.Contains("defaultValue: 1", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateTable(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("opcoes", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("respostas", migration, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task MigrationEstruturalDeveAdicionarSomenteMetadadosDoCampo()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarMetadadosCampoFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("AddColumn<bool>(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"obrigatorio\"", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"visivel\"", migration, StringComparison.Ordinal);
        Assert.Contains("AddColumn<int>(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"ordem\"", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"texto_ajuda\"", migration, StringComparison.Ordinal);
        Assert.Contains("defaultValue: 1", migration, StringComparison.Ordinal);
        Assert.Contains("defaultValue: true", migration, StringComparison.Ordinal);
        Assert.Contains("CreateIndex(", migration, StringComparison.Ordinal);
        Assert.Contains("ux_campos_formulario_servico_formulario_servico_id_ordem", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateTable(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("opcoes", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("respostas", migration, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<FormularioServico> CriarFormularioAsync(SGXSistemaChamadoDbContext context, string sufixo)
    {
        var departamento = new Departamento($"Tecnologia {sufixo}", $"T{sufixo[..Math.Min(2, sufixo.Length)].ToUpperInvariant()}", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var servico = new CatalogoServico(
            $"Servico {sufixo}",
            $"servico-{sufixo}",
            "Descricao valida",
            null,
            departamento.Id,
            null,
            null,
            null,
            null,
            null,
            VisibilidadeCatalogoServico.Interno,
            true,
            false,
            1,
            Guid.NewGuid(),
            "teste");

        context.CatalogosServico.Add(servico);
        await context.SaveChangesAsync();

        var formulario = new FormularioServico(
            servico.Id,
            $"Formulario {sufixo}",
            "Estrutura base do formulario.",
            "teste");

        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();

        return formulario;
    }
}
