using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class OpcaoCampoFormularioServicoPersistenceTests
{
    [Fact]
    public async Task EfDevePersistirOpcaoVinculadaAoCampo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var campo = await CriarCampoAsync(context, "persistencia-opcao", 1);
        var opcao = new OpcaoCampoFormularioServico(
            campo.Id,
            "infra",
            "Infraestrutura",
            1,
            "teste");

        context.OpcoesCamposFormularioServico.Add(opcao);
        await context.SaveChangesAsync();

        var salvo = await context.OpcoesCamposFormularioServico
            .Include(x => x.CampoFormularioServico)
            .SingleAsync(x => x.Id == opcao.Id);

        Assert.Equal(campo.Id, salvo.CampoFormularioServicoId);
        Assert.NotNull(salvo.CampoFormularioServico);
        Assert.Equal("campopersistenciaopcao", salvo.CampoFormularioServico.Nome);
        Assert.Equal("infra", salvo.Valor);
        Assert.Equal("Infraestrutura", salvo.Rotulo);
        Assert.Equal(1, salvo.Ordem);
    }

    [Fact]
    public async Task DeveConfigurarIndiceUnicoPorCampoEValorNoModelo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(OpcaoCampoFormularioServico));
        Assert.NotNull(entityType);

        var indice = Assert.Single(
            entityType!.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId), nameof(OpcaoCampoFormularioServico.Valor) }));

        Assert.Equal("ux_opcao_form_serv_valor", indice.GetDatabaseName());
    }

    [Fact]
    public async Task DeveConfigurarIndiceUnicoPorCampoEOrdemNoModelo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(OpcaoCampoFormularioServico));
        Assert.NotNull(entityType);

        var indice = Assert.Single(
            entityType!.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(OpcaoCampoFormularioServico.CampoFormularioServicoId), nameof(OpcaoCampoFormularioServico.Ordem) }));

        Assert.Equal("ux_opcao_form_serv_ordem", indice.GetDatabaseName());
    }

    [Fact]
    public async Task MigrationEstruturalDeveCriarSomenteTabelaDeOpcoes()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarOpcoesCampoFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"opcoes_campos_formulario_servico\"", migration, StringComparison.Ordinal);
        Assert.Contains("campo_formulario_servico_id", migration, StringComparison.Ordinal);
        Assert.Contains("ux_opcao_form_serv_valor", migration, StringComparison.Ordinal);
        Assert.Contains("ux_opcao_form_serv_ordem", migration, StringComparison.Ordinal);
        Assert.Contains("ForeignKey(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("respostas", migration, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<CampoFormularioServico> CriarCampoAsync(SGXSistemaChamadoDbContext context, string sufixo, int ordem)
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

        var versao = new FormularioServicoVersao(
            formulario.Id,
            1,
            false,
            null,
            "teste");

        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();

        var campo = new CampoFormularioServico(
            versao.Id,
            $"campo{sufixo.Replace("-", string.Empty)}",
            $"Campo {sufixo}",
            TipoCampoFormularioServico.SelecaoUnica,
            false,
            ordem,
            null,
            true,
            "teste");

        context.CamposFormularioServico.Add(campo);
        await context.SaveChangesAsync();

        return campo;
    }
}
