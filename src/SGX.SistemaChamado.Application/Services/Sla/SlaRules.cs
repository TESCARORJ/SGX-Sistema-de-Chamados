using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public static class SlaRules
{
    public static bool EstaProximoDoVencimento(ChamadoSla? chamadoSla, DateTime agoraUtc)
    {
        return CalcularSituacao(chamadoSla, agoraUtc) == SituacaoSlaChamadoEnum.ProximoDoVencimento;
    }

    public static SituacaoSlaChamadoEnum CalcularSituacao(ChamadoSla? chamadoSla, DateTime agoraUtc)
    {
        if (chamadoSla is null)
        {
            return SituacaoSlaChamadoEnum.NaoAplicavel;
        }

        if (chamadoSla.Pausado)
        {
            return SituacaoSlaChamadoEnum.Pausado;
        }

        if (chamadoSla.DataResolucao.HasValue)
        {
            return chamadoSla.ResolucaoViolada
                ? SituacaoSlaChamadoEnum.Violado
                : SituacaoSlaChamadoEnum.Cumprido;
        }

        if (agoraUtc > chamadoSla.PrazoResolucao)
        {
            return SituacaoSlaChamadoEnum.Vencido;
        }

        var minutosRestantes = (chamadoSla.PrazoResolucao - agoraUtc).TotalMinutes;
        var minutosTotais = (chamadoSla.PrazoResolucao - chamadoSla.DataInicio).TotalMinutes;
        var limitePercentual = minutosTotais * 0.2d;

        if (minutosRestantes <= 60 || minutosRestantes <= limitePercentual)
        {
            return SituacaoSlaChamadoEnum.ProximoDoVencimento;
        }

        return SituacaoSlaChamadoEnum.DentroDoPrazo;
    }

    public static int? CalcularTempoRestanteMinutos(ChamadoSla? chamadoSla, DateTime agoraUtc)
    {
        if (chamadoSla is null || chamadoSla.DataResolucao.HasValue)
        {
            return null;
        }

        var minutos = (int)Math.Floor((chamadoSla.PrazoResolucao - agoraUtc).TotalMinutes);
        return Math.Max(0, minutos);
    }

    public static int? CalcularTempoExcedidoMinutos(ChamadoSla? chamadoSla, DateTime agoraUtc)
    {
        if (chamadoSla is null)
        {
            return null;
        }

        var referencia = chamadoSla.DataResolucao ?? agoraUtc;
        if (referencia <= chamadoSla.PrazoResolucao)
        {
            return 0;
        }

        return (int)Math.Floor((referencia - chamadoSla.PrazoResolucao).TotalMinutes);
    }
}
