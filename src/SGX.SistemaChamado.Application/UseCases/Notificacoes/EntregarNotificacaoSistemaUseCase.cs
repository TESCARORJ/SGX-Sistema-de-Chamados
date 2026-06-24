using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class EntregarNotificacaoSistemaUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IRepository<Usuario> usuarioRepository,
    INotificacaoProcessamentoRepository notificacaoProcessamentoRepository,
    IUnitOfWork unitOfWork) : IEntregarNotificacaoSistemaUseCase
{
    public async Task<EntregarNotificacaoSistemaResponse> ExecutarAsync(
        EntregarNotificacaoSistemaRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new EntregarNotificacaoSistemaRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var entregueEmUtc = request.EntregueEm.Kind == DateTimeKind.Utc
            ? request.EntregueEm
            : request.EntregueEm.ToUniversalTime();

        var notificacao = await notificacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.NotificacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");

        ValidarCanalSistema(notificacao);
        var destinatarioUsuarioId = notificacao.DestinatarioUsuarioId
            ?? throw new InvalidOperationException("A notificacao do canal Sistema exige destinatario interno.");

        if (notificacao.Status == StatusNotificacao.Enviada)
        {
            return CriarRespostaIdempotente(notificacao, destinatarioUsuarioId);
        }

        if (notificacao.Status != StatusNotificacao.EmProcessamento)
        {
            throw new InvalidOperationException("A notificacao do canal Sistema deve estar em processamento para ser entregue.");
        }

        if (string.IsNullOrWhiteSpace(notificacao.Conteudo))
        {
            throw new InvalidOperationException("A notificacao do canal Sistema exige conteudo materializado para entrega.");
        }

        var destinatario = await usuarioRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == destinatarioUsuarioId, cancellationToken);

        if (destinatario is null)
        {
            throw new InvalidOperationException("O destinatario interno da notificacao nao foi encontrado.");
        }

        if (!destinatario.Ativo
            || destinatario.Situacao != SituacaoUsuario.Ativo
            || (destinatario.BloqueadoAte.HasValue && destinatario.BloqueadoAte.Value > DateTime.UtcNow))
        {
            throw new InvalidOperationException("O destinatario interno da notificacao nao esta elegivel para entrega no canal Sistema.");
        }

        var concluida = await notificacaoProcessamentoRepository.TentarRegistrarSucessoAsync(
            request.NotificacaoId,
            entregueEmUtc,
            "entrega.sistema",
            null,
            cancellationToken);

        if (concluida)
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var enviada = await notificacaoRepository.Query()
                .AsNoTracking()
                .SingleAsync(x => x.Id == request.NotificacaoId, cancellationToken);

            return new EntregarNotificacaoSistemaResponse(
                enviada.Id,
                destinatarioUsuarioId,
                true,
                false,
                enviada.Status,
                enviada.EnviadaEm);
        }

        var concorrente = await notificacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.NotificacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("A notificacao informada nao foi encontrada.");

        ValidarCanalSistema(concorrente);

        if (concorrente.Status == StatusNotificacao.Enviada)
        {
            return CriarRespostaIdempotente(concorrente, destinatarioUsuarioId);
        }

        throw new InvalidOperationException("A notificacao do canal Sistema nao pode ser concluida no estado atual.");
    }

    private static void ValidarCanalSistema(Notificacao notificacao)
    {
        if (notificacao.Canal != CanalNotificacao.Sistema)
        {
            throw new InvalidOperationException("A entrega interna so pode ser executada para notificacoes do canal Sistema.");
        }
    }

    private static EntregarNotificacaoSistemaResponse CriarRespostaIdempotente(Notificacao notificacao, Guid destinatarioUsuarioId)
        => new(
            notificacao.Id,
            destinatarioUsuarioId,
            false,
            true,
            notificacao.Status,
            notificacao.EnviadaEm);
}
