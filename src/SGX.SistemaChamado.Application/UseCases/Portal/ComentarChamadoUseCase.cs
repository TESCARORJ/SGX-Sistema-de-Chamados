using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class ComentarChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IComentarChamadoUseCase
{
    public async Task<ComentarioChamadoResponse> ExecutarAsync(Guid chamadoId, ComentarioChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var chamado = await chamadoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!PortalUseCaseHelpers.PodeAcessarChamado(usuarioAtual, chamado))
        {
            throw new UnauthorizedAccessException("Acesso negado ao chamado solicitado.");
        }

        var comentario = new ComentarioChamado(
            chamadoId,
            usuarioAtual.Id,
            request.Mensagem,
            false,
            usuarioAtual.Login);

        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamadoId,
            TipoHistoricoChamado.ComentarioAdicionado,
            PortalUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ComentarioAdicionado),
            usuarioAtual.Id,
            usuarioAtual.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);

        chamado.AtualizarAuditoria(usuarioAtual.Login);
        chamadoRepository.Update(chamado);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ComentarioChamadoResponse(
            comentario.Id,
            comentario.UsuarioId,
            usuarioAtual.Nome,
            comentario.Mensagem,
            comentario.CriadoEm);
    }
}
