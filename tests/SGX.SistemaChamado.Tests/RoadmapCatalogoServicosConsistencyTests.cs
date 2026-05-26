using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapCatalogoServicosConsistencyTests
{
    [Fact]
    public void DeveManterApenasUmItemCanonicoParaCatalogoDeServicos()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itens = context.RoadmapItsmItens
            .Where(x => Normalizar(x.Area) == "catalogo de servicos")
            .ToArray();

        Assert.Single(itens);

        var item = itens[0];
        Assert.Equal(SeedData.RoadmapItsmItem12Id, item.Id);
        Assert.Equal("Catalogo de Servicos", item.Area);
        Assert.Equal("Conhecimento", item.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, item.StatusTecnico);
        Assert.Equal(90, item.PercentualImplementacao);
    }

    [Fact]
    public async Task DetalheDoRoadmapDoCatalogoDeServicosDeveRefletirStatusFinal()
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

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem12Id);

        Assert.Equal("Catalogo de Servicos", detalhe.Area);
        Assert.Equal("Conhecimento", detalhe.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, detalhe.StatusTecnico);
        Assert.Equal("Homologação funcional preparada", detalhe.StatusTecnicoDescricao);
        Assert.Equal(90, detalhe.PercentualImplementacao);
        Assert.False(detalhe.PercentualCalculadoPorChecklist);
    }

    private static string Normalizar(string valor)
    {
        var formD = valor.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(formD.Length);
        foreach (var caractere in formD)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(caractere);
            if (categoria != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(caractere);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }
}
