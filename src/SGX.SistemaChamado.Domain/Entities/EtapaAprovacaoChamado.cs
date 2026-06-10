using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class EtapaAprovacaoChamado : AuditableEntity
{
    private const int MaximoTitulo = 200;
    private const int MaximoDescricao = 4000;
    private const int MaximoRamo = 80;
    private const int MaximoMotivoCancelamento = 1000;
    private const int MaximoEscopoResumoSnapshot = 4000;
    private const int MaximoRegraNomeSnapshot = 180;
    private const int MaximoRegraCriterioSnapshot = 4000;
    private const int MaximoGrupoAprovadorSnapshot = 180;

    public Guid InstanciaAprovacaoChamadoId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public StatusEtapaAprovacaoChamado Status { get; private set; } = StatusEtapaAprovacaoChamado.Pendente;
    public TipoEtapaAprovacaoChamado TipoEtapa { get; private set; }
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; private set; } = TipoFluxoAprovacao.Simples;
    public int Ordem { get; private set; }
    public int Nivel { get; private set; }
    public string? Ramo { get; private set; }
    public bool Obrigatoria { get; private set; } = true;
    public bool CriticaParaConsolidacao { get; private set; }
    public bool PermiteReenvio { get; private set; }
    public bool PermiteFallback { get; private set; }
    public bool PermiteDelegacao { get; private set; }
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; private set; } = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido;
    public Guid? AprovadorEspecificoUsuarioId { get; private set; }
    public Guid? AprovadorPadraoUsuarioId { get; private set; }
    public Guid? AprovadorResolvidoUsuarioId { get; private set; }
    public string? GrupoAprovadorSnapshot { get; private set; }
    public int? QuorumMinimo { get; private set; }
    public int? QuantidadeAprovacoesNecessarias { get; private set; }
    public Guid SolicitanteId { get; private set; }
    public DateTime SolicitadaEm { get; private set; }
    public int? PrazoDecisaoHoras { get; private set; }
    public DateTime? DeveExpirarEm { get; private set; }
    public DateTime? ExpiradaEm { get; private set; }
    public DateTime? CanceladaEm { get; private set; }
    public Guid? CanceladaPorUsuarioId { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public DateTime? DecididaEm { get; private set; }
    public string? EscopoResumoSnapshot { get; private set; }
    public string? RegraNomeSnapshot { get; private set; }
    public int? RegraVersaoSnapshot { get; private set; }
    public string? RegraCriterioSnapshot { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public InstanciaAprovacaoChamado InstanciaAprovacaoChamado { get; private set; } = default!;
    public Usuario Solicitante { get; private set; } = default!;
    public Usuario? AprovadorEspecificoUsuario { get; private set; }
    public Usuario? AprovadorPadraoUsuario { get; private set; }
    public Usuario? AprovadorResolvidoUsuario { get; private set; }
    public Usuario? CanceladaPorUsuario { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }
    public ICollection<DecisaoAprovacaoChamado> Decisoes { get; private set; } = [];

    private EtapaAprovacaoChamado()
    {
    }

    public EtapaAprovacaoChamado(
        Guid instanciaAprovacaoChamadoId,
        Guid solicitanteId,
        TipoEtapaAprovacaoChamado tipoEtapa,
        TipoFluxoAprovacao tipoFluxoAprovacao,
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        int ordem,
        int nivel,
        Guid criadoPorUsuarioId,
        string criadoPor,
        string? titulo = null,
        string? descricao = null,
        string? ramo = null,
        bool obrigatoria = true,
        bool criticaParaConsolidacao = false,
        bool permiteReenvio = false,
        bool permiteFallback = false,
        bool permiteDelegacao = false,
        Guid? aprovadorEspecificoUsuarioId = null,
        Guid? aprovadorPadraoUsuarioId = null,
        Guid? aprovadorResolvidoUsuarioId = null,
        string? grupoAprovadorSnapshot = null,
        int? quorumMinimo = null,
        int? quantidadeAprovacoesNecessarias = null,
        int? prazoDecisaoHoras = null,
        DateTime? deveExpirarEm = null,
        string? escopoResumoSnapshot = null,
        string? regraNomeSnapshot = null,
        int? regraVersaoSnapshot = null,
        string? regraCriterioSnapshot = null,
        StatusEtapaAprovacaoChamado statusInicial = StatusEtapaAprovacaoChamado.Pendente)
    {
        if (instanciaAprovacaoChamadoId == Guid.Empty)
        {
            throw new ArgumentException("A instancia de aprovacao da etapa e obrigatoria.", nameof(instanciaAprovacaoChamadoId));
        }

        if (solicitanteId == Guid.Empty)
        {
            throw new ArgumentException("O solicitante da etapa de aprovacao e obrigatorio.", nameof(solicitanteId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da etapa de aprovacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        InstanciaAprovacaoChamadoId = instanciaAprovacaoChamadoId;
        SolicitanteId = solicitanteId;
        SolicitadaEm = DateTime.UtcNow;

        DefinirTitulo(titulo, tipoEtapa, nivel, ordem);
        DefinirDescricao(descricao);
        DefinirEstrutura(tipoEtapa, tipoFluxoAprovacao, ordem, nivel, ramo, obrigatoria, criticaParaConsolidacao);
        DefinirComportamentos(permiteReenvio, permiteFallback, permiteDelegacao);
        DefinirResolucaoAprovador(
            tipoResolucaoAprovador,
            aprovadorEspecificoUsuarioId,
            aprovadorPadraoUsuarioId,
            aprovadorResolvidoUsuarioId,
            grupoAprovadorSnapshot,
            quorumMinimo,
            quantidadeAprovacoesNecessarias);
        DefinirPrazo(prazoDecisaoHoras, deveExpirarEm);
        DefinirSnapshot(escopoResumoSnapshot, regraNomeSnapshot, regraVersaoSnapshot, regraCriterioSnapshot);
        DefinirStatusInicial(statusInicial);

        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void RegistrarDecisaoResumo(
        StatusEtapaAprovacaoChamado statusFinal,
        Guid? aprovadorResolvidoUsuarioId,
        Guid atualizadoPorUsuarioId,
        string atualizadoPor)
    {
        if (statusFinal != StatusEtapaAprovacaoChamado.Aprovada && statusFinal != StatusEtapaAprovacaoChamado.Reprovada)
        {
            throw new InvalidOperationException("A decisao resumida da etapa deve ser aprovada ou reprovada.");
        }

        if (Status != StatusEtapaAprovacaoChamado.Pendente
            && Status != StatusEtapaAprovacaoChamado.AguardandoEtapaAnterior
            && Status != StatusEtapaAprovacaoChamado.EmReavaliacao)
        {
            throw new InvalidOperationException("Somente etapas pendentes, aguardando etapa anterior ou em reavaliacao podem receber decisao resumida.");
        }

        if (aprovadorResolvidoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador resolvido informado para a etapa e invalido.", nameof(aprovadorResolvidoUsuarioId));
        }

        Status = statusFinal;
        AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId;
        DecididaEm = DateTime.UtcNow;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarAguardandoEtapaAnterior(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusEtapaAprovacaoChamado.AguardandoEtapaAnterior;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarEmReavaliacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusEtapaAprovacaoChamado.EmReavaliacao;
        DecididaEm = null;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarCancelada(Guid canceladaPorUsuarioId, Guid atualizadoPorUsuarioId, string atualizadoPor, string? motivoCancelamento = null)
    {
        if (canceladaPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de cancelamento da etapa e obrigatorio.", nameof(canceladaPorUsuarioId));
        }

        Status = StatusEtapaAprovacaoChamado.Cancelada;
        CanceladaEm = DateTime.UtcNow;
        CanceladaPorUsuarioId = canceladaPorUsuarioId;
        MotivoCancelamento = NormalizarTexto(motivoCancelamento, MaximoMotivoCancelamento, nameof(motivoCancelamento));
        DecididaEm = CanceladaEm;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarExpirada(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusEtapaAprovacaoChamado.Expirada;
        ExpiradaEm = DateTime.UtcNow;
        DecididaEm = ExpiradaEm;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarSubstituida(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusEtapaAprovacaoChamado.Substituida;
        DecididaEm = DateTime.UtcNow;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarIgnorada(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusEtapaAprovacaoChamado.Ignorada;
        DecididaEm = DateTime.UtcNow;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    private void DefinirTitulo(string? titulo, TipoEtapaAprovacaoChamado tipoEtapa, int nivel, int ordem)
    {
        Titulo = string.IsNullOrWhiteSpace(titulo)
            ? $"Etapa {tipoEtapa} - nivel {nivel} ordem {ordem}"
            : NormalizarTextoObrigatorio(
                titulo,
                MaximoTitulo,
                "O titulo da etapa de aprovacao e obrigatorio.",
                nameof(titulo));
    }

    private void DefinirDescricao(string? descricao)
        => Descricao = NormalizarTexto(descricao, MaximoDescricao, nameof(descricao));

    private void DefinirEstrutura(
        TipoEtapaAprovacaoChamado tipoEtapa,
        TipoFluxoAprovacao tipoFluxoAprovacao,
        int ordem,
        int nivel,
        string? ramo,
        bool obrigatoria,
        bool criticaParaConsolidacao)
    {
        if (!Enum.IsDefined(tipoEtapa))
        {
            throw new ArgumentException("O tipo da etapa de aprovacao informado e invalido.", nameof(tipoEtapa));
        }

        if (!Enum.IsDefined(tipoFluxoAprovacao))
        {
            throw new ArgumentException("O tipo de fluxo da etapa informado e invalido.", nameof(tipoFluxoAprovacao));
        }

        if (ordem < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem da etapa de aprovacao nao pode ser negativa.");
        }

        if (nivel <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivel), "O nivel da etapa de aprovacao deve ser maior que zero.");
        }

        var ramoNormalizado = NormalizarTexto(ramo, MaximoRamo, nameof(ramo));
        if (tipoFluxoAprovacao == TipoFluxoAprovacao.Paralela && string.IsNullOrWhiteSpace(ramoNormalizado))
        {
            throw new InvalidOperationException("Etapas paralelas devem informar um ramo identificavel.");
        }

        if (criticaParaConsolidacao && !obrigatoria)
        {
            throw new InvalidOperationException("Etapas criticas para consolidacao devem ser obrigatorias.");
        }

        TipoEtapa = tipoEtapa;
        TipoFluxoAprovacao = tipoFluxoAprovacao;
        Ordem = ordem;
        Nivel = nivel;
        Ramo = ramoNormalizado;
        Obrigatoria = obrigatoria;
        CriticaParaConsolidacao = criticaParaConsolidacao;
    }

    private void DefinirComportamentos(bool permiteReenvio, bool permiteFallback, bool permiteDelegacao)
    {
        PermiteReenvio = permiteReenvio;
        PermiteFallback = permiteFallback;
        PermiteDelegacao = permiteDelegacao;
    }

    private void DefinirResolucaoAprovador(
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        Guid? aprovadorEspecificoUsuarioId,
        Guid? aprovadorPadraoUsuarioId,
        Guid? aprovadorResolvidoUsuarioId,
        string? grupoAprovadorSnapshot,
        int? quorumMinimo,
        int? quantidadeAprovacoesNecessarias)
    {
        if (!Enum.IsDefined(tipoResolucaoAprovador))
        {
            throw new ArgumentException("O tipo de resolucao de aprovador da etapa e invalido.", nameof(tipoResolucaoAprovador));
        }

        if (aprovadorEspecificoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador especifico da etapa e invalido.", nameof(aprovadorEspecificoUsuarioId));
        }

        if (aprovadorPadraoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador padrao da etapa e invalido.", nameof(aprovadorPadraoUsuarioId));
        }

        if (aprovadorResolvidoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador resolvido da etapa e invalido.", nameof(aprovadorResolvidoUsuarioId));
        }

        if (quorumMinimo.HasValue && quorumMinimo.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quorumMinimo), "O quorum minimo da etapa deve ser maior que zero.");
        }

        if (quantidadeAprovacoesNecessarias.HasValue && quantidadeAprovacoesNecessarias.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantidadeAprovacoesNecessarias), "A quantidade de aprovacoes necessarias da etapa deve ser maior que zero.");
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico && !aprovadorEspecificoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Etapas com aprovador especifico devem informar o usuario aprovador.");
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao && !aprovadorPadraoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Etapas com aprovador padrao devem informar o usuario padrao.");
        }

        var grupoNormalizado = NormalizarTexto(grupoAprovadorSnapshot, MaximoGrupoAprovadorSnapshot, nameof(grupoAprovadorSnapshot));
        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo && string.IsNullOrWhiteSpace(grupoNormalizado))
        {
            throw new InvalidOperationException("Etapas com resolucao futura por grupo devem informar ao menos um snapshot descritivo do grupo.");
        }

        TipoResolucaoAprovador = tipoResolucaoAprovador;
        AprovadorEspecificoUsuarioId = aprovadorEspecificoUsuarioId;
        AprovadorPadraoUsuarioId = aprovadorPadraoUsuarioId;
        AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId;
        GrupoAprovadorSnapshot = grupoNormalizado;
        QuorumMinimo = quorumMinimo;
        QuantidadeAprovacoesNecessarias = quantidadeAprovacoesNecessarias;
    }

    private void DefinirPrazo(int? prazoDecisaoHoras, DateTime? deveExpirarEm)
    {
        if (prazoDecisaoHoras.HasValue && prazoDecisaoHoras.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoDecisaoHoras), "O prazo de decisao da etapa deve ser maior que zero.");
        }

        if (deveExpirarEm.HasValue && deveExpirarEm.Value < SolicitadaEm)
        {
            throw new InvalidOperationException("A expiracao planejada da etapa nao pode ser anterior a solicitacao.");
        }

        PrazoDecisaoHoras = prazoDecisaoHoras;
        DeveExpirarEm = deveExpirarEm;
    }

    private void DefinirSnapshot(
        string? escopoResumoSnapshot,
        string? regraNomeSnapshot,
        int? regraVersaoSnapshot,
        string? regraCriterioSnapshot)
    {
        EscopoResumoSnapshot = NormalizarTexto(escopoResumoSnapshot, MaximoEscopoResumoSnapshot, nameof(escopoResumoSnapshot));
        RegraNomeSnapshot = NormalizarTexto(regraNomeSnapshot, MaximoRegraNomeSnapshot, nameof(regraNomeSnapshot));
        RegraCriterioSnapshot = NormalizarTexto(regraCriterioSnapshot, MaximoRegraCriterioSnapshot, nameof(regraCriterioSnapshot));

        if (regraVersaoSnapshot.HasValue && regraVersaoSnapshot.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regraVersaoSnapshot), "A versao snapshot da regra da etapa deve ser maior que zero.");
        }

        RegraVersaoSnapshot = regraVersaoSnapshot;
    }

    private void DefinirStatusInicial(StatusEtapaAprovacaoChamado statusInicial)
    {
        if (!Enum.IsDefined(statusInicial))
        {
            throw new ArgumentException("O status inicial da etapa informado e invalido.", nameof(statusInicial));
        }

        Status = statusInicial;
    }

    private void AtualizarAuditoriaUsuario(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da etapa de aprovacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }

    private static string NormalizarTextoObrigatorio(string? valor, int tamanhoMaximo, string mensagemObrigatorio, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException(mensagemObrigatorio, paramName);
        }

        var textoNormalizado = valor.Trim();
        if (textoNormalizado.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {tamanhoMaximo} caracteres.", paramName);
        }

        return textoNormalizado;
    }

    private static string? NormalizarTexto(string? valor, int tamanhoMaximo, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var textoNormalizado = valor.Trim();
        if (textoNormalizado.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {tamanhoMaximo} caracteres.", paramName);
        }

        return textoNormalizado;
    }
}
