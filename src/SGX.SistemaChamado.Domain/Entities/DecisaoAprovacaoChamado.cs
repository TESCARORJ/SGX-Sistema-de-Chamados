using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class DecisaoAprovacaoChamado : AuditableEntity
{
    private const int MaximoPapelDecisor = 120;
    private const int MaximoAutoridadeDecisor = 180;
    private const int MaximoGrupoAprovadorSnapshot = 180;
    private const int MaximoJustificativa = 4000;
    private const int MaximoObservacao = 2000;
    private const int MaximoEscopoDecididoSnapshot = 4000;
    private const int MaximoRegraNomeSnapshot = 180;
    private const int MaximoRegraCriterioSnapshot = 4000;
    private const int MaximoRamoEtapaSnapshot = 80;

    public Guid InstanciaAprovacaoChamadoId { get; private set; }
    public Guid? EtapaAprovacaoChamadoId { get; private set; }
    public TipoDecisaoAprovacaoChamado TipoDecisao { get; private set; }
    public ResultadoDecisaoAprovacaoChamado Resultado { get; private set; }
    public DateTime DataDecisao { get; private set; }
    public Guid? DecisorUsuarioId { get; private set; }
    public string? PapelDecisorSnapshot { get; private set; }
    public string? AutoridadeDecisorSnapshot { get; private set; }
    public bool DecisorEhAprovadorEspecifico { get; private set; }
    public bool DecisorEhAprovadorPadrao { get; private set; }
    public bool DecisorEhMembroGrupo { get; private set; }
    public bool DecisorPorDelegacao { get; private set; }
    public string? GrupoAprovadorSnapshot { get; private set; }
    public int? QuorumEsperado { get; private set; }
    public int? QuorumAtingido { get; private set; }
    public string? Justificativa { get; private set; }
    public string? Observacao { get; private set; }
    public string? EscopoDecididoSnapshot { get; private set; }
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; private set; }
    public bool DecisaoParcial { get; private set; }
    public bool DecisaoFinal { get; private set; }
    public bool LiberaAvanco { get; private set; }
    public bool MantemBloqueio { get; private set; }
    public bool ExigeReavaliacao { get; private set; }
    public bool PermiteNovaSolicitacao { get; private set; }
    public bool CancelaFluxo { get; private set; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaAnterior { get; private set; }
    public StatusInstanciaAprovacaoChamado StatusInstanciaNovo { get; private set; }
    public StatusEtapaAprovacaoChamado? StatusEtapaAnterior { get; private set; }
    public StatusEtapaAprovacaoChamado? StatusEtapaNovo { get; private set; }
    public Guid? StatusChamadoAnteriorId { get; private set; }
    public Guid? StatusChamadoNovoId { get; private set; }
    public int? NivelEtapaSnapshot { get; private set; }
    public int? OrdemEtapaSnapshot { get; private set; }
    public string? RamoEtapaSnapshot { get; private set; }
    public string? RegraNomeSnapshot { get; private set; }
    public int? RegraVersaoSnapshot { get; private set; }
    public string? RegraCriterioSnapshot { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public InstanciaAprovacaoChamado InstanciaAprovacaoChamado { get; private set; } = default!;
    public EtapaAprovacaoChamado? EtapaAprovacaoChamado { get; private set; }
    public Usuario? DecisorUsuario { get; private set; }
    public StatusChamado? StatusChamadoAnterior { get; private set; }
    public StatusChamado? StatusChamadoNovo { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }

    private DecisaoAprovacaoChamado()
    {
    }

    public DecisaoAprovacaoChamado(
        Guid instanciaAprovacaoChamadoId,
        TipoDecisaoAprovacaoChamado tipoDecisao,
        ResultadoDecisaoAprovacaoChamado resultado,
        EfeitoOperacionalRegraAprovacao efeitoOperacional,
        StatusInstanciaAprovacaoChamado statusInstanciaAnterior,
        StatusInstanciaAprovacaoChamado statusInstanciaNovo,
        Guid criadoPorUsuarioId,
        string criadoPor,
        Guid? etapaAprovacaoChamadoId = null,
        Guid? decisorUsuarioId = null,
        string? papelDecisorSnapshot = null,
        string? autoridadeDecisorSnapshot = null,
        bool decisorEhAprovadorEspecifico = false,
        bool decisorEhAprovadorPadrao = false,
        bool decisorEhMembroGrupo = false,
        bool decisorPorDelegacao = false,
        string? grupoAprovadorSnapshot = null,
        int? quorumEsperado = null,
        int? quorumAtingido = null,
        string? justificativa = null,
        string? observacao = null,
        string? escopoDecididoSnapshot = null,
        bool decisaoParcial = false,
        bool decisaoFinal = false,
        bool liberaAvanco = false,
        bool mantemBloqueio = false,
        bool exigeReavaliacao = false,
        bool permiteNovaSolicitacao = false,
        bool cancelaFluxo = false,
        StatusEtapaAprovacaoChamado? statusEtapaAnterior = null,
        StatusEtapaAprovacaoChamado? statusEtapaNovo = null,
        Guid? statusChamadoAnteriorId = null,
        Guid? statusChamadoNovoId = null,
        int? nivelEtapaSnapshot = null,
        int? ordemEtapaSnapshot = null,
        string? ramoEtapaSnapshot = null,
        string? regraNomeSnapshot = null,
        int? regraVersaoSnapshot = null,
        string? regraCriterioSnapshot = null,
        DateTime? dataDecisao = null)
    {
        if (instanciaAprovacaoChamadoId == Guid.Empty)
        {
            throw new ArgumentException("A instancia da decisao de aprovacao e obrigatoria.", nameof(instanciaAprovacaoChamadoId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da decisao de aprovacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        if (etapaAprovacaoChamadoId == Guid.Empty)
        {
            throw new ArgumentException("A etapa da decisao de aprovacao informada e invalida.", nameof(etapaAprovacaoChamadoId));
        }

        if (decisorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O decisor informado para a decisao de aprovacao e invalido.", nameof(decisorUsuarioId));
        }

        if (statusChamadoAnteriorId == Guid.Empty)
        {
            throw new ArgumentException("O status anterior do chamado informado para a decisao e invalido.", nameof(statusChamadoAnteriorId));
        }

        if (statusChamadoNovoId == Guid.Empty)
        {
            throw new ArgumentException("O status novo do chamado informado para a decisao e invalido.", nameof(statusChamadoNovoId));
        }

        InstanciaAprovacaoChamadoId = instanciaAprovacaoChamadoId;
        EtapaAprovacaoChamadoId = etapaAprovacaoChamadoId;

        DefinirTipoResultadoEfeito(tipoDecisao, resultado, efeitoOperacional);
        DefinirDataDecisao(dataDecisao);
        DefinirDecisor(decisorUsuarioId, papelDecisorSnapshot, autoridadeDecisorSnapshot);
        DefinirMarcadoresDecisor(decisorEhAprovadorEspecifico, decisorEhAprovadorPadrao, decisorEhMembroGrupo, decisorPorDelegacao);
        DefinirContextoGrupoEQuorum(grupoAprovadorSnapshot, quorumEsperado, quorumAtingido);
        DefinirNarrativa(justificativa, observacao, escopoDecididoSnapshot);
        DefinirConsequencias(decisaoParcial, decisaoFinal, liberaAvanco, mantemBloqueio, exigeReavaliacao, permiteNovaSolicitacao, cancelaFluxo);
        DefinirStatus(statusInstanciaAnterior, statusInstanciaNovo, statusEtapaAnterior, statusEtapaNovo);
        DefinirStatusChamado(statusChamadoAnteriorId, statusChamadoNovoId);
        DefinirSnapshots(regraNomeSnapshot, regraVersaoSnapshot, regraCriterioSnapshot, nivelEtapaSnapshot, ordemEtapaSnapshot, ramoEtapaSnapshot);

        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void RegistrarAtualizacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da decisao de aprovacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }

    private void DefinirTipoResultadoEfeito(
        TipoDecisaoAprovacaoChamado tipoDecisao,
        ResultadoDecisaoAprovacaoChamado resultado,
        EfeitoOperacionalRegraAprovacao efeitoOperacional)
    {
        if (!Enum.IsDefined(tipoDecisao))
        {
            throw new ArgumentException("O tipo da decisao de aprovacao informado e invalido.", nameof(tipoDecisao));
        }

        if (!Enum.IsDefined(resultado))
        {
            throw new ArgumentException("O resultado da decisao de aprovacao informado e invalido.", nameof(resultado));
        }

        if (!Enum.IsDefined(efeitoOperacional))
        {
            throw new ArgumentException("O efeito operacional da decisao de aprovacao informado e invalido.", nameof(efeitoOperacional));
        }

        TipoDecisao = tipoDecisao;
        Resultado = resultado;
        EfeitoOperacional = efeitoOperacional;
    }

    private void DefinirDataDecisao(DateTime? dataDecisao)
    {
        DataDecisao = dataDecisao ?? DateTime.UtcNow;
    }

    private void DefinirDecisor(Guid? decisorUsuarioId, string? papelDecisorSnapshot, string? autoridadeDecisorSnapshot)
    {
        DecisorUsuarioId = decisorUsuarioId;
        PapelDecisorSnapshot = NormalizarTexto(papelDecisorSnapshot, MaximoPapelDecisor, nameof(papelDecisorSnapshot));
        AutoridadeDecisorSnapshot = NormalizarTexto(autoridadeDecisorSnapshot, MaximoAutoridadeDecisor, nameof(autoridadeDecisorSnapshot));
    }

    private void DefinirMarcadoresDecisor(
        bool decisorEhAprovadorEspecifico,
        bool decisorEhAprovadorPadrao,
        bool decisorEhMembroGrupo,
        bool decisorPorDelegacao)
    {
        DecisorEhAprovadorEspecifico = decisorEhAprovadorEspecifico;
        DecisorEhAprovadorPadrao = decisorEhAprovadorPadrao;
        DecisorEhMembroGrupo = decisorEhMembroGrupo;
        DecisorPorDelegacao = decisorPorDelegacao;
    }

    private void DefinirContextoGrupoEQuorum(string? grupoAprovadorSnapshot, int? quorumEsperado, int? quorumAtingido)
    {
        if (quorumEsperado.HasValue && quorumEsperado.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quorumEsperado), "O quorum esperado da decisao deve ser maior que zero.");
        }

        if (quorumAtingido.HasValue && quorumAtingido.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quorumAtingido), "O quorum atingido da decisao deve ser maior que zero.");
        }

        if (quorumAtingido.HasValue && !quorumEsperado.HasValue)
        {
            throw new InvalidOperationException("A decisao nao pode registrar quorum atingido sem quorum esperado.");
        }

        GrupoAprovadorSnapshot = NormalizarTexto(grupoAprovadorSnapshot, MaximoGrupoAprovadorSnapshot, nameof(grupoAprovadorSnapshot));
        QuorumEsperado = quorumEsperado;
        QuorumAtingido = quorumAtingido;
    }

    private void DefinirNarrativa(string? justificativa, string? observacao, string? escopoDecididoSnapshot)
    {
        Justificativa = NormalizarTexto(justificativa, MaximoJustificativa, nameof(justificativa));
        Observacao = NormalizarTexto(observacao, MaximoObservacao, nameof(observacao));
        EscopoDecididoSnapshot = NormalizarTexto(escopoDecididoSnapshot, MaximoEscopoDecididoSnapshot, nameof(escopoDecididoSnapshot));
    }

    private void DefinirConsequencias(
        bool decisaoParcial,
        bool decisaoFinal,
        bool liberaAvanco,
        bool mantemBloqueio,
        bool exigeReavaliacao,
        bool permiteNovaSolicitacao,
        bool cancelaFluxo)
    {
        if (liberaAvanco && mantemBloqueio)
        {
            throw new InvalidOperationException("A decisao nao pode liberar avanço e manter bloqueio ao mesmo tempo.");
        }

        DecisaoParcial = decisaoParcial;
        DecisaoFinal = decisaoFinal;
        LiberaAvanco = liberaAvanco;
        MantemBloqueio = mantemBloqueio;
        ExigeReavaliacao = exigeReavaliacao;
        PermiteNovaSolicitacao = permiteNovaSolicitacao;
        CancelaFluxo = cancelaFluxo;
    }

    private void DefinirStatus(
        StatusInstanciaAprovacaoChamado statusInstanciaAnterior,
        StatusInstanciaAprovacaoChamado statusInstanciaNovo,
        StatusEtapaAprovacaoChamado? statusEtapaAnterior,
        StatusEtapaAprovacaoChamado? statusEtapaNovo)
    {
        if (!Enum.IsDefined(statusInstanciaAnterior))
        {
            throw new ArgumentException("O status anterior da instancia informado para a decisao e invalido.", nameof(statusInstanciaAnterior));
        }

        if (!Enum.IsDefined(statusInstanciaNovo))
        {
            throw new ArgumentException("O status novo da instancia informado para a decisao e invalido.", nameof(statusInstanciaNovo));
        }

        if (EtapaAprovacaoChamadoId.HasValue)
        {
            if (!statusEtapaAnterior.HasValue || !statusEtapaNovo.HasValue)
            {
                throw new InvalidOperationException("Decisoes vinculadas a etapa devem informar status anterior e novo da etapa.");
            }

            if (!Enum.IsDefined(statusEtapaAnterior.Value) || !Enum.IsDefined(statusEtapaNovo.Value))
            {
                throw new ArgumentException("O status da etapa informado para a decisao e invalido.");
            }
        }
        else if (statusEtapaAnterior.HasValue || statusEtapaNovo.HasValue)
        {
            throw new InvalidOperationException("Decisoes sem etapa nao podem informar status de etapa.");
        }

        StatusInstanciaAnterior = statusInstanciaAnterior;
        StatusInstanciaNovo = statusInstanciaNovo;
        StatusEtapaAnterior = statusEtapaAnterior;
        StatusEtapaNovo = statusEtapaNovo;
    }

    private void DefinirStatusChamado(Guid? statusChamadoAnteriorId, Guid? statusChamadoNovoId)
    {
        StatusChamadoAnteriorId = statusChamadoAnteriorId;
        StatusChamadoNovoId = statusChamadoNovoId;
    }

    private void DefinirSnapshots(
        string? regraNomeSnapshot,
        int? regraVersaoSnapshot,
        string? regraCriterioSnapshot,
        int? nivelEtapaSnapshot,
        int? ordemEtapaSnapshot,
        string? ramoEtapaSnapshot)
    {
        if (regraVersaoSnapshot.HasValue && regraVersaoSnapshot.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regraVersaoSnapshot), "A versao snapshot da regra da decisao deve ser maior que zero.");
        }

        if (nivelEtapaSnapshot.HasValue && nivelEtapaSnapshot.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivelEtapaSnapshot), "O nivel da etapa em snapshot deve ser maior que zero.");
        }

        if (ordemEtapaSnapshot.HasValue && ordemEtapaSnapshot.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordemEtapaSnapshot), "A ordem da etapa em snapshot nao pode ser negativa.");
        }

        if (!EtapaAprovacaoChamadoId.HasValue && (nivelEtapaSnapshot.HasValue || ordemEtapaSnapshot.HasValue || !string.IsNullOrWhiteSpace(ramoEtapaSnapshot)))
        {
            throw new InvalidOperationException("Decisoes sem etapa nao podem registrar snapshot estrutural de etapa.");
        }

        RegraNomeSnapshot = NormalizarTexto(regraNomeSnapshot, MaximoRegraNomeSnapshot, nameof(regraNomeSnapshot));
        RegraVersaoSnapshot = regraVersaoSnapshot;
        RegraCriterioSnapshot = NormalizarTexto(regraCriterioSnapshot, MaximoRegraCriterioSnapshot, nameof(regraCriterioSnapshot));
        NivelEtapaSnapshot = nivelEtapaSnapshot;
        OrdemEtapaSnapshot = ordemEtapaSnapshot;
        RamoEtapaSnapshot = NormalizarTexto(ramoEtapaSnapshot, MaximoRamoEtapaSnapshot, nameof(ramoEtapaSnapshot));
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
