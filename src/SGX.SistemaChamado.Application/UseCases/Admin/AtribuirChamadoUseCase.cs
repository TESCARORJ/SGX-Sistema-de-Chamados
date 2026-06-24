using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AtribuirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IProcessarEventoCandidatoNotificacaoUseCase? processarEventoCandidatoNotificacaoUseCase = null,
    ILogger<AtribuirChamadoUseCase>? logger = null) : IAtribuirChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AtribuirChamadoRequest request, CancellationToken cancellationToken = default)
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

        if (!AdminUseCaseHelpers.EhAdministrador(usuario))
        {
            throw new InvalidOperationException("Atribuicao de responsavel permitida apenas para Administrador nesta sprint.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Responsavel)
            .Include(x => x.GrupoTecnico)
            .Include(x => x.FilaAtendimento)
            .Include(x => x.ChamadoSla)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");
        var responsavelAnterior = chamado.Responsavel?.Nome;

        var responsavel = await usuarioRepository.Query()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == request.ResponsavelId && x.Ativo && x.Situacao == SituacaoUsuario.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Responsavel informado nao encontrado ou inativo.");

        var possuiPerfilAtendimento = responsavel.UsuarioPerfis.Any(p =>
            p.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador ||
            p.PerfilAcesso.TipoPerfil == TipoPerfil.Atendente);

        if (!possuiPerfilAtendimento)
        {
            throw new InvalidOperationException("Usuario destino nao possui perfil de atendimento.");
        }

        if (chamado.GrupoTecnicoId.HasValue)
        {
            if (chamado.GrupoTecnico is null || !chamado.GrupoTecnico.Ativo)
            {
                throw new InvalidOperationException("Grupo tecnico do chamado nao encontrado ou inativo.");
            }

            var membroAtivo = await membroGrupoTecnicoRepository.Query()
                .AsNoTracking()
                .AnyAsync(x =>
                    x.GrupoTecnicoId == chamado.GrupoTecnicoId.Value &&
                    x.UsuarioId == responsavel.Id &&
                    x.Ativo,
                    cancellationToken);

            if (!membroAtivo)
            {
                throw new InvalidOperationException("Responsavel informado nao e membro ativo do grupo tecnico do chamado.");
            }

            if (chamado.FilaAtendimentoId.HasValue)
            {
                if (chamado.FilaAtendimento is null || !chamado.FilaAtendimento.Ativo)
                {
                    throw new InvalidOperationException("Fila de atendimento do chamado nao encontrada ou inativa.");
                }

                if (chamado.FilaAtendimento.GrupoTecnicoId != chamado.GrupoTecnicoId.Value)
                {
                    throw new InvalidOperationException("Fila de atendimento do chamado nao pertence ao grupo tecnico do chamado.");
                }
            }
        }

        chamado.AtribuirResponsavel(responsavel.Id, usuario.Login);
        await slaService.RegistrarPrimeiraRespostaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        var descricaoHistorico = string.IsNullOrWhiteSpace(responsavelAnterior)
            ? $"Responsavel alterado para {responsavel.Nome}"
            : $"Responsavel alterado de {responsavelAnterior} para {responsavel.Nome}";

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ResponsavelAlterado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ResponsavelAlterado, descricaoHistorico),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await TentarIntegrarNotificacaoAsync(chamado, responsavel, usuario, historico, cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new { Responsavel = responsavelAnterior },
                new { Responsavel = responsavel.Nome });

            await auditoriaService.RegistrarEdicaoAsync(
                "Chamados",
                "Chamado",
                chamadoId.ToString(),
                "Responsavel do chamado alterado.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "AtribuicaoResponsavel",
                    resultado: "Sucesso",
                    observacao: $"Responsavel atual: {atualizado.Responsavel?.Nome}"),
                cancellationToken: cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task TentarIntegrarNotificacaoAsync(
        Chamado chamado,
        Usuario responsavel,
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
                    $"responsavel-alterado:{historico.Id}",
                    new EventoCandidatoNotificacao(
                        TipoEventoNotificacao.EventoChamado,
                        chamado.Id,
                        usuarioAtual.Id,
                        historico.CriadoEm,
                        $"chamado:{chamado.Id}",
                        $"responsavel-alterado:{historico.Id}",
                        new Dictionary<string, string>
                        {
                            ["evento"] = "responsavel-alterado"
                        }),
                    new Dictionary<string, string>
                    {
                        ["chamado.codigo"] = chamado.Codigo,
                        ["chamado.titulo"] = chamado.Titulo,
                        ["chamado.status"] = chamado.Status.Nome,
                        ["evento.nome"] = "Responsavel alterado",
                        ["evento.descricao"] = historico.Descricao,
                        ["evento.ocorrido_em"] = historico.CriadoEm.ToString("O"),
                        ["responsavel.nome"] = responsavel.Nome
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
                "Falha ao integrar notificacao de atribuicao. ChamadoId={ChamadoId} HistoricoId={HistoricoId}",
                chamado.Id,
                historico.Id);
        }
    }
}
