using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapAprovacaoChamadosConsistencyTests
{
    private static readonly HashSet<string> NomesNormalizadosAprovacaoChamados =
    [
        "aprovacaodechamados",
        "aprovacaochamados",
        "aprovacaochamado"
    ];

    [Fact]
    public void DeveManterItemUnicoDeAprovacaoChamadosComStatusFinal()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itensAtivos = context.RoadmapItsmItens
            .Where(EhCandidatoAprovacaoChamados)
            .OrderBy(x => x.CriadoEm)
            .ThenBy(x => x.Id)
            .ToArray();

        Assert.Single(itensAtivos);

        var item = itensAtivos[0];
        Assert.Equal(SeedData.RoadmapItsmItem13Id, item.Id);
        Assert.Equal("Aprovacao de chamados", item.Area);
        Assert.Equal("Atendimento", item.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, item.StatusTecnico);
        Assert.Equal(90, item.PercentualImplementacao);
    }

    [Fact]
    public async Task DetalheDoRoadmapDeAprovacaoChamadosDeveExibirHomologacaoFuncionalPreparada()
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

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem13Id);

        Assert.Equal("Aprovacao de chamados", detalhe.Area);
        Assert.Equal("Atendimento", detalhe.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.RequerValidacao, detalhe.StatusTecnico);
        Assert.Equal("Homologação funcional preparada", detalhe.StatusTecnicoDescricao);
        Assert.Equal(90, detalhe.PercentualImplementacao);
    }

    private static bool EhCandidatoAprovacaoChamados(RoadmapItsmItem item)
    {
        var areaNormalizada = Normalizar(item.Area);
        return NomesNormalizadosAprovacaoChamados.Contains(areaNormalizada);
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
