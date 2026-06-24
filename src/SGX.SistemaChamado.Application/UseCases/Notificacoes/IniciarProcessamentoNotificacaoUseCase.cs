using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class IniciarProcessamentoNotificacaoUseCase(
    IRepository<Notificacao> notificacaoRepository,
    INotificacaoProcessamentoRepository notificacaoProcessamentoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IIniciarProcessamentoNotificacaoUseCase
{
    public async Task<IniciarProcessamentoNotificacaoResponse> ExecutarAsync(
        IniciarProcessamentoNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new IniciarProcessamentoNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var iniciadaEmUtc = request.IniciadaEm.Kind == DateTimeKind.Utc
            ? request.IniciadaEm
            : request.IniciadaEm.ToUniversalTime();

        var iniciado = await notificacaoProcessamentoRepository.TentarIniciarProcessamentoAsync(
            request.NotificacaoId,
            iniciadaEmUtc,
            usuarioAtual.Login,
            usuarioAtual.Id,
            SelecionarNotificacoesProcessaveisUseCase.LimiteTentativasPadrao,
            cancellationToken);

        if (!iniciado)
        {
            var existente = await notificacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.NotificacaoId, cancellationToken);

            if (existente is null)
            {
                throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");
            }

            throw new InvalidOperationException("A notificacao nao esta elegivel para iniciar processamento.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizada = await notificacaoRepository.Query()
            .AsNoTracking()
            .SingleAsync(x => x.Id == request.NotificacaoId, cancellationToken);

        return new IniciarProcessamentoNotificacaoResponse(
            atualizada.Id,
            atualizada.Status,
            atualizada.QuantidadeTentativas,
            atualizada.ProcessadaEm);
    }
}
