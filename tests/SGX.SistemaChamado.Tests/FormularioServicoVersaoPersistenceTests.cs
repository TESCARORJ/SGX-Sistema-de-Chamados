using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoVersaoPersistenceTests
{
    [Fact]
    public async Task EfDevePersistirVersaoVinculadaAoFormulario()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var formulario = await CriarFormularioAsync(context, "versao-persistencia");
        var versao = new FormularioServicoVersao(formulario.Id, 1, false, null, "teste");

        context.FormulariosServicoVersoes.Add(versao);
        await context.SaveChangesAsync();

        var salvo = await context.FormulariosServicoVersoes
            .Include(x => x.FormularioServico)
            .SingleAsync(x => x.Id == versao.Id);

        Assert.Equal(formulario.Id, salvo.FormularioServicoId);
        Assert.NotNull(salvo.FormularioServico);
        Assert.Equal("Formulario versao-persistencia", salvo.FormularioServico.Nome);
        Assert.Equal(1, salvo.Numero);
    }

    [Fact]
    public async Task DeveConfigurarIndiceUnicoPorFormularioENumeroNoModelo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(FormularioServicoVersao));
        Assert.NotNull(entityType);

        var indice = Assert.Single(
            entityType!.GetIndexes(),
            x => x.IsUnique && x.Properties.Select(p => p.Name).SequenceEqual(new[] { nameof(FormularioServicoVersao.FormularioServicoId), nameof(FormularioServicoVersao.Numero) }));

        Assert.Equal("ux_form_serv_versao_num", indice.GetDatabaseName());
    }

    [Fact]
    public async Task MigrationEstruturalDeveCriarSomenteEstruturaDeVersoes()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarVersionamentoFormularioServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("formularios_servico_versoes", migration, StringComparison.Ordinal);
        Assert.Contains("formulario_servico_versao_id", migration, StringComparison.Ordinal);
        Assert.Contains("ux_form_serv_versao_num", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("UpdateData(", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("respostas", migration, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<FormularioServico> CriarFormularioAsync(SGXSistemaChamadoDbContext context, string sufixo)
    {
        var departamento = new Departamento($"Tecnologia {sufixo}", $"T{sufixo[..Math.Min(2, sufixo.Length)].ToUpperInvariant()}", null, "teste");
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        var servico = new CatalogoServico(
            $"Servico {sufixo}",
            $"servico-versao-{sufixo}",
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
