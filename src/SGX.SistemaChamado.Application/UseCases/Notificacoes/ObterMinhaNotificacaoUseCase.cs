using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ObterMinhaNotificacaoUseCase(
    IRepository<Notificacao> notificacaoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterMinhaNotificacaoUseCase
{
    public async Task<MinhaNotificacaoDetalheResponse> ExecutarAsync(
        Guid notificacaoId,
        CancellationToken cancellationToken = default)
    {
        if (notificacaoId == Guid.Empty)
        {
            throw new KeyNotFoundException("Notificacao nao encontrada.");
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var notificacao = await notificacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == notificacaoId)
            .Where(x => x.Ativo)
            .Where(x => x.Canal == CanalNotificacao.Sistema)
            .Where(x => x.Status == StatusNotificacao.Enviada)
            .Where(x => x.DestinatarioUsuarioId == usuarioAtual.Id)
            .Select(x => new MinhaNotificacaoDetalheResponse(
                x.Id,
                x.TipoEvento,
                x.Assunto,
                x.Conteudo,
                x.EnviadaEm!.Value,
                x.LidaEm.HasValue,
                x.LidaEm,
                x.ChamadoId,
                x.ChaveCorrelacao))
            .SingleOrDefaultAsync(cancellationToken);

        return notificacao ?? throw new KeyNotFoundException("Notificacao nao encontrada.");
    }
}
