using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaMonitoringService(
    IRepository<Chamado> chamadoRepository,
    IRepository<ConfiguracaoAlertaSla> configuracaoRepository,
    ISlaEventService slaEventService,
    IUnitOfWork unitOfWork,
    ILogger<SlaMonitoringService> logger) : ISlaMonitoringService
{
    public async Task ExecutarVerificacaoAsync(CancellationToken cancellationToken = default)
    {
        var configuracao = await configuracaoRepository.Query()
            .AsNoTracking()
            .OrderByDescending(x => x.Ativo)
            .ThenBy(x => x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);

        if (configuracao is null || !configuracao.Ativo)
        {
            return;
        }

        var agora = DateTime.UtcNow;
        var chamados = await chamadoRepository.Query()
            .Include(x => x.ChamadoSla)
            .Where(x => x.Ativo && x.ChamadoSla != null)
            .ToListAsync(cancellationToken);

        foreach (var chamado in chamados)
        {
            try
            {
                await VerificarChamadoAsync(chamado, configuracao, agora, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao verificar SLA do chamado {ChamadoId}.", chamado.Id);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task VerificarChamadoAsync(
        Chamado chamado,
        ConfiguracaoAlertaSla configuracao,
        DateTime agora,
        CancellationToken cancellationToken)
    {
        var sla = chamado.ChamadoSla;
        if (sla is null || sla.Pausado)
        {
            return;
        }

        if (!sla.DataPrimeiraResposta.HasValue)
        {
            if (agora > sla.PrazoPrimeiraResposta)
            {
                await RegistrarAlertaAsync(
                    sla,
                    TipoEventoSla.AlertaPrimeiraRespostaVencida,
                    "Alerta de primeira resposta vencida.",
                    "primeira-resposta-vencida",
                    agora,
                    cancellationToken);
            }
            else if (sla.PrazoPrimeiraResposta <= agora.AddMinutes(configuracao.MinutosAntesVencimentoPrimeiraResposta))
            {
                await RegistrarAlertaAsync(
                    sla,
                    TipoEventoSla.AlertaPrimeiraRespostaProximoVencimento,
                    "Alerta de primeira resposta proxima do vencimento.",
                    "primeira-resposta-proximo-vencimento",
                    agora,
                    cancellationToken);
            }
        }

        if (!sla.DataResolucao.HasValue)
        {
            if (agora > sla.PrazoResolucao)
            {
                await RegistrarAlertaAsync(
                    sla,
                    TipoEventoSla.AlertaResolucaoVencida,
                    "Alerta de resolucao vencida.",
                    "resolucao-vencida",
                    agora,
                    cancellationToken);
            }
            else if (sla.PrazoResolucao <= agora.AddMinutes(configuracao.MinutosAntesVencimentoResolucao))
            {
                await RegistrarAlertaAsync(
                    sla,
                    TipoEventoSla.AlertaResolucaoProximoVencimento,
                    "Alerta de resolucao proxima do vencimento.",
                    "resolucao-proximo-vencimento",
                    agora,
                    cancellationToken);
            }
        }
    }

    private Task RegistrarAlertaAsync(
        ChamadoSla sla,
        TipoEventoSla tipoEvento,
        string descricao,
        string marco,
        DateTime agora,
        CancellationToken cancellationToken)
    {
        return slaEventService.RegistrarAsync(
            sla,
            tipoEvento,
            descricao,
            agora,
            "monitoramento.sla",
            chaveIdempotencia: $"chamado-sla:{sla.Id}:{marco}",
            cancellationToken: cancellationToken);
    }
}
