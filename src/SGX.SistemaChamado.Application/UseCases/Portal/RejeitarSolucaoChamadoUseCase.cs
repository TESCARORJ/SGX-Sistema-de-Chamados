using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class RejeitarSolucaoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusChamadoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IRepository<ComentarioChamado> comentarioChamadoRepository,
    IUnitOfWork unitOfWork,
    IUsuarioContextoAplicacaoService usuarioContextoService,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null) : IRejeitarSolucaoChamadoUseCase
{
    public async Task<ChamadoDetalheResponse> ExecutarAsync(Guid chamadoId, RejeitarSolucaoChamadoRequest request, CancellationToken cancellationToken = default)
    {
        var validator = new RejeitarSolucaoChamadoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Aprovacoes)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken);

        if (chamado == null)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var usuarioContexto = await usuarioContextoService.ObterAsync(cancellationToken);

        if (chamado.SolicitanteId != usuarioContexto.Id)
        {
            throw new UnauthorizedAccessException("Apenas o solicitante pode rejeitar a solucao deste chamado.");
        }

        if (validarBloqueioMovimentacaoUseCase != null)
        {
            var validacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(
                new DTOs.Chamados.ValidarBloqueioMovimentacaoAprovacaoPendenteRequest
                {
                    ChamadoId = chamadoId,
                    TipoAcao = DTOs.Chamados.TipoAcaoMovimentacaoChamado.RejeitarSolucao
                },
                cancellationToken);

            if (validacao.Bloqueado)
            {
                throw new InvalidOperationException(validacao.MensagemUsuario);
            }
        }

        var statusEmAndamento = await statusChamadoRepository.Query()
            .FirstOrDefaultAsync(x => x.Codigo == StatusChamadoEnum.EmAtendimento, cancellationToken);

        if (statusEmAndamento == null)
        {
            throw new InvalidOperationException("Status 'Em Atendimento' nao configurado no sistema.");
        }

        var statusAnteriorNome = chamado.Status.Nome;

        chamado.RejeitarSolucao(statusEmAndamento.Id, usuarioContexto.Id, request.MotivoRejeicao, usuarioContexto.Login);

        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamadoId,
            TipoHistoricoChamado.SolucaoRejeitada,
            $"A solucao do chamado foi rejeitada pelo solicitante. O status retornou de '{statusAnteriorNome}' para '{statusEmAndamento.Nome}'.",
            usuarioContexto.Id,
            usuarioContexto.Login);

        var comentario = new ComentarioChamado(
            chamadoId,
            usuarioContexto.Id,
            $"[SOLUCAO REJEITADA]\nMotivo: {request.MotivoRejeicao}",
            true,
            usuarioContexto.Login);

        await historicoChamadoRepository.AddAsync(historico, cancellationToken);
        await comentarioChamadoRepository.AddAsync(comentario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.RejeitarSolucaoChamado,
                Descricao = "Solucao do chamado rejeitada pelo solicitante.",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuarioContexto.Id,
                    UsuarioExecutorLogin = usuarioContexto.Login,
                    DataEventoUtc = chamado.SolucaoRejeitadaEm,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusEmAndamento.Nome,
                    chamado.ResolvidoEm,
                    SolucaoRejeitadaEm = (DateTime?)null
                }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuarioContexto.Id,
                    UsuarioExecutorLogin = usuarioContexto.Login,
                    DataEventoUtc = chamado.SolucaoRejeitadaEm,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusEmAndamento.Nome,
                    chamado.ResolvidoEm,
                    SolucaoRejeitadaEm = chamado.SolucaoRejeitadaEm,
                    SolucaoRejeitadaPorUsuarioId = chamado.SolucaoRejeitadaPorUsuarioId,
                    MotivoRejeicaoSolucao = chamado.MotivoRejeicaoSolucao
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "portal",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: "RejeicaoSolucao",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {statusEmAndamento.Nome}."),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true,
                UsuarioId = usuarioContexto.Id,
                UsuarioLogin = usuarioContexto.Login
            }, cancellationToken);
        }

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

        return PortalUseCaseHelpers.MapDetalhe(atualizado, usuarioContexto);
    }
}
