using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AlterarStatusChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAlterarStatusChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarStatusChamadoRequest request, CancellationToken cancellationToken = default)
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

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Status)
            .Include(x => x.SlaControle)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var novoStatus = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StatusId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Status informado nao encontrado ou inativo.");

        var statusAnterior = chamado.Status;
        chamado.AlterarStatus(novoStatus.Id, usuario.Login);
        await slaService.AplicarMudancaStatusAsync(chamado, statusAnterior, novoStatus, usuario.Login, DateTime.UtcNow);
        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.StatusAlterado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.StatusAlterado, $"Status alterado para {novoStatus.Nome}"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
