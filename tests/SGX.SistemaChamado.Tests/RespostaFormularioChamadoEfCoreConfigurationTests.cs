using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class RespostaFormularioChamadoEfCoreConfigurationTests
{
    [Fact]
    public async Task MetadataDeRespostaFormularioChamadoDeveEstarConsistente()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(RespostaFormularioChamado));
        Assert.NotNull(entityType);

        Assert.Equal("respostas_formulario_chamado", entityType!.GetTableName());
        Assert.Equal("chamado_id", entityType.FindProperty(nameof(RespostaFormularioChamado.ChamadoId))!.GetColumnName());
        Assert.Equal("formulario_servico_versao_id", entityType.FindProperty(nameof(RespostaFormularioChamado.FormularioServicoVersaoId))!.GetColumnName());
        Assert.Equal("campo_formulario_servico_id", entityType.FindProperty(nameof(RespostaFormularioChamado.CampoFormularioServicoId))!.GetColumnName());
        Assert.Equal("valor", entityType.FindProperty(nameof(RespostaFormularioChamado.Valor))!.GetColumnName());
        Assert.Equal("valores_json", entityType.FindProperty(nameof(RespostaFormularioChamado.ValoresJson))!.GetColumnName());
        Assert.Equal(RespostaFormularioChamado.TamanhoMaximoValor, entityType.FindProperty(nameof(RespostaFormularioChamado.Valor))!.GetMaxLength());
        Assert.Equal(16000, entityType.FindProperty(nameof(RespostaFormularioChamado.ValoresJson))!.GetMaxLength());

        var fkChamado = Assert.Single(
            entityType.GetForeignKeys(),
            x => x.PrincipalEntityType.ClrType == typeof(Chamado));

        var fkVersao = Assert.Single(
            entityType.GetForeignKeys(),
            x => x.PrincipalEntityType.ClrType == typeof(FormularioServicoVersao));

        var fkCampo = Assert.Single(
            entityType.GetForeignKeys(),
            x => x.PrincipalEntityType.ClrType == typeof(CampoFormularioServico));

        Assert.Equal(nameof(RespostaFormularioChamado.ChamadoId), Assert.Single(fkChamado.Properties).Name);
        Assert.Equal(nameof(RespostaFormularioChamado.FormularioServicoVersaoId), Assert.Single(fkVersao.Properties).Name);
        Assert.Equal(nameof(RespostaFormularioChamado.CampoFormularioServicoId), Assert.Single(fkCampo.Properties).Name);
        Assert.Equal(DeleteBehavior.Restrict, fkChamado.DeleteBehavior);
        Assert.Equal(DeleteBehavior.Restrict, fkVersao.DeleteBehavior);
        Assert.Equal(DeleteBehavior.Restrict, fkCampo.DeleteBehavior);
    }

    [Fact]
    public async Task IndicesDeRespostaFormularioChamadoDevemEstarConsistentes()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(RespostaFormularioChamado));
        Assert.NotNull(entityType);

        var indiceChamado = Assert.Single(
            entityType!.GetIndexes(),
            x => !x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(RespostaFormularioChamado.ChamadoId) }));

        var indiceVersao = Assert.Single(
            entityType.GetIndexes(),
            x => !x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(RespostaFormularioChamado.FormularioServicoVersaoId) }));

        var indiceCampo = Assert.Single(
            entityType.GetIndexes(),
            x => !x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(RespostaFormularioChamado.CampoFormularioServicoId) }));

        var indiceChamadoCampo = Assert.Single(
            entityType.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(RespostaFormularioChamado.ChamadoId), nameof(RespostaFormularioChamado.CampoFormularioServicoId) }));

        var indiceChamadoVersao = Assert.Single(
            entityType.GetIndexes(),
            x => !x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(RespostaFormularioChamado.ChamadoId), nameof(RespostaFormularioChamado.FormularioServicoVersaoId) }));

        Assert.Equal("ix_resp_form_chamado", indiceChamado.GetDatabaseName());
        Assert.Equal("ix_resp_form_versao", indiceVersao.GetDatabaseName());
        Assert.Equal("ix_resp_form_campo", indiceCampo.GetDatabaseName());
        Assert.Equal("ux_resp_form_chamado_cmp", indiceChamadoCampo.GetDatabaseName());
        Assert.Equal("ix_resp_form_chamado_ver", indiceChamadoVersao.GetDatabaseName());
    }

    [Fact]
    public async Task NomesDeIndicesDeRespostaFormularioChamadoDevemSerSegurosParaPostgreSql()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(RespostaFormularioChamado));
        Assert.NotNull(entityType);

        var nomes = entityType!.GetIndexes()
            .Select(x => x.GetDatabaseName())
            .OfType<string>()
            .ToArray();

        Assert.All(nomes, nome => Assert.True(nome.Length <= 63, $"Indice excede limite do PostgreSQL: {nome}"));
    }
}
