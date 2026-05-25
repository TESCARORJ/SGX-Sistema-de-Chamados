using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class ObterStatusAprovacaoChamadoPortalUseCase(
    IRepository<Chamado> chamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IObterStatusAprovacaoChamadoPortalUseCase
{
    public async Task<PortalStatusAprovacaoChamadoDto> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Aprovacoes)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!PortalUseCaseHelpers.PodeAcessarChamado(usuarioAtual, chamado))
        {
            throw new UnauthorizedAccessException("Acesso negado ao chamado solicitado.");
        }

        return PortalUseCaseHelpers.MapStatusAprovacao(chamado);
    }
}
