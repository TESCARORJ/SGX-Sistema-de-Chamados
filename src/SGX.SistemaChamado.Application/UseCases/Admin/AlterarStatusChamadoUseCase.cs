using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AlterarStatusChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAlterarStatusChamadoUseCase
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
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Responsavel)
            .Include(x => x.Aprovacoes)
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var novoStatus = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StatusId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Status informado nao encontrado ou inativo.");

        if (novoStatus.Codigo is StatusChamadoEnum.EmAtendimento or StatusChamadoEnum.Resolvido or StatusChamadoEnum.Encerrado)
        {
            var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
            if (estadoAprovacao.BloqueiaAvancoAtendimento)
            {
                throw new InvalidOperationException(estadoAprovacao.MensagemBloqueio ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
            }
        }

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

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new { Status = statusAnterior.Nome },
                new { Status = novoStatus.Nome });

            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Status do chamado alterado.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "AlteracaoStatus",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {atualizado.Status}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
