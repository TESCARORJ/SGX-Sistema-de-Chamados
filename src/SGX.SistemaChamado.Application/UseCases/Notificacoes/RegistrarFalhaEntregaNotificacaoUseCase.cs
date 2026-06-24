using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class RegistrarFalhaEntregaNotificacaoUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IRegistrarFalhaEntregaNotificacaoUseCase
{
    public async Task<RegistrarFalhaEntregaNotificacaoResponse> ExecutarAsync(
        RegistrarFalhaEntregaNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new RegistrarFalhaEntregaNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var notificacao = await notificacaoRepository.GetByIdAsync(request.NotificacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var falhouEmUtc = request.FalhouEm.Kind == DateTimeKind.Utc
            ? request.FalhouEm
            : request.FalhouEm.ToUniversalTime();

        notificacao.RegistrarFalha(request.Erro, falhouEmUtc, usuarioAtual.Login, usuarioAtual.Id);

        if (request.FalhaTransitoria
            && notificacao.QuantidadeTentativas < SelecionarNotificacoesProcessaveisUseCase.LimiteTentativasPadrao)
        {
            notificacao.ReagendarAposFalha(
                CalcularProximaTentativa(falhouEmUtc, notificacao.QuantidadeTentativas),
                usuarioAtual.Login,
                usuarioAtual.Id);
        }

        notificacaoRepository.Update(notificacao);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegistrarFalhaEntregaNotificacaoResponse(
            notificacao.Id,
            notificacao.Status,
            notificacao.QuantidadeTentativas,
            notificacao.AgendadaEm,
            notificacao.FalhouEm,
            notificacao.UltimoErro);
    }

    internal static DateTime CalcularProximaTentativa(DateTime falhouEmUtc, int quantidadeTentativas)
    {
        var atraso = quantidadeTentativas switch
        {
            <= 1 => TimeSpan.FromMinutes(1),
            2 => TimeSpan.FromMinutes(5),
            3 => TimeSpan.FromMinutes(15),
            4 => TimeSpan.FromMinutes(30),
            _ => TimeSpan.FromMinutes(60)
        };

        return falhouEmUtc.Add(atraso);
    }
}
