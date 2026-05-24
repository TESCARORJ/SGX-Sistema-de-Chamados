using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AbrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    ICodigoChamadoService codigoChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAbrirChamadoUseCase
{
    private const string DescricaoHistoricoCriacaoPortal = "Chamado criado pelo portal";
    private const string DescricaoHistoricoCriacaoCatalogo = "Chamado aberto a partir do servico do catalogo";

    public async Task<ChamadoDetalheResponse> ExecutarAsync(CriarChamadoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var servicoCatalogo = await ResolverServicoCatalogoAsync(request, usuarioAtual, cancellationToken);

        var categoriaIdEfetiva = servicoCatalogo?.CategoriaId ?? request.CategoriaId;
        if (!categoriaIdEfetiva.HasValue || categoriaIdEfetiva.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Categoria obrigatoria.");
        }

        var prioridadeIdEfetiva = servicoCatalogo?.PrioridadePadraoId ?? request.PrioridadeId;
        if (!prioridadeIdEfetiva.HasValue || prioridadeIdEfetiva.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Prioridade obrigatoria.");
        }

        var departamentoIdEfetivo = servicoCatalogo?.DepartamentoResponsavelId ?? request.DepartamentoId;
        var subcategoriaIdEfetiva = servicoCatalogo?.SubcategoriaId ?? request.SubcategoriaId;
        var catalogoServicoIdEfetivo = servicoCatalogo?.Id;

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == categoriaIdEfetiva.Value && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria nao encontrada ou inativa.");

        var prioridade = await prioridadeRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == prioridadeIdEfetiva.Value && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Prioridade nao encontrada ou inativa.");

        SubcategoriaChamado? subcategoria = null;
        if (subcategoriaIdEfetiva.HasValue)
        {
            subcategoria = await subcategoriaRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == subcategoriaIdEfetiva.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Subcategoria nao encontrada ou inativa.");

            if (subcategoria.CategoriaChamadoId != categoria.Id)
            {
                throw new InvalidOperationException("A subcategoria selecionada nao pertence a categoria informada.");
            }
        }

        if (request.TipoSolicitacaoId.HasValue)
        {
            _ = await tipoSolicitacaoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.TipoSolicitacaoId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Tipo de solicitacao nao encontrado ou inativo.");
        }

        if (request.LocalUnidadeId.HasValue)
        {
            _ = await localUnidadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.LocalUnidadeId.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Local/unidade nao encontrado ou inativo.");
        }

        if (departamentoIdEfetivo.HasValue)
        {
            _ = await departamentoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == departamentoIdEfetivo.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Departamento nao encontrado ou inativo.");
        }

        var statusAberto = await statusRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Ativo && x.Codigo == StatusChamadoEnum.Aberto, cancellationToken)
            ?? throw new InvalidOperationException("Status inicial 'Aberto' nao encontrado.");

        var codigo = await codigoChamadoService.GerarAsync(cancellationToken);
        var chamado = new Chamado(
            codigo,
            request.Titulo,
            request.Descricao,
            usuarioAtual.Id,
            categoria.Id,
            prioridade.Id,
            statusAberto.Id,
            OrigemChamado.Portal,
            usuarioAtual.Login,
            departamentoIdEfetivo,
            subcategoria?.Id,
            request.TipoSolicitacaoId,
            request.LocalUnidadeId,
            catalogoServicoIdEfetivo);

        await chamadoRepository.AddAsync(chamado, cancellationToken);

        var historicoCriado = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.Criado,
            DescricaoHistoricoCriacaoPortal,
            usuarioAtual.Id,
            usuarioAtual.Login);

        await historicoRepository.AddAsync(historicoCriado, cancellationToken);

        if (servicoCatalogo is not null)
        {
            var descricaoCatalogo = $"{DescricaoHistoricoCriacaoCatalogo}: {servicoCatalogo.Nome} - departamento {servicoCatalogo.DepartamentoResponsavel?.Nome ?? "nao informado"}.";
            var historicoCatalogo = new HistoricoChamado(
                chamado.Id,
                TipoHistoricoChamado.ChamadoCriadoPorCatalogoServico,
                descricaoCatalogo,
                usuarioAtual.Id,
                usuarioAtual.Login);

            await historicoRepository.AddAsync(historicoCatalogo, cancellationToken);
        }

        await slaService.InicializarNaAberturaAsync(chamado, usuarioAtual.Login, DateTime.UtcNow, servicoCatalogo?.SlaPadraoId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var chamadoCriado = await chamadoRepository.Query()
            .Include(x => x.Status)
            .Include(x => x.Prioridade)
            .Include(x => x.Categoria)
            .Include(x => x.Subcategoria)
            .Include(x => x.TipoSolicitacao)
            .Include(x => x.LocalUnidade)
            .Include(x => x.Departamento)
            .Include(x => x.Solicitante)
            .Include(x => x.Responsavel)
            .Include(x => x.Comentarios).ThenInclude(x => x.Usuario)
            .Include(x => x.Anexos).ThenInclude(x => x.Usuario)
            .Include(x => x.Historicos).ThenInclude(x => x.Usuario)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.PoliticaSla)
            .Include(x => x.ChamadoSla).ThenInclude(x => x.CalendarioCorporativo)
            .FirstAsync(x => x.Id == chamado.Id, cancellationToken);

        if (auditoriaService is not null)
        {
            var dadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
            {
                chamadoCriado.Id,
                chamadoCriado.Codigo,
                chamadoCriado.Titulo,
                Status = chamadoCriado.Status.Nome,
                Prioridade = chamadoCriado.Prioridade.Nome,
                Categoria = chamadoCriado.Categoria.Nome,
                Subcategoria = chamadoCriado.Subcategoria?.Nome,
                TipoSolicitacao = chamadoCriado.TipoSolicitacao?.Nome,
                LocalUnidade = chamadoCriado.LocalUnidade?.Nome,
                chamadoCriado.DepartamentoId,
                chamadoCriado.CatalogoServicoId,
                SolicitanteId = chamadoCriado.SolicitanteId
            });

            await auditoriaService.RegistrarCriacaoAsync(
                "Chamados",
                "Chamado",
                chamadoCriado.Id.ToString(),
                "Chamado aberto.",
                dadosDepois: dadosDepois,
                metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                    origem: "api",
                    modulo: "Chamados",
                    entidade: "Chamado",
                    entidadeId: chamadoCriado.Id.ToString(),
                    codigo: chamadoCriado.Codigo,
                    nome: chamadoCriado.Titulo,
                    operacao: "Abertura",
                    resultado: "Sucesso",
                    observacao: "Abertura via portal"),
                cancellationToken: cancellationToken);
        }

        return PortalUseCaseHelpers.MapDetalhe(chamadoCriado, usuarioAtual);
    }

    private async Task<CatalogoServico?> ResolverServicoCatalogoAsync(
        CriarChamadoRequest request,
        UsuarioContextoAplicacao usuarioAtual,
        CancellationToken cancellationToken)
    {
        if (!request.CatalogoServicoId.HasValue && string.IsNullOrWhiteSpace(request.CatalogoServicoSlug))
        {
            return null;
        }

        CatalogoServico? servico;

        if (request.CatalogoServicoId.HasValue)
        {
            servico = await catalogoServicoRepository.Query()
                .AsNoTracking()
                .Include(x => x.DepartamentoResponsavel)
                .Include(x => x.Categoria)
                .Include(x => x.Subcategoria)
                .Include(x => x.PrioridadePadrao)
                .Include(x => x.SlaPadrao)
                .FirstOrDefaultAsync(x => x.Id == request.CatalogoServicoId.Value, cancellationToken);
        }
        else
        {
            var slug = request.CatalogoServicoSlug!.Trim().ToLowerInvariant();
            servico = await catalogoServicoRepository.Query()
                .AsNoTracking()
                .Include(x => x.DepartamentoResponsavel)
                .Include(x => x.Categoria)
                .Include(x => x.Subcategoria)
                .Include(x => x.PrioridadePadrao)
                .Include(x => x.SlaPadrao)
                .FirstOrDefaultAsync(x => x.Slug == slug, cancellationToken);
        }

        if (servico is null)
        {
            throw new InvalidOperationException("Servico do catalogo nao encontrado.");
        }

        if (servico.Status != StatusCatalogoServico.Publicado)
        {
            throw new InvalidOperationException("Somente servicos publicados podem ser usados para abertura de chamado.");
        }

        if (!servico.Ativo)
        {
            throw new InvalidOperationException("Somente servicos ativos podem ser usados para abertura de chamado.");
        }

        if (!servico.PermiteAberturaChamado)
        {
            throw new InvalidOperationException("Este servico esta disponivel apenas para consulta.");
        }

        if (!PortalCatalogoServicosVisibilidadeHelper.PodeVisualizarServico(usuarioAtual, servico.Visibilidade))
        {
            throw new InvalidOperationException("Servico do catalogo indisponivel para o seu perfil.");
        }

        if (!servico.CategoriaId.HasValue)
        {
            throw new InvalidOperationException("Servico do catalogo sem categoria configurada para abertura de chamado.");
        }

        if (!servico.PrioridadePadraoId.HasValue)
        {
            throw new InvalidOperationException("Servico do catalogo sem prioridade padrao configurada para abertura de chamado.");
        }

        if (servico.SubcategoriaId.HasValue && !servico.CategoriaId.HasValue)
        {
            throw new InvalidOperationException("Servico do catalogo com subcategoria invalida para abertura de chamado.");
        }

        return servico;
    }
}
