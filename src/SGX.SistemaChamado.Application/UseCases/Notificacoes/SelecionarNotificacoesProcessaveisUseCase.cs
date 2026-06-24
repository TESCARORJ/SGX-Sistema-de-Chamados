using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class SelecionarNotificacoesProcessaveisUseCase(
    IRepository<Domain.Entities.Notificacao> notificacaoRepository) : ISelecionarNotificacoesProcessaveisUseCase
{
    internal const int LimiteTentativasPadrao = 5;

    public async Task<IReadOnlyCollection<NotificacaoProcessavelResponse>> ExecutarAsync(
        SelecionarNotificacoesProcessaveisRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new SelecionarNotificacoesProcessaveisRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var dataReferenciaUtc = request.DataReferencia.Kind == DateTimeKind.Utc
            ? request.DataReferencia
            : request.DataReferencia.ToUniversalTime();

        var notificacoes = await notificacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => x.QuantidadeTentativas < LimiteTentativasPadrao)
            .Where(x =>
                x.Status == StatusNotificacao.Pendente
                || (x.Status == StatusNotificacao.Agendada
                    && x.AgendadaEm.HasValue
                    && x.AgendadaEm.Value <= dataReferenciaUtc))
            .OrderBy(x => x.AgendadaEm ?? x.CriadoEm)
            .ThenBy(x => x.CriadoEm)
            .ThenBy(x => x.Id)
            .Take(request.Limite)
            .Select(x => new NotificacaoProcessavelResponse(
                x.Id,
                x.Canal,
                x.QuantidadeTentativas,
                x.AgendadaEm))
            .ToArrayAsync(cancellationToken);

        return notificacoes;
    }
}
