using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Services.Sla;

public sealed class SlaService(
    ISlaCalculator slaCalculator,
    IRepository<ChamadoSla> chamadoSlaRepository,
    IRepository<CalendarioCorporativo> calendarioRepository,
    ISlaBusinessTimeCalculator businessTimeCalculator,
    ISlaEventService slaEventService) : ISlaService
{
    public async Task InicializarNaAberturaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
        => await InicializarNaAberturaAsync(chamado, usuarioLogin, agoraUtc, null, cancellationToken);

    public async Task InicializarNaAberturaAsync(
        Chamado chamado,
        string usuarioLogin,
        DateTime agoraUtc,
        Guid? politicaSlaIdPreferencial,
        CancellationToken cancellationToken = default)
    {
        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, politicaSlaIdPreferencial, cancellationToken);
        if (prazos is null)
        {
            return;
        }

        var prazoPrimeiraResposta = CalcularPrazo(agoraUtc, prazos.PrazoPrimeiraRespostaMinutos, prazos);
        var prazoResolucao = CalcularPrazo(agoraUtc, prazos.PrazoResolucaoMinutos, prazos);

        if (chamado.ChamadoSla is null)
        {
            var registro = new ChamadoSla(
                chamado.Id,
                prazos.PoliticaSlaId,
                chamado.PrioridadeId,
                agoraUtc,
                prazoPrimeiraResposta,
                prazoResolucao,
                prazos.PausarQuandoAguardandoSolicitante,
                prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null,
                prazos.CalendarioCorporativoId,
                usuarioLogin);

            await chamadoSlaRepository.AddAsync(registro, cancellationToken);
            await slaEventService.RegistrarAsync(
                registro,
                TipoEventoSla.SlaAplicado,
                "SLA aplicado ao chamado.",
                agoraUtc,
                usuarioLogin,
                chaveIdempotencia: $"chamado-sla:{registro.Id}:sla-aplicado",
                cancellationToken: cancellationToken);
            return;
        }

        chamado.ChamadoSla.AtualizarPrazos(
            prazos.PoliticaSlaId,
            chamado.PrioridadeId,
            agoraUtc,
            prazoPrimeiraResposta,
            prazoResolucao,
            prazos.PausarQuandoAguardandoSolicitante,
            prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null,
            prazos.CalendarioCorporativoId,
            usuarioLogin);
    }

    public async Task RegistrarPrimeiraRespostaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.ChamadoSla is null || chamado.EncerradoEm.HasValue)
        {
            return;
        }

        var registrarEvento = !chamado.ChamadoSla.DataPrimeiraResposta.HasValue;
        var minutos = await CalcularMinutosDecorridosAsync(chamado.ChamadoSla, agoraUtc, cancellationToken);
        chamado.ChamadoSla.RegistrarPrimeiraResposta(agoraUtc, minutos, usuarioLogin);

        if (!registrarEvento)
        {
            return;
        }

        var tipo = chamado.ChamadoSla.PrimeiraRespostaViolada
            ? TipoEventoSla.PrimeiraRespostaVencida
            : TipoEventoSla.PrimeiraRespostaDentroDoPrazo;

        await slaEventService.RegistrarAsync(
            chamado.ChamadoSla,
            tipo,
            chamado.ChamadoSla.PrimeiraRespostaViolada
                ? "Primeira resposta registrada fora do prazo de SLA."
                : "Primeira resposta registrada dentro do prazo de SLA.",
            agoraUtc,
            usuarioLogin,
            chaveIdempotencia: $"chamado-sla:{chamado.ChamadoSla.Id}:primeira-resposta-registrada",
            cancellationToken: cancellationToken);
    }

    public async Task AplicarMudancaPrioridadeAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.ChamadoSla is null || chamado.EncerradoEm.HasValue || chamado.ChamadoSla.DataResolucao.HasValue)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        if (prazos is null)
        {
            return;
        }

        chamado.ChamadoSla.AtualizarPrazos(
            prazos.PoliticaSlaId,
            chamado.PrioridadeId,
            agoraUtc,
            CalcularPrazo(agoraUtc, prazos.PrazoPrimeiraRespostaMinutos, prazos),
            CalcularPrazo(agoraUtc, prazos.PrazoResolucaoMinutos, prazos),
            prazos.PausarQuandoAguardandoSolicitante,
            prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null,
            prazos.CalendarioCorporativoId,
            usuarioLogin);
    }

    public async Task AplicarMudancaCategoriaAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.ChamadoSla is null || chamado.EncerradoEm.HasValue || chamado.ChamadoSla.DataResolucao.HasValue)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        if (prazos is null)
        {
            return;
        }

        chamado.ChamadoSla.AtualizarPrazos(
            prazos.PoliticaSlaId,
            chamado.PrioridadeId,
            agoraUtc,
            CalcularPrazo(agoraUtc, prazos.PrazoPrimeiraRespostaMinutos, prazos),
            CalcularPrazo(agoraUtc, prazos.PrazoResolucaoMinutos, prazos),
            prazos.PausarQuandoAguardandoSolicitante,
            prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null,
            prazos.CalendarioCorporativoId,
            usuarioLogin);
    }

    public async Task AplicarMudancaStatusAsync(Chamado chamado, StatusChamado statusAnterior, StatusChamado statusAtual, string usuarioLogin, DateTime agoraUtc)
    {
        if (chamado.ChamadoSla is null)
        {
            return;
        }

        if (chamado.ChamadoSla.PausarQuandoAguardandoSolicitante)
        {
            if (statusAnterior.Codigo != StatusChamadoEnum.AguardandoSolicitante && statusAtual.Codigo == StatusChamadoEnum.AguardandoSolicitante)
            {
                chamado.ChamadoSla.IniciarPausa(agoraUtc, usuarioLogin);
                await slaEventService.RegistrarAsync(
                    chamado.ChamadoSla,
                    TipoEventoSla.SlaPausado,
                    "SLA pausado porque o chamado entrou em Aguardando solicitante.",
                    agoraUtc,
                    usuarioLogin,
                    chaveIdempotencia: $"chamado-sla:{chamado.ChamadoSla.Id}:sla-pausado:{agoraUtc:yyyyMMddHHmm}",
                    cancellationToken: CancellationToken.None);
            }
            else if (statusAnterior.Codigo == StatusChamadoEnum.AguardandoSolicitante && statusAtual.Codigo != StatusChamadoEnum.AguardandoSolicitante)
            {
                var minutosPausa = await CalcularMinutosPausaAsync(chamado.ChamadoSla, agoraUtc, CancellationToken.None);
                chamado.ChamadoSla.FinalizarPausa(agoraUtc, minutosPausa, usuarioLogin);
                await slaEventService.RegistrarAsync(
                    chamado.ChamadoSla,
                    TipoEventoSla.SlaRetomado,
                    "SLA retomado porque o chamado saiu de Aguardando solicitante.",
                    agoraUtc,
                    usuarioLogin,
                    chaveIdempotencia: $"chamado-sla:{chamado.ChamadoSla.Id}:sla-retomado:{agoraUtc:yyyyMMddHHmm}",
                    cancellationToken: CancellationToken.None);
            }
        }

        if (statusAtual.Codigo == StatusChamadoEnum.EmAtendimento)
        {
            await RegistrarPrimeiraRespostaAsync(chamado, usuarioLogin, agoraUtc);
        }

        if (statusAtual.EhStatusFinal || statusAtual.Codigo is StatusChamadoEnum.Resolvido or StatusChamadoEnum.Encerrado)
        {
            await RegistrarEncerramentoAsync(chamado, usuarioLogin, agoraUtc);
        }
    }

    public async Task RegistrarEncerramentoAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc)
    {
        if (chamado.ChamadoSla is null)
        {
            return;
        }

        var registrarEvento = !chamado.ChamadoSla.DataResolucao.HasValue;
        var minutos = await CalcularMinutosDecorridosAsync(chamado.ChamadoSla, agoraUtc, CancellationToken.None);
        chamado.ChamadoSla.RegistrarResolucao(agoraUtc, minutos, usuarioLogin);

        if (!registrarEvento)
        {
            return;
        }

        var tipo = chamado.ChamadoSla.ResolucaoViolada
            ? TipoEventoSla.ResolucaoVencida
            : TipoEventoSla.ResolucaoDentroDoPrazo;

        await slaEventService.RegistrarAsync(
            chamado.ChamadoSla,
            tipo,
            chamado.ChamadoSla.ResolucaoViolada
                ? "Resolucao registrada fora do prazo de SLA."
                : "Resolucao registrada dentro do prazo de SLA.",
            agoraUtc,
            usuarioLogin,
            chaveIdempotencia: $"chamado-sla:{chamado.ChamadoSla.Id}:resolucao-registrada");
    }

    public async Task ReabrirAsync(Chamado chamado, string usuarioLogin, DateTime agoraUtc, CancellationToken cancellationToken = default)
    {
        if (chamado.ChamadoSla is null)
        {
            return;
        }

        var prazos = await slaCalculator.CalcularPrazosAsync(chamado.PrioridadeId, chamado.CategoriaId, chamado.DepartamentoId, cancellationToken);
        if (prazos is null)
        {
            return;
        }

        chamado.ChamadoSla.Reabrir(
            prazos.PoliticaSlaId,
            chamado.PrioridadeId,
            agoraUtc,
            CalcularPrazo(agoraUtc, prazos.PrazoResolucaoMinutos, prazos),
            prazos.PausarQuandoAguardandoSolicitante,
            prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null,
            prazos.CalendarioCorporativoId,
            usuarioLogin);
    }

    public bool EstaProximoDoVencimento(ChamadoSla? chamadoSla, DateTime agoraUtc)
        => SlaRules.EstaProximoDoVencimento(chamadoSla, agoraUtc);

    private DateTime CalcularPrazo(DateTime dataBaseUtc, int minutos, SlaPrazosAplicados prazos)
    {
        if (prazos.UsarHorarioComercial && prazos.CalendarioCorporativo is not null)
        {
            return businessTimeCalculator
                .AddBusinessMinutes(ToUtcOffset(dataBaseUtc), minutos, prazos.CalendarioCorporativo)
                .UtcDateTime;
        }

        return dataBaseUtc.AddMinutes(minutos);
    }

    private async Task<int> CalcularMinutosDecorridosAsync(ChamadoSla chamadoSla, DateTime referenciaUtc, CancellationToken cancellationToken)
    {
        if (chamadoSla.UsarHorarioComercial && chamadoSla.CalendarioCorporativoId.HasValue)
        {
            var calendario = await CarregarCalendarioAsync(chamadoSla.CalendarioCorporativoId.Value, cancellationToken);
            if (calendario is not null)
            {
                var minutos = businessTimeCalculator.CountBusinessMinutes(
                    ToUtcOffset(chamadoSla.DataInicio),
                    ToUtcOffset(referenciaUtc),
                    calendario);

                return Math.Max(0, minutos - chamadoSla.MinutosPausados);
            }
        }

        var corridos = (int)Math.Round((referenciaUtc - chamadoSla.DataInicio).TotalMinutes);
        return Math.Max(0, corridos - chamadoSla.MinutosPausados);
    }

    private async Task<int> CalcularMinutosPausaAsync(ChamadoSla chamadoSla, DateTime retomadoEmUtc, CancellationToken cancellationToken)
    {
        if (chamadoSla.DataPausa is null)
        {
            return 0;
        }

        if (chamadoSla.UsarHorarioComercial && chamadoSla.CalendarioCorporativoId.HasValue)
        {
            var calendario = await CarregarCalendarioAsync(chamadoSla.CalendarioCorporativoId.Value, cancellationToken);
            if (calendario is not null)
            {
                return businessTimeCalculator.CountBusinessMinutes(
                    ToUtcOffset(chamadoSla.DataPausa.Value),
                    ToUtcOffset(retomadoEmUtc),
                    calendario);
            }
        }

        return (int)Math.Max(0, Math.Round((retomadoEmUtc - chamadoSla.DataPausa.Value).TotalMinutes));
    }

    private Task<CalendarioCorporativo?> CarregarCalendarioAsync(Guid calendarioId, CancellationToken cancellationToken)
        => calendarioRepository.Query()
            .Include(x => x.HorariosAtendimento)
            .Include(x => x.Excecoes)
            .FirstOrDefaultAsync(x => x.Id == calendarioId && x.Ativo, cancellationToken);

    private static DateTimeOffset ToUtcOffset(DateTime dataUtc)
    {
        var utc = dataUtc.Kind == DateTimeKind.Utc
            ? dataUtc
            : DateTime.SpecifyKind(dataUtc, DateTimeKind.Utc);

        return new DateTimeOffset(utc, TimeSpan.Zero);
    }
}
