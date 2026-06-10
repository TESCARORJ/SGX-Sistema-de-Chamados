using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class InstanciaAprovacaoChamadoAdminUseCases(
    IRepository<InstanciaAprovacaoChamado> instanciaRepository,
    IRepository<Chamado> chamadoRepository,
    IRepository<ConfiguracaoRegraAprovacao> configuracaoRepository,
    IRepository<AprovacaoChamado> aprovacaoLegadaRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminInstanciaAprovacaoChamadoUseCases
{
    private static readonly ListarInstanciasAprovacaoChamadoRequestValidator ListarValidator = new();
    private static readonly PrepararInstanciaAprovacaoChamadoRequestValidator PrepararValidator = new();
    private static readonly CriarInstanciaAprovacaoChamadoManualRequestValidator CriarManualValidator = new();
    private static readonly ValidarInstanciaAprovacaoChamadoRequestValidator ValidarValidator = new();

    public async Task<PagedResultResponse<InstanciaAprovacaoChamadoResumoResponse>> ListarAsync(
        ListarInstanciasAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(ListarValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = AplicarFiltros(QueryInstancias(asNoTracking: true), request);
        query = AplicarOrdenacao(query, request.OrdenarPor, request.DirecaoOrdenacao);

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request.Pagina, request.TamanhoPagina);
        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<InstanciaAprovacaoChamadoResumoResponse>
        {
            Items = itens.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    public async Task<InstanciaAprovacaoChamadoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var instancia = await QueryInstanciasDetalhe(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Instancia de aprovacao nao encontrada.");

        return MapDetalhe(instancia);
    }

    public async Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPorChamadoAsync(
        Guid chamadoId,
        CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("ChamadoId invalido.", nameof(chamadoId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var itens = await QueryInstancias(asNoTracking: true)
            .Where(x => x.ChamadoId == chamadoId)
            .OrderByDescending(x => x.SolicitadaEm)
            .ToListAsync(cancellationToken);

        return itens.Select(MapResumo).ToArray();
    }

    public async Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPendentesAsync(
        Guid? aprovadorUsuarioId = null,
        CancellationToken cancellationToken = default)
    {
        if (aprovadorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("AprovadorUsuarioId invalido.", nameof(aprovadorUsuarioId));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = QueryInstancias(asNoTracking: true)
            .Where(x => x.Status == StatusInstanciaAprovacaoChamado.Pendente || x.Status == StatusInstanciaAprovacaoChamado.EmReavaliacao);

        if (aprovadorUsuarioId.HasValue)
        {
            var usuarioId = aprovadorUsuarioId.Value;
            query = query.Where(x =>
                x.AprovadorResolvidoUsuarioId == usuarioId ||
                x.AprovadorEspecificoUsuarioId == usuarioId ||
                x.AprovadorPadraoUsuarioId == usuarioId);
        }

        var itens = await query
            .OrderByDescending(x => x.Bloqueante)
            .ThenBy(x => x.DeveExpirarEm ?? DateTime.MaxValue)
            .ThenByDescending(x => x.SolicitadaEm)
            .ToListAsync(cancellationToken);

        return itens.Select(MapResumo).ToArray();
    }

    public async Task<IReadOnlyCollection<InstanciaAprovacaoChamadoResumoResponse>> ListarPorStatusAsync(
        StatusInstanciaAprovacaoChamado status,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var itens = await QueryInstancias(asNoTracking: true)
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.SolicitadaEm)
            .ToListAsync(cancellationToken);

        return itens.Select(MapResumo).ToArray();
    }

    public async Task<ValidarInstanciaAprovacaoChamadoResponse> ValidarAsync(
        ValidarInstanciaAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var erros = new List<string>();
        var alertas = new List<string>();
        var validacaoShape = await ValidarValidator.ValidateAsync(request, cancellationToken);

        if (!validacaoShape.IsValid)
        {
            erros.AddRange(validacaoShape.Errors.Select(x => x.ErrorMessage));
        }

        if (request.InstanciaAprovacaoChamadoId.HasValue)
        {
            var existe = await instanciaRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.InstanciaAprovacaoChamadoId.Value, cancellationToken);

            if (!existe)
            {
                erros.Add("InstanciaAprovacaoChamadoId informado nao foi encontrado.");
            }
        }

        await ValidarReferenciasAsync(request.Instancia, erros, alertas, cancellationToken);

        return new ValidarInstanciaAprovacaoChamadoResponse
        {
            Valida = erros.Count == 0,
            Erros = erros,
            Alertas = alertas
        };
    }

    public async Task<PrepararInstanciaAprovacaoChamadoResponse> PrepararAsync(
        PrepararInstanciaAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(PrepararValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var erros = new List<string>();
        var alertas = new List<string>();
        await ValidarReferenciasAsync(request, erros, alertas, cancellationToken);

        var modelo = await MontarModeloPreparacaoAsync(request, cancellationToken);

        if (modelo.EfeitoOperacional == EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco)
        {
            alertas.Add("A preparacao indica regra bloqueante, mas nenhum bloqueio operacional e executado neste item.");
        }

        alertas.Add("Preparacao conceitual concluida sem persistir instancia, etapas ou decisoes.");

        return new PrepararInstanciaAprovacaoChamadoResponse
        {
            PodeCriar = erros.Count == 0,
            Erros = erros,
            Alertas = alertas,
            Instancia = MapDetalhe(modelo, request.ChamadoId, request.SolicitanteId)
        };
    }

    public async Task<InstanciaAprovacaoChamadoResponse> CriarManualAsync(
        CriarInstanciaAprovacaoChamadoManualRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(CriarManualValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var erros = new List<string>();
        var alertas = new List<string>();
        await ValidarReferenciasAsync(MapPrepararRequest(request), erros, alertas, cancellationToken);

        if (erros.Count > 0)
        {
            LancarValidacao(erros);
        }

        var entidade = new InstanciaAprovacaoChamado(
            request.ChamadoId,
            request.SolicitanteId,
            request.Origem,
            request.TipoFluxoAprovacao,
            request.EfeitoOperacional,
            request.EscopoRegra,
            request.TipoRegra,
            request.ExigeAprovacao,
            request.Bloqueante,
            request.TipoResolucaoAprovador,
            usuarioAtual.Id,
            usuarioAtual.Login,
            request.ConfiguracaoRegraAprovacaoId,
            request.AprovacaoChamadoLegadaId,
            request.Titulo,
            request.Descricao,
            request.NaturezaChamado,
            request.TipoSolicitacaoId,
            request.CatalogoServicoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.ImpactoAvaliado,
            request.UrgenciaAvaliada,
            request.PrioridadeAvaliada,
            request.CustoAvaliado,
            request.NivelRiscoAvaliado,
            request.PermiteReenvio,
            request.PermiteFallback,
            request.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId,
            request.AprovadorResolvidoUsuarioId,
            request.PrazoDecisaoHoras,
            request.DeveExpirarEm,
            request.RegraNomeSnapshot,
            request.RegraVersaoSnapshot,
            request.RegraCriterioSnapshot);

        await instanciaRepository.AddAsync(entidade, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var criada = await QueryInstanciasDetalhe(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == entidade.Id, cancellationToken);

        return criada is not null
            ? MapDetalhe(criada)
            : MapDetalhe(entidade);
    }

    private IQueryable<InstanciaAprovacaoChamado> QueryInstancias(bool asNoTracking)
    {
        var query = instanciaRepository.Query()
            .Include(x => x.ConfiguracaoRegraAprovacao)
            .Include(x => x.AprovacaoChamadoLegada)
            .Include(x => x.Solicitante)
            .Include(x => x.AprovadorResolvidoUsuario)
            .Include(x => x.Etapas)
            .Include(x => x.Decisoes);

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<InstanciaAprovacaoChamado> QueryInstanciasDetalhe(bool asNoTracking)
    {
        var query = instanciaRepository.Query()
            .Include(x => x.ConfiguracaoRegraAprovacao)
            .Include(x => x.AprovacaoChamadoLegada)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.CatalogoServico)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.Solicitante)
            .Include(x => x.AprovadorEspecificoUsuario)
            .Include(x => x.AprovadorPadraoUsuario)
            .Include(x => x.AprovadorResolvidoUsuario)
            .Include(x => x.CanceladaPorUsuario)
            .Include(x => x.CriadoPorUsuario)
            .Include(x => x.AtualizadoPorUsuario)
            .Include(x => x.Etapas)
                .ThenInclude(x => x.AprovadorResolvidoUsuario)
            .Include(x => x.Etapas)
                .ThenInclude(x => x.AprovadorEspecificoUsuario)
            .Include(x => x.Etapas)
                .ThenInclude(x => x.AprovadorPadraoUsuario)
            .Include(x => x.Decisoes)
                .ThenInclude(x => x.DecisorUsuario);

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private static IQueryable<InstanciaAprovacaoChamado> AplicarFiltros(
        IQueryable<InstanciaAprovacaoChamado> query,
        ListarInstanciasAprovacaoChamadoRequest request)
    {
        if (request.ChamadoId.HasValue)
        {
            query = query.Where(x => x.ChamadoId == request.ChamadoId.Value);
        }

        if (request.ConfiguracaoRegraAprovacaoId.HasValue)
        {
            query = query.Where(x => x.ConfiguracaoRegraAprovacaoId == request.ConfiguracaoRegraAprovacaoId.Value);
        }

        if (request.AprovacaoChamadoLegadaId.HasValue)
        {
            query = query.Where(x => x.AprovacaoChamadoLegadaId == request.AprovacaoChamadoLegadaId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        if (request.Origem.HasValue)
        {
            query = query.Where(x => x.Origem == request.Origem.Value);
        }

        if (request.TipoFluxoAprovacao.HasValue)
        {
            query = query.Where(x => x.TipoFluxoAprovacao == request.TipoFluxoAprovacao.Value);
        }

        if (request.EfeitoOperacional.HasValue)
        {
            query = query.Where(x => x.EfeitoOperacional == request.EfeitoOperacional.Value);
        }

        if (request.EscopoRegra.HasValue)
        {
            query = query.Where(x => x.EscopoRegra == request.EscopoRegra.Value);
        }

        if (request.TipoRegra.HasValue)
        {
            query = query.Where(x => x.TipoRegra == request.TipoRegra.Value);
        }

        if (request.NaturezaChamado.HasValue)
        {
            query = query.Where(x => x.NaturezaChamado == request.NaturezaChamado.Value);
        }

        if (request.TipoSolicitacaoId.HasValue)
        {
            query = query.Where(x => x.TipoSolicitacaoId == request.TipoSolicitacaoId.Value);
        }

        if (request.CatalogoServicoId.HasValue)
        {
            query = query.Where(x => x.CatalogoServicoId == request.CatalogoServicoId.Value);
        }

        if (request.CategoriaId.HasValue)
        {
            query = query.Where(x => x.CategoriaId == request.CategoriaId.Value);
        }

        if (request.SubcategoriaId.HasValue)
        {
            query = query.Where(x => x.SubcategoriaId == request.SubcategoriaId.Value);
        }

        if (request.ImpactoAvaliado.HasValue)
        {
            query = query.Where(x => x.ImpactoAvaliado == request.ImpactoAvaliado.Value);
        }

        if (request.UrgenciaAvaliada.HasValue)
        {
            query = query.Where(x => x.UrgenciaAvaliada == request.UrgenciaAvaliada.Value);
        }

        if (request.PrioridadeAvaliada.HasValue)
        {
            query = query.Where(x => x.PrioridadeAvaliada == request.PrioridadeAvaliada.Value);
        }

        if (request.ExigeAprovacao.HasValue)
        {
            query = query.Where(x => x.ExigeAprovacao == request.ExigeAprovacao.Value);
        }

        if (request.Bloqueante.HasValue)
        {
            query = query.Where(x => x.Bloqueante == request.Bloqueante.Value);
        }

        if (request.TipoResolucaoAprovador.HasValue)
        {
            query = query.Where(x => x.TipoResolucaoAprovador == request.TipoResolucaoAprovador.Value);
        }

        if (request.AprovadorResolvidoUsuarioId.HasValue)
        {
            query = query.Where(x => x.AprovadorResolvidoUsuarioId == request.AprovadorResolvidoUsuarioId.Value);
        }

        if (request.SolicitanteId.HasValue)
        {
            query = query.Where(x => x.SolicitanteId == request.SolicitanteId.Value);
        }

        if (request.SolicitadaDe.HasValue)
        {
            query = query.Where(x => x.SolicitadaEm >= request.SolicitadaDe.Value);
        }

        if (request.SolicitadaAte.HasValue)
        {
            query = query.Where(x => x.SolicitadaEm <= request.SolicitadaAte.Value);
        }

        if (request.DeveExpirarDe.HasValue)
        {
            query = query.Where(x => x.DeveExpirarEm.HasValue && x.DeveExpirarEm.Value >= request.DeveExpirarDe.Value);
        }

        if (request.DeveExpirarAte.HasValue)
        {
            query = query.Where(x => x.DeveExpirarEm.HasValue && x.DeveExpirarEm.Value <= request.DeveExpirarAte.Value);
        }

        if (request.ApenasPendentes)
        {
            query = query.Where(x => x.Status == StatusInstanciaAprovacaoChamado.Pendente || x.Status == StatusInstanciaAprovacaoChamado.EmReavaliacao);
        }

        if (request.ApenasBloqueantes)
        {
            query = query.Where(x => x.Bloqueante);
        }

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim();
            query = query.Where(x => x.Titulo.Contains(termo) || (x.Descricao ?? string.Empty).Contains(termo) || (x.RegraNomeSnapshot ?? string.Empty).Contains(termo));
        }

        return query;
    }

    private static IQueryable<InstanciaAprovacaoChamado> AplicarOrdenacao(
        IQueryable<InstanciaAprovacaoChamado> query,
        string? ordenarPor,
        string? direcao)
    {
        var campo = string.IsNullOrWhiteSpace(ordenarPor) ? "solicitadaem" : ordenarPor.Trim().ToLowerInvariant();
        var desc = AdminCadastrosHelpers.DirecaoDesc(direcao);

        return campo switch
        {
            "titulo" => desc ? query.OrderByDescending(x => x.Titulo) : query.OrderBy(x => x.Titulo),
            "status" => desc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            "origem" => desc ? query.OrderByDescending(x => x.Origem) : query.OrderBy(x => x.Origem),
            "deveexpirarem" => desc ? query.OrderByDescending(x => x.DeveExpirarEm) : query.OrderBy(x => x.DeveExpirarEm),
            "decididaem" => desc ? query.OrderByDescending(x => x.DecididaEm) : query.OrderBy(x => x.DecididaEm),
            "criadoem" => desc ? query.OrderByDescending(x => x.CriadoEm) : query.OrderBy(x => x.CriadoEm),
            "atualizadoem" => desc ? query.OrderByDescending(x => x.AtualizadoEm) : query.OrderBy(x => x.AtualizadoEm),
            _ => desc ? query.OrderByDescending(x => x.SolicitadaEm) : query.OrderBy(x => x.SolicitadaEm)
        };
    }

    private async Task ValidarReferenciasAsync(
        PrepararInstanciaAprovacaoChamadoRequest request,
        ICollection<string> erros,
        ICollection<string> alertas,
        CancellationToken cancellationToken)
    {
        if (!await chamadoRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.ChamadoId, cancellationToken))
        {
            erros.Add("Chamado informado nao foi encontrado.");
        }

        if (!await usuarioRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.SolicitanteId, cancellationToken))
        {
            erros.Add("Solicitante informado nao foi encontrado.");
        }

        ConfiguracaoRegraAprovacao? configuracao = null;
        if (request.ConfiguracaoRegraAprovacaoId.HasValue)
        {
            configuracao = await configuracaoRepository.Query().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ConfiguracaoRegraAprovacaoId.Value, cancellationToken);

            if (configuracao is null)
            {
                erros.Add("ConfiguracaoRegraAprovacaoId informado nao foi encontrado.");
            }
            else
            {
                if (!configuracao.Ativo)
                {
                    alertas.Add("A configuracao de regra informada esta inativa.");
                }

                var agora = request.SolicitadaEm ?? DateTime.UtcNow;
                if (configuracao.VigenteDe.HasValue && configuracao.VigenteDe.Value > agora)
                {
                    alertas.Add("A configuracao de regra informada ainda nao iniciou vigencia.");
                }

                if (configuracao.VigenteAte.HasValue && configuracao.VigenteAte.Value < agora)
                {
                    alertas.Add("A configuracao de regra informada ja encerrou a vigencia.");
                }
            }
        }

        if (request.AprovacaoChamadoLegadaId.HasValue)
        {
            var aprovacaoLegada = await aprovacaoLegadaRepository.Query().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.AprovacaoChamadoLegadaId.Value, cancellationToken);

            if (aprovacaoLegada is null)
            {
                erros.Add("AprovacaoChamadoLegadaId informada nao foi encontrada.");
            }
            else
            {
                var emUso = await instanciaRepository.Query().AsNoTracking()
                    .AnyAsync(x => x.AprovacaoChamadoLegadaId == request.AprovacaoChamadoLegadaId.Value, cancellationToken);

                if (emUso)
                {
                    erros.Add("AprovacaoChamadoLegadaId informada ja esta vinculada a outra instancia.");
                }
            }
        }

        if (request.TipoSolicitacaoId.HasValue && !await tipoSolicitacaoRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.TipoSolicitacaoId.Value, cancellationToken))
        {
            erros.Add("TipoSolicitacaoId informado nao foi encontrado.");
        }

        if (request.CatalogoServicoId.HasValue && !await catalogoServicoRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.CatalogoServicoId.Value, cancellationToken))
        {
            erros.Add("CatalogoServicoId informado nao foi encontrado.");
        }

        if (request.CategoriaId.HasValue && !await categoriaRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.CategoriaId.Value, cancellationToken))
        {
            erros.Add("CategoriaId informada nao foi encontrada.");
        }

        if (request.SubcategoriaId.HasValue && !await subcategoriaRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.SubcategoriaId.Value, cancellationToken))
        {
            erros.Add("SubcategoriaId informada nao foi encontrada.");
        }

        if (request.AprovadorEspecificoUsuarioId.HasValue && !await usuarioRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.AprovadorEspecificoUsuarioId.Value, cancellationToken))
        {
            erros.Add("AprovadorEspecificoUsuarioId informado nao foi encontrado.");
        }

        if (request.AprovadorPadraoUsuarioId.HasValue && !await usuarioRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.AprovadorPadraoUsuarioId.Value, cancellationToken))
        {
            erros.Add("AprovadorPadraoUsuarioId informado nao foi encontrado.");
        }

        if (request.AprovadorResolvidoUsuarioId.HasValue && !await usuarioRepository.Query().AsNoTracking().AnyAsync(x => x.Id == request.AprovadorResolvidoUsuarioId.Value, cancellationToken))
        {
            erros.Add("AprovadorResolvidoUsuarioId informado nao foi encontrado.");
        }
    }

    private async Task<ModeloPreparacaoInstancia> MontarModeloPreparacaoAsync(
        PrepararInstanciaAprovacaoChamadoRequest request,
        CancellationToken cancellationToken)
    {
        var configuracao = request.ConfiguracaoRegraAprovacaoId.HasValue
            ? await configuracaoRepository.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ConfiguracaoRegraAprovacaoId.Value, cancellationToken)
            : null;

        var solicitadaEm = request.SolicitadaEm ?? DateTime.UtcNow;
        var prazoDecisaoHoras = request.PrazoDecisaoHoras ?? configuracao?.PrazoDecisaoHoras;
        var deveExpirarEm = request.DeveExpirarEm ?? (prazoDecisaoHoras.HasValue ? solicitadaEm.AddHours(prazoDecisaoHoras.Value) : null);

        return new ModeloPreparacaoInstancia(
            request.ConfiguracaoRegraAprovacaoId,
            configuracao?.Nome,
            request.AprovacaoChamadoLegadaId,
            string.IsNullOrWhiteSpace(request.Titulo) ? $"Aprovacao do chamado {request.ChamadoId}" : request.Titulo.Trim(),
            string.IsNullOrWhiteSpace(request.Descricao) ? configuracao?.Descricao : request.Descricao?.Trim(),
            StatusInstanciaAprovacaoChamado.Pendente,
            request.Origem,
            request.TipoFluxoAprovacao ?? configuracao?.TipoFluxoAprovacao ?? TipoFluxoAprovacao.Simples,
            request.EfeitoOperacional ?? configuracao?.EfeitoOperacional ?? EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            request.EscopoRegra ?? configuracao?.EscopoRegra ?? EscopoRegraAprovacao.AtendimentoChamado,
            request.TipoRegra ?? configuracao?.TipoRegra ?? TipoRegraAprovacao.Geral,
            request.NaturezaChamado ?? configuracao?.NaturezaChamado,
            request.TipoSolicitacaoId ?? configuracao?.TipoSolicitacaoId,
            request.CatalogoServicoId ?? configuracao?.CatalogoServicoId,
            request.CategoriaId ?? configuracao?.CategoriaId,
            request.SubcategoriaId ?? configuracao?.SubcategoriaId,
            request.ImpactoAvaliado,
            request.UrgenciaAvaliada,
            request.PrioridadeAvaliada,
            request.CustoAvaliado,
            request.NivelRiscoAvaliado,
            request.ExigeAprovacao ?? configuracao?.ExigeAprovacao ?? true,
            request.Bloqueante ?? configuracao?.Bloqueante ?? false,
            request.PermiteReenvio || configuracao?.PermiteReenvio == true,
            request.PermiteFallback || configuracao?.PermiteFallback == true,
            request.TipoResolucaoAprovador ?? configuracao?.TipoResolucaoAprovador ?? TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            request.AprovadorEspecificoUsuarioId ?? configuracao?.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId ?? configuracao?.AprovadorPadraoUsuarioId,
            request.AprovadorResolvidoUsuarioId,
            request.SolicitanteId,
            solicitadaEm,
            prazoDecisaoHoras,
            deveExpirarEm,
            null,
            null,
            null,
            null,
            null,
            string.IsNullOrWhiteSpace(request.RegraNomeSnapshot) ? configuracao?.Nome : request.RegraNomeSnapshot?.Trim(),
            request.RegraVersaoSnapshot ?? configuracao?.Versao,
            string.IsNullOrWhiteSpace(request.RegraCriterioSnapshot) ? configuracao?.Descricao : request.RegraCriterioSnapshot?.Trim());
    }

    private static PrepararInstanciaAprovacaoChamadoRequest MapPrepararRequest(CriarInstanciaAprovacaoChamadoManualRequest request)
        => new()
        {
            ChamadoId = request.ChamadoId,
            SolicitanteId = request.SolicitanteId,
            ConfiguracaoRegraAprovacaoId = request.ConfiguracaoRegraAprovacaoId,
            AprovacaoChamadoLegadaId = request.AprovacaoChamadoLegadaId,
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            Origem = request.Origem,
            TipoFluxoAprovacao = request.TipoFluxoAprovacao,
            EfeitoOperacional = request.EfeitoOperacional,
            EscopoRegra = request.EscopoRegra,
            TipoRegra = request.TipoRegra,
            NaturezaChamado = request.NaturezaChamado,
            TipoSolicitacaoId = request.TipoSolicitacaoId,
            CatalogoServicoId = request.CatalogoServicoId,
            CategoriaId = request.CategoriaId,
            SubcategoriaId = request.SubcategoriaId,
            ImpactoAvaliado = request.ImpactoAvaliado,
            UrgenciaAvaliada = request.UrgenciaAvaliada,
            PrioridadeAvaliada = request.PrioridadeAvaliada,
            CustoAvaliado = request.CustoAvaliado,
            NivelRiscoAvaliado = request.NivelRiscoAvaliado,
            ExigeAprovacao = request.ExigeAprovacao,
            Bloqueante = request.Bloqueante,
            PermiteReenvio = request.PermiteReenvio,
            PermiteFallback = request.PermiteFallback,
            TipoResolucaoAprovador = request.TipoResolucaoAprovador,
            AprovadorEspecificoUsuarioId = request.AprovadorEspecificoUsuarioId,
            AprovadorPadraoUsuarioId = request.AprovadorPadraoUsuarioId,
            AprovadorResolvidoUsuarioId = request.AprovadorResolvidoUsuarioId,
            PrazoDecisaoHoras = request.PrazoDecisaoHoras,
            DeveExpirarEm = request.DeveExpirarEm,
            RegraNomeSnapshot = request.RegraNomeSnapshot,
            RegraVersaoSnapshot = request.RegraVersaoSnapshot,
            RegraCriterioSnapshot = request.RegraCriterioSnapshot
        };

    private static InstanciaAprovacaoChamadoResumoResponse MapResumo(InstanciaAprovacaoChamado instancia)
        => new(
            instancia.Id,
            instancia.ChamadoId,
            instancia.ConfiguracaoRegraAprovacaoId,
            instancia.AprovacaoChamadoLegadaId,
            instancia.Titulo,
            instancia.Status,
            instancia.Status.ToString(),
            instancia.Origem,
            instancia.Origem.ToString(),
            instancia.TipoFluxoAprovacao,
            instancia.TipoFluxoAprovacao.ToString(),
            instancia.EfeitoOperacional,
            instancia.EfeitoOperacional.ToString(),
            instancia.ExigeAprovacao,
            instancia.Bloqueante,
            instancia.SolicitanteId,
            instancia.Solicitante?.Nome,
            instancia.AprovadorResolvidoUsuarioId,
            instancia.AprovadorResolvidoUsuario?.Nome,
            instancia.SolicitadaEm,
            instancia.DeveExpirarEm,
            instancia.DecididaEm,
            instancia.RegraNomeSnapshot,
            instancia.RegraVersaoSnapshot,
            instancia.Etapas.Count,
            instancia.Decisoes.Count,
            PossuiPendencia(instancia.Status),
            instancia.CriadoEm,
            instancia.AtualizadoEm);

    private static InstanciaAprovacaoChamadoResponse MapDetalhe(InstanciaAprovacaoChamado instancia)
        => new(
            instancia.Id,
            instancia.ChamadoId,
            instancia.ConfiguracaoRegraAprovacaoId,
            instancia.ConfiguracaoRegraAprovacao?.Nome,
            instancia.AprovacaoChamadoLegadaId,
            instancia.AprovacaoChamadoLegada?.Status.ToString(),
            instancia.Titulo,
            instancia.Descricao,
            instancia.Status,
            instancia.Status.ToString(),
            instancia.Origem,
            instancia.Origem.ToString(),
            instancia.TipoFluxoAprovacao,
            instancia.TipoFluxoAprovacao.ToString(),
            instancia.EfeitoOperacional,
            instancia.EfeitoOperacional.ToString(),
            instancia.EscopoRegra,
            instancia.EscopoRegra.ToString(),
            instancia.TipoRegra,
            instancia.TipoRegra.ToString(),
            instancia.NaturezaChamado,
            instancia.TipoSolicitacaoId,
            instancia.TipoSolicitacao?.Nome,
            instancia.CatalogoServicoId,
            instancia.CatalogoServico?.Nome,
            instancia.CategoriaId,
            instancia.Categoria?.Nome,
            instancia.SubcategoriaId,
            instancia.Subcategoria?.Nome,
            instancia.ImpactoAvaliado,
            instancia.UrgenciaAvaliada,
            instancia.PrioridadeAvaliada,
            instancia.CustoAvaliado,
            instancia.NivelRiscoAvaliado,
            instancia.ExigeAprovacao,
            instancia.Bloqueante,
            instancia.PermiteReenvio,
            instancia.PermiteFallback,
            instancia.TipoResolucaoAprovador,
            instancia.TipoResolucaoAprovador.ToString(),
            instancia.AprovadorEspecificoUsuarioId,
            instancia.AprovadorEspecificoUsuario?.Nome,
            instancia.AprovadorPadraoUsuarioId,
            instancia.AprovadorPadraoUsuario?.Nome,
            instancia.AprovadorResolvidoUsuarioId,
            instancia.AprovadorResolvidoUsuario?.Nome,
            instancia.SolicitanteId,
            instancia.Solicitante?.Nome,
            instancia.SolicitadaEm,
            instancia.PrazoDecisaoHoras,
            instancia.DeveExpirarEm,
            instancia.ExpiradaEm,
            instancia.CanceladaEm,
            instancia.CanceladaPorUsuarioId,
            instancia.CanceladaPorUsuario?.Nome,
            instancia.MotivoCancelamento,
            instancia.DecididaEm,
            instancia.RegraNomeSnapshot,
            instancia.RegraVersaoSnapshot,
            instancia.RegraCriterioSnapshot,
            instancia.Etapas
                .OrderBy(x => x.Nivel)
                .ThenBy(x => x.Ordem)
                .ThenBy(x => x.Ramo)
                .Select(x => new InstanciaAprovacaoChamadoEtapaResumoResponse(
                    x.Id,
                    x.Status,
                    x.Status.ToString(),
                    x.TipoEtapa,
                    x.TipoEtapa.ToString(),
                    x.TipoFluxoAprovacao,
                    x.TipoFluxoAprovacao.ToString(),
                    x.Ordem,
                    x.Nivel,
                    x.Ramo,
                    x.Obrigatoria,
                    x.CriticaParaConsolidacao,
                    ResolverAprovadorEtapaId(x),
                    ResolverAprovadorEtapaNome(x),
                    x.DeveExpirarEm,
                    x.DecididaEm))
                .ToArray(),
            instancia.Decisoes
                .OrderByDescending(x => x.DataDecisao)
                .Select(x => new InstanciaAprovacaoChamadoDecisaoResumoResponse(
                    x.Id,
                    x.InstanciaAprovacaoChamadoId,
                    x.EtapaAprovacaoChamadoId,
                    x.TipoDecisao,
                    x.TipoDecisao.ToString(),
                    x.Resultado,
                    x.Resultado.ToString(),
                    x.DataDecisao,
                    x.DecisorUsuarioId,
                    x.DecisorUsuario?.Nome,
                    x.DecisaoParcial,
                    x.DecisaoFinal,
                    x.LiberaAvanco,
                    x.MantemBloqueio,
                    x.ExigeReavaliacao,
                    x.CancelaFluxo))
                .ToArray(),
            instancia.Etapas.Count,
            instancia.Decisoes.Count,
            PossuiPendencia(instancia.Status),
            instancia.CriadoPorUsuarioId,
            instancia.CriadoPorUsuario?.Nome,
            instancia.AtualizadoPorUsuarioId,
            instancia.AtualizadoPorUsuario?.Nome,
            instancia.CriadoEm,
            instancia.AtualizadoEm);

    private static InstanciaAprovacaoChamadoResponse MapDetalhe(
        ModeloPreparacaoInstancia modelo,
        Guid chamadoId,
        Guid solicitanteId)
        => new(
            Guid.Empty,
            chamadoId,
            modelo.ConfiguracaoRegraAprovacaoId,
            modelo.ConfiguracaoRegraNome,
            modelo.AprovacaoChamadoLegadaId,
            null,
            modelo.Titulo,
            modelo.Descricao,
            modelo.Status,
            modelo.Status.ToString(),
            modelo.Origem,
            modelo.Origem.ToString(),
            modelo.TipoFluxoAprovacao,
            modelo.TipoFluxoAprovacao.ToString(),
            modelo.EfeitoOperacional,
            modelo.EfeitoOperacional.ToString(),
            modelo.EscopoRegra,
            modelo.EscopoRegra.ToString(),
            modelo.TipoRegra,
            modelo.TipoRegra.ToString(),
            modelo.NaturezaChamado,
            modelo.TipoSolicitacaoId,
            null,
            modelo.CatalogoServicoId,
            null,
            modelo.CategoriaId,
            null,
            modelo.SubcategoriaId,
            null,
            modelo.ImpactoAvaliado,
            modelo.UrgenciaAvaliada,
            modelo.PrioridadeAvaliada,
            modelo.CustoAvaliado,
            modelo.NivelRiscoAvaliado,
            modelo.ExigeAprovacao,
            modelo.Bloqueante,
            modelo.PermiteReenvio,
            modelo.PermiteFallback,
            modelo.TipoResolucaoAprovador,
            modelo.TipoResolucaoAprovador.ToString(),
            modelo.AprovadorEspecificoUsuarioId,
            null,
            modelo.AprovadorPadraoUsuarioId,
            null,
            modelo.AprovadorResolvidoUsuarioId,
            null,
            solicitanteId,
            null,
            modelo.SolicitadaEm,
            modelo.PrazoDecisaoHoras,
            modelo.DeveExpirarEm,
            modelo.ExpiradaEm,
            modelo.CanceladaEm,
            modelo.CanceladaPorUsuarioId,
            null,
            modelo.MotivoCancelamento,
            modelo.DecididaEm,
            modelo.RegraNomeSnapshot,
            modelo.RegraVersaoSnapshot,
            modelo.RegraCriterioSnapshot,
            [],
            [],
            0,
            0,
            PossuiPendencia(modelo.Status),
            Guid.Empty,
            null,
            null,
            null,
            default,
            null);

    private static bool PossuiPendencia(StatusInstanciaAprovacaoChamado status)
        => status == StatusInstanciaAprovacaoChamado.Pendente || status == StatusInstanciaAprovacaoChamado.EmReavaliacao;

    private static Guid? ResolverAprovadorEtapaId(EtapaAprovacaoChamado etapa)
        => etapa.AprovadorResolvidoUsuarioId ?? etapa.AprovadorEspecificoUsuarioId ?? etapa.AprovadorPadraoUsuarioId;

    private static string? ResolverAprovadorEtapaNome(EtapaAprovacaoChamado etapa)
        => etapa.AprovadorResolvidoUsuario?.Nome ?? etapa.AprovadorEspecificoUsuario?.Nome ?? etapa.AprovadorPadraoUsuario?.Nome;

    private static void LancarValidacao(IEnumerable<string> erros)
        => throw new ValidationException(erros.Select(erro => new FluentValidation.Results.ValidationFailure(string.Empty, erro)));

    private static async Task ValidarOuFalharAsync<T>(IValidator<T> validator, T request, CancellationToken cancellationToken)
        => await validator.ValidateAndThrowAsync(request, cancellationToken);

    private sealed record ModeloPreparacaoInstancia(
        Guid? ConfiguracaoRegraAprovacaoId,
        string? ConfiguracaoRegraNome,
        Guid? AprovacaoChamadoLegadaId,
        string Titulo,
        string? Descricao,
        StatusInstanciaAprovacaoChamado Status,
        OrigemInstanciaAprovacaoChamado Origem,
        TipoFluxoAprovacao TipoFluxoAprovacao,
        EfeitoOperacionalRegraAprovacao EfeitoOperacional,
        EscopoRegraAprovacao EscopoRegra,
        TipoRegraAprovacao TipoRegra,
        NaturezaChamadoEnum? NaturezaChamado,
        Guid? TipoSolicitacaoId,
        Guid? CatalogoServicoId,
        Guid? CategoriaId,
        Guid? SubcategoriaId,
        ImpactoChamadoEnum? ImpactoAvaliado,
        UrgenciaChamadoEnum? UrgenciaAvaliada,
        PrioridadeChamadoEnum? PrioridadeAvaliada,
        decimal? CustoAvaliado,
        int? NivelRiscoAvaliado,
        bool ExigeAprovacao,
        bool Bloqueante,
        bool PermiteReenvio,
        bool PermiteFallback,
        TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador,
        Guid? AprovadorEspecificoUsuarioId,
        Guid? AprovadorPadraoUsuarioId,
        Guid? AprovadorResolvidoUsuarioId,
        Guid SolicitanteId,
        DateTime SolicitadaEm,
        int? PrazoDecisaoHoras,
        DateTime? DeveExpirarEm,
        DateTime? ExpiradaEm,
        DateTime? CanceladaEm,
        Guid? CanceladaPorUsuarioId,
        string? MotivoCancelamento,
        DateTime? DecididaEm,
        string? RegraNomeSnapshot,
        int? RegraVersaoSnapshot,
        string? RegraCriterioSnapshot);
}
