using System.Reflection;
using System.Runtime.ExceptionServices;
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

public sealed class ConfiguracaoRegraAprovacaoAdminUseCases(
    IRepository<ConfiguracaoRegraAprovacao> configuracaoRegraAprovacaoRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdminConfiguracaoRegraAprovacaoUseCases
{
    private static readonly ListarConfiguracoesRegrasAprovacaoRequestValidator ListarValidator = new();
    private static readonly CriarConfiguracaoRegraAprovacaoRequestValidator CriarValidator = new();
    private static readonly AtualizarConfiguracaoRegraAprovacaoRequestValidator AtualizarValidator = new();
    private static readonly AlterarStatusConfiguracaoRegraAprovacaoRequestValidator AlterarStatusValidator = new();
    private static readonly ValidarConfiguracaoRegraAprovacaoRequestValidator ValidarValidator = new();
    private static readonly ContextoAvaliacaoRegraAprovacaoRequestValidator ContextoValidator = new();

    public async Task<PagedResultResponse<ConfiguracaoRegraAprovacaoResumoResponse>> ListarAsync(
        ListarConfiguracoesRegrasAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(ListarValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var query = QueryConfiguracoes(asNoTracking: true);

        if (!string.IsNullOrWhiteSpace(request.Termo))
        {
            var termo = request.Termo.Trim();
            query = query.Where(x =>
                x.Nome.Contains(termo) ||
                (x.Descricao ?? string.Empty).Contains(termo));
        }

        if (request.Ativo.HasValue)
        {
            query = query.Where(x => x.Ativo == request.Ativo.Value);
        }

        if (request.TipoRegra.HasValue)
        {
            query = query.Where(x => x.TipoRegra == request.TipoRegra.Value);
        }

        if (request.EscopoRegra.HasValue)
        {
            query = query.Where(x => x.EscopoRegra == request.EscopoRegra.Value);
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

        if (request.EfeitoOperacional.HasValue)
        {
            query = query.Where(x => x.EfeitoOperacional == request.EfeitoOperacional.Value);
        }

        if (request.TipoFluxoAprovacao.HasValue)
        {
            query = query.Where(x => x.TipoFluxoAprovacao == request.TipoFluxoAprovacao.Value);
        }

        if (request.TipoResolucaoAprovador.HasValue)
        {
            query = query.Where(x => x.TipoResolucaoAprovador == request.TipoResolucaoAprovador.Value);
        }

        if (request.Bloqueante.HasValue)
        {
            query = query.Where(x => x.Bloqueante == request.Bloqueante.Value);
        }

        if (request.ExigeAprovacao.HasValue)
        {
            query = query.Where(x => x.ExigeAprovacao == request.ExigeAprovacao.Value);
        }

        if (request.VigenteEm.HasValue)
        {
            var dataReferencia = request.VigenteEm.Value;
            query = query.Where(x =>
                (!x.VigenteDe.HasValue || x.VigenteDe.Value <= dataReferencia) &&
                (!x.VigenteAte.HasValue || x.VigenteAte.Value >= dataReferencia));
        }

        query = AplicarOrdenacao(query, request.OrdenarPor, request.DirecaoOrdenacao);

        var (pagina, tamanho) = AdminCadastrosHelpers.NormalizarPaginacao(request.Pagina, request.TamanhoPagina);
        var total = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .ToListAsync(cancellationToken);

        return new PagedResultResponse<ConfiguracaoRegraAprovacaoResumoResponse>
        {
            Items = itens.Select(MapResumo).ToArray(),
            Total = total,
            Pagina = pagina,
            TamanhoPagina = tamanho
        };
    }

    public async Task<ConfiguracaoRegraAprovacaoResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var regra = await QueryConfiguracoes(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra de aprovacao nao encontrada.");

        return MapDetalhe(regra);
    }

    public async Task<ConfiguracaoRegraAprovacaoResponse> CriarAsync(
        CriarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(CriarValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        await ValidarRelacionamentosAsync(
            request.TipoSolicitacaoId,
            request.CatalogoServicoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId,
            cancellationToken);

        var nomeNormalizado = request.Nome.Trim();
        var duplicada = await configuracaoRegraAprovacaoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Nome == nomeNormalizado && x.Versao == request.Versao, cancellationToken);

        if (duplicada)
        {
            throw new InvalidOperationException("Ja existe configuracao de regra com o mesmo nome e versao.");
        }

        var regra = CriarEntidade(request, usuarioAtual);
        if (!request.Ativo)
        {
            regra.DesativarRegra(usuarioAtual.Id, usuarioAtual.Login);
        }

        await configuracaoRegraAprovacaoRepository.AddAsync(regra, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var criada = await QueryConfiguracoes(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == regra.Id, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra de aprovacao nao encontrada apos criacao.");

        return MapDetalhe(criada);
    }

    public async Task<ConfiguracaoRegraAprovacaoResponse> AtualizarAsync(
        Guid id,
        AtualizarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        await ValidarOuFalharAsync(AtualizarValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var regra = await QueryConfiguracoes(asNoTracking: false)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra de aprovacao nao encontrada.");

        await ValidarRelacionamentosAsync(
            request.TipoSolicitacaoId,
            request.CatalogoServicoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId,
            cancellationToken);

        var nomeNormalizado = request.Nome.Trim();
        var duplicada = await configuracaoRegraAprovacaoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id != id && x.Nome == nomeNormalizado && x.Versao == request.Versao, cancellationToken);

        if (duplicada)
        {
            throw new InvalidOperationException("Ja existe configuracao de regra com o mesmo nome e versao.");
        }

        var regraValidada = CriarEntidade(request, usuarioAtual);
        if (!request.Ativo)
        {
            regraValidada.DesativarRegra(usuarioAtual.Id, usuarioAtual.Login);
        }

        ConfiguracaoRegraAprovacaoUpdater.AplicarAtualizacao(regra, regraValidada, usuarioAtual);

        configuracaoRegraAprovacaoRepository.Update(regra);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizada = await QueryConfiguracoes(asNoTracking: true)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra de aprovacao nao encontrada apos atualizacao.");

        return MapDetalhe(atualizada);
    }

    public async Task<AlterarSituacaoCadastroResponse> AlterarStatusAsync(
        Guid id,
        AlterarStatusConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id invalido.", nameof(id));
        }

        await ValidarOuFalharAsync(AlterarStatusValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdministrador(usuarioAtual);

        var regra = await configuracaoRegraAprovacaoRepository.Query()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra de aprovacao nao encontrada.");

        if (request.Ativo)
        {
            regra.AtivarRegra(usuarioAtual.Id, usuarioAtual.Login);
        }
        else
        {
            regra.DesativarRegra(usuarioAtual.Id, usuarioAtual.Login);
        }

        configuracaoRegraAprovacaoRepository.Update(regra);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var mensagem = request.Ativo
            ? "Configuracao de regra de aprovacao ativada com sucesso."
            : "Configuracao de regra de aprovacao inativada com sucesso.";

        return new AlterarSituacaoCadastroResponse(regra.Id, regra.Ativo, mensagem);
    }

    public async Task<ValidarConfiguracaoRegraAprovacaoResponse> ValidarAsync(
        ValidarConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var resultado = await ValidarValidator.ValidateAsync(request, cancellationToken);
        var erros = resultado.Errors.Select(x => x.ErrorMessage).Distinct().ToList();

        if (!erros.Any())
        {
            try
            {
                await ValidarRelacionamentosAsync(
                    request.Configuracao.TipoSolicitacaoId,
                    request.Configuracao.CatalogoServicoId,
                    request.Configuracao.CategoriaId,
                    request.Configuracao.SubcategoriaId,
                    request.Configuracao.AprovadorEspecificoUsuarioId,
                    request.Configuracao.AprovadorPadraoUsuarioId,
                    cancellationToken);

                var nomeNormalizado = request.Configuracao.Nome.Trim();
                var duplicada = await configuracaoRegraAprovacaoRepository.Query()
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Nome == nomeNormalizado &&
                        x.Versao == request.Configuracao.Versao &&
                        (!request.ConfiguracaoRegraAprovacaoId.HasValue || x.Id != request.ConfiguracaoRegraAprovacaoId.Value),
                        cancellationToken);

                if (duplicada)
                {
                    erros.Add("Ja existe configuracao de regra com o mesmo nome e versao.");
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                erros.Add(ex.Message);
            }
        }

        var alertas = CriarAlertasValidacao(request.Configuracao);

        return new ValidarConfiguracaoRegraAprovacaoResponse
        {
            Valida = erros.Count == 0,
            Erros = erros,
            Alertas = alertas
        };
    }

    public async Task<IReadOnlyCollection<RegraAprovacaoCandidataResponse>> ListarRegrasCandidatasAsync(
        ContextoAvaliacaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        await ValidarOuFalharAsync(ContextoValidator, request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        AdminCadastrosHelpers.GarantirAdminOuAtendente(usuarioAtual);

        var dataReferencia = request.DataReferencia ?? DateTime.UtcNow;

        var regras = await configuracaoRegraAprovacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .ToListAsync(cancellationToken);

        return regras
            .Where(x => x.EstaVigenteEm(dataReferencia))
            .Where(x => RegraSatisfazContexto(x, request))
            .OrderByDescending(x => x.Prioridade)
            .ThenByDescending(CalcularEspecificidade)
            .ThenBy(x => x.Ordem)
            .ThenByDescending(x => x.Versao)
            .ThenBy(x => x.Nome)
            .Select(x => MapCandidata(x, "Regra ativa, vigente e compatível com o contexto informado."))
            .ToArray();
    }

    public async Task<AvaliacaoConfiguracaoRegraAprovacaoResponse> AvaliarRegraAsync(
        ContextoAvaliacaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        var candidatas = await ListarRegrasCandidatasAsync(request, cancellationToken);
        var melhor = candidatas.FirstOrDefault();

        if (melhor is null)
        {
            return new AvaliacaoConfiguracaoRegraAprovacaoResponse
            {
                RegraAplicavel = false,
                Motivo = "Nenhuma configuracao de regra ativa e vigente correspondeu ao contexto informado.",
                Avisos = ["A avaliacao e apenas conceitual e nao gera aprovacao, bloqueio ou alteracao de status."]
            };
        }

        var avisos = new List<string>
        {
            "A avaliacao e apenas conceitual e nao gera aprovacao, instancias, etapas, decisoes ou bloqueio operacional."
        };

        if (melhor.TipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.NaoDefinido && melhor.ExigeAprovacao)
        {
            avisos.Add("A melhor regra exige aprovacao, mas a estrategia de resolucao de aprovador ainda nao esta definida.");
        }

        return new AvaliacaoConfiguracaoRegraAprovacaoResponse
        {
            RegraAplicavel = true,
            MelhorRegra = melhor,
            RegrasCandidatas = candidatas,
            ExigeAprovacao = melhor.ExigeAprovacao,
            Bloqueante = melhor.Bloqueante,
            EfeitoOperacional = melhor.EfeitoOperacional,
            TipoFluxoAprovacao = melhor.TipoFluxoAprovacao,
            TipoResolucaoAprovador = melhor.TipoResolucaoAprovador,
            AprovadorEspecificoUsuarioId = melhor.AprovadorEspecificoUsuarioId,
            AprovadorPadraoUsuarioId = melhor.AprovadorPadraoUsuarioId,
            PrazoDecisaoHoras = melhor.PrazoDecisaoHoras,
            Motivo = "Melhor regra selecionada por prioridade, especificidade, ordem e versao.",
            Avisos = avisos
        };
    }

    private IQueryable<ConfiguracaoRegraAprovacao> QueryConfiguracoes(bool asNoTracking)
    {
        var query = configuracaoRegraAprovacaoRepository.Query()
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.CatalogoServico)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.AprovadorEspecificoUsuario)
            .Include(x => x.AprovadorPadraoUsuario)
            .AsQueryable();

        return asNoTracking ? query.AsNoTracking() : query;
    }

    private IQueryable<ConfiguracaoRegraAprovacao> AplicarOrdenacao(
        IQueryable<ConfiguracaoRegraAprovacao> query,
        string? ordenarPor,
        string? direcaoOrdenacao)
    {
        var desc = AdminCadastrosHelpers.DirecaoDesc(direcaoOrdenacao);

        return (ordenarPor ?? "prioridade").Trim().ToLowerInvariant() switch
        {
            "nome" => desc ? query.OrderByDescending(x => x.Nome) : query.OrderBy(x => x.Nome),
            "ordem" => desc ? query.OrderByDescending(x => x.Ordem) : query.OrderBy(x => x.Ordem),
            "versao" => desc ? query.OrderByDescending(x => x.Versao) : query.OrderBy(x => x.Versao),
            "criadoem" => desc ? query.OrderByDescending(x => x.CriadoEm) : query.OrderBy(x => x.CriadoEm),
            "atualizadoem" => desc ? query.OrderByDescending(x => x.AtualizadoEm ?? x.CriadoEm) : query.OrderBy(x => x.AtualizadoEm ?? x.CriadoEm),
            "vigentede" => desc ? query.OrderByDescending(x => x.VigenteDe) : query.OrderBy(x => x.VigenteDe),
            "vigenteate" => desc ? query.OrderByDescending(x => x.VigenteAte) : query.OrderBy(x => x.VigenteAte),
            _ => desc
                ? query.OrderByDescending(x => x.Prioridade).ThenBy(x => x.Ordem).ThenByDescending(x => x.Versao)
                : query.OrderBy(x => x.Prioridade).ThenBy(x => x.Ordem).ThenBy(x => x.Versao)
        };
    }

    private async Task ValidarRelacionamentosAsync(
        Guid? tipoSolicitacaoId,
        Guid? catalogoServicoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        Guid? aprovadorEspecificoUsuarioId,
        Guid? aprovadorPadraoUsuarioId,
        CancellationToken cancellationToken)
    {
        if (tipoSolicitacaoId.HasValue)
        {
            var tipoSolicitacaoValido = await tipoSolicitacaoRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == tipoSolicitacaoId.Value && x.Ativo, cancellationToken);

            if (!tipoSolicitacaoValido)
            {
                throw new InvalidOperationException("Tipo de solicitacao informado nao encontrado ou inativo.");
            }
        }

        if (catalogoServicoId.HasValue)
        {
            var catalogoValido = await catalogoServicoRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == catalogoServicoId.Value && x.Ativo, cancellationToken);

            if (!catalogoValido)
            {
                throw new InvalidOperationException("Catalogo de servico informado nao encontrado ou inativo.");
            }
        }

        if (categoriaId.HasValue)
        {
            var categoriaValida = await categoriaRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == categoriaId.Value && x.Ativo, cancellationToken);

            if (!categoriaValida)
            {
                throw new InvalidOperationException("Categoria informada nao encontrada ou inativa.");
            }
        }

        if (subcategoriaId.HasValue)
        {
            var subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == subcategoriaId.Value && x.Ativo, cancellationToken);

            if (subcategoria is null)
            {
                throw new InvalidOperationException("Subcategoria informada nao encontrada ou inativa.");
            }

            if (categoriaId.HasValue && subcategoria.CategoriaChamadoId != categoriaId.Value)
            {
                throw new InvalidOperationException("Subcategoria informada nao pertence a categoria selecionada.");
            }
        }

        if (aprovadorEspecificoUsuarioId.HasValue)
        {
            var usuarioValido = await usuarioRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == aprovadorEspecificoUsuarioId.Value && x.Ativo, cancellationToken);

            if (!usuarioValido)
            {
                throw new InvalidOperationException("Aprovador especifico informado nao encontrado ou inativo.");
            }
        }

        if (aprovadorPadraoUsuarioId.HasValue)
        {
            var usuarioValido = await usuarioRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Id == aprovadorPadraoUsuarioId.Value && x.Ativo, cancellationToken);

            if (!usuarioValido)
            {
                throw new InvalidOperationException("Aprovador padrao informado nao encontrado ou inativo.");
            }
        }
    }

    private static ConfiguracaoRegraAprovacao CriarEntidade(
        CriarConfiguracaoRegraAprovacaoRequest request,
        UsuarioContextoAplicacao usuarioAtual)
        => new(
            request.Nome,
            request.TipoRegra,
            request.EscopoRegra,
            request.EfeitoOperacional,
            request.TipoFluxoAprovacao,
            request.TipoResolucaoAprovador,
            request.Ordem,
            request.Prioridade,
            request.Versao,
            usuarioAtual.Id,
            usuarioAtual.Login,
            request.Descricao,
            request.NaturezaChamado,
            request.TipoSolicitacaoId,
            request.CatalogoServicoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.ImpactoMinimo,
            request.UrgenciaMinima,
            request.PrioridadeMinima,
            request.CustoMinimo,
            request.NivelRiscoMinimo,
            request.ExigeAprovacao,
            request.Bloqueante,
            request.PermiteReenvio,
            request.PermiteFallback,
            request.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId,
            request.PrazoDecisaoHoras,
            request.VigenteDe,
            request.VigenteAte);

    private static ConfiguracaoRegraAprovacao CriarEntidade(
        AtualizarConfiguracaoRegraAprovacaoRequest request,
        UsuarioContextoAplicacao usuarioAtual)
        => new(
            request.Nome,
            request.TipoRegra,
            request.EscopoRegra,
            request.EfeitoOperacional,
            request.TipoFluxoAprovacao,
            request.TipoResolucaoAprovador,
            request.Ordem,
            request.Prioridade,
            request.Versao,
            usuarioAtual.Id,
            usuarioAtual.Login,
            request.Descricao,
            request.NaturezaChamado,
            request.TipoSolicitacaoId,
            request.CatalogoServicoId,
            request.CategoriaId,
            request.SubcategoriaId,
            request.ImpactoMinimo,
            request.UrgenciaMinima,
            request.PrioridadeMinima,
            request.CustoMinimo,
            request.NivelRiscoMinimo,
            request.ExigeAprovacao,
            request.Bloqueante,
            request.PermiteReenvio,
            request.PermiteFallback,
            request.AprovadorEspecificoUsuarioId,
            request.AprovadorPadraoUsuarioId,
            request.PrazoDecisaoHoras,
            request.VigenteDe,
            request.VigenteAte);

    private static ConfiguracaoRegraAprovacaoResumoResponse MapResumo(ConfiguracaoRegraAprovacao regra)
        => new(
            regra.Id,
            regra.Nome,
            regra.TipoRegra,
            regra.TipoRegra.ToString(),
            regra.EscopoRegra,
            regra.EscopoRegra.ToString(),
            regra.EfeitoOperacional,
            regra.EfeitoOperacional.ToString(),
            regra.TipoFluxoAprovacao,
            regra.TipoFluxoAprovacao.ToString(),
            regra.TipoResolucaoAprovador,
            regra.TipoResolucaoAprovador.ToString(),
            regra.NaturezaChamado,
            regra.ExigeAprovacao,
            regra.Bloqueante,
            regra.Prioridade,
            regra.Versao,
            regra.Ativo,
            regra.VigenteDe,
            regra.VigenteAte,
            regra.CriadoEm,
            regra.AtualizadoEm);

    private static ConfiguracaoRegraAprovacaoResponse MapDetalhe(ConfiguracaoRegraAprovacao regra)
        => new(
            regra.Id,
            regra.Nome,
            regra.Descricao,
            regra.TipoRegra,
            regra.TipoRegra.ToString(),
            regra.EscopoRegra,
            regra.EscopoRegra.ToString(),
            regra.Ordem,
            regra.Prioridade,
            regra.Versao,
            regra.NaturezaChamado,
            regra.TipoSolicitacaoId,
            regra.TipoSolicitacao?.Nome,
            regra.CatalogoServicoId,
            regra.CatalogoServico?.Nome,
            regra.CategoriaId,
            regra.Categoria?.Nome,
            regra.SubcategoriaId,
            regra.Subcategoria?.Nome,
            regra.ImpactoMinimo,
            regra.UrgenciaMinima,
            regra.PrioridadeMinima,
            regra.CustoMinimo,
            regra.NivelRiscoMinimo,
            regra.ExigeAprovacao,
            regra.Bloqueante,
            regra.PermiteReenvio,
            regra.PermiteFallback,
            regra.EfeitoOperacional,
            regra.EfeitoOperacional.ToString(),
            regra.TipoFluxoAprovacao,
            regra.TipoFluxoAprovacao.ToString(),
            regra.TipoResolucaoAprovador,
            regra.TipoResolucaoAprovador.ToString(),
            regra.AprovadorEspecificoUsuarioId,
            regra.AprovadorEspecificoUsuario?.Nome,
            regra.AprovadorPadraoUsuarioId,
            regra.AprovadorPadraoUsuario?.Nome,
            regra.PrazoDecisaoHoras,
            regra.VigenteDe,
            regra.VigenteAte,
            regra.Ativo,
            regra.CriadoPorUsuarioId,
            regra.AtualizadoPorUsuarioId,
            regra.CriadoEm,
            regra.AtualizadoEm);

    private static bool RegraSatisfazContexto(ConfiguracaoRegraAprovacao regra, ContextoAvaliacaoRegraAprovacaoRequest contexto)
    {
        if (regra.NaturezaChamado.HasValue && regra.NaturezaChamado != contexto.NaturezaChamado)
        {
            return false;
        }

        if (regra.TipoSolicitacaoId.HasValue && regra.TipoSolicitacaoId != contexto.TipoSolicitacaoId)
        {
            return false;
        }

        if (regra.CatalogoServicoId.HasValue && regra.CatalogoServicoId != contexto.CatalogoServicoId)
        {
            return false;
        }

        if (regra.CategoriaId.HasValue && regra.CategoriaId != contexto.CategoriaId)
        {
            return false;
        }

        if (regra.SubcategoriaId.HasValue && regra.SubcategoriaId != contexto.SubcategoriaId)
        {
            return false;
        }

        if (regra.ImpactoMinimo.HasValue && (!contexto.ImpactoChamado.HasValue || contexto.ImpactoChamado.Value < regra.ImpactoMinimo.Value))
        {
            return false;
        }

        if (regra.UrgenciaMinima.HasValue && (!contexto.UrgenciaChamado.HasValue || contexto.UrgenciaChamado.Value < regra.UrgenciaMinima.Value))
        {
            return false;
        }

        if (regra.PrioridadeMinima.HasValue && (!contexto.PrioridadeChamado.HasValue || contexto.PrioridadeChamado.Value < regra.PrioridadeMinima.Value))
        {
            return false;
        }

        if (regra.CustoMinimo.HasValue && (!contexto.Custo.HasValue || contexto.Custo.Value < regra.CustoMinimo.Value))
        {
            return false;
        }

        if (regra.NivelRiscoMinimo.HasValue && (!contexto.NivelRisco.HasValue || contexto.NivelRisco.Value < regra.NivelRiscoMinimo.Value))
        {
            return false;
        }

        return true;
    }

    private static int CalcularEspecificidade(ConfiguracaoRegraAprovacao regra)
    {
        var total = 0;
        if (regra.NaturezaChamado.HasValue) total++;
        if (regra.TipoSolicitacaoId.HasValue) total++;
        if (regra.CatalogoServicoId.HasValue) total++;
        if (regra.CategoriaId.HasValue) total++;
        if (regra.SubcategoriaId.HasValue) total++;
        if (regra.ImpactoMinimo.HasValue) total++;
        if (regra.UrgenciaMinima.HasValue) total++;
        if (regra.PrioridadeMinima.HasValue) total++;
        if (regra.CustoMinimo.HasValue) total++;
        if (regra.NivelRiscoMinimo.HasValue) total++;
        return total;
    }

    private static RegraAprovacaoCandidataResponse MapCandidata(ConfiguracaoRegraAprovacao regra, string motivo)
        => new(
            regra.Id,
            regra.Nome,
            regra.Versao,
            regra.Prioridade,
            regra.Ordem,
            CalcularEspecificidade(regra),
            regra.ExigeAprovacao,
            regra.Bloqueante,
            regra.EfeitoOperacional,
            regra.EfeitoOperacional.ToString(),
            regra.TipoFluxoAprovacao,
            regra.TipoFluxoAprovacao.ToString(),
            regra.TipoResolucaoAprovador,
            regra.TipoResolucaoAprovador.ToString(),
            regra.AprovadorEspecificoUsuarioId,
            regra.AprovadorPadraoUsuarioId,
            regra.PrazoDecisaoHoras,
            motivo);

    private static IReadOnlyCollection<string> CriarAlertasValidacao(CriarConfiguracaoRegraAprovacaoRequest request)
    {
        var alertas = new List<string>();

        if (!request.Ativo)
        {
            alertas.Add("A configuracao sera criada inativa e nao sera considerada em avaliacoes puras ate ser ativada.");
        }

        if (!request.NaturezaChamado.HasValue &&
            !request.TipoSolicitacaoId.HasValue &&
            !request.CatalogoServicoId.HasValue &&
            !request.CategoriaId.HasValue &&
            !request.SubcategoriaId.HasValue &&
            !request.ImpactoMinimo.HasValue &&
            !request.UrgenciaMinima.HasValue &&
            !request.PrioridadeMinima.HasValue &&
            !request.CustoMinimo.HasValue &&
            !request.NivelRiscoMinimo.HasValue)
        {
            alertas.Add("A configuracao nao possui criterios especificos e pode se tornar ampla demais para avaliacoes futuras.");
        }

        if (request.ExigeAprovacao && request.TipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.NaoDefinido)
        {
            alertas.Add("A configuracao exige aprovacao, mas ainda nao define uma estrategia objetiva de resolucao de aprovador.");
        }

        return alertas;
    }

    private static async Task ValidarOuFalharAsync<T>(
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var resultado = await validator.ValidateAsync(request, cancellationToken);
        if (!resultado.IsValid)
        {
            throw new ValidationException(resultado.Errors);
        }
    }

    private static class ConfiguracaoRegraAprovacaoUpdater
    {
        private static readonly string[] PropriedadesMutaveis =
        [
            nameof(ConfiguracaoRegraAprovacao.Nome),
            nameof(ConfiguracaoRegraAprovacao.Descricao),
            nameof(ConfiguracaoRegraAprovacao.TipoRegra),
            nameof(ConfiguracaoRegraAprovacao.EscopoRegra),
            nameof(ConfiguracaoRegraAprovacao.Ordem),
            nameof(ConfiguracaoRegraAprovacao.Prioridade),
            nameof(ConfiguracaoRegraAprovacao.Versao),
            nameof(ConfiguracaoRegraAprovacao.NaturezaChamado),
            nameof(ConfiguracaoRegraAprovacao.TipoSolicitacaoId),
            nameof(ConfiguracaoRegraAprovacao.CatalogoServicoId),
            nameof(ConfiguracaoRegraAprovacao.CategoriaId),
            nameof(ConfiguracaoRegraAprovacao.SubcategoriaId),
            nameof(ConfiguracaoRegraAprovacao.ImpactoMinimo),
            nameof(ConfiguracaoRegraAprovacao.UrgenciaMinima),
            nameof(ConfiguracaoRegraAprovacao.PrioridadeMinima),
            nameof(ConfiguracaoRegraAprovacao.CustoMinimo),
            nameof(ConfiguracaoRegraAprovacao.NivelRiscoMinimo),
            nameof(ConfiguracaoRegraAprovacao.ExigeAprovacao),
            nameof(ConfiguracaoRegraAprovacao.Bloqueante),
            nameof(ConfiguracaoRegraAprovacao.PermiteReenvio),
            nameof(ConfiguracaoRegraAprovacao.PermiteFallback),
            nameof(ConfiguracaoRegraAprovacao.EfeitoOperacional),
            nameof(ConfiguracaoRegraAprovacao.TipoFluxoAprovacao),
            nameof(ConfiguracaoRegraAprovacao.TipoResolucaoAprovador),
            nameof(ConfiguracaoRegraAprovacao.AprovadorEspecificoUsuarioId),
            nameof(ConfiguracaoRegraAprovacao.AprovadorPadraoUsuarioId),
            nameof(ConfiguracaoRegraAprovacao.PrazoDecisaoHoras),
            nameof(ConfiguracaoRegraAprovacao.VigenteDe),
            nameof(ConfiguracaoRegraAprovacao.VigenteAte),
            nameof(ConfiguracaoRegraAprovacao.Ativo)
        ];

        private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void AplicarAtualizacao(
            ConfiguracaoRegraAprovacao destino,
            ConfiguracaoRegraAprovacao origemValidada,
            UsuarioContextoAplicacao usuarioAtual)
        {
            foreach (var nomePropriedade in PropriedadesMutaveis)
            {
                var propriedade = typeof(ConfiguracaoRegraAprovacao).GetProperty(nomePropriedade, Flags)
                    ?? throw new InvalidOperationException($"Propriedade {nomePropriedade} nao encontrada para atualizar regra de aprovacao.");

                propriedade.SetValue(destino, propriedade.GetValue(origemValidada));
            }

            InvocarMetodoPrivado(destino, "RegistrarAtualizacaoUsuario", usuarioAtual.Id, usuarioAtual.Login);
        }

        private static void InvocarMetodoPrivado(object alvo, string nomeMetodo, params object?[] argumentos)
        {
            var metodo = alvo.GetType().GetMethod(nomeMetodo, Flags)
                ?? throw new InvalidOperationException($"Metodo {nomeMetodo} nao encontrado para atualizar regra de aprovacao.");

            try
            {
                metodo.Invoke(alvo, argumentos);
            }
            catch (TargetInvocationException ex) when (ex.InnerException is not null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }
    }
}
