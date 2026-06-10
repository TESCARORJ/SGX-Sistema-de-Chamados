using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase(
    IRepository<InstanciaAprovacaoChamado> instanciaRepository,
    IRepository<EtapaAprovacaoChamado> etapaRepository,
    IRepository<DecisaoAprovacaoChamado> decisaoRepository,
    IRepository<Usuario> usuarioRepository,
    IAdminConfiguracaoRegraAprovacaoUseCases configuracaoRegraAprovacaoUseCases,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReavaliarAprovacaoChamadoPorMudancaDadosSensiveisUseCase
{
    private static readonly ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequestValidator Validator = new();

    public async Task<ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse> ExecutarAsync(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request,
        CancellationToken cancellationToken = default)
    {
        await Validator.ValidateAndThrowAsync(request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        var usuarioResponsavelId = request.UsuarioId ?? usuarioAtual.Id;

        var usuarioResponsavelExiste = await usuarioRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == usuarioResponsavelId, cancellationToken);

        if (!usuarioResponsavelExiste)
        {
            throw new InvalidOperationException("O usuario responsavel pela reavaliacao nao foi encontrado.");
        }

        var mudancas = DetectarMudancasSensiveis(request);
        if (mudancas.Count == 0)
        {
            return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
            {
                ReavaliacaoExecutada = false,
                ReavaliacaoNecessaria = false,
                MudancasSensiveisDetectadas = mudancas,
                PermiteContinuar = true,
                Motivo = "Nenhuma mudanca sensivel foi identificada no contexto informado.",
                Avisos =
                [
                    "A regra foi avaliada de forma consultiva e nao alterou aprovacao, status do chamado ou SLA."
                ]
            };
        }

        var contextoAnterior = MontarContextoAnterior(request);
        var contextoNovo = MontarContextoNovo(request);
        var avaliacaoAnterior = await configuracaoRegraAprovacaoUseCases.AvaliarRegraAsync(contextoAnterior, cancellationToken);
        var avaliacaoNova = await configuracaoRegraAprovacaoUseCases.AvaliarRegraAsync(contextoNovo, cancellationToken);

        var instancia = await ObterInstanciaAsync(request, cancellationToken);
        if (instancia is null)
        {
            var exigeNovaAprovacaoSemInstancia = avaliacaoNova.RegraAplicavel && avaliacaoNova.ExigeAprovacao;
            return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
            {
                ReavaliacaoExecutada = false,
                ReavaliacaoNecessaria = false,
                MudancasSensiveisDetectadas = mudancas,
                ExigeNovaAprovacao = exigeNovaAprovacaoSemInstancia,
                MantemBloqueio = false,
                PermiteContinuar = !exigeNovaAprovacaoSemInstancia,
                Motivo = exigeNovaAprovacaoSemInstancia
                    ? "Nao existe instancia relacionada para reavaliar, mas o novo contexto indica necessidade de nova aprovacao futura."
                    : "Nao existe instancia relacionada para reavaliacao e o novo contexto nao exige nova aprovacao automatica nesta etapa.",
                Avisos =
                [
                    "Nenhuma instancia foi criada automaticamente.",
                    "A regra nao alterou status do chamado, AguardandoAprovacao, BloqueiaAvancoAtendimento ou SLA."
                ]
            };
        }

        if (instancia.ChamadoId != request.ChamadoId)
        {
            throw new InvalidOperationException("A instancia informada nao pertence ao chamado informado.");
        }

        var statusInstanciaAnterior = instancia.Status;

        if (instancia.Status is StatusInstanciaAprovacaoChamado.Cancelada or StatusInstanciaAprovacaoChamado.Expirada or StatusInstanciaAprovacaoChamado.Substituida)
        {
            var exigeNovaAprovacaoInativa = avaliacaoNova.RegraAplicavel && avaliacaoNova.ExigeAprovacao;
            return CriarRespostaConsultiva(
                instancia,
                statusInstanciaAnterior,
                mudancas,
                exigeNovaAprovacaoInativa,
                !exigeNovaAprovacaoInativa,
                "A instancia existente esta encerrada para reavaliacao controlada nesta etapa.",
                "Instancias canceladas, expiradas ou substituidas nao sao reativadas automaticamente.");
        }

        if (instancia.Status == StatusInstanciaAprovacaoChamado.Reprovada)
        {
            var exigeNovaAprovacaoReprovada = avaliacaoNova.RegraAplicavel && avaliacaoNova.ExigeAprovacao;
            return CriarRespostaConsultiva(
                instancia,
                statusInstanciaAnterior,
                mudancas,
                exigeNovaAprovacaoReprovada,
                !exigeNovaAprovacaoReprovada,
                "A instancia existente ja foi reprovada e nao sera reaberta automaticamente.",
                "Uma nova solicitacao ou nova geracao controlada de aprovacao deve ser tratada em fluxo posterior.");
        }

        var aprovacaoContinuaValida = AprovacaoAnteriorCobreNovoContexto(instancia, request, avaliacaoNova);
        if (aprovacaoContinuaValida)
        {
            return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
            {
                ReavaliacaoExecutada = false,
                ReavaliacaoNecessaria = false,
                InstanciaAprovacaoChamadoId = instancia.Id,
                StatusInstanciaAnterior = statusInstanciaAnterior,
                StatusInstanciaNovo = statusInstanciaAnterior,
                MudancasSensiveisDetectadas = mudancas,
                ExigeNovaAprovacao = false,
                MantemBloqueio = instancia.Bloqueante,
                PermiteContinuar = PermiteContinuar(instancia.Bloqueante, statusInstanciaAnterior, false),
                Motivo = "A aprovacao existente continua valida para o novo contexto sensivel informado.",
                Avisos =
                [
                    "Nenhuma decisao de reavaliacao foi criada porque o novo contexto permanece coberto pelo escopo aprovado.",
                    "A regra nao alterou status do chamado, AguardandoAprovacao ou SLA."
                ]
            };
        }

        var exigeNovaAprovacao = avaliacaoNova.RegraAplicavel && avaliacaoNova.ExigeAprovacao;
        var statusInstanciaNovo = StatusInstanciaAprovacaoChamado.EmReavaliacao;
        var escopoAnteriorSnapshot = MontarEscopoSnapshotAnterior(request, instancia);
        var escopoNovoSnapshot = MontarEscopoSnapshotNovo(request);
        var resultadoDecisao = exigeNovaAprovacao
            ? ResultadoDecisaoAprovacaoChamado.RequerNovaAprovacao
            : ResultadoDecisaoAprovacaoChamado.RequerAjuste;

        if (ExisteReavaliacaoDuplicada(instancia, escopoNovoSnapshot, resultadoDecisao))
        {
            return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
            {
                ReavaliacaoExecutada = false,
                ReavaliacaoNecessaria = true,
                InstanciaAprovacaoChamadoId = instancia.Id,
                StatusInstanciaAnterior = statusInstanciaAnterior,
                StatusInstanciaNovo = instancia.Status,
                MudancasSensiveisDetectadas = mudancas,
                ExigeNovaAprovacao = exigeNovaAprovacao,
                MantemBloqueio = instancia.Bloqueante,
                PermiteContinuar = PermiteContinuar(instancia.Bloqueante, instancia.Status, exigeNovaAprovacao),
                Motivo = "Ja existe registro de reavaliacao equivalente para o novo escopo informado.",
                Avisos =
                [
                    "Nenhuma decisao duplicada foi criada.",
                    "A trilha historica anterior foi preservada integralmente."
                ]
            };
        }

        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            etapaAprovacaoChamadoId: null,
            tipoDecisao: TipoDecisaoAprovacaoChamado.Reavaliacao,
            resultado: resultadoDecisao,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.RequerReavaliacao,
            statusInstanciaAnterior: statusInstanciaAnterior,
            statusInstanciaNovo: statusInstanciaNovo,
            criadoPorUsuarioId: usuarioAtual.Id,
            criadoPor: usuarioAtual.Login,
            decisorUsuarioId: usuarioResponsavelId,
            papelDecisorSnapshot: "Responsavel pela reavaliacao de aprovacao",
            autoridadeDecisorSnapshot: ObterAutoridadeDecisor(instancia, usuarioResponsavelId),
            decisorEhAprovadorEspecifico: usuarioResponsavelId == instancia.AprovadorEspecificoUsuarioId,
            decisorEhAprovadorPadrao: usuarioResponsavelId == instancia.AprovadorPadraoUsuarioId,
            decisorEhMembroGrupo: false,
            decisorPorDelegacao: false,
            grupoAprovadorSnapshot: null,
            justificativa: request.Motivo.Trim(),
            observacao: CriarObservacaoReavaliacao(escopoAnteriorSnapshot, escopoNovoSnapshot, avaliacaoAnterior, avaliacaoNova, mudancas),
            escopoDecididoSnapshot: escopoNovoSnapshot,
            decisaoParcial: false,
            decisaoFinal: false,
            liberaAvanco: false,
            mantemBloqueio: instancia.Bloqueante,
            exigeReavaliacao: true,
            permiteNovaSolicitacao: exigeNovaAprovacao,
            cancelaFluxo: false,
            statusEtapaAnterior: null,
            statusEtapaNovo: null,
            nivelEtapaSnapshot: null,
            ordemEtapaSnapshot: null,
            ramoEtapaSnapshot: null,
            regraNomeSnapshot: avaliacaoNova.MelhorRegra?.NomeRegra ?? instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: avaliacaoNova.MelhorRegra?.VersaoRegra ?? instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: MontarSnapshotRegra(instancia, avaliacaoAnterior, avaliacaoNova));

        if (instancia.Status != StatusInstanciaAprovacaoChamado.EmReavaliacao)
        {
            instancia.MarcarEmReavaliacao(usuarioAtual.Id, usuarioAtual.Login);
            instanciaRepository.Update(instancia);
        }

        foreach (var etapa in instancia.Etapas.Where(PodeMarcarEtapaEmReavaliacao))
        {
            etapa.MarcarEmReavaliacao(usuarioAtual.Id, usuarioAtual.Login);
            etapaRepository.Update(etapa);
        }

        await decisaoRepository.AddAsync(decisao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse
        {
            ReavaliacaoExecutada = true,
            ReavaliacaoNecessaria = true,
            InstanciaAprovacaoChamadoId = instancia.Id,
            DecisaoAprovacaoChamadoId = decisao.Id,
            StatusInstanciaAnterior = statusInstanciaAnterior,
            StatusInstanciaNovo = statusInstanciaNovo,
            MudancasSensiveisDetectadas = mudancas,
            ExigeNovaAprovacao = exigeNovaAprovacao,
            MantemBloqueio = instancia.Bloqueante,
            PermiteContinuar = PermiteContinuar(instancia.Bloqueante, statusInstanciaNovo, exigeNovaAprovacao),
            Motivo = exigeNovaAprovacao
                ? "O novo contexto extrapolou o escopo aprovado e a instancia foi colocada em reavaliacao."
                : "O novo contexto exige revisao controlada da aprovacao existente, sem aprovacao ou rejeicao automatica.",
            Avisos =
            [
                "Decisoes anteriores foram preservadas e nenhuma aprovacao anterior foi apagada.",
                "A regra nao alterou status do chamado, AguardandoAprovacao, BloqueiaAvancoAtendimento ou SLA.",
                "Nenhuma nova instancia, endpoint, controller ou workflow completo foi criado automaticamente."
            ]
        };
    }

    private async Task<InstanciaAprovacaoChamado?> ObterInstanciaAsync(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request,
        CancellationToken cancellationToken)
    {
        var query = instanciaRepository.Query()
            .Include(x => x.Etapas)
            .Include(x => x.Decisoes)
            .Where(x => x.ChamadoId == request.ChamadoId);

        if (request.InstanciaAprovacaoChamadoId.HasValue)
        {
            return await query.FirstOrDefaultAsync(x => x.Id == request.InstanciaAprovacaoChamadoId.Value, cancellationToken);
        }

        return await query
            .OrderByDescending(x => x.CriadoEm)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static ContextoAvaliacaoRegraAprovacaoRequest MontarContextoAnterior(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request)
        => new()
        {
            NaturezaChamado = request.NaturezaAnterior,
            TipoSolicitacaoId = request.TipoSolicitacaoAnteriorId,
            CatalogoServicoId = request.CatalogoServicoAnteriorId,
            CategoriaId = request.CategoriaAnteriorId,
            SubcategoriaId = request.SubcategoriaAnteriorId,
            ImpactoChamado = request.ImpactoAnterior,
            UrgenciaChamado = request.UrgenciaAnterior,
            PrioridadeChamado = request.PrioridadeAnterior,
            Custo = request.CustoAnterior,
            NivelRisco = request.NivelRiscoAnterior,
            DataReferencia = request.DataReferencia ?? DateTime.UtcNow
        };

    private static ContextoAvaliacaoRegraAprovacaoRequest MontarContextoNovo(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request)
        => new()
        {
            NaturezaChamado = request.NaturezaNova,
            TipoSolicitacaoId = request.TipoSolicitacaoNovoId,
            CatalogoServicoId = request.CatalogoServicoNovoId,
            CategoriaId = request.CategoriaNovaId,
            SubcategoriaId = request.SubcategoriaNovaId,
            ImpactoChamado = request.ImpactoNovo,
            UrgenciaChamado = request.UrgenciaNova,
            PrioridadeChamado = request.PrioridadeNova,
            Custo = request.CustoNovo,
            NivelRisco = request.NivelRiscoNovo,
            DataReferencia = request.DataReferencia ?? DateTime.UtcNow
        };

    private static List<string> DetectarMudancasSensiveis(ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request)
    {
        var mudancas = new List<string>();

        AdicionarSeMudou(mudancas, "NaturezaChamado", request.NaturezaAnterior, request.NaturezaNova);
        AdicionarSeMudou(mudancas, "TipoSolicitacaoId", request.TipoSolicitacaoAnteriorId, request.TipoSolicitacaoNovoId);
        AdicionarSeMudou(mudancas, "CatalogoServicoId", request.CatalogoServicoAnteriorId, request.CatalogoServicoNovoId);
        AdicionarSeMudou(mudancas, "CategoriaId", request.CategoriaAnteriorId, request.CategoriaNovaId);
        AdicionarSeMudou(mudancas, "SubcategoriaId", request.SubcategoriaAnteriorId, request.SubcategoriaNovaId);
        AdicionarSeMudou(mudancas, "ImpactoChamado", request.ImpactoAnterior, request.ImpactoNovo);
        AdicionarSeMudou(mudancas, "UrgenciaChamado", request.UrgenciaAnterior, request.UrgenciaNova);
        AdicionarSeMudou(mudancas, "PrioridadeChamado", request.PrioridadeAnterior, request.PrioridadeNova);
        AdicionarSeMudou(mudancas, "Custo", request.CustoAnterior, request.CustoNovo);
        AdicionarSeMudou(mudancas, "NivelRisco", request.NivelRiscoAnterior, request.NivelRiscoNovo);

        if (!string.Equals(request.EscopoAnteriorSnapshot?.Trim(), request.EscopoNovoSnapshot?.Trim(), StringComparison.Ordinal))
        {
            mudancas.Add("EscopoSensivelSnapshot");
        }

        return mudancas;
    }

    private static bool AprovacaoAnteriorCobreNovoContexto(
        InstanciaAprovacaoChamado instancia,
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request,
        AvaliacaoConfiguracaoRegraAprovacaoResponse avaliacaoNova)
    {
        if (!avaliacaoNova.RegraAplicavel || !avaliacaoNova.ExigeAprovacao)
        {
            return true;
        }

        if (avaliacaoNova.MelhorRegra?.ConfiguracaoRegraAprovacaoId != instancia.ConfiguracaoRegraAprovacaoId)
        {
            return false;
        }

        if (!ContextoIdentificadorCoberto(instancia.NaturezaChamado, request.NaturezaNova) ||
            !ContextoIdentificadorCoberto(instancia.TipoSolicitacaoId, request.TipoSolicitacaoNovoId) ||
            !ContextoIdentificadorCoberto(instancia.CatalogoServicoId, request.CatalogoServicoNovoId) ||
            !ContextoIdentificadorCoberto(instancia.CategoriaId, request.CategoriaNovaId) ||
            !ContextoIdentificadorCoberto(instancia.SubcategoriaId, request.SubcategoriaNovaId))
        {
            return false;
        }

        if (!ContextoEscalarCoberto(instancia.ImpactoAvaliado, request.ImpactoNovo) ||
            !ContextoEscalarCoberto(instancia.UrgenciaAvaliada, request.UrgenciaNova) ||
            !ContextoEscalarCoberto(instancia.PrioridadeAvaliada, request.PrioridadeNova) ||
            !ContextoEscalarCoberto(instancia.CustoAvaliado, request.CustoNovo) ||
            !ContextoEscalarCoberto(instancia.NivelRiscoAvaliado, request.NivelRiscoNovo))
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(request.EscopoAnteriorSnapshot) &&
            !string.IsNullOrWhiteSpace(request.EscopoNovoSnapshot) &&
            !string.Equals(request.EscopoAnteriorSnapshot.Trim(), request.EscopoNovoSnapshot.Trim(), StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static bool ContextoIdentificadorCoberto<T>(T? valorAprovado, T? valorNovo)
        where T : struct
        => !valorAprovado.HasValue || valorAprovado.Value.Equals(valorNovo);

    private static bool ContextoEscalarCoberto<T>(T? valorAprovado, T? valorNovo)
        where T : struct, IComparable
    {
        if (!valorAprovado.HasValue)
        {
            return true;
        }

        if (!valorNovo.HasValue)
        {
            return false;
        }

        return valorNovo.Value.CompareTo(valorAprovado.Value) <= 0;
    }

    private static bool ExisteReavaliacaoDuplicada(
        InstanciaAprovacaoChamado instancia,
        string escopoNovoSnapshot,
        ResultadoDecisaoAprovacaoChamado resultado)
        => instancia.Decisoes.Any(x =>
            x.TipoDecisao == TipoDecisaoAprovacaoChamado.Reavaliacao &&
            x.Resultado == resultado &&
            x.StatusInstanciaNovo == StatusInstanciaAprovacaoChamado.EmReavaliacao &&
            string.Equals(x.EscopoDecididoSnapshot, escopoNovoSnapshot, StringComparison.Ordinal));

    private static string MontarEscopoSnapshotAnterior(
        ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request,
        InstanciaAprovacaoChamado instancia)
        => !string.IsNullOrWhiteSpace(request.EscopoAnteriorSnapshot)
            ? request.EscopoAnteriorSnapshot.Trim()
            : CriarResumoContexto(
                request.NaturezaAnterior,
                request.TipoSolicitacaoAnteriorId,
                request.CatalogoServicoAnteriorId,
                request.CategoriaAnteriorId,
                request.SubcategoriaAnteriorId,
                request.ImpactoAnterior,
                request.UrgenciaAnterior,
                request.PrioridadeAnterior,
                request.CustoAnterior,
                request.NivelRiscoAnterior,
                instancia.RegraCriterioSnapshot);

    private static string MontarEscopoSnapshotNovo(ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisRequest request)
        => !string.IsNullOrWhiteSpace(request.EscopoNovoSnapshot)
            ? request.EscopoNovoSnapshot.Trim()
            : CriarResumoContexto(
                request.NaturezaNova,
                request.TipoSolicitacaoNovoId,
                request.CatalogoServicoNovoId,
                request.CategoriaNovaId,
                request.SubcategoriaNovaId,
                request.ImpactoNovo,
                request.UrgenciaNova,
                request.PrioridadeNova,
                request.CustoNovo,
                request.NivelRiscoNovo,
                null);

    private static string CriarResumoContexto(
        NaturezaChamadoEnum? natureza,
        Guid? tipoSolicitacaoId,
        Guid? catalogoServicoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        ImpactoChamadoEnum? impacto,
        UrgenciaChamadoEnum? urgencia,
        PrioridadeChamadoEnum? prioridade,
        decimal? custo,
        int? nivelRisco,
        string? escopoComplementar)
    {
        var partes = new List<string>
        {
            $"Natureza={natureza?.ToString() ?? "null"}",
            $"TipoSolicitacaoId={tipoSolicitacaoId?.ToString() ?? "null"}",
            $"CatalogoServicoId={catalogoServicoId?.ToString() ?? "null"}",
            $"CategoriaId={categoriaId?.ToString() ?? "null"}",
            $"SubcategoriaId={subcategoriaId?.ToString() ?? "null"}",
            $"Impacto={impacto?.ToString() ?? "null"}",
            $"Urgencia={urgencia?.ToString() ?? "null"}",
            $"Prioridade={prioridade?.ToString() ?? "null"}",
            $"Custo={custo?.ToString("0.##") ?? "null"}",
            $"NivelRisco={nivelRisco?.ToString() ?? "null"}"
        };

        if (!string.IsNullOrWhiteSpace(escopoComplementar))
        {
            partes.Add($"Escopo={escopoComplementar.Trim()}");
        }

        return string.Join("; ", partes);
    }

    private static string CriarObservacaoReavaliacao(
        string escopoAnteriorSnapshot,
        string escopoNovoSnapshot,
        AvaliacaoConfiguracaoRegraAprovacaoResponse avaliacaoAnterior,
        AvaliacaoConfiguracaoRegraAprovacaoResponse avaliacaoNova,
        IReadOnlyCollection<string> mudancas)
    {
        var regraAnterior = avaliacaoAnterior.MelhorRegra is null
            ? "sem_regra"
            : $"{avaliacaoAnterior.MelhorRegra.NomeRegra} v{avaliacaoAnterior.MelhorRegra.VersaoRegra}";
        var regraNova = avaliacaoNova.MelhorRegra is null
            ? "sem_regra"
            : $"{avaliacaoNova.MelhorRegra.NomeRegra} v{avaliacaoNova.MelhorRegra.VersaoRegra}";

        return $"Mudancas={string.Join(", ", mudancas)} | RegraAnterior={regraAnterior} | RegraNova={regraNova} | EscopoAnterior={escopoAnteriorSnapshot} | EscopoNovo={escopoNovoSnapshot}";
    }

    private static string? ObterAutoridadeDecisor(InstanciaAprovacaoChamado instancia, Guid usuarioResponsavelId)
    {
        if (usuarioResponsavelId == instancia.AprovadorEspecificoUsuarioId)
        {
            return "Aprovador especifico vinculado a instancia";
        }

        if (usuarioResponsavelId == instancia.AprovadorPadraoUsuarioId)
        {
            return "Aprovador padrao vinculado a instancia";
        }

        if (usuarioResponsavelId == instancia.AprovadorResolvidoUsuarioId)
        {
            return "Aprovador resolvido vinculado a instancia";
        }

        return "Responsavel operacional pela reavaliacao";
    }

    private static string? MontarSnapshotRegra(
        InstanciaAprovacaoChamado instancia,
        AvaliacaoConfiguracaoRegraAprovacaoResponse avaliacaoAnterior,
        AvaliacaoConfiguracaoRegraAprovacaoResponse avaliacaoNova)
        => $"RegraAnterior={avaliacaoAnterior.MelhorRegra?.NomeRegra ?? instancia.RegraNomeSnapshot ?? "sem_regra"}; RegraNova={avaliacaoNova.MelhorRegra?.NomeRegra ?? instancia.RegraNomeSnapshot ?? "sem_regra"}; CriterioAnterior={instancia.RegraCriterioSnapshot ?? "nao_informado"}";

    private static bool PodeMarcarEtapaEmReavaliacao(EtapaAprovacaoChamado etapa)
        => etapa.Status == StatusEtapaAprovacaoChamado.Pendente ||
           etapa.Status == StatusEtapaAprovacaoChamado.Aprovada;

    private static bool PermiteContinuar(
        bool mantemBloqueio,
        StatusInstanciaAprovacaoChamado statusInstancia,
        bool exigeNovaAprovacao)
    {
        if (exigeNovaAprovacao)
        {
            return false;
        }

        return !(mantemBloqueio &&
                 (statusInstancia == StatusInstanciaAprovacaoChamado.Pendente ||
                  statusInstancia == StatusInstanciaAprovacaoChamado.EmReavaliacao));
    }

    private static ReavaliarAprovacaoChamadoPorMudancaDadosSensiveisResponse CriarRespostaConsultiva(
        InstanciaAprovacaoChamado instancia,
        StatusInstanciaAprovacaoChamado statusInstanciaAnterior,
        IReadOnlyCollection<string> mudancas,
        bool exigeNovaAprovacao,
        bool permiteContinuar,
        string motivo,
        string avisoPrincipal)
        => new()
        {
            ReavaliacaoExecutada = false,
            ReavaliacaoNecessaria = false,
            InstanciaAprovacaoChamadoId = instancia.Id,
            StatusInstanciaAnterior = statusInstanciaAnterior,
            StatusInstanciaNovo = statusInstanciaAnterior,
            MudancasSensiveisDetectadas = mudancas,
            ExigeNovaAprovacao = exigeNovaAprovacao,
            MantemBloqueio = instancia.Bloqueante,
            PermiteContinuar = permiteContinuar,
            Motivo = motivo,
            Avisos =
            [
                avisoPrincipal,
                "A regra nao reativou instancia nem alterou status do chamado ou SLA."
            ]
        };

    private static void AdicionarSeMudou<T>(ICollection<string> mudancas, string nomeCampo, T valorAnterior, T valorNovo)
    {
        if (!EqualityComparer<T>.Default.Equals(valorAnterior, valorNovo))
        {
            mudancas.Add(nomeCampo);
        }
    }
}
