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

public sealed class CancelarChamadoUseCase(
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
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null) : ICancelarChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, CancelarChamadoRequest request, CancellationToken cancellationToken = default)
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

        if (string.IsNullOrWhiteSpace(request.Motivo))
        {
            throw new ArgumentException("Motivo obrigatório");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Aprovacoes)
            .Include(x => x.ChamadoSla)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        acoesChamadoService.ValidarAcaoDisponivel(chamado, AcaoChamadoEnum.Cancelar, usuario);

        await GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(chamado.Id, cancellationToken);

        if (chamado.EncerradoEm.HasValue || chamado.Status.Codigo == StatusChamadoEnum.Encerrado || chamado.Status.Codigo == StatusChamadoEnum.Cancelado)
        {
            throw new InvalidOperationException("Chamado ja esta em estado final (encerrado ou cancelado).");
        }

        var statusCancelado = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Cancelado, cancellationToken)
            ?? throw new InvalidOperationException("Status Cancelado nao configurado.");

        fluxoStatusChamadoService.ValidarStatusPermitido(chamado.NaturezaChamado, statusCancelado.Codigo);

        // Cancelar pode não precisar de GarantirChamadoSemDependenciaAtivaAsync se o cancelamento ignora bloqueios,
        // mas vamos aplicar para não permitir cancelar chamado bloqueado administrativamente
        await GarantirChamadoSemDependenciaAtivaAsync(chamado.Id, cancellationToken);

        chamado.Cancelar(statusCancelado.Id, request.Motivo, usuario.Login);
        await slaService.RegistrarEncerramentoAsync(chamado, usuario.Login, DateTime.UtcNow);
        chamadoRepository.Update(chamado);

        var comentario = new ComentarioChamado(
            chamado.Id,
            usuario.Id,
            $"Motivo do Cancelamento: {request.Motivo}",
            request.ComentarioInterno,
            usuario.Login);
        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Cancelado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.Cancelado, $"Chamado cancelado. Motivo: {request.Motivo}"),
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
                Acao = TipoAcaoAuditoria.AlteracaoStatus,
                Descricao = "Chamado cancelado.",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new { Status = chamado.Status.Nome, EncerradoEm = (DateTime?)null }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = statusCancelado.Nome,
                    EncerradoEm = chamado.EncerradoEm,
                    Motivo = request.Motivo
                }),
                UsuarioLogin = usuario.Login
            });
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task GarantirChamadoSemDependenciaAtivaAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var temDependenciaAtiva = await relacionamentosChamadoUseCases.EstaBloqueadoPorDependenciaAsync(chamadoId, cancellationToken);

        if (temDependenciaAtiva)
        {
            throw new InvalidOperationException("Este chamado possui dependencia ativa e nao pode ser cancelado enquanto estiver bloqueado por outro chamado.");
        }
    }

    private async Task GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        if (validarBloqueioMovimentacaoUseCase is not null)
        {
            var validacao = await validarBloqueioMovimentacaoUseCase.ExecutarAsync(new ValidarBloqueioMovimentacaoAprovacaoPendenteRequest { ChamadoId = chamadoId, TipoAcao = TipoAcaoMovimentacaoChamado.Cancelar }, cancellationToken);
            if (validacao.Bloqueado)
            {
                throw new InvalidOperationException(validacao.MensagemUsuario);
            }
        }
    }
}
