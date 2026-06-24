using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class RegistrarSucessoEntregaNotificacaoUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IRegistrarSucessoEntregaNotificacaoUseCase
{
    public async Task ExecutarAsync(
        RegistrarSucessoEntregaNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new RegistrarSucessoEntregaNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var notificacao = await notificacaoRepository.GetByIdAsync(request.NotificacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        notificacao.RegistrarEnvio(request.EnviadaEm, usuarioAtual.Login, usuarioAtual.Id);

        notificacaoRepository.Update(notificacao);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
