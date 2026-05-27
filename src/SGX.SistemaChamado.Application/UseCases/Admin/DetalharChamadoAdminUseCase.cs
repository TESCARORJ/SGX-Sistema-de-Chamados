using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class DetalharChamadoAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IFluxoStatusChamadoService fluxoStatusChamadoService,
    IAcoesChamadoService acoesChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IDetalharChamadoAdminUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var statusPermitidosCodigos = fluxoStatusChamadoService
            .ObterStatusPermitidos(chamado.NaturezaChamado)
            .Select(x => (int)x)
            .Distinct()
            .OrderBy(x => x)
            .ToArray();

        var acoesDisponiveis = acoesChamadoService.ObterAcoesDisponiveis(chamado, usuario);

        return AdminUseCaseHelpers.MapDetalhe(chamado, statusPermitidosCodigos, acoesDisponiveis);
    }
}
