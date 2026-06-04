using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class ChamadoAprovacoesUseCases(
    IRepository<Chamado> chamadoRepository,
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminChamadoAprovacoesUseCases
{
    public async Task<ChamadoAprovacaoAdminResponse> CriarAsync(
        Guid chamadoId,
        CriarChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.Titulo))
        {
            throw new ArgumentException("O titulo da aprovacao e obrigatorio.", nameof(request));
        }

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        Usuario? aprovador = null;
        if (request.AprovadorUsuarioId.HasValue)
        {
            aprovador = await ObterUsuarioAtivoAsync(request.AprovadorUsuarioId.Value, "Aprovador nao encontrado ou inativo.", cancellationToken);
        }

        var solicitadoPorId = request.SolicitadoPorUsuarioId ?? usuario.Id;
        var solicitadoPor = await ObterUsuarioAtivoAsync(solicitadoPorId, "Solicitante da aprovacao nao encontrado ou inativo.", cancellationToken);

        var aprovacao = new AprovacaoChamado(
            chamadoId,
            TipoOrigemAprovacaoChamado.Manual,
            usuario.Id,
            usuario.Login,
            solicitadoPor.Id,
            "Aprovacao vinculada ao chamado",
            request.JustificativaSolicitacao,
            request.Titulo,
            request.Descricao,
            aprovador?.Id,
            bloqueiaAvancoAtendimento: false);

        await aprovacaoChamadoRepository.AddAsync(aprovacao, cancellationToken);
        await RegistrarHistoricoAsync(
            chamadoId,
            TipoHistoricoChamado.AprovacaoCriada,
            $"Aprovacao vinculada criada: {aprovacao.Titulo}.",
            usuario,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapResponse(aprovacao, aprovador?.Nome, solicitadoPor.Nome);
    }

    public async Task<IReadOnlyList<ChamadoAprovacaoAdminResponse>> ListarPorChamadoAsync(
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

        var query = aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Aprovador)
            .Include(x => x.Solicitante)
            .Where(x => x.ChamadoId == chamadoId);

        if (!incluirInativas)
        {
            query = query.Where(x => x.Ativo);
        }

        var aprovacoes = await query
            .OrderBy(x => x.Status != StatusAprovacaoChamado.Pendente)
            .ThenByDescending(x => x.SolicitadaEm)
            .ToListAsync(cancellationToken);

        return aprovacoes
            .Select(x => MapResponse(x, x.Aprovador?.Nome, x.Solicitante?.Nome))
            .ToList();
    }

    public Task<ChamadoAprovacaoAdminResponse> AprovarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default)
        => DecidirAsync(chamadoId, aprovacaoId, request, aprovar: true, cancellationToken);

    public Task<ChamadoAprovacaoAdminResponse> ReprovarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        DecidirChamadoAprovacaoAdminRequest request,
        CancellationToken cancellationToken = default)
        => DecidirAsync(chamadoId, aprovacaoId, request, aprovar: false, cancellationToken);

    public async Task CancelarAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        CancelarChamadoAprovacaoAdminRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        var aprovacao = await ObterAprovacaoDoChamadoAsync(chamadoId, aprovacaoId, cancellationToken);

        aprovacao.CancelarVinculada(usuario.Id, usuario.Login, request?.MotivoCancelamento);

        var descricao = string.IsNullOrWhiteSpace(aprovacao.MotivoCancelamento)
            ? $"Aprovacao vinculada cancelada: {aprovacao.Titulo}."
            : $"Aprovacao vinculada cancelada: {aprovacao.Titulo}. Motivo: {aprovacao.MotivoCancelamento}";

        await RegistrarHistoricoAsync(chamadoId, TipoHistoricoChamado.AprovacaoCancelada, descricao, usuario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> PossuiAprovacaoPendenteAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        return await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.ChamadoId == chamadoId &&
                x.Ativo &&
                x.Status == StatusAprovacaoChamado.Pendente,
                cancellationToken);
    }

    public async Task<bool> PossuiAprovacaoPendenteBloqueanteAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        await GarantirPermissaoOperacionalAsync(cancellationToken);
        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        return await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.ChamadoId == chamadoId &&
                x.Ativo &&
                x.BloqueiaAvancoAtendimento &&
                x.Status == StatusAprovacaoChamado.Pendente,
                cancellationToken);
    }

    private async Task<ChamadoAprovacaoAdminResponse> DecidirAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        DecidirChamadoAprovacaoAdminRequest request,
        bool aprovar,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuario = await GarantirPermissaoOperacionalAsync(cancellationToken);
        var aprovacao = await ObterAprovacaoDoChamadoAsync(chamadoId, aprovacaoId, cancellationToken);

        if (aprovar)
        {
            aprovacao.Aprovar(usuario.Id, usuario.Id, usuario.Login, request.JustificativaDecisao);
        }
        else
        {
            aprovacao.Reprovar(usuario.Id, usuario.Id, usuario.Login, request.JustificativaDecisao);
        }

        var tipoHistorico = aprovar
            ? TipoHistoricoChamado.AprovacaoAprovada
            : TipoHistoricoChamado.AprovacaoReprovada;
        var descricao = aprovar
            ? $"Aprovacao vinculada aprovada: {aprovacao.Titulo}."
            : $"Aprovacao vinculada reprovada: {aprovacao.Titulo}.";

        if (!string.IsNullOrWhiteSpace(aprovacao.JustificativaDecisao))
        {
            descricao += $" Justificativa: {aprovacao.JustificativaDecisao}";
        }

        await RegistrarHistoricoAsync(chamadoId, tipoHistorico, descricao, usuario, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapResponse(aprovacao, aprovacao.Aprovador?.Nome ?? usuario.Nome, aprovacao.Solicitante?.Nome);
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

    private async Task<AprovacaoChamado> ObterAprovacaoDoChamadoAsync(
        Guid chamadoId,
        Guid aprovacaoId,
        CancellationToken cancellationToken)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado informado e invalido.", nameof(chamadoId));
        }

        if (aprovacaoId == Guid.Empty)
        {
            throw new ArgumentException("A aprovacao informada e invalida.", nameof(aprovacaoId));
        }

        await GarantirChamadoExisteAsync(chamadoId, cancellationToken);

        var aprovacao = await aprovacaoChamadoRepository.Query()
            .Include(x => x.Aprovador)
            .Include(x => x.Solicitante)
            .FirstOrDefaultAsync(x => x.Id == aprovacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("Aprovacao vinculada nao encontrada.");

        if (aprovacao.ChamadoId != chamadoId)
        {
            throw new InvalidOperationException("Aprovacao vinculada nao pertence ao chamado informado.");
        }

        return aprovacao;
    }

    private async Task<Usuario> ObterUsuarioAtivoAsync(Guid usuarioId, string mensagemErro, CancellationToken cancellationToken)
        => await usuarioRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == usuarioId && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException(mensagemErro);

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

    private static ChamadoAprovacaoAdminResponse MapResponse(
        AprovacaoChamado aprovacao,
        string? aprovadorNome,
        string? solicitadoPorNome)
        => new()
        {
            Id = aprovacao.Id,
            ChamadoId = aprovacao.ChamadoId,
            Titulo = aprovacao.Titulo,
            Descricao = aprovacao.Descricao,
            Status = aprovacao.Status,
            StatusDescricao = aprovacao.Status.ToString(),
            AprovadorUsuarioId = aprovacao.AprovadorId,
            AprovadorNome = aprovadorNome,
            SolicitadoPorUsuarioId = aprovacao.SolicitanteId,
            SolicitadoPorNome = solicitadoPorNome,
            JustificativaSolicitacao = aprovacao.JustificativaSolicitacao,
            JustificativaDecisao = aprovacao.JustificativaDecisao,
            BloqueiaAvancoAtendimento = aprovacao.BloqueiaAvancoAtendimento,
            SolicitadaEm = aprovacao.SolicitadaEm,
            DecididoEm = aprovacao.DecididaEm,
            CanceladoEm = aprovacao.CanceladoEm,
            MotivoCancelamento = aprovacao.MotivoCancelamento,
            CriadoEm = aprovacao.CriadoEm,
            Ativo = aprovacao.Ativo
        };
}
