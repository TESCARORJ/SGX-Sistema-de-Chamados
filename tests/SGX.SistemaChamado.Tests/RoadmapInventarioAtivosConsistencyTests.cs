using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapInventarioAtivosConsistencyTests
{
    private static readonly HashSet<string> NomesNormalizadosInventarioAtivos =
    [
        "inventarioativos",
        "inventariodeativos",
        "inventario",
        "ativos"
    ];

    [Fact]
    public void DeveManterApenasUmItemAtivoCanonicoParaInventarioAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itensAtivos = context.RoadmapItsmItens
            .Where(EhCandidatoInventarioAtivos)
            .ToArray();

        Assert.Single(itensAtivos);

        var item = itensAtivos[0];
        Assert.Equal(SeedData.RoadmapItsmItem11Id, item.Id);
        Assert.Equal("Inventario/Ativos", item.Area);
        Assert.Equal("Infraestrutura", item.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, item.StatusTecnico);
        Assert.Equal(90, item.PercentualImplementacao);
    }

    [Fact]
    public async Task DetalheDoRoadmapDeInventarioAtivosDeveRefletirStatusFinal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Administrador Teste",
                "admin@sgx.local",
                "admin.teste",
                ["Administrador"])));

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem11Id);

        Assert.Equal("Inventario/Ativos", detalhe.Area);
        Assert.Equal("Infraestrutura", detalhe.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, detalhe.StatusTecnico);
        Assert.Equal("Homologação funcional preparada", detalhe.StatusTecnicoDescricao);
        Assert.Equal(0, detalhe.PercentualImplementacao);
        Assert.Equal(0, detalhe.QuantidadeChecklistAtivo);
        Assert.Equal(0, detalhe.QuantidadeChecklistConcluido);
        Assert.False(detalhe.PercentualCalculadoPorChecklist);
    }

    [Fact]
    public void NaoDeveExistirDuplicidadeAtivaPorVariacoesDeNomeDoInventarioAtivos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var duplicados = context.RoadmapItsmItens
            .Where(EhCandidatoInventarioAtivos)
            .OrderBy(x => x.CriadoEm)
            .ThenBy(x => x.Id)
            .ToArray();

        Assert.Single(duplicados);
        Assert.Equal("Inventario/Ativos", duplicados[0].Area);
        Assert.Equal("Infraestrutura", duplicados[0].Categoria);
        Assert.Equal(90, duplicados[0].PercentualImplementacao);
    }

    private static bool EhCandidatoInventarioAtivos(RoadmapItsmItem item)
    {
        var areaNormalizada = Normalizar(item.Area);
        if (NomesNormalizadosInventarioAtivos.Contains(areaNormalizada) is false)
        {
            return false;
        }

        if (areaNormalizada is "inventarioativos" or "inventariodeativos")
        {
            return true;
        }

        var categoriaNormalizada = Normalizar(item.Categoria);
        return categoriaNormalizada is "infraestrutura" or "ativos";
    }

    private static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var formD = valor.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(formD.Length);

        foreach (var caractere in formD)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(caractere);
            if (categoria == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (!char.IsLetterOrDigit(caractere))
            {
                continue;
            }

            builder.Append(caractere);
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
