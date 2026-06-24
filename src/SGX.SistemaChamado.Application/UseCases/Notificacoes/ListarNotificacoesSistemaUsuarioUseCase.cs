using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ListarNotificacoesSistemaUsuarioUseCase(
    IRepository<Notificacao> notificacaoRepository) : IListarNotificacoesSistemaUsuarioUseCase
{
    public async Task<IReadOnlyCollection<NotificacaoSistemaResumoResponse>> ExecutarAsync(
        ListarNotificacoesSistemaUsuarioRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (request.UsuarioId == Guid.Empty)
        {
            throw new ValidationException("O usuario informado e obrigatorio.");
        }

        var notificacoes = await notificacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => x.Canal == CanalNotificacao.Sistema)
            .Where(x => x.Status == StatusNotificacao.Enviada)
            .Where(x => x.DestinatarioUsuarioId == request.UsuarioId)
            .OrderByDescending(x => x.EnviadaEm ?? x.CriadoEm)
            .ThenByDescending(x => x.CriadoEm)
            .ThenByDescending(x => x.Id)
            .Select(x => new NotificacaoSistemaResumoResponse(
                x.Id,
                x.DestinatarioUsuarioId!.Value,
                x.Assunto,
                x.Conteudo,
                x.ChaveIdempotencia,
                x.EnviadaEm,
                x.CriadoEm))
            .ToArrayAsync(cancellationToken);

        return notificacoes;
    }
}
