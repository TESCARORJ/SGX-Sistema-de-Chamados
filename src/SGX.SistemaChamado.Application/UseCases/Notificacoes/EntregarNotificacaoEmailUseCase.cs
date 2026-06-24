using System.Net.Mail;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class EntregarNotificacaoEmailUseCase(
    IRepository<Notificacao> notificacaoRepository,
    ITransportadorEmailNotificacao transportadorEmailNotificacao,
    IRegistrarSucessoEntregaNotificacaoUseCase registrarSucessoEntregaNotificacaoUseCase,
    IRegistrarFalhaEntregaNotificacaoUseCase registrarFalhaEntregaNotificacaoUseCase,
    ILogger<EntregarNotificacaoEmailUseCase> logger) : IEntregarNotificacaoEmailUseCase
{
    public async Task<EntregarNotificacaoEmailResponse> ExecutarAsync(
        EntregarNotificacaoEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new EntregarNotificacaoEmailRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var notificacao = await notificacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.NotificacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");

        ValidarCanalEmail(notificacao);

        if (notificacao.Status == StatusNotificacao.Enviada)
        {
            return CriarRespostaIdempotente(notificacao);
        }

        if (notificacao.Status != StatusNotificacao.EmProcessamento)
        {
            throw new InvalidOperationException("A notificacao do canal Email deve estar em processamento para ser entregue.");
        }

        var entregueEmUtc = request.EntregueEm;
        var erroValidacaoPayload = ObterErroValidacaoPayload(notificacao);
        if (erroValidacaoPayload is not null)
        {
            return await RegistrarFalhaAsync(
                notificacao,
                erroValidacaoPayload,
                falhaTransitoria: false,
                entregueEmUtc,
                cancellationToken);
        }

        ResultadoTransporteEmailNotificacao resultadoTransporte;
        try
        {
            resultadoTransporte = await transportadorEmailNotificacao.EnviarAsync(
                new MensagemEmailNotificacao(
                    notificacao.DestinatarioEndereco!,
                    notificacao.Assunto!,
                    notificacao.Conteudo,
                    ConteudoHtmlPadrao,
                    notificacao.ChaveCorrelacao),
                cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }

        if (resultadoTransporte.Sucesso)
        {
            logger.LogInformation(
                "Entrega de notificacao por e-mail concluida. NotificacaoId={NotificacaoId} Destinatario={DestinatarioMascarado}",
                notificacao.Id,
                MascararEndereco(notificacao.DestinatarioEndereco!));

            return await RegistrarSucessoAsync(notificacao, entregueEmUtc, cancellationToken);
        }

        logger.LogWarning(
            "Entrega de notificacao por e-mail falhou. NotificacaoId={NotificacaoId} FalhaTransitoria={FalhaTransitoria} Destinatario={DestinatarioMascarado}",
            notificacao.Id,
            resultadoTransporte.FalhaTransitoria,
            MascararEndereco(notificacao.DestinatarioEndereco!));

        return await RegistrarFalhaAsync(
            notificacao,
            resultadoTransporte.Erro ?? "Falha no transporte de e-mail.",
            resultadoTransporte.FalhaTransitoria,
            entregueEmUtc,
            cancellationToken);
    }

    private async Task<EntregarNotificacaoEmailResponse> RegistrarSucessoAsync(
        Notificacao notificacao,
        DateTime entregueEmUtc,
        CancellationToken cancellationToken)
    {
        try
        {
            await registrarSucessoEntregaNotificacaoUseCase.ExecutarAsync(
                new RegistrarSucessoEntregaNotificacaoRequest(notificacao.Id, entregueEmUtc),
                cancellationToken);
        }
        catch (InvalidOperationException)
        {
            var concorrente = await RecarregarNotificacaoAsync(notificacao.Id, cancellationToken);
            if (concorrente.Status == StatusNotificacao.Enviada)
            {
                return CriarRespostaIdempotente(concorrente);
            }

            throw;
        }

        var enviada = await RecarregarNotificacaoAsync(notificacao.Id, cancellationToken);
        return new EntregarNotificacaoEmailResponse(
            enviada.Id,
            enviada.DestinatarioEndereco ?? string.Empty,
            true,
            false,
            false,
            enviada.Status,
            enviada.QuantidadeTentativas,
            enviada.EnviadaEm,
            null,
            null);
    }

    private async Task<EntregarNotificacaoEmailResponse> RegistrarFalhaAsync(
        Notificacao notificacao,
        string erro,
        bool falhaTransitoria,
        DateTime falhouEmUtc,
        CancellationToken cancellationToken)
    {
        try
        {
            var falha = await registrarFalhaEntregaNotificacaoUseCase.ExecutarAsync(
                new RegistrarFalhaEntregaNotificacaoRequest(
                    notificacao.Id,
                    erro,
                    falhaTransitoria,
                    falhouEmUtc),
                cancellationToken);

            return new EntregarNotificacaoEmailResponse(
                notificacao.Id,
                notificacao.DestinatarioEndereco ?? string.Empty,
                false,
                false,
                falha.Status == StatusNotificacao.Agendada,
                falha.Status,
                falha.QuantidadeTentativas,
                null,
                falha.AgendadaEm,
                falha.UltimoErro);
        }
        catch (InvalidOperationException)
        {
            var concorrente = await RecarregarNotificacaoAsync(notificacao.Id, cancellationToken);
            if (concorrente.Status == StatusNotificacao.Enviada)
            {
                return CriarRespostaIdempotente(concorrente);
            }

            if (concorrente.Status is StatusNotificacao.Agendada or StatusNotificacao.Falhou)
            {
                return new EntregarNotificacaoEmailResponse(
                    concorrente.Id,
                    concorrente.DestinatarioEndereco ?? string.Empty,
                    false,
                    false,
                    concorrente.Status == StatusNotificacao.Agendada,
                    concorrente.Status,
                    concorrente.QuantidadeTentativas,
                    concorrente.EnviadaEm,
                    concorrente.AgendadaEm,
                    concorrente.UltimoErro);
            }

            throw;
        }
    }

    private async Task<Notificacao> RecarregarNotificacaoAsync(Guid notificacaoId, CancellationToken cancellationToken)
        => await notificacaoRepository.Query()
            .AsNoTracking()
            .SingleAsync(x => x.Id == notificacaoId, cancellationToken);

    private static void ValidarCanalEmail(Notificacao notificacao)
    {
        if (notificacao.Canal != CanalNotificacao.Email)
        {
            throw new InvalidOperationException("A entrega outbound so pode ser executada para notificacoes do canal Email.");
        }
    }

    private static string? ObterErroValidacaoPayload(Notificacao notificacao)
    {
        if (string.IsNullOrWhiteSpace(notificacao.DestinatarioEndereco))
        {
            return "A notificacao do canal Email exige destinatario com endereco persistido.";
        }

        if (!EhEnderecoValido(notificacao.DestinatarioEndereco))
        {
            return "A notificacao do canal Email possui destinatario com endereco invalido.";
        }

        if (string.IsNullOrWhiteSpace(notificacao.Assunto))
        {
            return "A notificacao do canal Email exige assunto materializado para entrega.";
        }

        if (string.IsNullOrWhiteSpace(notificacao.Conteudo))
        {
            return "A notificacao do canal Email exige conteudo materializado para entrega.";
        }

        return null;
    }

    private static bool EhEnderecoValido(string endereco)
    {
        try
        {
            _ = new MailAddress(endereco);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static EntregarNotificacaoEmailResponse CriarRespostaIdempotente(Notificacao notificacao)
        => new(
            notificacao.Id,
            notificacao.DestinatarioEndereco ?? string.Empty,
            false,
            true,
            false,
            notificacao.Status,
            notificacao.QuantidadeTentativas,
            notificacao.EnviadaEm,
            null,
            null);

    private static string MascararEndereco(string endereco)
    {
        var partes = endereco.Split('@', 2, StringSplitOptions.TrimEntries);
        if (partes.Length != 2 || partes[0].Length <= 2)
        {
            return "***";
        }

        return $"{partes[0][..2]}***@{partes[1]}";
    }

    private const bool ConteudoHtmlPadrao = true;
}
