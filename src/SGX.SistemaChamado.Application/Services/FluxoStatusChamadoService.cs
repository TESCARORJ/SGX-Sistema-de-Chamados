using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services;

public sealed class FluxoStatusChamadoService : IFluxoStatusChamadoService
{
    private static readonly IReadOnlyCollection<StatusChamadoEnum> StatusIncidenteRequisicao =
    [
        StatusChamadoEnum.Aberto,
        StatusChamadoEnum.EmAtendimento,
        StatusChamadoEnum.AguardandoSolicitante,
        StatusChamadoEnum.Resolvido,
        StatusChamadoEnum.Encerrado,
        StatusChamadoEnum.Cancelado
    ];

    private static readonly IReadOnlyCollection<StatusChamadoEnum> StatusMudanca =
    [
        StatusChamadoEnum.Aberto,
        StatusChamadoEnum.EmAnalise,
        StatusChamadoEnum.AguardandoAprovacao,
        StatusChamadoEnum.Aprovada,
        StatusChamadoEnum.Reprovada,
        StatusChamadoEnum.EmExecucao,
        StatusChamadoEnum.Concluida,
        StatusChamadoEnum.Encerrado,
        StatusChamadoEnum.Cancelado
    ];

    private static readonly IReadOnlyCollection<StatusChamadoEnum> StatusProblema =
    [
        StatusChamadoEnum.Aberto,
        StatusChamadoEnum.EmAnalise,
        StatusChamadoEnum.CausaRaizIdentificada,
        StatusChamadoEnum.SolucaoDeContorno,
        StatusChamadoEnum.Resolvido,
        StatusChamadoEnum.Encerrado,
        StatusChamadoEnum.Cancelado
    ];

    private static readonly IReadOnlyCollection<StatusChamadoEnum> StatusEventoAlerta =
    [
        StatusChamadoEnum.Aberto,
        StatusChamadoEnum.EmAnalise,
        StatusChamadoEnum.Correlacionado,
        StatusChamadoEnum.Tratado,
        StatusChamadoEnum.Encerrado,
        StatusChamadoEnum.Cancelado
    ];

    private static readonly IReadOnlyCollection<StatusChamadoEnum> StatusTarefaOperacional =
    [
        StatusChamadoEnum.Aberto,
        StatusChamadoEnum.Planejada,
        StatusChamadoEnum.EmExecucao,
        StatusChamadoEnum.Concluida,
        StatusChamadoEnum.Encerrado,
        StatusChamadoEnum.Cancelado
    ];

    public IReadOnlyCollection<StatusChamadoEnum> ObterStatusPermitidos(NaturezaChamadoEnum natureza)
    {
        if (!Enum.IsDefined(natureza))
        {
            throw new ArgumentException("Natureza do chamado invalida.", nameof(natureza));
        }

        return natureza switch
        {
            NaturezaChamadoEnum.Incidente => StatusIncidenteRequisicao,
            NaturezaChamadoEnum.Requisicao => StatusIncidenteRequisicao,
            NaturezaChamadoEnum.Mudanca => StatusMudanca,
            NaturezaChamadoEnum.Problema => StatusProblema,
            NaturezaChamadoEnum.EventoAlerta => StatusEventoAlerta,
            NaturezaChamadoEnum.TarefaOperacional => StatusTarefaOperacional,
            _ => throw new ArgumentOutOfRangeException(nameof(natureza), natureza, "Natureza do chamado nao suportada.")
        };
    }

    public bool StatusEhPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status)
    {
        if (!Enum.IsDefined(status))
        {
            return false;
        }

        var statusPermitidos = ObterStatusPermitidos(natureza);
        return statusPermitidos.Contains(status);
    }

    public void ValidarStatusPermitido(NaturezaChamadoEnum natureza, StatusChamadoEnum status)
    {
        if (StatusEhPermitido(natureza, status))
        {
            return;
        }

        throw new InvalidOperationException(
            $"O status '{status}' nao e permitido para chamados da natureza '{natureza}'.");
    }
}
