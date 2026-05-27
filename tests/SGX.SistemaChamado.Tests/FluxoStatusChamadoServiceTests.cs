using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class FluxoStatusChamadoServiceTests
{
    [Theory]
    [MemberData(nameof(CenariosFluxoPorNatureza))]
    public void ObterStatusPermitidosPorNaturezaDeveRespeitarMatriz(
        NaturezaChamadoEnum natureza,
        StatusChamadoEnum[] esperado)
    {
        var service = new FluxoStatusChamadoService();

        var statusPermitidos = service.ObterStatusPermitidos(natureza);

        Assert.Equal(esperado, statusPermitidos);
    }

    [Theory]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.EmAnalise)]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.AguardandoAprovacao)]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.Aprovada)]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.Reprovada)]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.EmExecucao)]
    [InlineData(NaturezaChamadoEnum.Mudanca, StatusChamadoEnum.Concluida)]
    [InlineData(NaturezaChamadoEnum.Problema, StatusChamadoEnum.CausaRaizIdentificada)]
    [InlineData(NaturezaChamadoEnum.Problema, StatusChamadoEnum.SolucaoDeContorno)]
    [InlineData(NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.Correlacionado)]
    [InlineData(NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.Tratado)]
    [InlineData(NaturezaChamadoEnum.TarefaOperacional, StatusChamadoEnum.Planejada)]
    [InlineData(NaturezaChamadoEnum.TarefaOperacional, StatusChamadoEnum.EmExecucao)]
    [InlineData(NaturezaChamadoEnum.TarefaOperacional, StatusChamadoEnum.Concluida)]
    public void StatusEhPermitidoDeveRetornarTrueParaStatusCompativel(
        NaturezaChamadoEnum natureza,
        StatusChamadoEnum status)
    {
        var service = new FluxoStatusChamadoService();

        var permitido = service.StatusEhPermitido(natureza, status);

        Assert.True(permitido);
    }

    [Fact]
    public void ValidarStatusPermitidoLancaExcecaoQuandoStatusIncompativel()
    {
        var service = new FluxoStatusChamadoService();

        var ex = Assert.Throws<InvalidOperationException>(() =>
            service.ValidarStatusPermitido(NaturezaChamadoEnum.EventoAlerta, StatusChamadoEnum.AguardandoSolicitante));

        Assert.Contains("nao e permitido", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    public static IEnumerable<object[]> CenariosFluxoPorNatureza()
    {
        yield return
        [
            NaturezaChamadoEnum.Incidente,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.EmAtendimento,
                StatusChamadoEnum.AguardandoSolicitante,
                StatusChamadoEnum.Resolvido,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];

        yield return
        [
            NaturezaChamadoEnum.Requisicao,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.EmAtendimento,
                StatusChamadoEnum.AguardandoSolicitante,
                StatusChamadoEnum.Resolvido,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];

        yield return
        [
            NaturezaChamadoEnum.Mudanca,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.EmAnalise,
                StatusChamadoEnum.AguardandoAprovacao,
                StatusChamadoEnum.Aprovada,
                StatusChamadoEnum.Reprovada,
                StatusChamadoEnum.EmExecucao,
                StatusChamadoEnum.Concluida,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];

        yield return
        [
            NaturezaChamadoEnum.Problema,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.EmAnalise,
                StatusChamadoEnum.CausaRaizIdentificada,
                StatusChamadoEnum.SolucaoDeContorno,
                StatusChamadoEnum.Resolvido,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];

        yield return
        [
            NaturezaChamadoEnum.EventoAlerta,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.EmAnalise,
                StatusChamadoEnum.Correlacionado,
                StatusChamadoEnum.Tratado,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];

        yield return
        [
            NaturezaChamadoEnum.TarefaOperacional,
            new[]
            {
                StatusChamadoEnum.Aberto,
                StatusChamadoEnum.Planejada,
                StatusChamadoEnum.EmExecucao,
                StatusChamadoEnum.Concluida,
                StatusChamadoEnum.Encerrado,
                StatusChamadoEnum.Cancelado
            }
        ];
    }
}
