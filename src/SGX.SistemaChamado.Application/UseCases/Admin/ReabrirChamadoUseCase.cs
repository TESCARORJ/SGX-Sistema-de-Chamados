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

public sealed class ReabrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IFluxoStatusChamadoService fluxoStatusChamadoService,
    IAcoesChamadoService acoesChamadoService,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null,
    IValidarBloqueioMovimentacaoAprovacaoPendenteUseCase? validarBloqueioMovimentacaoUseCase = null,
    IRepository<ParametroSistema>? parametroRepository = null) : IReabrirChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, ReabrirChamadoRequest request, CancellationToken cancellationToken = default)
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
            .Include(x => x.ChamadoSla)
            .Include(x => x.Aprovacoes)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        acoesChamadoService.ValidarAcaoDisponivel(chamado, AcaoChamadoEnum.Reabrir, usuario);

        var statusAnterior = chamado.Status.Nome;
        var encerradoAnterior = chamado.EncerradoEm;
        await GarantirMovimentacaoPermitidaAsync(chamado, cancellationToken);

        var podeReabrir = chamado.EncerradoEm.HasValue || chamado.Status.EhStatusFinal;

        if (!podeReabrir)
        {
            throw new InvalidOperationException("Somente chamados em status final podem ser reabertos.");
        }

        var msgReabertura = "Chamado reaberto.";
        var msgHistorico = "Chamado reaberto";

        if (parametroRepository is not null && chamado.EncerradoEm.HasValue)
        {
            var parametro = await parametroRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Chave == ConfiguracaoReaberturaChamadoConstantes.ChaveParametroPrazoMaximoHoras, cancellationToken);

            if (parametro is { Ativo: true })
            {
                if (!double.TryParse(parametro.Valor, out var horasPrazo))
                {
                    throw new InvalidOperationException("A configuracao administrativa do prazo maximo de reabertura esta invalida.");
                }

                var horasPassadas = (DateTime.UtcNow - chamado.EncerradoEm.Value).TotalHours;
                if (horasPassadas > horasPrazo)
                {
                    throw new InvalidOperationException($"O prazo maximo de reabertura de {horasPrazo} horas foi excedido.");
                }

                msgReabertura = "Chamado reaberto por politica de prazo.";
                msgHistorico = msgReabertura;
            }
        }

        var statusDestinoCodigo = ObterStatusDestinoReabertura(chamado.NaturezaChamado, fluxoStatusChamadoService);

        var statusDestino = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == statusDestinoCodigo, cancellationToken)
            ?? throw new InvalidOperationException($"Status de reabertura '{statusDestinoCodigo}' nao configurado.");

        fluxoStatusChamadoService.ValidarStatusPermitido(chamado.NaturezaChamado, statusDestino.Codigo);

        chamado.Reabrir(statusDestino.Id, usuario.Login);
        await slaService.ReabrirAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        if (!string.IsNullOrWhiteSpace(request.Mensagem))
        {
            var comentario = new ComentarioChamado(
                chamado.Id,
                usuario.Id,
                request.Mensagem,
                false,
                usuario.Login);

            await comentarioRepository.AddAsync(comentario, cancellationToken);
        }

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Reaberto,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.Reaberto, msgHistorico),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        if (auditoriaService is not null)
        {
            var statusAtualNome = atualizado.Status.Nome;

            await auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
            {
                Modulo = "Chamados",
                Entidade = "Chamado",
                EntidadeId = chamadoId.ToString(),
                Acao = TipoAcaoAuditoria.ReabrirChamado,
                Descricao = msgReabertura,
                DadosAntes = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = statusAnterior,
                    EncerradoEm = encerradoAnterior
                }),
                DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
                {
                    Status = statusAtualNome,
                    EncerradoEm = atualizado.EncerradoEm,
                    Mensagem = request.Mensagem
                }),
                Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoId.ToString(),
                    codigo: atualizado.Codigo,
                    nome: atualizado.Titulo,
                    operacao: "Reabertura",
                    resultado: "Sucesso",
                    observacao: $"Status atual: {statusAtualNome}")
            }, cancellationToken);
        }

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
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
                TipoAcao = TipoAcaoMovimentacaoChamado.Reabrir
            },
            cancellationToken);

        if (avaliacao.Bloqueado)
        {
            throw new InvalidOperationException(avaliacao.MensagemUsuario ?? AprovacaoChamadoHelper.MensagemBloqueioAprovacaoPendente);
        }
    }

    private static StatusChamadoEnum ObterStatusDestinoReabertura(
        NaturezaChamadoEnum natureza,
        IFluxoStatusChamadoService fluxoStatusChamadoService)
    {
        var candidatos = new[]
        {
            StatusChamadoEnum.EmAtendimento,
            StatusChamadoEnum.EmAnalise,
            StatusChamadoEnum.Aberto
        };

        foreach (var candidato in candidatos)
        {
            if (fluxoStatusChamadoService.StatusEhPermitido(natureza, candidato))
            {
                return candidato;
            }
        }

        throw new InvalidOperationException($"Nao foi possivel determinar status de reabertura para a natureza '{natureza}'.");
    }
}
