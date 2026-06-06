using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class TransferirGrupoTecnicoChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<GrupoTecnico> grupoTecnicoRepository,
    IRepository<FilaAtendimento> filaAtendimentoRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : ITransferirGrupoTecnicoChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, TransferirGrupoTecnicoChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        if (request.GrupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico de destino e obrigatorio.", nameof(request));
        }

        if (request.FilaAtendimentoId == Guid.Empty)
        {
            throw new ArgumentException("A fila de atendimento de destino informada e invalida.", nameof(request));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.GrupoTecnico)
            .Include(x => x.FilaAtendimento)
            .Include(x => x.Responsavel)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var grupoDestino = await grupoTecnicoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == request.GrupoTecnicoId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Grupo tecnico de destino nao encontrado ou inativo.");

        FilaAtendimento? filaDestino = null;
        if (request.FilaAtendimentoId.HasValue)
        {
            filaDestino = await filaAtendimentoRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.FilaAtendimentoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Fila de atendimento de destino nao encontrada ou inativa.");

            if (filaDestino.GrupoTecnicoId != grupoDestino.Id)
            {
                throw new InvalidOperationException("Fila de atendimento de destino nao pertence ao grupo tecnico informado.");
            }
        }

        var grupoOrigemId = chamado.GrupoTecnicoId;
        var filaOrigemId = chamado.FilaAtendimentoId;
        var responsavelOrigemId = chamado.ResponsavelId;
        var grupoOrigemNome = chamado.GrupoTecnico?.Nome ?? "Sem grupo tecnico";
        var filaOrigemNome = chamado.FilaAtendimento?.Nome ?? "Sem fila";
        var responsavelOrigemNome = chamado.Responsavel?.Nome ?? "Responsavel anterior";
        var filaDestinoId = filaDestino?.Id;

        if (!grupoOrigemId.HasValue)
        {
            throw new InvalidOperationException("Chamado sem grupo tecnico deve ser direcionado antes de ser transferido entre grupos.");
        }

        if (grupoOrigemId == grupoDestino.Id && filaOrigemId == filaDestinoId)
        {
            return AdminUseCaseHelpers.MapDetalhe(
                await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
                    .FirstAsync(x => x.Id == chamadoId, cancellationToken));
        }

        if (grupoOrigemId == grupoDestino.Id)
        {
            throw new InvalidOperationException("Transferencia para o mesmo grupo tecnico nao deve alterar fila nesta etapa.");
        }

        chamado.TransferirGrupoTecnico(grupoDestino.Id, filaDestinoId, usuario.Login);
        chamadoRepository.Update(chamado);

        foreach (var historico in CriarHistoricosAuditoria(
            chamado.Id,
            grupoOrigemId,
            filaOrigemId,
            responsavelOrigemId,
            grupoOrigemNome,
            filaOrigemNome,
            responsavelOrigemNome,
            grupoDestino.Nome,
            filaDestino,
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

    private static IEnumerable<HistoricoChamado> CriarHistoricosAuditoria(
        Guid chamadoId,
        Guid? grupoOrigemId,
        Guid? filaOrigemId,
        Guid? responsavelOrigemId,
        string grupoOrigemNome,
        string filaOrigemNome,
        string responsavelOrigemNome,
        string grupoDestinoNome,
        FilaAtendimento? filaDestino,
        Guid usuarioId,
        string usuarioLogin)
    {
        var tipoGrupo = grupoOrigemId.HasValue
            ? TipoHistoricoChamado.GrupoTecnicoTransferido
            : TipoHistoricoChamado.GrupoTecnicoDefinido;
        var descricaoGrupo = grupoOrigemId.HasValue
            ? $"Grupo tecnico transferido de {grupoOrigemNome} para {grupoDestinoNome}."
            : $"Grupo tecnico definido como {grupoDestinoNome}.";

        yield return CriarHistorico(chamadoId, tipoGrupo, descricaoGrupo, usuarioId, usuarioLogin);

        if (!filaOrigemId.HasValue && filaDestino is not null)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoDefinida,
                $"Fila de atendimento definida como {filaDestino.Nome}.",
                usuarioId,
                usuarioLogin);
        }
        else if (filaOrigemId.HasValue && filaDestino is null)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoRemovida,
                $"Fila de atendimento removida: {filaOrigemNome}.",
                usuarioId,
                usuarioLogin);
        }
        else if (filaOrigemId.HasValue && filaDestino is not null && filaOrigemId.Value != filaDestino.Id)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.FilaAtendimentoTransferida,
                $"Fila de atendimento transferida de {filaOrigemNome} para {filaDestino.Nome}.",
                usuarioId,
                usuarioLogin);
        }

        if (responsavelOrigemId.HasValue)
        {
            yield return CriarHistorico(
                chamadoId,
                TipoHistoricoChamado.ResponsavelRemovidoPorTransferenciaGrupo,
                $"Responsavel individual {responsavelOrigemNome} removido pela transferencia de grupo tecnico.",
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
        => new(
            chamadoId,
            tipo,
            AdminUseCaseHelpers.ObterDescricaoHistorico(tipo, descricao),
            usuarioId,
            usuarioLogin);
}
