using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ContarMinhasNotificacoesNaoLidasUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IContarMinhasNotificacoesNaoLidasUseCase
{
    public async Task<ContagemMinhasNotificacoesNaoLidasResponse> ExecutarAsync(
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var total = await notificacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => x.Canal == CanalNotificacao.Sistema)
            .Where(x => x.Status == StatusNotificacao.Enviada)
            .Where(x => x.DestinatarioUsuarioId == usuarioAtual.Id)
            .Where(x => !x.LidaEm.HasValue)
            .CountAsync(cancellationToken);

        return new ContagemMinhasNotificacoesNaoLidasResponse(total);
    }
}
