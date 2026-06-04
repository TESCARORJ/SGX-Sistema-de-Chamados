using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ChamadoTarefasUseCases(
    IRepository<Chamado> chamadoRepository,
    IRepository<ChamadoTarefa> chamadoTarefaRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminChamadoTarefasUseCases
{
    public async Task<ChamadoTarefaAdminResponse> CriarAsync(
        Guid chamadoId,
        CriarChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        ArgumentNullException.ThrowIfNull(request);

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoAtivoAsync(chamadoId, cancellationToken);

        Usuario? responsavel = null;
        if (request.ResponsavelUsuarioId.HasValue)
        {
            responsavel = await usuarioRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ResponsavelUsuarioId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Responsavel nao encontrado ou inativo.");
        }

        var tarefa = new ChamadoTarefa(
            chamadoId,
            request.Titulo,
            request.Descricao,
            request.ResponsavelUsuarioId,
            request.Prazo,
            usuario.Id,
            usuario.Login);

        await chamadoTarefaRepository.AddAsync(tarefa, cancellationToken);
        await RegistrarHistoricoAsync(
            chamadoId,
            TipoHistoricoChamado.TarefaCriada,
            $"Tarefa vinculada criada: {tarefa.Titulo}.",
            usuario,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapResponse(tarefa, responsavel?.Nome);
    }

    public async Task<IReadOnlyList<ChamadoTarefaAdminResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        bool incluirInativas = false,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        var query = chamadoTarefaRepository.Query()
            .AsNoTracking()
            .Include(x => x.ResponsavelUsuario)
            .Where(x => x.ChamadoId == chamadoId);

        if (!incluirInativas)
        {
            query = query.Where(x => x.Ativo);
        }

        var tarefas = await query
            .OrderBy(x => x.Status == StatusTarefaChamadoEnum.Concluida)
            .ThenBy(x => x.Prazo ?? DateTime.MaxValue)
            .ThenByDescending(x => x.CriadoEm)
            .ToListAsync(cancellationToken);

        return tarefas
            .Select(x => MapResponse(x, x.ResponsavelUsuario?.Nome))
            .ToList();
    }

    public async Task<ChamadoTarefaAdminResponse> AtualizarStatusAsync(
        Guid chamadoId,
        Guid tarefaId,
        AtualizarStatusChamadoTarefaAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        if (tarefaId == Guid.Empty)
        {
            throw new ArgumentException("A tarefa informada e invalida.", nameof(tarefaId));
        }

        ArgumentNullException.ThrowIfNull(request);

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        var tarefa = await ObterTarefaDoChamadoAsync(chamadoId, tarefaId, cancellationToken);
        var statusAnterior = tarefa.Status;

        tarefa.AlterarStatus(request.Status, usuario.Id, usuario.Login);

        var tipoHistorico = tarefa.Status == StatusTarefaChamadoEnum.Concluida
            ? TipoHistoricoChamado.TarefaConcluida
            : TipoHistoricoChamado.TarefaStatusAlterado;
        var descricao = tarefa.Status == StatusTarefaChamadoEnum.Concluida
            ? $"Tarefa vinculada concluida: {tarefa.Titulo}."
            : $"Status da tarefa vinculada alterado de {statusAnterior} para {tarefa.Status}: {tarefa.Titulo}.";

        await RegistrarHistoricoAsync(chamadoId, tipoHistorico, descricao, usuario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapResponse(tarefa, tarefa.ResponsavelUsuario?.Nome);
    }

    public async Task CancelarAsync(
        Guid chamadoId,
        Guid tarefaId,
        CancelarChamadoTarefaAdminRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        if (tarefaId == Guid.Empty)
        {
            throw new ArgumentException("A tarefa informada e invalida.", nameof(tarefaId));
        }

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        var tarefa = await ObterTarefaDoChamadoAsync(chamadoId, tarefaId, cancellationToken);

        tarefa.Cancelar(usuario.Id, usuario.Login, request?.MotivoCancelamento);

        var descricao = string.IsNullOrWhiteSpace(tarefa.MotivoCancelamento)
            ? $"Tarefa vinculada cancelada: {tarefa.Titulo}."
            : $"Tarefa vinculada cancelada: {tarefa.Titulo}. Motivo: {tarefa.MotivoCancelamento}";

        await RegistrarHistoricoAsync(chamadoId, TipoHistoricoChamado.TarefaCancelada, descricao, usuario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<UsuarioContextoAplicacao> GarantirPermissaoOperacionalAsync(CancellationToken cancellationToken)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        return usuario;
    }

    private async Task GarantirChamadoAtivoAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken);

        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }
    }

    private async Task GarantirChamadoExisteAsync(Guid chamadoId, CancellationToken cancellationToken)
    {
        var chamadoExiste = await chamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == chamadoId, cancellationToken);

        if (!chamadoExiste)
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }
    }

    private async Task<ChamadoTarefa> ObterTarefaDoChamadoAsync(
        Guid chamadoId,
        Guid tarefaId,
        CancellationToken cancellationToken)
    {
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        var tarefa = await chamadoTarefaRepository.Query()
            .Include(x => x.ResponsavelUsuario)
            .FirstOrDefaultAsync(x => x.Id == tarefaId, cancellationToken)
            ?? throw new KeyNotFoundException("Tarefa vinculada nao encontrada.");

        if (tarefa.ChamadoId != chamadoId)
        {
            throw new InvalidOperationException("Tarefa vinculada nao pertence ao chamado informado.");
        }

        return tarefa;
    }

    private async Task RegistrarHistoricoAsync(
        Guid chamadoId,
        TipoHistoricoChamado tipo,
        string descricao,
        UsuarioContextoAplicacao usuario,
        CancellationToken cancellationToken)
    {
        await historicoChamadoRepository.AddAsync(new HistoricoChamado(
            chamadoId,
            tipo,
            descricao,
            usuario.Id,
            usuario.Login), cancellationToken);
    }

    private static ChamadoTarefaAdminResponse MapResponse(ChamadoTarefa tarefa, string? responsavelNome)
        => new()
        {
            Id = tarefa.Id,
            ChamadoId = tarefa.ChamadoId,
            Titulo = tarefa.Titulo,
            Descricao = tarefa.Descricao,
            Status = tarefa.Status,
            StatusDescricao = tarefa.Status.ToString(),
            ResponsavelUsuarioId = tarefa.ResponsavelUsuarioId,
            ResponsavelNome = responsavelNome,
            Prazo = tarefa.Prazo,
            CriadoEm = tarefa.CriadoEm,
            CriadoPor = tarefa.CriadoPor,
            AtualizadoEm = tarefa.AtualizadoEm,
            ConcluidoEm = tarefa.ConcluidoEm,
            CanceladoEm = tarefa.CanceladoEm,
            MotivoCancelamento = tarefa.MotivoCancelamento,
            Ativo = tarefa.Ativo
        };
}
