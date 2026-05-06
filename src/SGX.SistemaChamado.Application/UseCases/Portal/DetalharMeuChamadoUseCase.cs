using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class DetalharMeuChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IDetalharMeuChamadoUseCase
{
    public async Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Departamento)
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.SlaControle)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!PortalUseCaseHelpers.PodeAcessarChamado(usuarioAtual, chamado))
        {
            throw new UnauthorizedAccessException("Acesso negado ao chamado solicitado.");
        }

        return PortalUseCaseHelpers.MapDetalhe(chamado);
    }
}
