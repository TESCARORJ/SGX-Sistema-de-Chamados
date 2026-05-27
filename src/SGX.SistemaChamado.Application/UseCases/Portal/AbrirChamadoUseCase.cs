using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
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
    IRepository<InventarioAtivo> inventarioAtivoRepository,
    IRepository<StatusChamado> statusRepository,
    IRepository<HistoricoChamado> historicoRepository,
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IRepository<HistoricoInventarioAtivo> historicoInventarioAtivoRepository,
    ISlaService slaService,
    ICodigoChamadoService codigoChamadoService,
    IPrioridadeChamadoMatrizService prioridadeChamadoMatrizService,
    ICamposObrigatoriosChamadoService camposObrigatoriosChamadoService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork,
    IAuditoriaService? auditoriaService = null) : IAbrirChamadoUseCase
{
    private const string DescricaoHistoricoCriacaoPortal = "Chamado criado pelo portal";
    private const string DescricaoHistoricoCriacaoCatalogo = "Chamado aberto a partir do servico do catalogo";
    private const string DescricaoHistoricoCriacaoComAtivo = "Chamado aberto com ativo vinculado";
    private const string DescricaoHistoricoAprovacaoCatalogo = "Aprovacao solicitada automaticamente por servico de catalogo que requer aprovacao";
    private const string JustificativaAprovacaoCatalogo = "Aprovacao automatica solicitada por regra do catalogo de servicos";

    public async Task<ChamadoDetalheResponse> ExecutarAsync(CriarChamadoRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validacoesCamposObrigatorios = camposObrigatoriosChamadoService.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = request.NaturezaChamado ?? NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = request.ImpactoChamado,
            UrgenciaChamado = request.UrgenciaChamado,
            Titulo = request.Titulo,
            Descricao = request.Descricao,
            CategoriaId = request.CategoriaId,
            TipoSolicitacaoId = request.TipoSolicitacaoId,
            CatalogoServicoId = request.CatalogoServicoId,
            CatalogoServicoSlug = request.CatalogoServicoSlug,
            Origem = "Portal"
        });

        var primeiraFalha = validacoesCamposObrigatorios.FirstOrDefault();
        if (primeiraFalha is not null)
        {
            throw new InvalidOperationException(primeiraFalha.Mensagem);
        }

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var servicoCatalogo = await ResolverServicoCatalogoAsync(request, usuarioAtual, cancellationToken);

        var categoriaIdEfetiva = servicoCatalogo?.CategoriaId ?? request.CategoriaId;
        if (!categoriaIdEfetiva.HasValue || categoriaIdEfetiva.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Categoria obrigatoria.");
        }

        var prioridadeIdFallback = servicoCatalogo?.PrioridadePadraoId ?? request.PrioridadeId;
        
        var naturezaChamado = request.NaturezaChamado ?? NaturezaChamadoEnum.Requisicao;
        if (!Enum.IsDefined(naturezaChamado))
        {
            throw new InvalidOperationException("Natureza do chamado invalida.");
        }

        var impactoChamado = request.ImpactoChamado ?? ImpactoChamadoEnum.Baixo;
        if (!Enum.IsDefined(impactoChamado))
        {
            throw new InvalidOperationException("Impacto do chamado invalido.");
        }

        var urgenciaChamado = request.UrgenciaChamado ?? UrgenciaChamadoEnum.Baixa;
        if (!Enum.IsDefined(urgenciaChamado))
        {
            throw new InvalidOperationException("Urgencia do chamado invalida.");
        }

        var departamentoIdEfetivo = servicoCatalogo?.DepartamentoResponsavelId ?? request.DepartamentoId;
        var subcategoriaIdEfetiva = servicoCatalogo?.SubcategoriaId ?? request.SubcategoriaId;
        var catalogoServicoIdEfetivo = servicoCatalogo?.Id;
        InventarioAtivo? inventarioAtivo = null;

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == categoriaIdEfetiva.Value && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria nao encontrada ou inativa.");

        var prioridade = await prioridadeChamadoMatrizService.ObterPrioridadeAsync(impactoChamado, urgenciaChamado, cancellationToken);
        if (prioridade is null)
        {
            if (!prioridadeIdFallback.HasValue || prioridadeIdFallback.Value == Guid.Empty)
            {
                throw new InvalidOperationException("Prioridade obrigatoria.");
            }

            prioridade = await prioridadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == prioridadeIdFallback.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Prioridade nao encontrada ou inativa.");
        }

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

        if (request.InventarioAtivoId.HasValue)
        {
            inventarioAtivo = await inventarioAtivoRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InventarioAtivoId.Value, cancellationToken)
                ?? throw new InvalidOperationException("Ativo de inventario nao encontrado.");

            if (!inventarioAtivo.Ativo)
            {
                throw new InvalidOperationException("Ativo de inventario informado esta inativo.");
            }
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
            catalogoServicoIdEfetivo,
            inventarioAtivo?.Id,
            naturezaChamado,
            impactoChamado,
            urgenciaChamado);

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

        if (servicoCatalogo?.RequerAprovacao == true)
        {
            var jaExistePendente = await aprovacaoChamadoRepository.Query()
                .AsNoTracking()
                .AnyAsync(x => x.Ativo && x.ChamadoId == chamado.Id && x.Status == StatusAprovacaoChamado.Pendente, cancellationToken);

            if (!jaExistePendente)
            {
                var aprovacao = new AprovacaoChamado(
                    chamado.Id,
                    TipoOrigemAprovacaoChamado.CatalogoServico,
                    usuarioAtual.Id,
                    usuarioAtual.Login,
                    chamado.SolicitanteId,
                    servicoCatalogo.Nome,
                    JustificativaAprovacaoCatalogo);

                await aprovacaoChamadoRepository.AddAsync(aprovacao, cancellationToken);

                var historicoAprovacao = new HistoricoChamado(
                    chamado.Id,
                    TipoHistoricoChamado.AprovacaoSolicitada,
                    $"{DescricaoHistoricoAprovacaoCatalogo}: {servicoCatalogo.Nome}",
                    usuarioAtual.Id,
                    usuarioAtual.Login);

                await historicoRepository.AddAsync(historicoAprovacao, cancellationToken);
            }
        }

        if (inventarioAtivo is not null)
        {
            var descricaoAtivo = $"{DescricaoHistoricoCriacaoComAtivo}: {inventarioAtivo.Codigo} - {inventarioAtivo.Nome}";
            if (!string.IsNullOrWhiteSpace(inventarioAtivo.NumeroPatrimonio))
            {
                descricaoAtivo += $" (Patrimonio: {inventarioAtivo.NumeroPatrimonio})";
            }

            var historicoAtivoNoChamado = new HistoricoChamado(
                chamado.Id,
                TipoHistoricoChamado.AtivoVinculado,
                descricaoAtivo,
                usuarioAtual.Id,
                usuarioAtual.Login);

            await historicoRepository.AddAsync(historicoAtivoNoChamado, cancellationToken);

            var historicoAtivo = new HistoricoInventarioAtivo(
                inventarioAtivo.Id,
                TipoMovimentacaoAtivo.VinculoChamado,
                usuarioAtual.Id,
                usuarioAtual.Login,
                $"Chamado {chamado.Codigo} vinculado na abertura ({chamado.Titulo}).");

            await historicoInventarioAtivoRepository.AddAsync(historicoAtivo, cancellationToken);
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
            .Include(x => x.InventarioAtivo)
            .Include(x => x.Aprovacoes)
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
                NaturezaChamado = chamadoCriado.NaturezaChamado.ToString(),
                ImpactoChamado = chamadoCriado.ImpactoChamado.ToString(),
                UrgenciaChamado = chamadoCriado.UrgenciaChamado.ToString(),
                Categoria = chamadoCriado.Categoria.Nome,
                Subcategoria = chamadoCriado.Subcategoria?.Nome,
                TipoSolicitacao = chamadoCriado.TipoSolicitacao?.Nome,
                LocalUnidade = chamadoCriado.LocalUnidade?.Nome,
                chamadoCriado.DepartamentoId,
                chamadoCriado.CatalogoServicoId,
                chamadoCriado.InventarioAtivoId,
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

            if (servicoCatalogo?.RequerAprovacao == true)
            {
                var aprovacaoGerada = chamadoCriado.Aprovacoes
                    .Where(x => x.Ativo && x.Status == StatusAprovacaoChamado.Pendente && x.TipoOrigem == TipoOrigemAprovacaoChamado.CatalogoServico)
                    .OrderByDescending(x => x.SolicitadaEm)
                    .ThenByDescending(x => x.CriadoEm)
                    .FirstOrDefault();

                if (aprovacaoGerada is not null)
                {
                    await auditoriaService.RegistrarCriacaoAsync(
                        "Aprovacao de Chamados",
                        "AprovacaoChamado",
                        aprovacaoGerada.Id.ToString(),
                        "Aprovacao automatica gerada na abertura do chamado por servico de catalogo.",
                        dadosDepois: AuditoriaDiffHelper.SerializarSeguro(new
                        {
                            aprovacaoGerada.Id,
                            aprovacaoGerada.ChamadoId,
                            Status = aprovacaoGerada.Status.ToString(),
                            TipoOrigem = aprovacaoGerada.TipoOrigem.ToString(),
                            aprovacaoGerada.OrigemDescricao,
                            aprovacaoGerada.JustificativaSolicitacao,
                            aprovacaoGerada.SolicitadaEm
                        }),
                        metadados: AuditoriaDiffHelper.CriarMetadadosPadrao(
                            origem: "api",
                            modulo: "Aprovacao de Chamados",
                            entidade: "AprovacaoChamado",
                            entidadeId: aprovacaoGerada.Id.ToString(),
                            codigo: chamadoCriado.Codigo,
                            nome: chamadoCriado.Titulo,
                            operacao: "SolicitacaoAutomaticaPorCatalogo",
                            resultado: "Sucesso"),
                        cancellationToken: cancellationToken);
                }
            }
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

        if (servico.SubcategoriaId.HasValue && !servico.CategoriaId.HasValue)
        {
            throw new InvalidOperationException("Servico do catalogo com subcategoria invalida para abertura de chamado.");
        }

        return servico;
    }
}
