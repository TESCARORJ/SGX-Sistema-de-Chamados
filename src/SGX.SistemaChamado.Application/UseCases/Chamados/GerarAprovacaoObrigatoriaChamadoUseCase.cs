using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class GerarAprovacaoObrigatoriaChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<InstanciaAprovacaoChamado> instanciaAprovacaoRepository,
    IRepository<ConfiguracaoRegraAprovacao> configuracaoRegraAprovacaoRepository,
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IGerarAprovacaoObrigatoriaChamadoUseCase
{
    private const string MotivoSemRegra = "Nenhuma configuracao de regra aplicavel exigiu aprovacao para o contexto informado.";
    private const string MotivoRegraInformativa = "A regra aplicavel encontrada nao exige aprovacao obrigatoria.";
    private const string MotivoChamadoFinal = "Chamados em status final nao geram aprovacao obrigatoria automaticamente nesta etapa.";
    private const string MotivoDuplicidadeInstancia = "Ja existe instancia de aprovacao equivalente pendente ou em reavaliacao para o chamado.";
    private const string MotivoDuplicidadeLegado = "Ja existe aprovacao legada pendente por catalogo para o chamado.";

    private static readonly GerarAprovacaoObrigatoriaChamadoRequestValidator Validator = new();
    private static readonly CriarInstanciaAprovacaoChamadoManualRequestValidator CriarInstanciaValidator = new();

    public async Task<GerarAprovacaoObrigatoriaChamadoResponse> ExecutarAsync(
        GerarAprovacaoObrigatoriaChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        await Validator.ValidateAndThrowAsync(request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var chamado = await chamadoRepository.Query()
            .Include(x => x.Status)
            .Include(x => x.CatalogoServico)
            .Include(x => x.Prioridade)
            .FirstOrDefaultAsync(x => x.Id == request.ChamadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (chamado.Status.EhStatusFinal && !request.ForcarReavaliacao)
        {
            return CriarRespostaSemGeracao(MotivoChamadoFinal);
        }

        var contexto = MontarContexto(chamado, request);
        var avaliacao = await AvaliarRegraAsync(contexto, cancellationToken);

        if (!avaliacao.RegraAplicavel || avaliacao.MelhorRegra is null)
        {
            return CriarRespostaSemGeracao(MotivoSemRegra, avisos: avaliacao.Avisos);
        }

        if (!avaliacao.ExigeAprovacao)
        {
            return CriarRespostaSemGeracao(
                MotivoRegraInformativa,
                configuracaoRegraAprovacaoId: avaliacao.MelhorRegra.ConfiguracaoRegraAprovacaoId,
                nomeRegra: avaliacao.MelhorRegra.NomeRegra,
                versaoRegra: avaliacao.MelhorRegra.VersaoRegra,
                efeitoOperacional: avaliacao.EfeitoOperacional,
                tipoFluxoAprovacao: avaliacao.TipoFluxoAprovacao,
                tipoResolucaoAprovador: avaliacao.TipoResolucaoAprovador,
                avisos: avaliacao.Avisos);
        }

        var configuracao = await configuracaoRegraAprovacaoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == avaliacao.MelhorRegra.ConfiguracaoRegraAprovacaoId, cancellationToken)
            ?? throw new KeyNotFoundException("Configuracao de regra aplicavel nao encontrada.");

        var jaExisteInstanciaEquivalente = await ExisteInstanciaEquivalenteAsync(chamado, configuracao, contexto, cancellationToken);
        if (jaExisteInstanciaEquivalente)
        {
            return CriarRespostaSemGeracao(
                MotivoDuplicidadeInstancia,
                configuracao.Id,
                configuracao.Nome,
                configuracao.Versao,
                avaliacao.ExigeAprovacao,
                avaliacao.Bloqueante,
                avaliacao.EfeitoOperacional,
                avaliacao.TipoFluxoAprovacao,
                avaliacao.TipoResolucaoAprovador,
                ResolverAprovador(configuracao),
                avaliacao.PrazoDecisaoHoras,
                false,
                true,
                null,
                avaliacao.Avisos);
        }

        var existeLegadoEquivalente = await ExisteAprovacaoLegadaEquivalenteAsync(chamado, cancellationToken);
        if (existeLegadoEquivalente)
        {
            return CriarRespostaSemGeracao(
                MotivoDuplicidadeLegado,
                configuracao.Id,
                configuracao.Nome,
                configuracao.Versao,
                avaliacao.ExigeAprovacao,
                avaliacao.Bloqueante,
                avaliacao.EfeitoOperacional,
                avaliacao.TipoFluxoAprovacao,
                avaliacao.TipoResolucaoAprovador,
                ResolverAprovador(configuracao),
                avaliacao.PrazoDecisaoHoras,
                false,
                true,
                null,
                avaliacao.Avisos);
        }

        var solicitadaEm = DateTime.UtcNow;
        var prazoDecisaoHoras = configuracao.PrazoDecisaoHoras;
        DateTime? deveExpirarEm = prazoDecisaoHoras.HasValue ? solicitadaEm.AddHours(prazoDecisaoHoras.Value) : null;
        var aprovadorResolvidoUsuarioId = ResolverAprovador(configuracao);

        var criarRequest = new CriarInstanciaAprovacaoChamadoManualRequest
        {
            ChamadoId = chamado.Id,
            SolicitanteId = request.SolicitanteId ?? chamado.SolicitanteId,
            ConfiguracaoRegraAprovacaoId = configuracao.Id,
            Titulo = $"Aprovacao obrigatoria do chamado {chamado.Codigo}",
            Descricao = $"Instancia gerada automaticamente pelo motor de aprovacao a partir da regra {configuracao.Nome} v{configuracao.Versao}.",
            Origem = OrigemInstanciaAprovacaoChamado.RegraMotor,
            TipoFluxoAprovacao = configuracao.TipoFluxoAprovacao,
            EfeitoOperacional = configuracao.EfeitoOperacional,
            EscopoRegra = configuracao.EscopoRegra,
            TipoRegra = configuracao.TipoRegra,
            NaturezaChamado = contexto.NaturezaChamado,
            TipoSolicitacaoId = contexto.TipoSolicitacaoId,
            CatalogoServicoId = contexto.CatalogoServicoId,
            CategoriaId = contexto.CategoriaId,
            SubcategoriaId = contexto.SubcategoriaId,
            ImpactoAvaliado = contexto.ImpactoChamado,
            UrgenciaAvaliada = contexto.UrgenciaChamado,
            PrioridadeAvaliada = contexto.PrioridadeChamado,
            CustoAvaliado = contexto.Custo,
            NivelRiscoAvaliado = contexto.NivelRisco,
            ExigeAprovacao = configuracao.ExigeAprovacao,
            Bloqueante = configuracao.Bloqueante,
            PermiteReenvio = configuracao.PermiteReenvio,
            PermiteFallback = configuracao.PermiteFallback,
            TipoResolucaoAprovador = configuracao.TipoResolucaoAprovador,
            AprovadorEspecificoUsuarioId = configuracao.AprovadorEspecificoUsuarioId,
            AprovadorPadraoUsuarioId = configuracao.AprovadorPadraoUsuarioId,
            AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId,
            PrazoDecisaoHoras = prazoDecisaoHoras,
            DeveExpirarEm = deveExpirarEm,
            RegraNomeSnapshot = configuracao.Nome,
            RegraVersaoSnapshot = configuracao.Versao,
            RegraCriterioSnapshot = configuracao.Descricao
        };

        await CriarInstanciaValidator.ValidateAndThrowAsync(criarRequest, cancellationToken);

        var instancia = new InstanciaAprovacaoChamado(
            criarRequest.ChamadoId,
            criarRequest.SolicitanteId,
            criarRequest.Origem,
            criarRequest.TipoFluxoAprovacao,
            criarRequest.EfeitoOperacional,
            criarRequest.EscopoRegra,
            criarRequest.TipoRegra,
            criarRequest.ExigeAprovacao,
            criarRequest.Bloqueante,
            criarRequest.TipoResolucaoAprovador,
            usuarioAtual.Id,
            usuarioAtual.Login,
            criarRequest.ConfiguracaoRegraAprovacaoId,
            criarRequest.AprovacaoChamadoLegadaId,
            criarRequest.Titulo,
            criarRequest.Descricao,
            criarRequest.NaturezaChamado,
            criarRequest.TipoSolicitacaoId,
            criarRequest.CatalogoServicoId,
            criarRequest.CategoriaId,
            criarRequest.SubcategoriaId,
            criarRequest.ImpactoAvaliado,
            criarRequest.UrgenciaAvaliada,
            criarRequest.PrioridadeAvaliada,
            criarRequest.CustoAvaliado,
            criarRequest.NivelRiscoAvaliado,
            criarRequest.PermiteReenvio,
            criarRequest.PermiteFallback,
            criarRequest.AprovadorEspecificoUsuarioId,
            criarRequest.AprovadorPadraoUsuarioId,
            criarRequest.AprovadorResolvidoUsuarioId,
            criarRequest.PrazoDecisaoHoras,
            criarRequest.DeveExpirarEm,
            criarRequest.RegraNomeSnapshot,
            criarRequest.RegraVersaoSnapshot,
            criarRequest.RegraCriterioSnapshot);

        await instanciaAprovacaoRepository.AddAsync(instancia, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var avisos = new List<string>(avaliacao.Avisos);
        avisos.Add("A instancia foi criada sem gerar etapas, decisoes, bloqueio operacional ou alteracao de status do chamado.");

        return new GerarAprovacaoObrigatoriaChamadoResponse
        {
            GerouAprovacao = true,
            InstanciaAprovacaoChamadoId = instancia.Id,
            ConfiguracaoRegraAprovacaoId = configuracao.Id,
            NomeRegra = configuracao.Nome,
            VersaoRegra = configuracao.Versao,
            ExigeAprovacao = configuracao.ExigeAprovacao,
            Bloqueante = configuracao.Bloqueante,
            EfeitoOperacional = configuracao.EfeitoOperacional,
            TipoFluxoAprovacao = configuracao.TipoFluxoAprovacao,
            TipoResolucaoAprovador = configuracao.TipoResolucaoAprovador,
            AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId,
            PrazoDecisaoHoras = prazoDecisaoHoras,
            DeveExpirarEm = deveExpirarEm,
            JaExistiaAprovacaoEquivalente = false,
            Motivo = "Instancia de aprovacao obrigatoria gerada com sucesso.",
            Avisos = avisos
        };
    }

    private static ContextoAvaliacaoRegraAprovacaoRequest MontarContexto(
        Chamado chamado,
        GerarAprovacaoObrigatoriaChamadoRequest request)
        => new()
        {
            NaturezaChamado = request.NaturezaChamado ?? chamado.NaturezaChamado,
            TipoSolicitacaoId = request.TipoSolicitacaoId ?? chamado.TipoSolicitacaoId,
            CatalogoServicoId = request.CatalogoServicoId ?? chamado.CatalogoServicoId,
            CategoriaId = request.CategoriaId ?? chamado.CategoriaId,
            SubcategoriaId = request.SubcategoriaId ?? chamado.SubcategoriaId,
            ImpactoChamado = request.ImpactoChamado ?? chamado.ImpactoChamado,
            UrgenciaChamado = request.UrgenciaChamado ?? chamado.UrgenciaChamado,
            PrioridadeChamado = request.PrioridadeChamado ?? chamado.Prioridade.Nivel,
            Custo = request.Custo,
            NivelRisco = request.NivelRisco,
            DataReferencia = request.DataReferencia ?? DateTime.UtcNow
        };

    private async Task<AvaliacaoConfiguracaoRegraAprovacaoResponse> AvaliarRegraAsync(
        ContextoAvaliacaoRegraAprovacaoRequest contexto,
        CancellationToken cancellationToken)
    {
        var dataReferencia = contexto.DataReferencia ?? DateTime.UtcNow;
        var candidatas = await configuracaoRegraAprovacaoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Where(x => (!x.VigenteDe.HasValue || x.VigenteDe.Value <= dataReferencia) &&
                        (!x.VigenteAte.HasValue || x.VigenteAte.Value >= dataReferencia))
            .ToListAsync(cancellationToken);

        var regrasCandidatas = candidatas
            .Where(x => RegraSatisfazContexto(x, contexto))
            .OrderByDescending(x => x.Prioridade)
            .ThenByDescending(CalcularEspecificidade)
            .ThenBy(x => x.Ordem)
            .ThenByDescending(x => x.Versao)
            .ThenBy(x => x.Nome)
            .Select(x => new RegraAprovacaoCandidataResponse(
                x.Id,
                x.Nome,
                x.Versao,
                x.Prioridade,
                x.Ordem,
                CalcularEspecificidade(x),
                x.ExigeAprovacao,
                x.Bloqueante,
                x.EfeitoOperacional,
                x.EfeitoOperacional.ToString(),
                x.TipoFluxoAprovacao,
                x.TipoFluxoAprovacao.ToString(),
                x.TipoResolucaoAprovador,
                x.TipoResolucaoAprovador.ToString(),
                x.AprovadorEspecificoUsuarioId,
                x.AprovadorPadraoUsuarioId,
                x.PrazoDecisaoHoras,
                "Regra ativa, vigente e compativel com o contexto informado."))
            .ToArray();

        var melhorRegra = regrasCandidatas.FirstOrDefault();
        if (melhorRegra is null)
        {
            return new AvaliacaoConfiguracaoRegraAprovacaoResponse
            {
                RegraAplicavel = false,
                Motivo = "Nenhuma configuracao de regra ativa e vigente correspondeu ao contexto informado.",
                Avisos = ["A avaliacao foi executada de forma interna e nao gera aprovacao, bloqueio nem alteracao de status por si so."]
            };
        }

        var avisos = new List<string>
        {
            "A avaliacao foi executada de forma interna e nao altera status do chamado nem aciona bloqueio operacional."
        };

        if (melhorRegra.TipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.NaoDefinido && melhorRegra.ExigeAprovacao)
        {
            avisos.Add("A regra selecionada exige aprovacao, mas ainda nao possui aprovador resolvido automaticamente.");
        }

        return new AvaliacaoConfiguracaoRegraAprovacaoResponse
        {
            RegraAplicavel = true,
            MelhorRegra = melhorRegra,
            RegrasCandidatas = regrasCandidatas,
            ExigeAprovacao = melhorRegra.ExigeAprovacao,
            Bloqueante = melhorRegra.Bloqueante,
            EfeitoOperacional = melhorRegra.EfeitoOperacional,
            TipoFluxoAprovacao = melhorRegra.TipoFluxoAprovacao,
            TipoResolucaoAprovador = melhorRegra.TipoResolucaoAprovador,
            AprovadorEspecificoUsuarioId = melhorRegra.AprovadorEspecificoUsuarioId,
            AprovadorPadraoUsuarioId = melhorRegra.AprovadorPadraoUsuarioId,
            PrazoDecisaoHoras = melhorRegra.PrazoDecisaoHoras,
            Motivo = "Melhor regra selecionada por prioridade, especificidade, ordem e versao.",
            Avisos = avisos
        };
    }

    private async Task<bool> ExisteInstanciaEquivalenteAsync(
        Chamado chamado,
        ConfiguracaoRegraAprovacao configuracao,
        ContextoAvaliacaoRegraAprovacaoRequest contexto,
        CancellationToken cancellationToken)
    {
        return await instanciaAprovacaoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.ChamadoId == chamado.Id &&
                x.ConfiguracaoRegraAprovacaoId == configuracao.Id &&
                (x.Status == StatusInstanciaAprovacaoChamado.Pendente || x.Status == StatusInstanciaAprovacaoChamado.EmReavaliacao) &&
                x.NaturezaChamado == contexto.NaturezaChamado &&
                x.TipoSolicitacaoId == contexto.TipoSolicitacaoId &&
                x.CatalogoServicoId == contexto.CatalogoServicoId &&
                x.CategoriaId == contexto.CategoriaId &&
                x.SubcategoriaId == contexto.SubcategoriaId &&
                x.ImpactoAvaliado == contexto.ImpactoChamado &&
                x.UrgenciaAvaliada == contexto.UrgenciaChamado &&
                x.PrioridadeAvaliada == contexto.PrioridadeChamado &&
                x.CustoAvaliado == contexto.Custo &&
                x.NivelRiscoAvaliado == contexto.NivelRisco,
                cancellationToken);
    }

    private async Task<bool> ExisteAprovacaoLegadaEquivalenteAsync(Chamado chamado, CancellationToken cancellationToken)
    {
        if (!chamado.CatalogoServicoId.HasValue)
        {
            return false;
        }

        return await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x =>
                x.Ativo &&
                x.ChamadoId == chamado.Id &&
                x.Status == StatusAprovacaoChamado.Pendente &&
                x.TipoOrigem == TipoOrigemAprovacaoChamado.CatalogoServico,
                cancellationToken);
    }

    private static Guid? ResolverAprovador(ConfiguracaoRegraAprovacao configuracao)
        => configuracao.TipoResolucaoAprovador switch
        {
            TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico => configuracao.AprovadorEspecificoUsuarioId,
            TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao => configuracao.AprovadorPadraoUsuarioId,
            _ => null
        };

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

    private static GerarAprovacaoObrigatoriaChamadoResponse CriarRespostaSemGeracao(
        string motivo,
        Guid? configuracaoRegraAprovacaoId = null,
        string? nomeRegra = null,
        int? versaoRegra = null,
        bool exigeAprovacao = false,
        bool bloqueante = false,
        EfeitoOperacionalRegraAprovacao? efeitoOperacional = null,
        TipoFluxoAprovacao? tipoFluxoAprovacao = null,
        TipoResolucaoAprovadorRegraAprovacao? tipoResolucaoAprovador = null,
        Guid? aprovadorResolvidoUsuarioId = null,
        int? prazoDecisaoHoras = null,
        bool gerouAprovacao = false,
        bool jaExistiaAprovacaoEquivalente = false,
        DateTime? deveExpirarEm = null,
        IReadOnlyCollection<string>? avisos = null)
        => new()
        {
            GerouAprovacao = gerouAprovacao,
            ConfiguracaoRegraAprovacaoId = configuracaoRegraAprovacaoId,
            NomeRegra = nomeRegra,
            VersaoRegra = versaoRegra,
            ExigeAprovacao = exigeAprovacao,
            Bloqueante = bloqueante,
            EfeitoOperacional = efeitoOperacional,
            TipoFluxoAprovacao = tipoFluxoAprovacao,
            TipoResolucaoAprovador = tipoResolucaoAprovador,
            AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId,
            PrazoDecisaoHoras = prazoDecisaoHoras,
            DeveExpirarEm = deveExpirarEm,
            JaExistiaAprovacaoEquivalente = jaExistiaAprovacaoEquivalente,
            Motivo = motivo,
            Avisos = avisos ?? []
        };
}
