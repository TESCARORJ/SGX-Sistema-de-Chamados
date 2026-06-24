using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Application.UseCases.Chamados;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ResolverChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IFluxoStatusChamadoService fluxoStatusChamadoService,
    IAcoesChamadoService acoesChamadoService,
    IAdminRelacionamentosChamadoUseCases relacionamentosChamadoUseCases,
    IAdminChamadoAprovacoesUseCases chamadoAprovacoesUseCases,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null) : IResolverChamadoUseCase
{
    private const string MensagemBloqueioDependenciaAtiva =
        "Este chamado possui dependencia ativa e nao pode ser resolvido enquanto estiver bloqueado por outro chamado.";
    private const string MensagemBloqueioAprovacaoPendente =
        AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente;

    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ResolverChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Solucao))
        {
            throw new ArgumentException("Solucao tecnica obrigatoria para resolucao.", nameof(request.Solucao));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Aprovacoes)
            .Include(x => x.ChamadoSla)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        acoesChamadoService.ValidarAcaoDisponivel(chamado, AcaoChamadoEnum.Resolver, usuario);

        await GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(chamado.Id, cancellationToken);

        if (chamado.ResolvidoEm.HasValue || chamado.Status.Codigo == StatusChamadoEnum.Resolvido)
        {
            throw new InvalidOperationException("Chamado ja resolvido.");
        }
        
        if (chamado.EncerradoEm.HasValue || chamado.Status.Codigo == StatusChamadoEnum.Encerrado)
        {
            throw new InvalidOperationException("Chamado ja encerrado.");
        }

        var statusResolvido = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Resolvido, cancellationToken)
            ?? throw new InvalidOperationException("Status Resolvido nao configurado.");

        fluxoStatusChamadoService.ValidarStatusPermitido(chamado.NaturezaChamado, statusResolvido.Codigo);
        await GarantirChamadoSemDependenciaAtivaAsync(chamado.Id, cancellationToken);

        var statusAnteriorNome = chamado.Status.Nome;
        var dataEventoUtc = DateTime.UtcNow;

        chamado.Resolver(statusResolvido.Id, request.Solucao, usuario.Login);
        
        // Pausar SLA na resolução (o cliente já não está mais pendente)
        await slaService.RegistrarEncerramentoAsync(chamado, usuario.Login, DateTime.UtcNow);
        
        chamadoRepository.Update(chamado);

        var comentario = new ComentarioChamado(
            chamado.Id,
            usuario.Id,
            request.Solucao ?? string.Empty,
            request.ComentarioInterno,
            usuario.Login);
        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Resolvido,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.Resolvido, "Chamado resolvido"),
            usuario.Id,
            usuario.Login);
        await historicoRepository.AddAsync(historico, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.ResolverChamado,
                Descricao = "Chamado resolvido.",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuario.Id,
                    UsuarioExecutorLogin = usuario.Login,
                    DataEventoUtc = dataEventoUtc,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusResolvido.Nome,
                    ResolvidoEm = (DateTime?)null
                }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    ChamadoId = chamadoId,
                    UsuarioExecutorId = usuario.Id,
                    UsuarioExecutorLogin = usuario.Login,
                    DataEventoUtc = dataEventoUtc,
                    StatusAnterior = statusAnteriorNome,
                    StatusNovo = statusResolvido.Nome,
                    chamado.ResolvidoEm,
                    SolucaoTecnica = request.Solucao,
                    ComentarioInterno = request.ComentarioInterno,
                    TamanhoSolucao = request.Solucao?.Length ?? 0
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "Resolucao",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {atualizado.Status}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true,
                UsuarioId = usuario.Id,
                UsuarioLogin = usuario.Login
            }, cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task GarantirChamadoSemDependenciaAtivaAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        if (await relacionamentosChamadoUseCases.EstaBloqueadoPorDependenciaAsync(chamadoId, cancellationToken))
        {
            throw new InvalidOperationException(MensagemBloqueioDependenciaAtiva);
        }
    }

    private async Task GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        if (validarBloqueioMovimentacaoUseCase is not null)
        {
            var avaliacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(
                new()
                {
                    ChamadoId = chamadoId,
                    TipoAcao = TipoAcaoMovimentacaoChamado.Resolver
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
}
