using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Application.Helpers;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AceitarSolucaoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null) : IAceitarSolucaoChamadoUseCase
{
    private const string MensagemBloqueioAprovacaoPendente =
        AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente;

    public async Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, AceitarSolucaoChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Aprovacoes)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!PortalUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new UnauthorizedAccessException("Acesso negado. Apenas o solicitante pode aceitar a solucao do chamado.");
        }

        if (chamado.Status.Codigo != StatusChamadoEnum.Resolvido)
        {
            throw new InvalidOperationException("Apenas chamados com status Resolvido podem ter a solucao aceita.");
        }

        await GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(chamado.Id, cancellationToken);
        var statusAnteriorNome = chamado.Status.Nome;

        var statusFechado = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Encerrado, cancellationToken)
            ?? throw new InvalidOperationException("Status Fechado (Encerrado) nao configurado.");

        chamado.AceitarSolucao(statusFechado.Id, usuario.Id, request.ObservacaoAceite, usuario.Login);
        chamadoRepository.Update(chamado);

        if (!string.IsNullOrWhiteSpace(request.ObservacaoAceite))
        {
            var comentario = new ComentarioChamado(
                chamado.Id,
                usuario.Id,
                $"Solucao aceita pelo solicitante. Observacao: {request.ObservacaoAceite}",
                false,
                usuario.Login);
            await comentarioRepository.AddAsync(comentario, cancellationToken);
        }

        var historicoDescricao = "Solucao aceita pelo solicitante.";
        if (!string.IsNullOrWhiteSpace(request.ObservacaoAceite))
        {
            historicoDescricao += $" Observacao: {request.ObservacaoAceite}";
        }

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.SolucaoAceita,
            historicoDescricao,
            usuario.Id,
            usuario.Login);
        await historicoRepository.AddAsync(historico, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.InventarioAtivo)
            .Include(x => x.Aprovacoes)
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .FirstOrDefaultAsync(x => x.Id == chamadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Erro ao recuperar chamado atualizado.");

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.AceitarSolucaoChamado,
                Descricao = "Solucao aceita pelo solicitante (fechamento definitivo).",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuario.Id,
                    UsuarioExecutorLogin = usuario.Login,
                    DataEventoUtc = chamado.AceitoEm,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusFechado.Nome,
                    EncerradoEm = (DateTime?)null,
                    AceitoEm = (DateTime?)null,
                    AceitoPorUsuarioId = (Guid?)null
                }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuario.Id,
                    UsuarioExecutorLogin = usuario.Login,
                    DataEventoUtc = chamado.AceitoEm,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusFechado.Nome,
                    EncerradoEm = chamado.EncerradoEm,
                    AceitoEm = chamado.AceitoEm,
                    AceitoPorUsuarioId = chamado.AceitoPorUsuarioId,
                    ObservacaoAceite = chamado.ObservacaoAceite
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "portal",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "AceiteSolucao",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {atualizado.Status}."),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true,
                UsuarioId = usuario.Id,
                UsuarioLogin = usuario.Login
            }, cancellationToken);
        }

        return PortalUseCaseHelpers.MapDetalhe(atualizado, usuario);
    }

    private async Task GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        if (validarBloqueioMovimentacaoUseCase is not null)
        {
            var validacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(new DTOs.Chamados.ValidarBloqueioMovimentacaoAprovacaoPendenteRequest { ChamadoId = chamadoId, TipoAcao = DTOs.Chamados.TipoAcaoMovimentacaoChamado.AceitarSolucao }, cancellationToken);
            if (validacao.Bloqueado)
            {
                throw new InvalidOperationException(validacao.MensagemUsuario ?? MensagemBloqueioAprovacaoPendente);
            }

            return;
        }

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Aprovacoes)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var estadoAprovacao = AprovacaoChamadoHelper.ObterEstado(chamado);
        if (estadoAprovacao.BloqueiaAvancoAtendimento)
        {
            throw new InvalidOperationException(estadoAprovacao.MensagemBloqueio ?? MensagemBloqueioAprovacaoPendente);
        }
    }
}
