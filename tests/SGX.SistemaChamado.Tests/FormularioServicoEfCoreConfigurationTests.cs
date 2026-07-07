using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoEfCoreConfigurationTests
{
    [Fact]
    public async Task MetadataDeFormularioServicoDeveEstarConsistente()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(FormularioServico));
        Assert.NotNull(entityType);

        Assert.Equal("formularios_servico", entityType!.GetTableName());
        Assert.Equal("catalogo_servico_id", entityType.FindProperty(nameof(FormularioServico.CatalogoServicoId))!.GetColumnName());
        Assert.Equal(180, entityType.FindProperty(nameof(FormularioServico.Nome))!.GetMaxLength());
        Assert.Equal(4000, entityType.FindProperty(nameof(FormularioServico.Descricao))!.GetMaxLength());

        var indiceCatalogo = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(FormularioServico.CatalogoServicoId) }));

        Assert.Equal("ux_formularios_servico_catalogo_servico_id", indiceCatalogo.GetDatabaseName());

        var foreignKey = Assert.Single(entityType.GetForeignKeys());
        Assert.Equal(nameof(FormularioServico.CatalogoServicoId), Assert.Single(foreignKey.Properties).Name);
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task MetadataDeFormularioServicoVersaoDeveEstarConsistente()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(FormularioServicoVersao));
        Assert.NotNull(entityType);

        Assert.Equal("formularios_servico_versoes", entityType!.GetTableName());
        Assert.Equal("formulario_servico_id", entityType.FindProperty(nameof(FormularioServicoVersao.FormularioServicoId))!.GetColumnName());
        Assert.Equal("numero", entityType.FindProperty(nameof(FormularioServicoVersao.Numero))!.GetColumnName());
        Assert.Equal("publicada", entityType.FindProperty(nameof(FormularioServicoVersao.Publicada))!.GetColumnName());

        var indiceNumero = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(FormularioServicoVersao.FormularioServicoId), nameof(FormularioServicoVersao.Numero) }));

        Assert.Equal("ux_form_serv_versao_num", indiceNumero.GetDatabaseName());

        var foreignKey = Assert.Single(entityType.GetForeignKeys());
        Assert.Equal(nameof(FormularioServicoVersao.FormularioServicoId), Assert.Single(foreignKey.Properties).Name);
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task MetadataDeCampoFormularioServicoDeveEstarConsistente()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(CampoFormularioServico));
        Assert.NotNull(entityType);

        Assert.Equal("campos_formulario_servico", entityType!.GetTableName());
        Assert.Equal("formulario_servico_versao_id", entityType.FindProperty(nameof(CampoFormularioServico.FormularioServicoVersaoId))!.GetColumnName());
        Assert.Equal(120, entityType.FindProperty(nameof(CampoFormularioServico.Nome))!.GetMaxLength());
        Assert.Equal(180, entityType.FindProperty(nameof(CampoFormularioServico.Rotulo))!.GetMaxLength());
        Assert.Equal(500, entityType.FindProperty(nameof(CampoFormularioServico.TextoAjuda))!.GetMaxLength());

        var tipoProperty = entityType.FindProperty(nameof(CampoFormularioServico.Tipo));
        Assert.NotNull(tipoProperty);
        Assert.Equal(typeof(int), tipoProperty!.GetProviderClrType());

        var indiceNome = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(CampoFormularioServico.FormularioServicoVersaoId), nameof(CampoFormularioServico.Nome) }));

        var indiceOrdem = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(CampoFormularioServico.FormularioServicoVersaoId), nameof(CampoFormularioServico.Ordem) }));

        Assert.Equal("ux_campo_form_serv_nome", indiceNome.GetDatabaseName());
        Assert.Equal("ux_campo_form_serv_ordem", indiceOrdem.GetDatabaseName());

        var foreignKey = Assert.Single(entityType.GetForeignKeys());
        Assert.Equal(nameof(CampoFormularioServico.FormularioServicoVersaoId), Assert.Single(foreignKey.Properties).Name);
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task MetadataDeOpcaoCampoFormularioServicoDeveEstarConsistente()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(OpcaoCampoFormularioServico));
        Assert.NotNull(entityType);

        Assert.Equal("opcoes_campos_formulario_servico", entityType!.GetTableName());
        Assert.Equal("campo_formulario_servico_id", entityType.FindProperty(nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId))!.GetColumnName());
        Assert.Equal(120, entityType.FindProperty(nameof(OpcaoCampoFormularioServico.Valor))!.GetMaxLength());
        Assert.Equal(180, entityType.FindProperty(nameof(OpcaoCampoFormularioServico.Rotulo))!.GetMaxLength());

        var indiceValor = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId), nameof(OpcaoCampoFormularioServico.Valor) }));

        var indiceOrdem = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId), nameof(OpcaoCampoFormularioServico.Ordem) }));

        Assert.Equal("ux_opcao_form_serv_valor", indiceValor.GetDatabaseName());
        Assert.Equal("ux_opcao_form_serv_ordem", indiceOrdem.GetDatabaseName());

        var foreignKey = Assert.Single(entityType.GetForeignKeys());
        Assert.Equal(nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId), Assert.Single(foreignKey.Properties).Name);
        Assert.Equal(DeleteBehavior.Restrict, foreignKey.DeleteBehavior);
    }

    [Fact]
    public async Task NomesDeIndicesDevemSerSegurosParaPostgreSql()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var nomes = new[]
        {
            "ux_formularios_servico_catalogo_servico_id",
            "ix_formularios_servico_ativo",
            "ix_form_serv_versao_form",
            "ux_form_serv_versao_num",
            "ix_form_serv_versao_pub",
            "ix_form_serv_versao_ativo",
            "ix_campo_form_serv_versao",
            "ux_campo_form_serv_nome",
            "ux_campo_form_serv_ordem",
            "ix_campos_formulario_servico_ativo",
            "ix_opcao_form_serv_campo",
            "ux_opcao_form_serv_valor",
            "ux_opcao_form_serv_ordem",
            "ix_opcao_form_serv_ativo"
        };

        Assert.All(nomes, nome => Assert.True(nome.Length <= 63, $"Indice excede limite do PostgreSQL: {nome}"));
    }

    [Fact]
    public async Task SnapshotEMigrationsDevemRefletirEstruturaEfAtualSemItensFuturos()
    {
        var baseDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var snapshot = await File.ReadAllTextAsync(Path.Combine(baseDir, "SGXSistemaChamadoDbContextModelSnapshot.cs"));
        var migrationVersao = await File.ReadAllTextAsync(Directory.GetFiles(baseDir, "*_AdicionarVersionamentoFormularioServico.cs").Single());
        var migrationChecklist = await File.ReadAllTextAsync(Directory.GetFiles(baseDir, "*_SincronizarChecklistSprint8VersionamentoFormularioServico.cs").Single());

        Assert.Contains("formularios_servico_versoes", snapshot, StringComparison.Ordinal);
        Assert.Contains("formulario_servico_versao_id", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_form_serv_versao_num", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_campo_form_serv_nome", snapshot, StringComparison.Ordinal);
        Assert.Contains("ux_opcao_form_serv_valor", snapshot, StringComparison.Ordinal);

        Assert.Contains("INSERT INTO formularios_servico_versoes", migrationVersao, StringComparison.Ordinal);
        Assert.DoesNotContain("respostas", migrationVersao, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("UpdateData(", migrationChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateTable(", migrationChecklist, StringComparison.Ordinal);
        Assert.DoesNotContain("AddColumn(", migrationChecklist, StringComparison.Ordinal);
    }
}
