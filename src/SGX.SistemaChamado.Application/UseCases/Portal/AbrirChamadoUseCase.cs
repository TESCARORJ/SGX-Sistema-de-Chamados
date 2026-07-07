using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Portal;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Portal;

public sealed class AbrirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<RespostaFormularioChamado> respostaFormularioChamadoRepository,
    IRepository<CategoriaChamado> categoriaRepository,
    IRepository<SubcategoriaChamado> subcategoriaRepository,
    IRepository<PrioridadeChamado> prioridadeRepository,
    IRepository<TipoSolicitacao> tipoSolicitacaoRepository,
    IRepository<LocalUnidade> localUnidadeRepository,
    IRepository<Departamento> departamentoRepository,
    IRepository<CatalogoServico> catalogoServicoRepository,
    IRepository<FormularioServico> formularioServicoRepository,
    IRepository<GrupoTecnico> grupoTecnicoRepository,
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
    IAuditoriaService? auditoriaService = null,
    IProcessarEventoCandidatoNotificacaoUseCase? processarEventoCandidatoNotificacaoUseCase = null,
    ILogger<AbrirChamadoUseCase>? logger = null) : IAbrirChamadoUseCase
{
    private const int LimiteTextoCurto = 180;
    private const int LimiteTextoLongo = 4000;
    private const string DescricaoHistoricoCriacaoPortal = "Chamado criado pelo portal";
    private const string DescricaoHistoricoCriacaoCatalogo = "Chamado aberto a partir do servico do catalogo";
    private const string DescricaoHistoricoCriacaoComAtivo = "Chamado aberto com ativo vinculado";
    private const string DescricaoHistoricoFormularioPreenchido = "Chamado aberto com formulario do servico preenchido.";
    private const string DescricaoAuditoriaRespostasFormularioPersistidas = "Respostas do formulario persistidas na abertura guiada.";
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
        var contextoFormulario = await ValidarFormularioAsync(servicoCatalogo, request, cancellationToken);

        var categoriaIdEfetiva = servicoCatalogo?.CategoriaId ?? request.CategoriaId;
        if (!categoriaIdEfetiva.HasValue || categoriaIdEfetiva.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Categoria obrigatoria.");
        }

        var prioridadeIdCatalogo = servicoCatalogo?.PrioridadePadraoId;
        var prioridadeIdFallback = prioridadeIdCatalogo ?? request.PrioridadeId;

        var naturezaChamado = servicoCatalogo is not null
            ? NaturezaChamadoEnum.Requisicao
            : request.NaturezaChamado ?? NaturezaChamadoEnum.Requisicao;
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
        var grupoTecnicoIdEfetivo = await ResolverGrupoTecnicoCatalogoAsync(servicoCatalogo, cancellationToken);
        InventarioAtivo? inventarioAtivo = null;

        var categoria = await categoriaRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == categoriaIdEfetiva.Value && x.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Categoria nao encontrada ou inativa.");

        PrioridadeChamado? prioridade = null;
        if (prioridadeIdCatalogo.HasValue)
        {
            prioridade = await prioridadeRepository.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == prioridadeIdCatalogo.Value && x.Ativo, cancellationToken)
                ?? throw new InvalidOperationException("Prioridade nao encontrada ou inativa.");
        }
        else
        {
            prioridade = await prioridadeChamadoMatrizService.ObterPrioridadeAsync(impactoChamado, urgenciaChamado, cancellationToken);
        }

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
            urgenciaChamado,
            grupoTecnicoIdEfetivo);

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

        var quantidadeRespostasPersistidas = await PersistirRespostasFormularioAsync(chamado, contextoFormulario, usuarioAtual.Login, cancellationToken);

        if (quantidadeRespostasPersistidas > 0)
        {
            var historicoFormulario = new HistoricoChamado(
                chamado.Id,
                TipoHistoricoChamado.FormularioServicoPreenchidoNaAbertura,
                DescricaoHistoricoFormularioPreenchido,
                usuarioAtual.Id,
                usuarioAtual.Login);

            await historicoRepository.AddAsync(historicoFormulario, cancellationToken);
        }

        await slaService.InicializarNaAberturaAsync(chamado, usuarioAtual.Login, DateTime.UtcNow, servicoCatalogo?.SlaPadraoId, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await TentarIntegrarNotificacaoAsync(chamado, usuarioAtual, historicoCriado, cancellationToken);

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
            .Include(x => x.RespostasFormulario).ThenInclude(x => x.CampoFormularioServico)
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

            await RegistrarAuditoriaRespostasFormularioAsync(
                auditoriaService,
                chamadoCriado,
                usuarioAtual,
                contextoFormulario?.FormularioServicoVersaoId,
                quantidadeRespostasPersistidas,
                cancellationToken);

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

    private async Task TentarIntegrarNotificacaoAsync(
        Chamado chamado,
        UsuarioContextoAplicacao usuarioAtual,
        HistoricoChamado historicoCriado,
        CancellationToken cancellationToken)
    {
        if (processarEventoCandidatoNotificacaoUseCase is null)
        {
            return;
        }

        try
        {
            await processarEventoCandidatoNotificacaoUseCase.ExecutarAsync(
                new ProcessarEventoCandidatoNotificacaoRequest(
                    $"chamado-aberto:{chamado.Id}",
                    new EventoCandidatoNotificacao(
                        TipoEventoNotificacao.EventoChamado,
                        chamado.Id,
                        usuarioAtual.Id,
                        historicoCriado.CriadoEm,
                        $"chamado:{chamado.Id}",
                        $"chamado-aberto:{chamado.Id}",
                        new Dictionary<string, string>
                        {
                            ["evento"] = "chamado-aberto"
                        }),
                    new Dictionary<string, string>
                    {
                        ["chamado.codigo"] = chamado.Codigo,
                        ["chamado.titulo"] = chamado.Titulo,
                        ["chamado.status"] = "Aberto",
                        ["evento.nome"] = "Chamado aberto",
                        ["evento.descricao"] = historicoCriado.Descricao,
                        ["evento.ocorrido_em"] = historicoCriado.CriadoEm.ToString("O"),
                        ["solicitante.nome"] = usuarioAtual.Nome
                    },
                    [TipoParticipacaoDestinatarioNotificacao.Solicitante],
                    [CanalNotificacao.Sistema, CanalNotificacao.Email]),
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger?.LogError(
                ex,
                "Falha ao integrar notificacao de abertura. ChamadoId={ChamadoId} HistoricoId={HistoricoId}",
                chamado.Id,
                historicoCriado.Id);
        }
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

        if (servico.SubcategoriaId.HasValue && !servico.CategoriaId.HasValue)
        {
            throw new InvalidOperationException("Servico do catalogo com subcategoria invalida para abertura de chamado.");
        }

        return servico;
    }

    private async Task<ContextoPersistenciaFormulario?> ValidarFormularioAsync(
        CatalogoServico? servicoCatalogo,
        CriarChamadoRequest request,
        CancellationToken cancellationToken)
    {
        if (servicoCatalogo is null)
        {
            return null;
        }

        var formulario = await ObterFormularioAberturaAsync(servicoCatalogo.Id, cancellationToken);
        var versaoAplicavel = SelecionarVersaoAplicavel(formulario);
        if (versaoAplicavel is null)
        {
            ValidarAusenciaFormularioCompativel(request);
            return null;
        }

        var camposDaVersao = versaoAplicavel.Campos
            .OrderBy(x => x.Ordem)
            .ToArray();

        var respostasPorCampo = (request.RespostasFormulario ?? [])
            .Where(x => x.CampoFormularioServicoId != Guid.Empty)
            .ToDictionary(x => x.CampoFormularioServicoId, x => x);

        ValidarEscopoCampos(camposDaVersao, respostasPorCampo);

        var camposAplicaveis = camposDaVersao
            .Where(x => x.Ativo && x.Visivel)
            .OrderBy(x => x.Ordem)
            .ToArray();

        if (camposAplicaveis.Length == 0)
        {
            return new ContextoPersistenciaFormulario(versaoAplicavel.Id, [], respostasPorCampo);
        }

        ValidarObrigatoriedadeCampos(camposAplicaveis, respostasPorCampo);
        ValidarTipoEFormatoCampos(camposAplicaveis, respostasPorCampo);
        return new ContextoPersistenciaFormulario(versaoAplicavel.Id, camposAplicaveis, respostasPorCampo);
    }

    private async Task<int> PersistirRespostasFormularioAsync(
        Chamado chamado,
        ContextoPersistenciaFormulario? contextoFormulario,
        string usuarioAtualLogin,
        CancellationToken cancellationToken)
    {
        if (contextoFormulario is null
            || contextoFormulario.CamposAplicaveis.Length == 0
            || contextoFormulario.RespostasPorCampo.Count == 0)
        {
            return 0;
        }

        var quantidadePersistida = 0;

        foreach (var campo in contextoFormulario.CamposAplicaveis)
        {
            if (!contextoFormulario.RespostasPorCampo.TryGetValue(campo.Id, out var resposta) || !RespostaPossuiConteudo(resposta))
            {
                continue;
            }

            var respostaPersistida = new RespostaFormularioChamado(
                chamado.Id,
                contextoFormulario.FormularioServicoVersaoId,
                campo.Id,
                string.IsNullOrWhiteSpace(resposta.Valor) ? null : resposta.Valor,
                resposta.Valores,
                usuarioAtualLogin);

            await respostaFormularioChamadoRepository.AddAsync(respostaPersistida, cancellationToken);
            quantidadePersistida++;
        }

        return quantidadePersistida;
    }

    private static void ValidarAusenciaFormularioCompativel(CriarChamadoRequest request)
    {
        if (!PossuiRespostasFormularioPreenchidas(request.RespostasFormulario))
        {
            return;
        }

        throw new InvalidOperationException(
            "Servico do catalogo nao possui formulario configurado; respostas de formulario nao sao aceitas.");
    }

    private static void ValidarEscopoCampos(
        CampoFormularioServico[] camposDaVersao,
        IReadOnlyDictionary<Guid, RespostaFormularioAberturaRequest> respostasPorCampo)
    {
        if (respostasPorCampo.Count == 0)
        {
            return;
        }

        var camposPorId = camposDaVersao.ToDictionary(x => x.Id, x => x);

        foreach (var campoFormularioServicoId in respostasPorCampo.Keys)
        {
            if (!camposPorId.TryGetValue(campoFormularioServicoId, out var campo))
            {
                throw new InvalidOperationException(
                    $"Campo do formulario invalido: resposta enviada para campo fora do escopo do formulario aplicavel ({campoFormularioServicoId}).");
            }

            if (!campo.Ativo)
            {
                throw new InvalidOperationException(
                    $"Campo do formulario invalido: {campo.Rotulo} esta inativo e nao pode receber resposta.");
            }

            if (!campo.Visivel)
            {
                throw new InvalidOperationException(
                    $"Campo do formulario invalido: {campo.Rotulo} esta invisivel e nao pode receber resposta.");
            }
        }
    }

    private static void ValidarObrigatoriedadeCampos(
        CampoFormularioServico[] camposAplicaveis,
        IReadOnlyDictionary<Guid, RespostaFormularioAberturaRequest> respostasPorCampo)
    {
        var camposObrigatorios = camposAplicaveis
            .Where(x => x.Obrigatorio)
            .OrderBy(x => x.Ordem)
            .ToArray();

        if (camposObrigatorios.Length == 0)
        {
            return;
        }

        var campoObrigatorioNaoRespondido = camposObrigatorios
            .FirstOrDefault(campo =>
                !respostasPorCampo.TryGetValue(campo.Id, out var resposta) ||
                !RespostaPossuiConteudo(resposta));

        if (campoObrigatorioNaoRespondido is not null)
        {
            throw new InvalidOperationException(
                $"Campo obrigatorio do formulario nao respondido: {campoObrigatorioNaoRespondido.Rotulo}.");
        }
    }

    private static void ValidarTipoEFormatoCampos(
        IEnumerable<CampoFormularioServico> camposAplicaveis,
        IReadOnlyDictionary<Guid, RespostaFormularioAberturaRequest> respostasPorCampo)
    {
        foreach (var campo in camposAplicaveis)
        {
            if (!respostasPorCampo.TryGetValue(campo.Id, out var resposta) || !RespostaPossuiConteudo(resposta))
            {
                continue;
            }

            ValidarRespostaConformeTipo(campo, resposta);
        }
    }

    private async Task<Guid?> ResolverGrupoTecnicoCatalogoAsync(CatalogoServico? servicoCatalogo, CancellationToken cancellationToken)
    {
        if (!servicoCatalogo?.GrupoTecnicoId.HasValue ?? true)
        {
            return null;
        }

        var grupoTecnicoAtivo = await grupoTecnicoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == servicoCatalogo.GrupoTecnicoId.Value && x.Ativo, cancellationToken);

        return grupoTecnicoAtivo
            ? servicoCatalogo.GrupoTecnicoId.Value
            : null;
    }

    private async Task<FormularioServico?> ObterFormularioAberturaAsync(Guid catalogoServicoId, CancellationToken cancellationToken)
    {
        return await formularioServicoRepository.Query()
            .AsNoTracking()
            .Include(x => x.Versoes.Where(v => v.Ativo))
                .ThenInclude(x => x.Campos)
                    .ThenInclude(x => x.Opcoes)
            .FirstOrDefaultAsync(
                x => x.CatalogoServicoId == catalogoServicoId && x.Ativo,
                cancellationToken);
    }

    private static FormularioServicoVersao? SelecionarVersaoAplicavel(FormularioServico? formulario)
    {
        if (formulario is null)
        {
            return null;
        }

        return formulario.Versoes
            .Where(x => x.Ativo)
            .OrderByDescending(x => x.Publicada)
            .ThenByDescending(x => x.PublicadoEm ?? DateTime.MinValue)
            .ThenByDescending(x => x.Numero)
            .FirstOrDefault();
    }

    private static bool RespostaPossuiConteudo(RespostaFormularioAberturaRequest resposta)
        => !string.IsNullOrWhiteSpace(resposta.Valor)
            || resposta.Valores?.Any(x => !string.IsNullOrWhiteSpace(x)) == true;

    private static bool PossuiRespostasFormularioPreenchidas(IReadOnlyCollection<RespostaFormularioAberturaRequest>? respostasFormulario)
        => respostasFormulario?.Any(RespostaPossuiConteudo) == true;

    private static void ValidarRespostaConformeTipo(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        switch (campo.Tipo)
        {
            case TipoCampoFormularioServico.TextoCurto:
                ValidarTextoCurto(campo, resposta);
                break;
            case TipoCampoFormularioServico.TextoLongo:
                ValidarTextoLongo(campo, resposta);
                break;
            case TipoCampoFormularioServico.Numero:
                ValidarNumero(campo, resposta);
                break;
            case TipoCampoFormularioServico.Data:
                ValidarData(campo, resposta);
                break;
            case TipoCampoFormularioServico.Booleano:
                ValidarBooleano(campo, resposta);
                break;
            case TipoCampoFormularioServico.SelecaoUnica:
                ValidarSelecaoUnica(campo, resposta);
                break;
            case TipoCampoFormularioServico.SelecaoMultipla:
                ValidarSelecaoMultipla(campo, resposta);
                break;
            default:
                throw new InvalidOperationException($"Tipo de campo de formulario nao suportado: {campo.Tipo}.");
        }
    }

    private static void ValidarTextoCurto(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "texto curto");
        if (valor.Length > LimiteTextoCurto)
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve possuir no maximo {LimiteTextoCurto} caracteres.");
        }
    }

    private static void ValidarTextoLongo(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "texto longo");
        if (valor.Length > LimiteTextoLongo)
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve possuir no maximo {LimiteTextoLongo} caracteres.");
        }
    }

    private static void ValidarNumero(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "numero");
        if (!decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out _))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve conter um numero decimal valido.");
        }
    }

    private static void ValidarData(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "data");
        if (!DateOnly.TryParseExact(valor, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve conter uma data valida no formato yyyy-MM-dd.");
        }
    }

    private static void ValidarBooleano(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "booleano");
        if (!bool.TryParse(valor, out _))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve conter true ou false.");
        }
    }

    private static void ValidarSelecaoUnica(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        var valor = ObterValorUnicoObrigatorio(campo, resposta, "selecao unica");
        var opcoesAtivas = ObterOpcoesAtivas(campo);

        if (!opcoesAtivas.Contains(valor))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve usar uma opcao ativa configurada para selecao unica.");
        }
    }

    private static void ValidarSelecaoMultipla(CampoFormularioServico campo, RespostaFormularioAberturaRequest resposta)
    {
        if (!string.IsNullOrWhiteSpace(resposta.Valor))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve usar Valores para selecao multipla.");
        }

        if (resposta.Valores is null || resposta.Valores.Count == 0)
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve informar ao menos um valor.");
        }

        var opcoesAtivas = ObterOpcoesAtivas(campo);
        foreach (var valor in resposta.Valores
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Select(x => x.Trim()))
        {
            if (!opcoesAtivas.Contains(valor))
            {
                throw new InvalidOperationException(
                    $"Campo do formulario invalido: {campo.Rotulo} deve usar apenas opcoes ativas configuradas para selecao multipla.");
            }
        }
    }

    private static string ObterValorUnicoObrigatorio(
        CampoFormularioServico campo,
        RespostaFormularioAberturaRequest resposta,
        string descricaoTipo)
    {
        if (resposta.Valores is not null && resposta.Valores.Count > 0)
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} deve usar Valor para {descricaoTipo}.");
        }

        if (string.IsNullOrWhiteSpace(resposta.Valor))
        {
            throw new InvalidOperationException(
                $"Campo do formulario invalido: {campo.Rotulo} exige Valor preenchido para {descricaoTipo}.");
        }

        return resposta.Valor.Trim();
    }

    private static HashSet<string> ObterOpcoesAtivas(CampoFormularioServico campo)
        => campo.Opcoes
            .Where(x => x.Ativo)
            .Select(x => x.Valor)
            .ToHashSet(StringComparer.Ordinal);

    private static Task RegistrarAuditoriaRespostasFormularioAsync(
        IAuditoriaService auditoriaService,
        Chamado chamadoCriado,
        UsuarioContextoAplicacao usuarioAtual,
        Guid? formularioServicoVersaoId,
        int quantidadeRespostasPersistidas,
        CancellationToken cancellationToken)
    {
        if (quantidadeRespostasPersistidas <= 0 || !formularioServicoVersaoId.HasValue)
        {
            return Task.CompletedTask;
        }

        return auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Chamados",
            Entidade = "RespostaFormularioChamado",
            EntidadeId = chamadoCriado.Id.ToString(),
            Acao = TipoAcaoAuditoria.Criacao,
            Descricao = DescricaoAuditoriaRespostasFormularioPersistidas,
            DadosDepois = AuditoriaDiffHelper.SerializarSeguro(new
            {
                ChamadoId = chamadoCriado.Id,
                FormularioServicoVersaoId = formularioServicoVersaoId.Value,
                QuantidadeRespostasPersistidas = quantidadeRespostasPersistidas,
                Origem = "AberturaGuiadaCatalogo",
                UsuarioExecutorId = usuarioAtual.Id,
                UsuarioExecutorLogin = usuarioAtual.Login
            }),
            Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                origem: "AberturaGuiadaCatalogo",
                modulo: "Chamados",
                entidade: "RespostaFormularioChamado",
                entidadeId: chamadoCriado.Id.ToString(),
                codigo: chamadoCriado.Codigo,
                nome: chamadoCriado.Titulo,
                operacao: "PersistenciaRespostasFormulario",
                resultado: "Sucesso",
                observacao: $"Quantidade de respostas persistidas: {quantidadeRespostasPersistidas}."),
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true,
            UsuarioId = usuarioAtual.Id,
            UsuarioLogin = usuarioAtual.Login
        }, cancellationToken);
    }

    private sealed record ContextoPersistenciaFormulario(
        Guid FormularioServicoVersaoId,
        CampoFormularioServico[] CamposAplicaveis,
        IReadOnlyDictionary<Guid, RespostaFormularioAberturaRequest> RespostasPorCampo);
}
