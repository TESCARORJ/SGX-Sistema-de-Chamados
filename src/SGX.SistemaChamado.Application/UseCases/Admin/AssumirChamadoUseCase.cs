using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AssumirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null,
    IProcessarEventoCandidatoNotificacaoUseCase? processarEventoCandidatoNotificacaoUseCase = null,
    ILogger<AssumirChamadoUseCase>? logger = null) : IAssumirChamadoUseCase
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

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Responsavel)
            .Include(x => x.Aprovacoes)
            .Include(x => x.ChamadoSla)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");
        var responsavelAnterior = chamado.Responsavel?.Nome;

        await GarantirMovimentacaoPermitidaAsync(chamado, cancellationToken);

        if (chamado.ResponsavelId.HasValue && chamado.ResponsavelId.Value != usuario.Id && !AdminUseCaseHelpers.EhAdministrador(usuario))
        {
            throw new InvalidOperationException("Atendente so pode assumir chamado sem responsavel.");
        }

        chamado.AtribuirResponsavel(usuario.Id, usuario.Login);
        await slaService.RegistrarPrimeiraRespostaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ResponsavelAlterado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ResponsavelAlterado, $"Chamado assumido por {usuario.Nome}"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await TentarIntegrarNotificacaoAsync(chamado, usuario, historico, cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new { Responsavel = responsavelAnterior },
                new { Responsavel = usuario.Nome });

            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Chamado assumido.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "AssumirChamado",
                    resultado: "Sucesso",
                    observacao: $"Responsavel atual: {atualizado.Responsavel?.Nome}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task TentarIntegrarNotificacaoAsync(
        Chamado chamado,
        UsuarioContextoAplicacao usuarioAtual,
        HistoricoChamado historico,
        CancellationToken cancellationToken)
    {
        if (processarEventoCandidatoNotificacaoUseCase is null)
        {
            return;
        }

        try
        {
            await processarEventoCandidatoNotificacaoUseCase.ExecutarAsync(
                new ProcessarEventoCandidatoNotificacaoRequest(
                    $"chamado-assumido:{historico.Id}",
                    new EventoCandidatoNotificacao(
                        TipoEventoNotificacao.EventoChamado,
                        chamado.Id,
                        usuarioAtual.Id,
                        historico.CriadoEm,
                        $"chamado:{chamado.Id}",
                        $"chamado-assumido:{historico.Id}",
                        new Dictionary<string, string>
                        {
                            ["evento"] = "chamado-assumido"
                        }),
                    new Dictionary<string, string>
                    {
                        ["chamado.codigo"] = chamado.Codigo,
                        ["chamado.titulo"] = chamado.Titulo,
                        ["chamado.status"] = chamado.Status.Nome,
                        ["evento.nome"] = "Chamado assumido",
                        ["evento.descricao"] = historico.Descricao,
                        ["evento.ocorrido_em"] = historico.CriadoEm.ToString("O"),
                        ["responsavel.nome"] = usuarioAtual.Nome
                    },
                    [TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual],
                    [CanalNotificacao.Sistema, CanalNotificacao.Email],
                    ExcluirUsuarioOriginador: true),
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
                "Falha ao integrar notificacao de assuncao. ChamadoId={ChamadoId} HistoricoId={HistoricoId}",
                chamado.Id,
                historico.Id);
        }
    }

    private async Task GarantirMovimentacaoPermitidaAsync(Chamado chamado, CancellationToken cancellationToken)
    {
        if (validarBloqueioMovimentacaoUseCase is null)
        {
            var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
            if (estadoAprovacao.BloqueiaAvancoAtendimento)
            {
                throw new InvalidOperationException(estadoAprovacao.MensagemBloqueio ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
            }

            return;
        }

        var avaliacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(
            new()
            {
                ChamadoId = chamado.Id,
                TipoAcao = TipoAcaoMovimentacaoChamado.Assumir
            },
            cancellationToken);

        if (avaliacao.Bloqueado)
        {
            throw new InvalidOperationException(avaliacao.MensagemUsuario ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
        }
    }
}
