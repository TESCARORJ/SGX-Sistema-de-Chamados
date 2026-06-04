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

public sealed class EncerrarChamadoUseCase(
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
    IAuditoriaService? auditoriaService = null) : IEncerrarChamadoUseCase
{
    private const string MensagemBloqueioDependenciaAtiva =
        "Este chamado possui dependencia ativa e nao pode ser fechado enquanto estiver bloqueado por outro chamado.";
    private const string MensagemBloqueioAprovacaoPendente =
        AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente;

    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, EncerrarChamadoRequest request, CancellationToken cancellationToken = default)
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
            .Include(x => x.Aprovacoes)
            .Include(x => x.ChamadoSla)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        acoesChamadoService.ValidarAcaoDisponivel(chamado, AcaoChamadoEnum.Encerrar, usuario);

        await GarantirChamadoSemAprovacaoPendenteBloqueanteAsync(chamado.Id, cancellationToken);

        if (chamado.EncerradoEm.HasValue || chamado.Status.Codigo == StatusChamadoEnum.Encerrado)
        {
            throw new InvalidOperationException("Chamado ja encerrado.");
        }

        var statusEncerrado = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Encerrado, cancellationToken)
            ?? throw new InvalidOperationException("Status Encerrado nao configurado.");

        fluxoStatusChamadoService.ValidarStatusPermitido(chamado.NaturezaChamado, statusEncerrado.Codigo);
        await GarantirChamadoSemDependenciaAtivaAsync(chamado.Id, cancellationToken);

        chamado.Encerrar(statusEncerrado.Id, usuario.Login);
        await slaService.RegistrarEncerramentoAsync(chamado, usuario.Login, DateTime.UtcNow);
        chamadoRepository.Update(chamado);

        var comentario = new ComentarioChamado(
            chamado.Id,
            usuario.Id,
            request.Solucao,
            request.ComentarioInterno,
            usuario.Login);
        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Encerrado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.Encerrado, "Chamado encerrado"),
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
                Descricao = "Chamado encerrado.",
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new { Status = chamado.Status.Nome, EncerradoEm = (DateTime?)null }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = statusEncerrado.Nome,
                    chamado.EncerradoEm,
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
                    operacao: "Encerramento",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {atualizado.Status}"),
                Nivel = NivelAuditoria.Informacao,
                Sucesso = true
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
        if (await chamadoAprovacoesUseCases.PossuiAprovacaoPendenteBloqueanteAsync(chamadoId, cancellationToken))
        {
            throw new InvalidOperationException(MensagemBloqueioAprovacaoPendente);
        }
    }
}
