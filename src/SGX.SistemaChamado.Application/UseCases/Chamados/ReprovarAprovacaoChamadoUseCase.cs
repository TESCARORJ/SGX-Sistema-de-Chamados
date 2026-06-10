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

public sealed class ReprovarAprovacaoChamadoUseCase(
    IRepository<InstanciaAprovacaoChamado> instanciaRepository,
    IRepository<EtapaAprovacaoChamado> etapaRepository,
    IRepository<DecisaoAprovacaoChamado> decisaoRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IReprovarAprovacaoChamadoUseCase
{
    private static readonly ReprovarAprovacaoChamadoRequestValidator Validator = new();

    public async Task<ReprovarAprovacaoChamadoResponse> ExecutarAsync(
        ReprovarAprovacaoChamadoRequest request,
        CancellationToken cancellationToken = default)
    {
        await Validator.ValidateAndThrowAsync(request, cancellationToken);

        var usuarioAtual = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);

        var instancia = await instanciaRepository.Query()
            .Include(x => x.Etapas)
            .Include(x => x.Decisoes)
            .FirstOrDefaultAsync(x => x.Id == request.InstanciaAprovacaoChamadoId, cancellationToken)
            ?? throw new KeyNotFoundException("Instancia de aprovacao nao encontrada.");

        EtapaAprovacaoChamado? etapa = null;
        if (request.EtapaAprovacaoChamadoId.HasValue)
        {
            etapa = instancia.Etapas.FirstOrDefault(x => x.Id == request.EtapaAprovacaoChamadoId.Value)
                ?? throw new InvalidOperationException("A etapa informada nao pertence a instancia de aprovacao.");

            ValidarEtapaReprovavel(etapa);
        }

        ValidarInstanciaReprovavel(instancia);
        ValidarDuplicidadeFinal(instancia, etapa);

        var decisorUsuarioId = ResolverDecisor(instancia, etapa, request);
        if (!decisorUsuarioId.HasValue || decisorUsuarioId.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Nao foi possivel identificar um decisor valido para registrar a reprovacao.");
        }

        var decisorExiste = await usuarioRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == decisorUsuarioId.Value, cancellationToken);

        if (!decisorExiste)
        {
            throw new InvalidOperationException("O decisor informado para a reprovacao nao foi encontrado.");
        }

        var statusInstanciaAnterior = instancia.Status;
        var statusEtapaAnterior = etapa?.Status;

        var decisaoParcial = request.DecisaoParcial;
        var decisaoFinal = request.DecisaoFinal;

        if (!request.EtapaAprovacaoChamadoId.HasValue && !decisaoParcial && !decisaoFinal)
        {
            decisaoFinal = true;
        }

        if (!request.EtapaAprovacaoChamadoId.HasValue && PossuiEtapaObrigatoriaPendente(instancia))
        {
            throw new InvalidOperationException("A instancia nao pode ser reprovada diretamente enquanto existir etapa obrigatoria pendente.");
        }

        if (request.DecisaoParcial && request.CancelaFluxo)
        {
            throw new InvalidOperationException("Uma decisao parcial de reprovacao nao pode cancelar o fluxo logico nesta etapa.");
        }

        var statusInstanciaNovo = DeterminarStatusInstanciaNovo(instancia, etapa, request, decisaoFinal);
        StatusEtapaAprovacaoChamado? statusEtapaNovo = etapa is null
            ? null
            : StatusEtapaAprovacaoChamado.Reprovada;

        var mantemBloqueio = request.MantemBloqueio || (instancia.Bloqueante && statusInstanciaNovo != StatusInstanciaAprovacaoChamado.Aprovada);
        var escopoDecidido = string.IsNullOrWhiteSpace(request.EscopoDecididoSnapshot)
            ? etapa?.EscopoResumoSnapshot ?? instancia.RegraCriterioSnapshot
            : request.EscopoDecididoSnapshot.Trim();
        var efeitoOperacional = request.ExigeReavaliacao
            ? EfeitoOperacionalRegraAprovacao.RequerReavaliacao
            : instancia.EfeitoOperacional;

        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            etapaAprovacaoChamadoId: etapa?.Id,
            tipoDecisao: TipoDecisaoAprovacaoChamado.Rejeicao,
            resultado: ResultadoDecisaoAprovacaoChamado.Reprovada,
            efeitoOperacional: efeitoOperacional,
            statusInstanciaAnterior: statusInstanciaAnterior,
            statusInstanciaNovo: statusInstanciaNovo,
            criadoPorUsuarioId: usuarioAtual.Id,
            criadoPor: usuarioAtual.Login,
            decisorUsuarioId: decisorUsuarioId,
            papelDecisorSnapshot: request.PapelDecisorSnapshot,
            autoridadeDecisorSnapshot: string.IsNullOrWhiteSpace(request.AutoridadeDecisorSnapshot)
                ? ObterAutoridadeDecisor(instancia, etapa, decisorUsuarioId.Value)
                : request.AutoridadeDecisorSnapshot,
            decisorEhAprovadorEspecifico: decisorUsuarioId == (etapa?.AprovadorEspecificoUsuarioId ?? instancia.AprovadorEspecificoUsuarioId),
            decisorEhAprovadorPadrao: decisorUsuarioId == (etapa?.AprovadorPadraoUsuarioId ?? instancia.AprovadorPadraoUsuarioId),
            decisorEhMembroGrupo: (etapa?.TipoResolucaoAprovador ?? instancia.TipoResolucaoAprovador) == TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo,
            decisorPorDelegacao: false,
            grupoAprovadorSnapshot: etapa?.GrupoAprovadorSnapshot,
            justificativa: request.Justificativa,
            observacao: request.Observacao,
            escopoDecididoSnapshot: escopoDecidido,
            decisaoParcial: decisaoParcial,
            decisaoFinal: decisaoFinal,
            liberaAvanco: false,
            mantemBloqueio: mantemBloqueio,
            exigeReavaliacao: request.ExigeReavaliacao,
            permiteNovaSolicitacao: request.PermiteNovaSolicitacao,
            cancelaFluxo: request.CancelaFluxo,
            statusEtapaAnterior: statusEtapaAnterior,
            statusEtapaNovo: statusEtapaNovo,
            nivelEtapaSnapshot: etapa?.Nivel,
            ordemEtapaSnapshot: etapa?.Ordem,
            ramoEtapaSnapshot: etapa?.Ramo,
            regraNomeSnapshot: etapa?.RegraNomeSnapshot ?? instancia.RegraNomeSnapshot,
            regraVersaoSnapshot: etapa?.RegraVersaoSnapshot ?? instancia.RegraVersaoSnapshot,
            regraCriterioSnapshot: etapa?.RegraCriterioSnapshot ?? instancia.RegraCriterioSnapshot);

        if (etapa is not null)
        {
            etapa.RegistrarDecisaoResumo(StatusEtapaAprovacaoChamado.Reprovada, decisorUsuarioId.Value, usuarioAtual.Id, usuarioAtual.Login);
            etapaRepository.Update(etapa);
        }

        if (statusInstanciaNovo == StatusInstanciaAprovacaoChamado.Reprovada)
        {
            instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Reprovada, decisorUsuarioId.Value, usuarioAtual.Id, usuarioAtual.Login);
            instanciaRepository.Update(instancia);
        }

        await decisaoRepository.AddAsync(decisao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var avisos = new List<string>();
        if (etapa is not null && statusInstanciaNovo != StatusInstanciaAprovacaoChamado.Reprovada)
        {
            avisos.Add("A etapa foi reprovada, mas a instancia permanece pendente para tratamento posterior compativel.");
        }

        if (request.ExigeReavaliacao)
        {
            avisos.Add("A decisao sinalizou exigencia de reavaliacao futura, sem executar reavaliacao funcional completa neste item.");
        }

        avisos.Add("A reprovacao nao alterou status do chamado, SLA, comentarios, anexos ou outros fluxos operacionais externos.");

        return new ReprovarAprovacaoChamadoResponse
        {
            Reprovada = true,
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa?.Id,
            DecisaoAprovacaoChamadoId = decisao.Id,
            StatusInstanciaAnterior = statusInstanciaAnterior,
            StatusInstanciaNovo = statusInstanciaNovo,
            StatusEtapaAnterior = statusEtapaAnterior,
            StatusEtapaNovo = statusEtapaNovo,
            DecisaoFinal = decisaoFinal,
            MantemBloqueio = mantemBloqueio,
            ExigeReavaliacao = request.ExigeReavaliacao,
            PermiteNovaSolicitacao = request.PermiteNovaSolicitacao,
            CancelaFluxo = request.CancelaFluxo,
            Motivo = statusInstanciaNovo == StatusInstanciaAprovacaoChamado.Reprovada
                ? "Reprovacao registrada com consolidacao negativa da instancia."
                : "Reprovacao registrada com efeito parcial no escopo informado.",
            Avisos = avisos
        };
    }

    private static void ValidarInstanciaReprovavel(InstanciaAprovacaoChamado instancia)
    {
        if (!instancia.ExigeAprovacao)
        {
            throw new InvalidOperationException("A instancia informada nao exige aprovacao formal.");
        }

        if (instancia.Status != StatusInstanciaAprovacaoChamado.Pendente &&
            instancia.Status != StatusInstanciaAprovacaoChamado.EmReavaliacao)
        {
            throw instancia.Status switch
            {
                StatusInstanciaAprovacaoChamado.Aprovada => new InvalidOperationException("A instancia informada ja foi aprovada com decisao final."),
                StatusInstanciaAprovacaoChamado.Reprovada => new InvalidOperationException("A instancia informada ja foi reprovada com decisao final."),
                StatusInstanciaAprovacaoChamado.Cancelada => new InvalidOperationException("A instancia informada foi cancelada e nao pode ser reprovada."),
                StatusInstanciaAprovacaoChamado.Expirada => new InvalidOperationException("A instancia informada expirou e nao pode ser reprovada."),
                StatusInstanciaAprovacaoChamado.Substituida => new InvalidOperationException("A instancia informada foi substituida e nao pode ser reprovada."),
                _ => new InvalidOperationException("A instancia informada nao esta em um estado reprovavel.")
            };
        }
    }

    private static void ValidarEtapaReprovavel(EtapaAprovacaoChamado etapa)
    {
        if (etapa.Status != StatusEtapaAprovacaoChamado.Pendente &&
            etapa.Status != StatusEtapaAprovacaoChamado.EmReavaliacao)
        {
            throw etapa.Status switch
            {
                StatusEtapaAprovacaoChamado.Aprovada => new InvalidOperationException("A etapa informada ja foi aprovada e nao pode ser reprovada nesta etapa."),
                StatusEtapaAprovacaoChamado.Reprovada => new InvalidOperationException("A etapa informada ja foi reprovada."),
                StatusEtapaAprovacaoChamado.Cancelada => new InvalidOperationException("A etapa informada foi cancelada e nao pode ser reprovada."),
                StatusEtapaAprovacaoChamado.Expirada => new InvalidOperationException("A etapa informada expirou e nao pode ser reprovada."),
                StatusEtapaAprovacaoChamado.Substituida => new InvalidOperationException("A etapa informada foi substituida e nao pode ser reprovada."),
                StatusEtapaAprovacaoChamado.Ignorada => new InvalidOperationException("A etapa informada foi ignorada e nao pode ser reprovada."),
                StatusEtapaAprovacaoChamado.AguardandoEtapaAnterior => new InvalidOperationException("A etapa informada ainda aguarda etapa anterior e nao pode ser reprovada nesta implementacao."),
                _ => new InvalidOperationException("A etapa informada nao esta em um estado reprovavel.")
            };
        }
    }

    private static Guid? ResolverDecisor(
        InstanciaAprovacaoChamado instancia,
        EtapaAprovacaoChamado? etapa,
        ReprovarAprovacaoChamadoRequest request)
        => request.DecisorUsuarioId
           ?? etapa?.AprovadorResolvidoUsuarioId
           ?? etapa?.AprovadorEspecificoUsuarioId
           ?? etapa?.AprovadorPadraoUsuarioId
           ?? instancia.AprovadorResolvidoUsuarioId
           ?? instancia.AprovadorEspecificoUsuarioId
           ?? instancia.AprovadorPadraoUsuarioId;

    private static void ValidarDuplicidadeFinal(InstanciaAprovacaoChamado instancia, EtapaAprovacaoChamado? etapa)
    {
        var jaExisteDecisaoFinal = instancia.Decisoes.Any(x =>
            x.TipoDecisao == TipoDecisaoAprovacaoChamado.Rejeicao &&
            x.Resultado == ResultadoDecisaoAprovacaoChamado.Reprovada &&
            x.DecisaoFinal &&
            x.EtapaAprovacaoChamadoId == etapa?.Id);

        if (jaExisteDecisaoFinal)
        {
            throw new InvalidOperationException("Ja existe decisao final negativa registrada para o alvo informado.");
        }
    }

    private static StatusInstanciaAprovacaoChamado DeterminarStatusInstanciaNovo(
        InstanciaAprovacaoChamado instancia,
        EtapaAprovacaoChamado? etapa,
        ReprovarAprovacaoChamadoRequest request,
        bool decisaoFinal)
    {
        if (decisaoFinal)
        {
            return StatusInstanciaAprovacaoChamado.Reprovada;
        }

        if (etapa is null || request.DecisaoParcial)
        {
            return instancia.Status;
        }

        if (etapa.CriticaParaConsolidacao)
        {
            return StatusInstanciaAprovacaoChamado.Reprovada;
        }

        if (etapa.Obrigatoria && !request.PermiteNovaSolicitacao && !request.ExigeReavaliacao)
        {
            return StatusInstanciaAprovacaoChamado.Reprovada;
        }

        return instancia.Status;
    }

    private static bool PossuiEtapaObrigatoriaPendente(InstanciaAprovacaoChamado instancia)
        => instancia.Etapas.Any(x => x.Obrigatoria && EtapaImpedeConsolidacao(x.Status));

    private static bool EtapaImpedeConsolidacao(StatusEtapaAprovacaoChamado status)
        => status == StatusEtapaAprovacaoChamado.Pendente ||
           status == StatusEtapaAprovacaoChamado.AguardandoEtapaAnterior ||
           status == StatusEtapaAprovacaoChamado.EmReavaliacao;

    private static string? ObterAutoridadeDecisor(
        InstanciaAprovacaoChamado instancia,
        EtapaAprovacaoChamado? etapa,
        Guid decisorUsuarioId)
    {
        if (decisorUsuarioId == (etapa?.AprovadorEspecificoUsuarioId ?? instancia.AprovadorEspecificoUsuarioId))
        {
            return "Aprovador especifico da aprovacao";
        }

        if (decisorUsuarioId == (etapa?.AprovadorPadraoUsuarioId ?? instancia.AprovadorPadraoUsuarioId))
        {
            return "Aprovador padrao da aprovacao";
        }

        if (decisorUsuarioId == (etapa?.AprovadorResolvidoUsuarioId ?? instancia.AprovadorResolvidoUsuarioId))
        {
            return "Aprovador resolvido da aprovacao";
        }

        return null;
    }
}
