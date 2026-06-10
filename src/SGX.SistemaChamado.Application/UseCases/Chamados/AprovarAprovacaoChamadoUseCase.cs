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

public sealed class AprovarAprovacaoChamadoUseCase(
    IRepository<InstanciaAprovacaoChamado> instanciaRepository,
    IRepository<EtapaAprovacaoChamado> etapaRepository,
    IRepository<DecisaoAprovacaoChamado> decisaoRepository,
    IRepository<Usuario> usuarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAprovarAprovacaoChamadoUseCase
{
    private static readonly AprovarAprovacaoChamadoRequestValidator Validator = new();

    public async Task<AprovarAprovacaoChamadoResponse> ExecutarAsync(
        AprovarAprovacaoChamadoRequest request,
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

            ValidarEtapaAprovavel(etapa);
        }

        ValidarDuplicidadeFinal(instancia, etapa);
        ValidarInstanciaAprovavel(instancia);

        var decisorUsuarioId = ResolverDecisor(instancia, etapa, request);
        if (!decisorUsuarioId.HasValue || decisorUsuarioId.Value == Guid.Empty)
        {
            throw new InvalidOperationException("Nao foi possivel identificar um decisor valido para registrar a aprovacao.");
        }

        var decisorExiste = await usuarioRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == decisorUsuarioId.Value, cancellationToken);

        if (!decisorExiste)
        {
            throw new InvalidOperationException("O decisor informado para a aprovacao nao foi encontrado.");
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
            throw new InvalidOperationException("A instancia nao pode ser aprovada diretamente enquanto existir etapa obrigatoria pendente.");
        }

        if (request.EtapaAprovacaoChamadoId.HasValue && decisaoFinal && PossuiOutraEtapaObrigatoriaPendente(instancia, request.EtapaAprovacaoChamadoId.Value))
        {
            throw new InvalidOperationException("A instancia nao pode ser aprovada de forma final enquanto houver outra etapa obrigatoria pendente.");
        }

        var statusInstanciaNovo = DeterminarStatusInstanciaNovo(instancia, etapa, decisaoFinal);
        StatusEtapaAprovacaoChamado? statusEtapaNovo = etapa is null
            ? null
            : StatusEtapaAprovacaoChamado.Aprovada;

        var liberaAvanco = request.LiberaAvanco || statusInstanciaNovo == StatusInstanciaAprovacaoChamado.Aprovada;
        var mantemBloqueio = !liberaAvanco && (request.MantemBloqueio || (instancia.Bloqueante && statusInstanciaNovo != StatusInstanciaAprovacaoChamado.Aprovada));
        var escopoDecidido = string.IsNullOrWhiteSpace(request.EscopoDecididoSnapshot)
            ? etapa?.EscopoResumoSnapshot ?? instancia.RegraCriterioSnapshot
            : request.EscopoDecididoSnapshot.Trim();

        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            etapaAprovacaoChamadoId: etapa?.Id,
            tipoDecisao: TipoDecisaoAprovacaoChamado.Aprovacao,
            resultado: ResultadoDecisaoAprovacaoChamado.Aprovada,
            efeitoOperacional: instancia.EfeitoOperacional,
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
            liberaAvanco: liberaAvanco,
            mantemBloqueio: mantemBloqueio,
            exigeReavaliacao: false,
            permiteNovaSolicitacao: request.PermiteNovaSolicitacao,
            cancelaFluxo: false,
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
            etapa.RegistrarDecisaoResumo(StatusEtapaAprovacaoChamado.Aprovada, decisorUsuarioId.Value, usuarioAtual.Id, usuarioAtual.Login);
            etapaRepository.Update(etapa);
        }

        if (statusInstanciaNovo == StatusInstanciaAprovacaoChamado.Aprovada)
        {
            instancia.RegistrarDecisaoResumo(StatusInstanciaAprovacaoChamado.Aprovada, decisorUsuarioId.Value, usuarioAtual.Id, usuarioAtual.Login);
            instanciaRepository.Update(instancia);
        }

        await decisaoRepository.AddAsync(decisao, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var avisos = new List<string>();
        if (etapa is not null && statusInstanciaNovo != StatusInstanciaAprovacaoChamado.Aprovada)
        {
            avisos.Add("A etapa foi aprovada, mas a instancia permanece pendente ate consolidacao posterior ou decisao final valida.");
        }

        avisos.Add("A aprovacao nao alterou status do chamado, SLA, comentarios, anexos ou outros fluxos operacionais externos.");

        return new AprovarAprovacaoChamadoResponse
        {
            Aprovada = true,
            InstanciaAprovacaoChamadoId = instancia.Id,
            EtapaAprovacaoChamadoId = etapa?.Id,
            DecisaoAprovacaoChamadoId = decisao.Id,
            StatusInstanciaAnterior = statusInstanciaAnterior,
            StatusInstanciaNovo = statusInstanciaNovo,
            StatusEtapaAnterior = statusEtapaAnterior,
            StatusEtapaNovo = statusEtapaNovo,
            DecisaoFinal = decisaoFinal,
            LiberaAvanco = liberaAvanco,
            Motivo = statusInstanciaNovo == StatusInstanciaAprovacaoChamado.Aprovada
                ? "Aprovacao registrada e instancia consolidada como aprovada."
                : "Aprovacao registrada com consolidacao parcial da instancia.",
            Avisos = avisos
        };
    }

    private static void ValidarInstanciaAprovavel(InstanciaAprovacaoChamado instancia)
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
                StatusInstanciaAprovacaoChamado.Reprovada => new InvalidOperationException("A instancia informada foi reprovada e nao pode ser aprovada nesta etapa."),
                StatusInstanciaAprovacaoChamado.Cancelada => new InvalidOperationException("A instancia informada foi cancelada e nao pode ser aprovada."),
                StatusInstanciaAprovacaoChamado.Expirada => new InvalidOperationException("A instancia informada expirou e nao pode ser aprovada."),
                StatusInstanciaAprovacaoChamado.Substituida => new InvalidOperationException("A instancia informada foi substituida e nao pode ser aprovada."),
                _ => new InvalidOperationException("A instancia informada nao esta em um estado aprovavel.")
            };
        }
    }

    private static void ValidarEtapaAprovavel(EtapaAprovacaoChamado etapa)
    {
        if (etapa.Status != StatusEtapaAprovacaoChamado.Pendente &&
            etapa.Status != StatusEtapaAprovacaoChamado.EmReavaliacao)
        {
            throw etapa.Status switch
            {
                StatusEtapaAprovacaoChamado.Aprovada => new InvalidOperationException("A etapa informada ja foi aprovada."),
                StatusEtapaAprovacaoChamado.Reprovada => new InvalidOperationException("A etapa informada foi reprovada e nao pode ser aprovada nesta etapa."),
                StatusEtapaAprovacaoChamado.Cancelada => new InvalidOperationException("A etapa informada foi cancelada e nao pode ser aprovada."),
                StatusEtapaAprovacaoChamado.Expirada => new InvalidOperationException("A etapa informada expirou e nao pode ser aprovada."),
                StatusEtapaAprovacaoChamado.Substituida => new InvalidOperationException("A etapa informada foi substituida e nao pode ser aprovada."),
                StatusEtapaAprovacaoChamado.Ignorada => new InvalidOperationException("A etapa informada foi ignorada e nao pode ser aprovada."),
                StatusEtapaAprovacaoChamado.AguardandoEtapaAnterior => new InvalidOperationException("A etapa informada ainda aguarda etapa anterior e nao pode ser aprovada nesta implementacao."),
                _ => new InvalidOperationException("A etapa informada nao esta em um estado aprovavel.")
            };
        }
    }

    private static Guid? ResolverDecisor(
        InstanciaAprovacaoChamado instancia,
        EtapaAprovacaoChamado? etapa,
        AprovarAprovacaoChamadoRequest request)
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
            x.TipoDecisao == TipoDecisaoAprovacaoChamado.Aprovacao &&
            x.Resultado == ResultadoDecisaoAprovacaoChamado.Aprovada &&
            x.DecisaoFinal &&
            x.EtapaAprovacaoChamadoId == etapa?.Id);

        if (jaExisteDecisaoFinal)
        {
            throw new InvalidOperationException("Ja existe decisao final positiva registrada para o alvo informado.");
        }
    }

    private static StatusInstanciaAprovacaoChamado DeterminarStatusInstanciaNovo(
        InstanciaAprovacaoChamado instancia,
        EtapaAprovacaoChamado? etapa,
        bool decisaoFinal)
    {
        if (!decisaoFinal)
        {
            return instancia.Status;
        }

        if (etapa is null)
        {
            return StatusInstanciaAprovacaoChamado.Aprovada;
        }

        return PossuiOutraEtapaObrigatoriaPendente(instancia, etapa.Id)
            ? instancia.Status
            : StatusInstanciaAprovacaoChamado.Aprovada;
    }

    private static bool PossuiEtapaObrigatoriaPendente(InstanciaAprovacaoChamado instancia)
        => instancia.Etapas.Any(x => x.Obrigatoria && EtapaImpedeConsolidacao(x.Status));

    private static bool PossuiOutraEtapaObrigatoriaPendente(InstanciaAprovacaoChamado instancia, Guid etapaIgnoradaId)
        => instancia.Etapas.Any(x => x.Id != etapaIgnoradaId && x.Obrigatoria && EtapaImpedeConsolidacao(x.Status));

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
