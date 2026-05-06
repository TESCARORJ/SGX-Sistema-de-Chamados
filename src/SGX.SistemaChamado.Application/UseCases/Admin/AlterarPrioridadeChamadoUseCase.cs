using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AlterarPrioridadeChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAlterarPrioridadeChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AlterarPrioridadeChamadoRequest request, CancellationToken cancellationToken = default)
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
            .Include(x => x.SlaControle)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var prioridade = await prioridadeRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.PrioridadeId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Prioridade informada nao encontrada ou inativa.");

        chamado.AlterarPrioridade(prioridade.Id, usuario.Login);
        await slaService.AplicarMudancaPrioridadeAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.PrioridadeAlterada,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.PrioridadeAlterada, $"Prioridade alterada para {prioridade.Nome}"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
