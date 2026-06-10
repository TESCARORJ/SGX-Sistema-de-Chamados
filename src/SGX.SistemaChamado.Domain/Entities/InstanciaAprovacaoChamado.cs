using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class InstanciaAprovacaoChamado : AuditableEntity
{
    private const int MaximoTitulo = 200;
    private const int MaximoDescricao = 4000;
    private const int MaximoMotivoCancelamento = 1000;
    private const int MaximoRegraNomeSnapshot = 180;
    private const int MaximoRegraCriterioSnapshot = 4000;

    public Guid ChamadoId { get; private set; }
    public Guid? ConfiguracaoRegraAprovacaoId { get; private set; }
    public Guid? AprovacaoChamadoLegadaId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public StatusInstanciaAprovacaoChamado Status { get; private set; } = StatusInstanciaAprovacaoChamado.Pendente;
    public OrigemInstanciaAprovacaoChamado Origem { get; private set; }
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; private set; } = TipoFluxoAprovacao.Simples;
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; private set; }
    public EscopoRegraAprovacao EscopoRegra { get; private set; }
    public TipoRegraAprovacao TipoRegra { get; private set; }
    public NaturezaChamadoEnum? NaturezaChamado { get; private set; }
    public Guid? TipoSolicitacaoId { get; private set; }
    public Guid? CatalogoServicoId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public ImpactoChamadoEnum? ImpactoAvaliado { get; private set; }
    public UrgenciaChamadoEnum? UrgenciaAvaliada { get; private set; }
    public PrioridadeChamadoEnum? PrioridadeAvaliada { get; private set; }
    public decimal? CustoAvaliado { get; private set; }
    public int? NivelRiscoAvaliado { get; private set; }
    public bool ExigeAprovacao { get; private set; }
    public bool Bloqueante { get; private set; }
    public bool PermiteReenvio { get; private set; }
    public bool PermiteFallback { get; private set; }
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; private set; } = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido;
    public Guid? AprovadorEspecificoUsuarioId { get; private set; }
    public Guid? AprovadorPadraoUsuarioId { get; private set; }
    public Guid? AprovadorResolvidoUsuarioId { get; private set; }
    public Guid SolicitanteId { get; private set; }
    public DateTime SolicitadaEm { get; private set; }
    public int? PrazoDecisaoHoras { get; private set; }
    public DateTime? DeveExpirarEm { get; private set; }
    public DateTime? ExpiradaEm { get; private set; }
    public DateTime? CanceladaEm { get; private set; }
    public Guid? CanceladaPorUsuarioId { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public DateTime? DecididaEm { get; private set; }
    public string? RegraNomeSnapshot { get; private set; }
    public int? RegraVersaoSnapshot { get; private set; }
    public string? RegraCriterioSnapshot { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public ConfiguracaoRegraAprovacao? ConfiguracaoRegraAprovacao { get; private set; }
    public AprovacaoChamado? AprovacaoChamadoLegada { get; private set; }
    public TipoSolicitacao? TipoSolicitacao { get; private set; }
    public CatalogoServico? CatalogoServico { get; private set; }
    public CategoriaChamado? Categoria { get; private set; }
    public SubcategoriaChamado? Subcategoria { get; private set; }
    public Usuario Solicitante { get; private set; } = default!;
    public Usuario? AprovadorEspecificoUsuario { get; private set; }
    public Usuario? AprovadorPadraoUsuario { get; private set; }
    public Usuario? AprovadorResolvidoUsuario { get; private set; }
    public Usuario? CanceladaPorUsuario { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }
    public ICollection<EtapaAprovacaoChamado> Etapas { get; private set; } = [];
    public ICollection<DecisaoAprovacaoChamado> Decisoes { get; private set; } = [];

    private InstanciaAprovacaoChamado()
    {
    }

    public InstanciaAprovacaoChamado(
        Guid chamadoId,
        Guid solicitanteId,
        OrigemInstanciaAprovacaoChamado origem,
        TipoFluxoAprovacao tipoFluxoAprovacao,
        EfeitoOperacionalRegraAprovacao efeitoOperacional,
        EscopoRegraAprovacao escopoRegra,
        TipoRegraAprovacao tipoRegra,
        bool exigeAprovacao,
        bool bloqueante,
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        Guid criadoPorUsuarioId,
        string criadoPor,
        Guid? configuracaoRegraAprovacaoId = null,
        Guid? aprovacaoChamadoLegadaId = null,
        string? titulo = null,
        string? descricao = null,
        NaturezaChamadoEnum? naturezaChamado = null,
        Guid? tipoSolicitacaoId = null,
        Guid? catalogoServicoId = null,
        Guid? categoriaId = null,
        Guid? subcategoriaId = null,
        ImpactoChamadoEnum? impactoAvaliado = null,
        UrgenciaChamadoEnum? urgenciaAvaliada = null,
        PrioridadeChamadoEnum? prioridadeAvaliada = null,
        decimal? custoAvaliado = null,
        int? nivelRiscoAvaliado = null,
        bool permiteReenvio = false,
        bool permiteFallback = false,
        Guid? aprovadorEspecificoUsuarioId = null,
        Guid? aprovadorPadraoUsuarioId = null,
        Guid? aprovadorResolvidoUsuarioId = null,
        int? prazoDecisaoHoras = null,
        DateTime? deveExpirarEm = null,
        string? regraNomeSnapshot = null,
        int? regraVersaoSnapshot = null,
        string? regraCriterioSnapshot = null)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado da instancia de aprovacao e obrigatorio.", nameof(chamadoId));
        }

        if (solicitanteId == Guid.Empty)
        {
            throw new ArgumentException("O solicitante da instancia de aprovacao e obrigatorio.", nameof(solicitanteId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da instancia de aprovacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        ChamadoId = chamadoId;
        SolicitanteId = solicitanteId;
        SolicitadaEm = DateTime.UtcNow;

        DefinirRelacionamentosOpcionais(configuracaoRegraAprovacaoId, aprovacaoChamadoLegadaId);
        DefinirTitulo(titulo, origem);
        DefinirDescricao(descricao);
        DefinirOrigem(origem);
        DefinirFluxoEComportamento(tipoFluxoAprovacao, efeitoOperacional, escopoRegra, tipoRegra, exigeAprovacao, bloqueante, permiteReenvio, permiteFallback);
        DefinirEscopoAvaliado(
            naturezaChamado,
            tipoSolicitacaoId,
            catalogoServicoId,
            categoriaId,
            subcategoriaId,
            impactoAvaliado,
            urgenciaAvaliada,
            prioridadeAvaliada,
            custoAvaliado,
            nivelRiscoAvaliado);
        DefinirResolucaoAprovador(
            tipoResolucaoAprovador,
            aprovadorEspecificoUsuarioId,
            aprovadorPadraoUsuarioId,
            aprovadorResolvidoUsuarioId);
        DefinirPrazo(prazoDecisaoHoras, deveExpirarEm);
        DefinirSnapshotsRegra(regraNomeSnapshot, regraVersaoSnapshot, regraCriterioSnapshot);

        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void RegistrarDecisaoResumo(
        StatusInstanciaAprovacaoChamado statusFinal,
        Guid? aprovadorResolvidoUsuarioId,
        Guid atualizadoPorUsuarioId,
        string atualizadoPor)
    {
        if (statusFinal != StatusInstanciaAprovacaoChamado.Aprovada && statusFinal != StatusInstanciaAprovacaoChamado.Reprovada)
        {
            throw new InvalidOperationException("A decisao resumida da instancia deve ser aprovada ou reprovada.");
        }

        if (Status != StatusInstanciaAprovacaoChamado.Pendente && Status != StatusInstanciaAprovacaoChamado.EmReavaliacao)
        {
            throw new InvalidOperationException("Somente instancias pendentes ou em reavaliacao podem receber decisao resumida.");
        }

        if (aprovadorResolvidoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador resolvido informado para a instancia e invalido.", nameof(aprovadorResolvidoUsuarioId));
        }

        Status = statusFinal;
        AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId;
        DecididaEm = DateTime.UtcNow;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarCancelada(Guid canceladaPorUsuarioId, Guid atualizadoPorUsuarioId, string atualizadoPor, string? motivoCancelamento = null)
    {
        if (Status != StatusInstanciaAprovacaoChamado.Pendente && Status != StatusInstanciaAprovacaoChamado.EmReavaliacao)
        {
            throw new InvalidOperationException("Somente instancias pendentes ou em reavaliacao podem ser canceladas.");
        }

        if (canceladaPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de cancelamento da instancia e obrigatorio.", nameof(canceladaPorUsuarioId));
        }

        Status = StatusInstanciaAprovacaoChamado.Cancelada;
        CanceladaEm = DateTime.UtcNow;
        CanceladaPorUsuarioId = canceladaPorUsuarioId;
        MotivoCancelamento = NormalizarTexto(motivoCancelamento, MaximoMotivoCancelamento, nameof(motivoCancelamento));
        DecididaEm = CanceladaEm;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarExpirada(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (Status != StatusInstanciaAprovacaoChamado.Pendente && Status != StatusInstanciaAprovacaoChamado.EmReavaliacao)
        {
            throw new InvalidOperationException("Somente instancias pendentes ou em reavaliacao podem expirar.");
        }

        Status = StatusInstanciaAprovacaoChamado.Expirada;
        ExpiradaEm = DateTime.UtcNow;
        DecididaEm = ExpiradaEm;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarEmReavaliacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (Status != StatusInstanciaAprovacaoChamado.Pendente && Status != StatusInstanciaAprovacaoChamado.Aprovada && Status != StatusInstanciaAprovacaoChamado.Reprovada)
        {
            throw new InvalidOperationException("Somente instancias ativas podem entrar em reavaliacao.");
        }

        Status = StatusInstanciaAprovacaoChamado.EmReavaliacao;
        DecididaEm = null;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void MarcarSubstituida(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (Status == StatusInstanciaAprovacaoChamado.Cancelada || Status == StatusInstanciaAprovacaoChamado.Expirada || Status == StatusInstanciaAprovacaoChamado.Substituida)
        {
            throw new InvalidOperationException("A instancia informada nao pode ser substituida no estado atual.");
        }

        Status = StatusInstanciaAprovacaoChamado.Substituida;
        DecididaEm = DateTime.UtcNow;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    private void DefinirRelacionamentosOpcionais(Guid? configuracaoRegraAprovacaoId, Guid? aprovacaoChamadoLegadaId)
    {
        if (configuracaoRegraAprovacaoId == Guid.Empty)
        {
            throw new ArgumentException("A configuracao da regra informada para a instancia e invalida.", nameof(configuracaoRegraAprovacaoId));
        }

        if (aprovacaoChamadoLegadaId == Guid.Empty)
        {
            throw new ArgumentException("A aprovacao legada vinculada a instancia e invalida.", nameof(aprovacaoChamadoLegadaId));
        }

        ConfiguracaoRegraAprovacaoId = configuracaoRegraAprovacaoId;
        AprovacaoChamadoLegadaId = aprovacaoChamadoLegadaId;
    }

    private void DefinirTitulo(string? titulo, OrigemInstanciaAprovacaoChamado origem)
    {
        Titulo = string.IsNullOrWhiteSpace(titulo)
            ? $"Instancia de aprovacao {origem}"
            : NormalizarTextoObrigatorio(
                titulo,
                MaximoTitulo,
                "O titulo da instancia de aprovacao e obrigatorio.",
                nameof(titulo));
    }

    private void DefinirDescricao(string? descricao)
        => Descricao = NormalizarTexto(descricao, MaximoDescricao, nameof(descricao));

    private void DefinirOrigem(OrigemInstanciaAprovacaoChamado origem)
    {
        if (!Enum.IsDefined(origem))
        {
            throw new ArgumentException("A origem da instancia de aprovacao informada e invalida.", nameof(origem));
        }

        Origem = origem;
    }

    private void DefinirFluxoEComportamento(
        TipoFluxoAprovacao tipoFluxoAprovacao,
        EfeitoOperacionalRegraAprovacao efeitoOperacional,
        EscopoRegraAprovacao escopoRegra,
        TipoRegraAprovacao tipoRegra,
        bool exigeAprovacao,
        bool bloqueante,
        bool permiteReenvio,
        bool permiteFallback)
    {
        if (!Enum.IsDefined(tipoFluxoAprovacao))
        {
            throw new ArgumentException("O tipo de fluxo da instancia de aprovacao e invalido.", nameof(tipoFluxoAprovacao));
        }

        if (!Enum.IsDefined(efeitoOperacional))
        {
            throw new ArgumentException("O efeito operacional da instancia de aprovacao e invalido.", nameof(efeitoOperacional));
        }

        if (!Enum.IsDefined(escopoRegra))
        {
            throw new ArgumentException("O escopo da instancia de aprovacao e invalido.", nameof(escopoRegra));
        }

        if (!Enum.IsDefined(tipoRegra))
        {
            throw new ArgumentException("O tipo da regra snapshot da instancia de aprovacao e invalido.", nameof(tipoRegra));
        }

        switch (efeitoOperacional)
        {
            case EfeitoOperacionalRegraAprovacao.Permitir:
            case EfeitoOperacionalRegraAprovacao.Sinalizar:
                if (exigeAprovacao || bloqueante)
                {
                    throw new InvalidOperationException("Instancias com efeito de permitir ou sinalizar nao podem exigir aprovacao nem bloquear.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.ExigirAprovacao:
                if (!exigeAprovacao || bloqueante)
                {
                    throw new InvalidOperationException("Instancias nao bloqueantes devem exigir aprovacao e nao bloquear.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco:
                if (!exigeAprovacao || !bloqueante)
                {
                    throw new InvalidOperationException("Instancias bloqueantes devem exigir aprovacao e bloquear.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.RequerReavaliacao:
                if (bloqueante)
                {
                    throw new InvalidOperationException("Instancias de reavaliacao nao devem marcar bloqueio imediato nesta etapa.");
                }

                break;
        }

        TipoFluxoAprovacao = tipoFluxoAprovacao;
        EfeitoOperacional = efeitoOperacional;
        EscopoRegra = escopoRegra;
        TipoRegra = tipoRegra;
        ExigeAprovacao = exigeAprovacao;
        Bloqueante = bloqueante;
        PermiteReenvio = permiteReenvio;
        PermiteFallback = permiteFallback;
    }

    private void DefinirEscopoAvaliado(
        NaturezaChamadoEnum? naturezaChamado,
        Guid? tipoSolicitacaoId,
        Guid? catalogoServicoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        ImpactoChamadoEnum? impactoAvaliado,
        UrgenciaChamadoEnum? urgenciaAvaliada,
        PrioridadeChamadoEnum? prioridadeAvaliada,
        decimal? custoAvaliado,
        int? nivelRiscoAvaliado)
    {
        if (tipoSolicitacaoId == Guid.Empty)
        {
            throw new ArgumentException("O tipo de solicitacao avaliado na instancia e invalido.", nameof(tipoSolicitacaoId));
        }

        if (catalogoServicoId == Guid.Empty)
        {
            throw new ArgumentException("O catalogo de servico avaliado na instancia e invalido.", nameof(catalogoServicoId));
        }

        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria avaliada na instancia e invalida.", nameof(categoriaId));
        }

        if (subcategoriaId == Guid.Empty)
        {
            throw new ArgumentException("A subcategoria avaliada na instancia e invalida.", nameof(subcategoriaId));
        }

        if (subcategoriaId.HasValue && !categoriaId.HasValue)
        {
            throw new InvalidOperationException("Uma instancia com subcategoria avaliada deve informar tambem a categoria.");
        }

        if (naturezaChamado.HasValue && !Enum.IsDefined(naturezaChamado.Value))
        {
            throw new ArgumentException("A natureza avaliada da instancia e invalida.", nameof(naturezaChamado));
        }

        if (impactoAvaliado.HasValue && !Enum.IsDefined(impactoAvaliado.Value))
        {
            throw new ArgumentException("O impacto avaliado da instancia e invalido.", nameof(impactoAvaliado));
        }

        if (urgenciaAvaliada.HasValue && !Enum.IsDefined(urgenciaAvaliada.Value))
        {
            throw new ArgumentException("A urgencia avaliada da instancia e invalida.", nameof(urgenciaAvaliada));
        }

        if (prioridadeAvaliada.HasValue && !Enum.IsDefined(prioridadeAvaliada.Value))
        {
            throw new ArgumentException("A prioridade avaliada da instancia e invalida.", nameof(prioridadeAvaliada));
        }

        if (custoAvaliado.HasValue && custoAvaliado.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(custoAvaliado), "O custo avaliado da instancia nao pode ser negativo.");
        }

        if (nivelRiscoAvaliado.HasValue && nivelRiscoAvaliado.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivelRiscoAvaliado), "O nivel de risco avaliado da instancia deve ser maior que zero.");
        }

        NaturezaChamado = naturezaChamado;
        TipoSolicitacaoId = tipoSolicitacaoId;
        CatalogoServicoId = catalogoServicoId;
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;
        ImpactoAvaliado = impactoAvaliado;
        UrgenciaAvaliada = urgenciaAvaliada;
        PrioridadeAvaliada = prioridadeAvaliada;
        CustoAvaliado = custoAvaliado;
        NivelRiscoAvaliado = nivelRiscoAvaliado;
    }

    private void DefinirResolucaoAprovador(
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        Guid? aprovadorEspecificoUsuarioId,
        Guid? aprovadorPadraoUsuarioId,
        Guid? aprovadorResolvidoUsuarioId)
    {
        if (!Enum.IsDefined(tipoResolucaoAprovador))
        {
            throw new ArgumentException("O tipo de resolucao de aprovador da instancia e invalido.", nameof(tipoResolucaoAprovador));
        }

        if (aprovadorEspecificoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador especifico da instancia e invalido.", nameof(aprovadorEspecificoUsuarioId));
        }

        if (aprovadorPadraoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador padrao da instancia e invalido.", nameof(aprovadorPadraoUsuarioId));
        }

        if (aprovadorResolvidoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador resolvido da instancia e invalido.", nameof(aprovadorResolvidoUsuarioId));
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico && !aprovadorEspecificoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Instancias com aprovador especifico devem informar o usuario aprovador.");
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao && !aprovadorPadraoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Instancias com aprovador padrao devem informar o usuario padrao.");
        }

        if (aprovadorEspecificoUsuarioId.HasValue && tipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico)
        {
            throw new InvalidOperationException("A referencia de aprovador especifico da instancia so pode ser usada quando a resolucao for por aprovador especifico.");
        }

        if (aprovadorPadraoUsuarioId.HasValue && tipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao)
        {
            throw new InvalidOperationException("A referencia de aprovador padrao da instancia so pode ser usada quando a resolucao for por aprovador padrao.");
        }

        TipoResolucaoAprovador = tipoResolucaoAprovador;
        AprovadorEspecificoUsuarioId = aprovadorEspecificoUsuarioId;
        AprovadorPadraoUsuarioId = aprovadorPadraoUsuarioId;
        AprovadorResolvidoUsuarioId = aprovadorResolvidoUsuarioId;
    }

    private void DefinirPrazo(int? prazoDecisaoHoras, DateTime? deveExpirarEm)
    {
        if (prazoDecisaoHoras.HasValue && prazoDecisaoHoras.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoDecisaoHoras), "O prazo de decisao da instancia deve ser maior que zero.");
        }

        if (deveExpirarEm.HasValue && deveExpirarEm.Value < SolicitadaEm)
        {
            throw new InvalidOperationException("A expiracao planejada da instancia nao pode ser anterior a solicitacao.");
        }

        PrazoDecisaoHoras = prazoDecisaoHoras;
        DeveExpirarEm = deveExpirarEm;
    }

    private void DefinirSnapshotsRegra(string? regraNomeSnapshot, int? regraVersaoSnapshot, string? regraCriterioSnapshot)
    {
        var nomeNormalizado = NormalizarTexto(regraNomeSnapshot, MaximoRegraNomeSnapshot, nameof(regraNomeSnapshot));
        var criterioNormalizado = NormalizarTexto(regraCriterioSnapshot, MaximoRegraCriterioSnapshot, nameof(regraCriterioSnapshot));

        if (ConfiguracaoRegraAprovacaoId.HasValue)
        {
            if (string.IsNullOrWhiteSpace(nomeNormalizado))
            {
                throw new InvalidOperationException("Instancias vinculadas a uma configuracao de regra devem preservar o nome da regra em snapshot.");
            }

            if (!regraVersaoSnapshot.HasValue || regraVersaoSnapshot.Value <= 0)
            {
                throw new InvalidOperationException("Instancias vinculadas a uma configuracao de regra devem preservar a versao da regra em snapshot.");
            }
        }
        else if (regraVersaoSnapshot.HasValue && regraVersaoSnapshot.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(regraVersaoSnapshot), "A versao snapshot da regra, quando informada, deve ser maior que zero.");
        }

        RegraNomeSnapshot = nomeNormalizado;
        RegraVersaoSnapshot = regraVersaoSnapshot;
        RegraCriterioSnapshot = criterioNormalizado;
    }

    private void AtualizarAuditoriaUsuario(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da instancia de aprovacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
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
