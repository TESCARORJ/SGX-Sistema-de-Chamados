using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AprovacaoChamadosAdminUseCases(
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IRepository<Chamado> chamadoRepository,
    IRepository<HistoricoChamado> historicoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAdminAprovacaoChamadosUseCases
{
    public async Task<PagedResultResponse<AprovacaoChamadoListagemDto>> ListarAsync(
        FiltroAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Chamado)
            .Include(x => x.Solicitante)
            .Include(x => x.Aprovador)
            .AsQueryable();

        if (request.ChamadoId.HasValue)
        {
            query = query.Where(x => x.ChamadoId == request.ChamadoId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.TipoOrigem.HasValue)
        {
            query = query.Where(x => x.TipoOrigem == request.TipoOrigem.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.AprovadorId.HasValue)
        {
            query = query.Where(x => x.AprovadorId == request.AprovadorId.Value);
        }

        if (request.DataSolicitacaoInicial.HasValue)
        {
            var dataInicial = request.DataSolicitacaoInicial.Value.Date;
            query = query.Where(x => x.SolicitadaEm >= dataInicial);
        }

        if (request.DataSolicitacaoFinal.HasValue)
        {
            var dataFinalExclusiva = request.DataSolicitacaoFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.SolicitadaEm < dataFinalExclusiva);
        }

        if (request.DataDecisaoInicial.HasValue)
        {
            var dataInicial = request.DataDecisaoInicial.Value.Date;
            query = query.Where(x => x.DecididaEm.HasValue && x.DecididaEm.Value >= dataInicial);
        }

        if (request.DataDecisaoFinal.HasValue)
        {
            var dataFinalExclusiva = request.DataDecisaoFinal.Value.Date.AddDays(1);
            query = query.Where(x => x.DecididaEm.HasValue && x.DecididaEm.Value < dataFinalExclusiva);
        }

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim();
            query = query.Where(x =>
                x.Chamado.Codigo.Contains(termo) ||
                x.Chamado.Titulo.Contains(termo));
        }

        query = AplicarOrdenacao(query, request.OrdenarPor, request.DirecaoOrdenacao);

        var pagina = request.Pagina <= 0 ? 1 : request.Pagina;
        var tamanhoPagina = request.TamanhoPagina <= 0 ? 20 : Math.Min(request.TamanhoPagina, 100);

        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<AprovacaoChamadoListagemDto>
        {
            Items = itens.Select(MapListagem).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };
    }

    public async Task<AprovacaoChamadoDetalheDto> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da aprovacao invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var aprovacao = await QueryCompleta(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Aprovacao de chamado nao encontrada.");

        return MapDetalhe(aprovacao);
    }

    public async Task<AprovacaoChamadoDetalheDto> SolicitarAsync(
        Guid chamadoId,
        SolicitarAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("ChamadoId invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var jaExistePendente = await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.Ativo &&
                x.ChamadoId == chamadoId &&
                x.Status == StatusAprovacaoChamado.Pendente,
                cancellationToken);

        if (jaExistePendente)
        {
            throw new InvalidOperationException("Ja existe aprovacao pendente ativa para este chamado.");
        }

        var aprovacao = new AprovacaoChamado(
            chamadoId,
            request.TipoOrigem,
            usuarioAtual.Id,
            usuarioAtual.Login,
            chamado.SolicitanteId,
            request.OrigemDescricao,
            request.JustificativaSolicitacao);

        await aprovacaoChamadoRepository.AddAsync(aprovacao, cancellationToken);

        await AdicionarHistoricoAsync(
            chamadoId,
            TipoHistoricoChamado.AprovacaoSolicitada,
            usuarioAtual,
            $"Aprovacao solicitada ({request.TipoOrigem}).",
            cancellationToken);

        try
        {
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (EhViolacaoAprovacaoPendenteDuplicada(ex))
        {
            throw new InvalidOperationException("Ja existe aprovacao pendente ativa para este chamado.");
        }

        if (auditoriaService is not null)
        {
            await auditoriaService.RegistrarCriacaoAsync(
                "Aprovacao de Chamados",
                "AprovacaoChamado",
                aprovacao.Id.ToString(),
                "Aprovacao de chamado solicitada.",
                dadosDepois: AuditoriaDiffHelper.SerializarSeguro(new
                {
                    aprovacao.Id,
                    aprovacao.ChamadoId,
                    Status = aprovacao.Status.ToString(),
                    TipoOrigem = aprovacao.TipoOrigem.ToString(),
                    aprovacao.OrigemDescricao,
                    aprovacao.JustificativaSolicitacao,
                    aprovacao.SolicitadaEm
                }),
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Aprovacao de Chamados",
                    entidade: "AprovacaoChamado",
                    entidadeId: aprovacao.Id.ToString(),
                    codigo: chamado.Codigo,
                    nome: chamado.Titulo,
                    operacao: "SolicitarAprovacao",
                    resultado: "Sucesso"),
                cancellationToken: cancellationToken);
        }

        var aprovadoCriado = await QueryCompleta(asNoTracking: true)
            .FirstAsync(x => x.Id == aprovacao.Id, cancellationToken);

        return MapDetalhe(aprovadoCriado);
    }

    public Task<AprovacaoChamadoDetalheDto> AprovarAsync(
        Guid id,
        DecidirAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
        => DecidirAsync(id, request, TipoDecisao.Aprovar, cancellationToken);

    public Task<AprovacaoChamadoDetalheDto> ReprovarAsync(
        Guid id,
        DecidirAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
        => DecidirAsync(id, request, TipoDecisao.Reprovar, cancellationToken);

    public async Task<AprovacaoChamadoDetalheDto> CancelarAsync(
        Guid id,
        CancelarAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da aprovacao invalido.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(request.JustificativaDecisao))
        {
            throw new ArgumentException("JustificativaDecisao e obrigatoria para cancelamento.", nameof(request));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var aprovacao = await QueryCompleta(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Aprovacao de chamado nao encontrada.");

        if (aprovacao.Status != StatusAprovacaoChamado.Pendente)
        {
            throw new InvalidOperationException("Somente aprovacoes pendentes podem ser canceladas.");
        }

        var statusAnterior = aprovacao.Status;
        aprovacao.DefinirAprovador(usuarioAtual.Id, usuarioAtual.Id, usuarioAtual.Login);
        aprovacao.Cancelar(usuarioAtual.Id, usuarioAtual.Login, request.JustificativaDecisao);
        aprovacaoChamadoRepository.Update(aprovacao);

        await AdicionarHistoricoAsync(
            aprovacao.ChamadoId,
            TipoHistoricoChamado.AprovacaoCancelada,
            usuarioAtual,
            "Aprovacao cancelada.",
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new { Status = statusAnterior.ToString() },
                new { Status = aprovacao.Status.ToString(), aprovacao.JustificativaDecisao, aprovacao.DecididaEm });

            await auditoriaService.RegistrarEdicaoAsync(
                "Aprovacao de Chamados",
                "AprovacaoChamado",
                aprovacao.Id.ToString(),
                "Aprovacao de chamado cancelada.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Aprovacao de Chamados",
                    entidade: "AprovacaoChamado",
                    entidadeId: aprovacao.Id.ToString(),
                    codigo: aprovacao.Chamado.Codigo,
                    nome: aprovacao.Chamado.Titulo,
                    operacao: "CancelarAprovacao",
                    resultado: "Sucesso"),
                cancellationToken: cancellationToken);
        }

        var aprovadoAtualizado = await QueryCompleta(asNoTracking: true)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return MapDetalhe(aprovadoAtualizado);
    }

    private async Task<AprovacaoChamadoDetalheDto> DecidirAsync(
        Guid id,
        DecidirAprovacaoChamadoRequest request,
        TipoDecisao tipoDecisao,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id da aprovacao invalido.", nameof(id));
        }

        if (tipoDecisao == TipoDecisao.Reprovar && string.IsNullOrWhiteSpace(request.JustificativaDecisao))
        {
            throw new ArgumentException("JustificativaDecisao e obrigatoria para reprovacao.", nameof(request));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var aprovacao = await QueryCompleta(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Aprovacao de chamado nao encontrada.");

        if (aprovacao.Status != StatusAprovacaoChamado.Pendente)
        {
            throw new InvalidOperationException("Somente aprovacoes pendentes podem ser decididas.");
        }

        var statusAnterior = aprovacao.Status;
        if (tipoDecisao == TipoDecisao.Aprovar)
        {
            aprovacao.Aprovar(usuarioAtual.Id, usuarioAtual.Id, usuarioAtual.Login, request.JustificativaDecisao);
        }
        else
        {
            aprovacao.Reprovar(usuarioAtual.Id, usuarioAtual.Id, usuarioAtual.Login, request.JustificativaDecisao);
        }

        aprovacaoChamadoRepository.Update(aprovacao);

        var tipoHistorico = tipoDecisao == TipoDecisao.Aprovar
            ? TipoHistoricoChamado.ChamadoAprovado
            : TipoHistoricoChamado.ChamadoReprovado;
        var descricaoHistorico = tipoDecisao == TipoDecisao.Aprovar
            ? "Chamado aprovado."
            : "Chamado reprovado.";

        await AdicionarHistoricoAsync(
            aprovacao.ChamadoId,
            tipoHistorico,
            usuarioAtual,
            descricaoHistorico,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            var (dadosAntes, dadosDepois) = AuditoriaDiffHelper.CriarDiff(
                new { Status = statusAnterior.ToString() },
                new { Status = aprovacao.Status.ToString(), aprovacao.JustificativaDecisao, aprovacao.DecididaEm });

            await auditoriaService.RegistrarEdicaoAsync(
                "Aprovacao de Chamados",
                "AprovacaoChamado",
                aprovacao.Id.ToString(),
                tipoDecisao == TipoDecisao.Aprovar
                    ? "Aprovacao de chamado aprovada."
                    : "Aprovacao de chamado reprovada.",
                dadosAntes: dadosAntes,
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Aprovacao de Chamados",
                    entidade: "AprovacaoChamado",
                    entidadeId: aprovacao.Id.ToString(),
                    codigo: aprovacao.Chamado.Codigo,
                    nome: aprovacao.Chamado.Titulo,
                    operacao: tipoDecisao == TipoDecisao.Aprovar ? "AprovarAprovacao" : "ReprovarAprovacao",
                    resultado: "Sucesso"),
                cancellationToken: cancellationToken);
        }

        var aprovadoAtualizado = await QueryCompleta(asNoTracking: true)
            .FirstAsync(x => x.Id == id, cancellationToken);

        return MapDetalhe(aprovadoAtualizado);
    }

    private async Task AdicionarHistoricoAsync(
        Guid chamadoId,
        TipoHistoricoChamado tipo,
        UsuarioContextoAplicacao usuarioAtual,
        string detalhe,
        CancellationToken cancellationToken)
    {
        var historico = new HistoricoChamado(
            chamadoId,
            tipo,
            AdminUseCaseHelpers.ObterDescricaoHistorico(tipo, detalhe),
            usuarioAtual.Id,
            usuarioAtual.Login);

        await historicoChamadoRepository.AddAsync(historico, cancellationToken);
    }

    private IQueryable<AprovacaoChamado> QueryCompleta(bool asNoTracking)
    {
        var query = aprovacaoChamadoRepository.Query()
            .Include(x => x.Chamado)
            .Include(x => x.Solicitante)
            .Include(x => x.Aprovador)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private static IQueryable<AprovacaoChamado> AplicarOrdenacao(
        IQueryable<AprovacaoChamado> query,
        string? ordenarPor,
        string? direcao)
    {
        var desc = string.Equals(direcao, "desc", StringComparison.OrdinalIgnoreCase);
        var campo = (ordenarPor ?? string.Empty).Trim().ToLowerInvariant();

        return campo switch
        {
            "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "tipoorigem" => desc ? query.OrderByDescending(x => x.TipoOrigem) : query.OrderBy(x => x.TipoOrigem),
            "codigo" => desc ? query.OrderByDescending(x => x.Chamado.Codigo) : query.OrderBy(x => x.Chamado.Codigo),
            "titulo" => desc ? query.OrderByDescending(x => x.Chamado.Titulo) : query.OrderBy(x => x.Chamado.Titulo),
            "decididaem" => desc ? query.OrderByDescending(x => x.DecididaEm) : query.OrderBy(x => x.DecididaEm),
            _ => desc
                ? query.OrderByDescending(x => x.SolicitadaEm).ThenByDescending(x => x.CriadoEm)
                : query.OrderBy(x => x.SolicitadaEm).ThenBy(x => x.CriadoEm)
        };
    }

    private static AprovacaoChamadoListagemDto MapListagem(AprovacaoChamado aprovacao)
        => new()
        {
            Id = aprovacao.Id,
            ChamadoId = aprovacao.ChamadoId,
            NumeroProtocoloChamado = aprovacao.Chamado.Codigo,
            TituloChamado = aprovacao.Chamado.Titulo,
            Status = aprovacao.Status,
            StatusDescricao = aprovacao.Status.ToString(),
            TipoOrigem = aprovacao.TipoOrigem,
            TipoOrigemDescricao = aprovacao.TipoOrigem.ToString(),
            OrigemDescricao = aprovacao.OrigemDescricao,
            SolicitanteId = aprovacao.SolicitanteId,
            SolicitanteNome = aprovacao.Solicitante?.Nome,
            AprovadorId = aprovacao.AprovadorId,
            AprovadorNome = aprovacao.Aprovador?.Nome,
            SolicitadaEm = aprovacao.SolicitadaEm,
            DecididaEm = aprovacao.DecididaEm,
            Ativo = aprovacao.Ativo
        };

    private static AprovacaoChamadoDetalheDto MapDetalhe(AprovacaoChamado aprovacao)
        => new()
        {
            Id = aprovacao.Id,
            ChamadoId = aprovacao.ChamadoId,
            NumeroProtocoloChamado = aprovacao.Chamado.Codigo,
            TituloChamado = aprovacao.Chamado.Titulo,
            DescricaoChamado = aprovacao.Chamado.Descricao,
            Status = aprovacao.Status,
            StatusDescricao = aprovacao.Status.ToString(),
            TipoOrigem = aprovacao.TipoOrigem,
            TipoOrigemDescricao = aprovacao.TipoOrigem.ToString(),
            OrigemDescricao = aprovacao.OrigemDescricao,
            SolicitanteId = aprovacao.SolicitanteId,
            SolicitanteNome = aprovacao.Solicitante?.Nome,
            AprovadorId = aprovacao.AprovadorId,
            AprovadorNome = aprovacao.Aprovador?.Nome,
            JustificativaSolicitacao = aprovacao.JustificativaSolicitacao,
            JustificativaDecisao = aprovacao.JustificativaDecisao,
            SolicitadaEm = aprovacao.SolicitadaEm,
            DecididaEm = aprovacao.DecididaEm,
            CriadoEm = aprovacao.CriadoEm,
            AtualizadoEm = aprovacao.AtualizadoEm,
            Ativo = aprovacao.Ativo
        };

    private static bool EhViolacaoAprovacaoPendenteDuplicada(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("ux_aprovacoes_chamado_chamado_id_pendente_ativo", StringComparison.OrdinalIgnoreCase) == true
           || ex.Message.Contains("ux_aprovacoes_chamado_chamado_id_pendente_ativo", StringComparison.OrdinalIgnoreCase);

    private enum TipoDecisao
    {
        Aprovar = 1,
        Reprovar = 2
    }
}
