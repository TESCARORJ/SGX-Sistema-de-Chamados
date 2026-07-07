using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoPersistenceTests
{
    [Fact]
    public async Task EfDevePersistirFormularioVinculadoAoCatalogo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var departamento = new Departamento("Tecnologia", "TI", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var servico = new CatalogoServico(
            "Acesso VPN",
            "acesso-vpn-formulario",
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
            "Formulario inicial",
            "Estrutura base do formulario.",
            "teste");

        context.FormulariosServico.Add(formulario);
        await context.SaveChangesAsync();

        var salvo = await context.FormulariosServico
            .Include(x => x.CatalogoServico)
            .SingleAsync(x => x.Id == formulario.Id);

        Assert.Equal(servico.Id, salvo.CatalogoServicoId);
        Assert.NotNull(salvo.CatalogoServico);
        Assert.Equal("Acesso VPN", salvo.CatalogoServico.Nome);
    }

    [Fact]
    public async Task MigrationEstruturalDeveCriarSomenteTabelaBaseDoFormulario()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("CreateTable(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"formularios_servico\"", migration, StringComparison.Ordinal);
        Assert.Contains("catalogo_servico_id", migration, StringComparison.Ordinal);
        Assert.Contains("CreateIndex(", migration, StringComparison.Ordinal);
        Assert.Contains("ux_formularios_servico_catalogo_servico_id", migration, StringComparison.Ordinal);
        Assert.Contains("ForeignKey(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("campos_formulario", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("respostas_formulario", migration, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("chamados_formulario", migration, StringComparison.OrdinalIgnoreCase);
    }
}
