using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicoGrupoTecnicoPersistenceTests
{
    [Fact]
    public async Task EfDevePersistirGrupoTecnicoOpcionalNoCatalogo()
    {
        await using var context = PortalUseCasesTestFactory.CriarContexto();

        var departamento = new Departamento("Tecnologia", "TI", null, "teste");
        var grupoTecnico = new GrupoTecnico("Service Desk", null, "teste");
        context.Departamentos.Add(departamento);
        context.GruposTecnicos.Add(grupoTecnico);
        await context.SaveChangesAsync();

        var servico = new CatalogoServico(
            "Acesso VPN",
            "acesso-vpn",
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
            "teste",
            grupoTecnico.Id);

        context.CatalogosServico.Add(servico);
        await context.SaveChangesAsync();

        var salvo = await context.CatalogosServico
            .Include(x => x.GrupoTecnico)
            .SingleAsync(x => x.Id == servico.Id);

        Assert.Equal(grupoTecnico.Id, salvo.GrupoTecnicoId);
        Assert.NotNull(salvo.GrupoTecnico);
        Assert.Equal("Service Desk", salvo.GrupoTecnico!.Nome);
    }

    [Fact]
    public async Task MigrationDeveConterSomenteAlteracoesEstruturaisDoVinculo()
    {
        var migrationDir = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..",
            "src", "SGX.SistemaChamado.Infrastructure", "Persistence", "Migrations");

        var migrationPath = Directory
            .GetFiles(migrationDir, "*_AdicionarGrupoTecnicoNoCatalogoServico.cs")
            .Single();

        var migration = await File.ReadAllTextAsync(migrationPath);

        Assert.Contains("AddColumn<Guid>(", migration, StringComparison.Ordinal);
        Assert.Contains("name: \"grupo_tecnico_id\"", migration, StringComparison.Ordinal);
        Assert.Contains("CreateIndex(", migration, StringComparison.Ordinal);
        Assert.Contains("ix_catalogo_servicos_grupo_tecnico_id", migration, StringComparison.Ordinal);
        Assert.Contains("AddForeignKey(", migration, StringComparison.Ordinal);
        Assert.Contains("FK_catalogo_servicos_grupos_tecnicos_grupo_tecnico_id", migration, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateTable(", migration, StringComparison.Ordinal);
    }
}
