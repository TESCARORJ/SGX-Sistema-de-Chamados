using System.Globalization;
using System.Text;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Tests;

public sealed class RoadmapHistoricoAuditoriaChecklistConsistencyTests
{
    [Fact]
    public void DeveExistirExatamenteUmItemAtivoHistoricoAuditoriaComChecklistSemDuplicidade()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var itensAtivos = context.RoadmapItsmItens
            .Where(x => x.Ativo)
            .Where(x => Normalizar(x.Area) == "historicoauditoria")
            .ToArray();

        Assert.Single(itensAtivos);

        var item = itensAtivos[0];
        Assert.Equal("Histórico/Auditoria", item.Area);
        Assert.Equal("Governança", item.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, item.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, item.StatusTecnico);

        var checklistAtivo = context.RoadmapChecklistItens
            .Where(x => x.RoadmapItemId == item.Id && x.Ativo)
            .OrderBy(x => x.Ordem)
            .ToArray();

        Assert.Equal(63, checklistAtivo.Length);

        var duplicidadeOrdem = checklistAtivo
            .GroupBy(x => x.Ordem)
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToArray();
        Assert.Empty(duplicidadeOrdem);

        var duplicidadeOrdemDescricao = checklistAtivo
            .GroupBy(x => new { x.Ordem, Descricao = (x.Descricao ?? string.Empty).Trim().ToUpperInvariant() })
            .Where(x => x.Count() > 1)
            .Select(x => x.Key.Ordem)
            .ToArray();
        Assert.Empty(duplicidadeOrdemDescricao);

        Assert.All(checklistAtivo, x =>
        {
            Assert.True(x.Ativo);
            Assert.True(x.Concluido);
        });
    }

    [Fact]
    public async Task DeveCalcularPercentualPorChecklistNoItemHistoricoAuditoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = new UsuarioContextoAplicacao(
            Guid.NewGuid(),
            "Administrador Teste",
            "admin@sgx.local",
            "admin.teste",
            ["Administrador"]);

        var useCase = new ObterRoadmapItsmItemUseCase(
            PortalUseCasesTestFactory.Repo<RoadmapItsmItem>(context),
            new FakeUsuarioContextoAplicacaoService(usuario));

        var detalhe = await useCase.ExecutarAsync(SeedData.RoadmapItsmItem06Id);

        var totalAtivo = detalhe.QuantidadeChecklistAtivo;
        var totalConcluido = detalhe.QuantidadeChecklistConcluido;
        var percentualEsperado = totalAtivo == 0
            ? detalhe.PercentualImplementacao
            : (int)Math.Round((totalConcluido * 100.0) / totalAtivo, MidpointRounding.AwayFromZero);

        Assert.Equal("Histórico/Auditoria", detalhe.Area);
        Assert.Equal("Governança", detalhe.Categoria);
        Assert.Equal(StatusImplementacaoRoadmapItsm.ImplementadoFuncionalmente, detalhe.StatusImplementacao);
        Assert.Equal(StatusTecnicoRoadmapItsm.CompletoComPendenciasEvolutivas, detalhe.StatusTecnico);
        Assert.True(detalhe.PercentualCalculadoPorChecklist);
        Assert.Equal(63, totalAtivo);
        Assert.Equal(63, totalConcluido);
        Assert.Equal(percentualEsperado, detalhe.PercentualImplementacao);
        Assert.Equal(100, detalhe.PercentualImplementacao);
    }

    private static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        var normalizado = valor.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizado.Length);
        foreach (var c in normalizado)
        {
            var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (!char.IsWhiteSpace(c) && c != '/' && c != '-')
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }

        return sb.ToString();
    }
}
