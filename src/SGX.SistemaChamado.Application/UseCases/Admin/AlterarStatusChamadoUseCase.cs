using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
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
    IFluxoStatusChamadoService fluxoStatusChamadoService,
    IAcoesChamadoService acoesChamadoService,
    IAdminRelacionamentosChamadoUseCases relacionamentosChamadoUseCases,
    IAdminChamadoAprovacoesUseCases chamadoAprovacoesUseCases,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null,
    IProcessarEventoCandidatoNotificacaoUseCase? processarEventoCandidatoNotificacaoUseCase = null,
    ILogger<AlterarStatusChamadoUseCase>? logger = null) : IAlterarStatusChamadoUseCase
{
    private const string MensagemBloqueioDependenciaAtiva =
        "Este chamado possui dependencia ativa e nao pode ser fechado enquanto estiver bloqueado por outro chamado.";
    private const string MensagemBloqueioAprovacaoPendente =
        AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente;

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

        acoesChamadoService.ValidarAcaoDisponivel(chamado, AcaoChamadoEnum.AlterarStatus, usuario);

        var novoStatus = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.StatusId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Status informado nao encontrado ou inativo.");

        fluxoStatusChamadoService.ValidarStatusPermitido(chamado.NaturezaChamado, novoStatus.Codigo);

        await GarantirAlteracaoFinalSemAprovacaoPendenteBloqueanteAsync(chamado.Id, novoStatus, cancellationToken);
        await GarantirAlteracaoFinalSemDependenciaAtivaAsync(chamado.Id, novoStatus, cancellationToken);

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

        await TentarIntegrarNotificacaoAsync(chamado, novoStatus, usuario, historico, cancellationToken);

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

    private async Task TentarIntegrarNotificacaoAsync(
        Chamado chamado,
        StatusChamado novoStatus,
        UsuarioContextoAplicacao usuarioAtual,
        HistoricoChamado historico,
        CancellationToken cancellationToken)
    {
        if (processarEventoCandidatoNotificacaoUseCase is null || !DeveNotificarStatus(novoStatus.Codigo))
        {
            return;
        }

        try
        {
            await processarEventoCandidatoNotificacaoUseCase.ExecutarAsync(
                new ProcessarEventoCandidatoNotificacaoRequest(
                    $"status-alterado:{historico.Id}",
                    new EventoCandidatoNotificacao(
                        TipoEventoNotificacao.EventoChamado,
                        chamado.Id,
                        usuarioAtual.Id,
                        historico.CriadoEm,
                        $"chamado:{chamado.Id}",
                        $"status-alterado:{historico.Id}",
                        new Dictionary<string, string>
                        {
                            ["evento"] = "status-alterado"
                        }),
                    new Dictionary<string, string>
                    {
                        ["chamado.codigo"] = chamado.Codigo,
                        ["chamado.titulo"] = chamado.Titulo,
                        ["chamado.status"] = novoStatus.Nome,
                        ["evento.nome"] = "Status alterado",
                        ["evento.descricao"] = historico.Descricao,
                        ["evento.ocorrido_em"] = historico.CriadoEm.ToString("O"),
                        ["status.nome"] = novoStatus.Nome,
                        ["status.codigo"] = novoStatus.Codigo.ToString()
                    },
                    [TipoParticipacaoDestinatarioNotificacao.Solicitante],
                    [CanalNotificacao.Sistema, CanalNotificacao.Email]),
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(
                ex,
                "Falha ao integrar notificacao de status. ChamadoId={ChamadoId} HistoricoId={HistoricoId} Status={Status}",
                chamado.Id,
                historico.Id,
                novoStatus.Codigo);
        }
    }

    private async Task GarantirAlteracaoFinalSemDependenciaAtivaAsync(
        Guid chamadoId,
        StatusChamado novoStatus,
        CancellationToken cancellationToken)
    {
        if (!StatusRepresentaFechamentoOperacional(novoStatus))
        {
            return;
        }

        if (await relacionamentosChamadoUseCases.EstaBloqueadoPorDependenciaAsync(chamadoId, cancellationToken))
        {
            throw new InvalidOperationException(MensagemBloqueioDependenciaAtiva);
        }
    }

    private async Task GarantirAlteracaoFinalSemAprovacaoPendenteBloqueanteAsync(
        Guid chamadoId,
        StatusChamado novoStatus,
        CancellationToken cancellationToken)
    {
        if (!StatusRepresentaFechamentoOperacional(novoStatus))
        {
            return;
        }

        if (validarBloqueioMovimentacaoUseCase is not null)
        {
            var avaliacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(
                new()
                {
                    ChamadoId = chamadoId,
                    TipoAcao = TipoAcaoMovimentacaoChamado.AlterarStatus,
                    StatusDestinoId = novoStatus.Id
                },
                cancellationToken);

            if (avaliacao.Bloqueado)
            {
                throw new InvalidOperationException(avaliacao.MensagemUsuario ?? MensagemBloqueioAprovacaoPendente);
            }

            return;
        }

        if (await chamadoAprovacoesUseCases.PossuiAprovacaoPendenteBloqueanteAsync(chamadoId, cancellationToken))
        {
            throw new InvalidOperationException(MensagemBloqueioAprovacaoPendente);
        }
    }

    private static bool StatusRepresentaFechamentoOperacional(StatusChamado status)
        => status.EhStatusFinal ||
           status.Codigo is StatusChamadoEnum.Resolvido
               or StatusChamadoEnum.Encerrado
               or StatusChamadoEnum.Cancelado
               or StatusChamadoEnum.Concluida;

    private static bool DeveNotificarStatus(StatusChamadoEnum status)
        => status is StatusChamadoEnum.EmAtendimento
            or StatusChamadoEnum.AguardandoSolicitante
            or StatusChamadoEnum.Resolvido;
}
