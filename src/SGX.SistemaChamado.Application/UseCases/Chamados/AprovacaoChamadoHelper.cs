using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

internal static class AprovacaoChamadoHelper
{
    public const string MensagemBloqueioAprovacaoPendente = "Este chamado aguarda aprovacao antes de seguir para atendimento.";
    public const string MensagemBloqueioAprovacaoReprovada = "Este chamado foi reprovado e nao pode seguir para atendimento.";
    public const string MensagemPortalSemAprovacao = "Este chamado nao requer aprovacao.";
    public const string MensagemPortalAguardandoAprovacao = "Seu chamado esta aguardando aprovacao antes de seguir para atendimento.";
    public const string MensagemPortalAprovado = "Seu chamado foi aprovado e esta liberado para atendimento.";
    public const string MensagemPortalReprovado = "Seu chamado foi reprovado. Verifique a justificativa.";
    public const string MensagemPortalAprovacaoCancelada = "A aprovacao deste chamado foi cancelada.";

    public static EstadoAprovacaoChamado ObterEstado(Chamado chamado)
    {
        var aprovacoesAtivas = chamado.Aprovacoes
            .Where(x => x.Ativo)
            .ToArray();

        if (aprovacoesAtivas.Length == 0)
        {
            return EstadoAprovacaoChamado.SemAprovacao();
        }

        var pendente = aprovacoesAtivas
            .Where(x => x.Status == StatusAprovacaoChamado.Pendente)
            .OrderByDescending(x => x.SolicitadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .FirstOrDefault();

        if (pendente is not null)
        {
            return new EstadoAprovacaoChamado(
                RequerAprovacao: true,
                AprovacaoPendente: true,
                StatusAprovacao: StatusAprovacaoChamado.Pendente,
                AprovacaoChamadoId: pendente.Id,
                AprovacaoSolicitadaEm: pendente.SolicitadaEm,
                AprovacaoDecididaEm: pendente.DecididaEm,
                JustificativaAprovacao: null,
                JustificativaReprovacao: null,
                JustificativaDecisao: null,
                MensagemOrientativa: MensagemPortalAguardandoAprovacao,
                BloqueiaAvancoAtendimento: true,
                MensagemBloqueio: MensagemBloqueioAprovacaoPendente);
        }

        var ultimaDecisao = aprovacoesAtivas
            .OrderByDescending(x => x.DecididaEm ?? x.SolicitadaEm)
            .ThenByDescending(x => x.CriadoEm)
            .First();

        var bloqueiaPorReprovacao = ultimaDecisao.Status == StatusAprovacaoChamado.Reprovado;
        return new EstadoAprovacaoChamado(
            RequerAprovacao: true,
            AprovacaoPendente: false,
            StatusAprovacao: ultimaDecisao.Status,
            AprovacaoChamadoId: ultimaDecisao.Id,
            AprovacaoSolicitadaEm: ultimaDecisao.SolicitadaEm,
            AprovacaoDecididaEm: ultimaDecisao.DecididaEm,
            JustificativaAprovacao: ultimaDecisao.Status == StatusAprovacaoChamado.Aprovado ? ultimaDecisao.JustificativaDecisao : null,
            JustificativaReprovacao: ultimaDecisao.Status == StatusAprovacaoChamado.Reprovado ? ultimaDecisao.JustificativaDecisao : null,
            JustificativaDecisao: ultimaDecisao.JustificativaDecisao,
            MensagemOrientativa: ObterMensagemPortal(ultimaDecisao.Status),
            BloqueiaAvancoAtendimento: bloqueiaPorReprovacao,
            MensagemBloqueio: bloqueiaPorReprovacao ? MensagemBloqueioAprovacaoReprovada : null);
    }

    public static string ObterMensagemPortal(StatusAprovacaoChamado? statusAprovacao)
        => statusAprovacao switch
        {
            null => MensagemPortalSemAprovacao,
            StatusAprovacaoChamado.Pendente => MensagemPortalAguardandoAprovacao,
            StatusAprovacaoChamado.Aprovado => MensagemPortalAprovado,
            StatusAprovacaoChamado.Reprovado => MensagemPortalReprovado,
            StatusAprovacaoChamado.Cancelado => MensagemPortalAprovacaoCancelada,
            _ => MensagemPortalSemAprovacao
        };
}

internal sealed record EstadoAprovacaoChamado(
    bool RequerAprovacao,
    bool AprovacaoPendente,
    StatusAprovacaoChamado? StatusAprovacao,
    Guid? AprovacaoChamadoId,
    DateTime? AprovacaoSolicitadaEm,
    DateTime? AprovacaoDecididaEm,
    string? JustificativaAprovacao,
    string? JustificativaReprovacao,
    string? JustificativaDecisao,
    string MensagemOrientativa,
    bool BloqueiaAvancoAtendimento,
    string? MensagemBloqueio)
{
    public static EstadoAprovacaoChamado SemAprovacao()
        => new(
            RequerAprovacao: false,
            AprovacaoPendente: false,
            StatusAprovacao: null,
            AprovacaoChamadoId: null,
            AprovacaoSolicitadaEm: null,
            AprovacaoDecididaEm: null,
            JustificativaAprovacao: null,
            JustificativaReprovacao: null,
            JustificativaDecisao: null,
            MensagemOrientativa: AprovacaoChamadoHelper.MensagemPortalSemAprovacao,
            BloqueiaAvancoAtendimento: false,
            MensagemBloqueio: null);
}
