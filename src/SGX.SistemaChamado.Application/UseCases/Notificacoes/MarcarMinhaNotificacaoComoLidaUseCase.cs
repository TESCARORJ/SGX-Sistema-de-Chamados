using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class MarcarMinhaNotificacaoComoLidaUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IMarcarMinhaNotificacaoComoLidaUseCase
{
    public async Task<AlterarLeituraNotificacaoResponse> ExecutarAsync(
        Guid notificacaoId,
        CancellationToken cancellationToken = default)
    {
        if (notificacaoId == Guid.Empty)
        {
            throw new KeyNotFoundException("Notificacao nao encontrada.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var notificacao = await notificacaoRepository.Query()
            .Where(x => x.Id == notificacaoId)
            .Where(x => x.Ativo)
            .Where(x => x.Canal == CanalNotificacao.Sistema)
            .Where(x => x.Status == StatusNotificacao.Enviada)
            .Where(x => x.DestinatarioUsuarioId == usuarioAtual.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (notificacao is null)
        {
            throw new KeyNotFoundException("Notificacao nao encontrada.");
        }

        if (notificacao.Lida)
        {
            return new AlterarLeituraNotificacaoResponse(notificacao.Id, true, notificacao.LidaEm, false);
        }

        notificacao.MarcarComoLida(DateTime.UtcNow, usuarioAtual.Login, usuarioAtual.Id);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new AlterarLeituraNotificacaoResponse(notificacao.Id, true, notificacao.LidaEm, true);
    }
}
