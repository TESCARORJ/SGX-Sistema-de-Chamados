using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class DirecionarChamadoGrupoTecnicoAdminUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IRepository<FilaAtendimento> filaAtendimentoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IDirecionarChamadoGrupoTecnicoAdminUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, DirecionarChamadoGrupoTecnicoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (request.GrupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico e obrigatorio.", nameof(request));
        }

        if (request.FilaAtendimentoId == Guid.Empty)
        {
            throw new ArgumentException("A fila de atendimento informada e invalida.", nameof(request));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.GrupoTecnico)
            .Include(x => x.FilaAtendimento)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var grupoDestino = await grupoTecnicoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == request.GrupoTecnicoId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Grupo tecnico nao encontrado ou inativo.");

        if (chamado.GrupoTecnicoId.HasValue && chamado.GrupoTecnicoId.Value != grupoDestino.Id)
        {
            throw new InvalidOperationException("Chamado ja possui outro grupo tecnico. Use a transferencia entre grupos tecnicos para mudar o grupo responsavel.");
        }

        FilaAtendimento? filaDestino = null;
        if (request.FilaAtendimentoId.HasValue)
        {
            filaDestino = await filaAtendimentoRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.FilaAtendimentoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Fila de atendimento nao encontrada ou inativa.");

            if (filaDestino.GrupoTecnicoId != grupoDestino.Id)
            {
                throw new InvalidOperationException("Fila de atendimento nao pertence ao grupo tecnico informado.");
            }
        }

        var filaOrigemId = chamado.FilaAtendimentoId;
        var filaOrigemNome = chamado.FilaAtendimento?.Nome ?? "Sem fila";
        var grupoJaDefinido = chamado.GrupoTecnicoId == grupoDestino.Id;
        var filaFinal = await ObterFilaFinalAsync(chamado, grupoDestino.Id, filaDestino, cancellationToken);

        if (grupoJaDefinido && filaOrigemId == filaFinal?.Id)
        {
            return AdminUseCaseHelpers.MapDetalhe(
                await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
                    .FirstAsync(x => x.Id == chamadoId, cancellationToken));
        }

        chamado.DirecionarGrupoTecnico(grupoDestino.Id, filaFinal?.Id, usuario.Login);
        chamadoRepository.Update(chamado);

        foreach (var historico in CriarHistoricosAuditoria(
            chamado.Id,
            grupoJaDefinido,
            grupoDestino.Nome,
            filaOrigemId,
            filaOrigemNome,
            filaFinal,
            request.Observacao,
            usuario.Id,
            usuario.Login))
        {
            await historicoRepository.AddAsync(historico, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }

    private async Task<FilaAtendimento?> ObterFilaFinalAsync(
        Chamado chamado,
        Guid grupoDestinoId,
        FilaAtendimento? filaInformada,
        CancellationToken cancellationToken)
    {
        if (filaInformada is not null)
        {
            return filaInformada;
        }

        if (!chamado.FilaAtendimentoId.HasValue)
        {
            return null;
        }

        if (chamado.FilaAtendimento is not null)
        {
            return chamado.FilaAtendimento.GrupoTecnicoId == grupoDestinoId
                ? chamado.FilaAtendimento
                : null;
        }

        var filaAtual = await filaAtendimentoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == chamado.FilaAtendimentoId.Value && x.Ativo, cancellationToken);

        return filaAtual?.GrupoTecnicoId == grupoDestinoId ? filaAtual : null;
    }

    private static IEnumerable<HistoricoChamado> CriarHistoricosAuditoria(
        Guid chamadoId,
        bool grupoJaDefinido,
        string grupoDestinoNome,
        Guid? filaOrigemId,
        string filaOrigemNome,
        FilaAtendimento? filaFinal,
        string? observacao,
        Guid usuarioId,
        string usuarioLogin)
    {
        if (!grupoJaDefinido)
        {
            var descricaoGrupo = $"Grupo tecnico definido como {grupoDestinoNome}.";
            if (!string.IsNullOrWhiteSpace(observacao))
            {
                descricaoGrupo = $"{descricaoGrupo} Observacao: {observacao.Trim()}";
            }

            yield return CriarHistorico(chamadoId, TipoHistoricoChamado.GrupoTecnicoDefinido, descricaoGrupo, usuarioId, usuarioLogin);
        }

        if (!filaOrigemId.HasValue && filaFinal is not null)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoDefinida,
                $"Fila de atendimento definida como {filaFinal.Nome}.",
                usuarioId,
                usuarioLogin);
        }
        else if (filaOrigemId.HasValue && filaFinal is null)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoRemovida,
                $"Fila de atendimento removida: {filaOrigemNome}.",
                usuarioId,
                usuarioLogin);
        }
        else if (filaOrigemId.HasValue && filaFinal is not null && filaOrigemId.Value != filaFinal.Id)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoTransferida,
                $"Fila de atendimento transferida de {filaOrigemNome} para {filaFinal.Nome}.",
                usuarioId,
                usuarioLogin);
        }
    }

    private static HistoricoChamado CriarHistorico(
        Guid chamadoId,
        TipoHistoricoChamado tipo,
        string descricao,
        Guid usuarioId,
        string usuarioLogin)
        => new(chamadoId, tipo, AdminUseCaseHelpers.ObterDescricaoHistorico(tipo, descricao), usuarioId, usuarioLogin);
}
