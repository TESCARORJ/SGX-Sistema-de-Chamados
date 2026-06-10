using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ConfiguracaoRegraAprovacao : AuditableEntity
{
    private const int MaximoNome = 180;
    private const int MaximoDescricao = 4000;

    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public TipoRegraAprovacao TipoRegra { get; private set; }
    public EscopoRegraAprovacao EscopoRegra { get; private set; }
    public int Ordem { get; private set; }
    public int Prioridade { get; private set; }
    public int Versao { get; private set; } = 1;
    public NaturezaChamadoEnum? NaturezaChamado { get; private set; }
    public Guid? TipoSolicitacaoId { get; private set; }
    public Guid? CatalogoServicoId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public ImpactoChamadoEnum? ImpactoMinimo { get; private set; }
    public UrgenciaChamadoEnum? UrgenciaMinima { get; private set; }
    public PrioridadeChamadoEnum? PrioridadeMinima { get; private set; }
    public decimal? CustoMinimo { get; private set; }
    public int? NivelRiscoMinimo { get; private set; }
    public bool ExigeAprovacao { get; private set; }
    public bool Bloqueante { get; private set; }
    public bool PermiteReenvio { get; private set; }
    public bool PermiteFallback { get; private set; }
    public EfeitoOperacionalRegraAprovacao EfeitoOperacional { get; private set; }
    public TipoFluxoAprovacao TipoFluxoAprovacao { get; private set; } = TipoFluxoAprovacao.Simples;
    public TipoResolucaoAprovadorRegraAprovacao TipoResolucaoAprovador { get; private set; } = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido;
    public Guid? AprovadorEspecificoUsuarioId { get; private set; }
    public Guid? AprovadorPadraoUsuarioId { get; private set; }
    public int? PrazoDecisaoHoras { get; private set; }
    public DateTime? VigenteDe { get; private set; }
    public DateTime? VigenteAte { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public TipoSolicitacao? TipoSolicitacao { get; private set; }
    public CatalogoServico? CatalogoServico { get; private set; }
    public CategoriaChamado? Categoria { get; private set; }
    public SubcategoriaChamado? Subcategoria { get; private set; }
    public ICollection<InstanciaAprovacaoChamado> InstanciasAprovacao { get; private set; } = [];
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }
    public Usuario? AprovadorEspecificoUsuario { get; private set; }
    public Usuario? AprovadorPadraoUsuario { get; private set; }

    private ConfiguracaoRegraAprovacao()
    {
    }

    public ConfiguracaoRegraAprovacao(
        string nome,
        TipoRegraAprovacao tipoRegra,
        EscopoRegraAprovacao escopoRegra,
        EfeitoOperacionalRegraAprovacao efeitoOperacional,
        TipoFluxoAprovacao tipoFluxoAprovacao,
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        int ordem,
        int prioridade,
        int versao,
        Guid criadoPorUsuarioId,
        string criadoPor,
        string? descricao = null,
        NaturezaChamadoEnum? naturezaChamado = null,
        Guid? tipoSolicitacaoId = null,
        Guid? catalogoServicoId = null,
        Guid? categoriaId = null,
        Guid? subcategoriaId = null,
        ImpactoChamadoEnum? impactoMinimo = null,
        UrgenciaChamadoEnum? urgenciaMinima = null,
        PrioridadeChamadoEnum? prioridadeMinima = null,
        decimal? custoMinimo = null,
        int? nivelRiscoMinimo = null,
        bool exigeAprovacao = true,
        bool bloqueante = false,
        bool permiteReenvio = false,
        bool permiteFallback = false,
        Guid? aprovadorEspecificoUsuarioId = null,
        Guid? aprovadorPadraoUsuarioId = null,
        int? prazoDecisaoHoras = null,
        DateTime? vigenteDe = null,
        DateTime? vigenteAte = null)
    {
        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da configuracao da regra e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirTipoRegra(tipoRegra);
        DefinirEscopoRegra(escopoRegra);
        DefinirOrdem(ordem);
        DefinirPrioridade(prioridade);
        DefinirVersao(versao);
        DefinirCriterios(
            naturezaChamado,
            tipoSolicitacaoId,
            catalogoServicoId,
            categoriaId,
            subcategoriaId,
            impactoMinimo,
            urgenciaMinima,
            prioridadeMinima,
            custoMinimo,
            nivelRiscoMinimo);
        DefinirEfeitoOperacional(efeitoOperacional, exigeAprovacao, bloqueante);
        DefinirComportamentosAdicionais(permiteReenvio, permiteFallback);
        DefinirFluxoEAprovador(
            tipoFluxoAprovacao,
            tipoResolucaoAprovador,
            aprovadorEspecificoUsuarioId,
            aprovadorPadraoUsuarioId,
            prazoDecisaoHoras);
        DefinirVigencia(vigenteDe, vigenteAte);

        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void AtivarRegra(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacaoUsuario(atualizadoPorUsuarioId, atualizadoPor);
        Ativar(atualizadoPor);
    }

    public void DesativarRegra(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        RegistrarAtualizacaoUsuario(atualizadoPorUsuarioId, atualizadoPor);
        Desativar(atualizadoPor);
    }

    public bool EstaVigenteEm(DateTime dataReferenciaUtc)
    {
        if (VigenteDe.HasValue && dataReferenciaUtc < VigenteDe.Value)
        {
            return false;
        }

        if (VigenteAte.HasValue && dataReferenciaUtc > VigenteAte.Value)
        {
            return false;
        }

        return Ativo;
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da regra de aprovacao e obrigatorio.", nameof(nome));
        }

        var nomeNormalizado = nome.Trim();
        if (nomeNormalizado.Length > MaximoNome)
        {
            throw new ArgumentException($"O nome da regra de aprovacao deve possuir no maximo {MaximoNome} caracteres.", nameof(nome));
        }

        Nome = nomeNormalizado;
    }

    private void DefinirDescricao(string? descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            Descricao = null;
            return;
        }

        var descricaoNormalizada = descricao.Trim();
        if (descricaoNormalizada.Length > MaximoDescricao)
        {
            throw new ArgumentException($"A descricao da regra de aprovacao deve possuir no maximo {MaximoDescricao} caracteres.", nameof(descricao));
        }

        Descricao = descricaoNormalizada;
    }

    private void DefinirTipoRegra(TipoRegraAprovacao tipoRegra)
    {
        if (!Enum.IsDefined(tipoRegra))
        {
            throw new ArgumentException("O tipo da regra de aprovacao informado e invalido.", nameof(tipoRegra));
        }

        TipoRegra = tipoRegra;
    }

    private void DefinirEscopoRegra(EscopoRegraAprovacao escopoRegra)
    {
        if (!Enum.IsDefined(escopoRegra))
        {
            throw new ArgumentException("O escopo da regra de aprovacao informado e invalido.", nameof(escopoRegra));
        }

        EscopoRegra = escopoRegra;
    }

    private void DefinirOrdem(int ordem)
    {
        if (ordem < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem da regra de aprovacao nao pode ser negativa.");
        }

        Ordem = ordem;
    }

    private void DefinirPrioridade(int prioridade)
    {
        if (prioridade < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prioridade), "A prioridade da regra de aprovacao nao pode ser negativa.");
        }

        Prioridade = prioridade;
    }

    private void DefinirVersao(int versao)
    {
        if (versao <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(versao), "A versao da regra de aprovacao deve ser maior que zero.");
        }

        Versao = versao;
    }

    private void DefinirCriterios(
        NaturezaChamadoEnum? naturezaChamado,
        Guid? tipoSolicitacaoId,
        Guid? catalogoServicoId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        ImpactoChamadoEnum? impactoMinimo,
        UrgenciaChamadoEnum? urgenciaMinima,
        PrioridadeChamadoEnum? prioridadeMinima,
        decimal? custoMinimo,
        int? nivelRiscoMinimo)
    {
        if (tipoSolicitacaoId == Guid.Empty)
        {
            throw new ArgumentException("O tipo de solicitacao informado para a regra e invalido.", nameof(tipoSolicitacaoId));
        }

        if (catalogoServicoId == Guid.Empty)
        {
            throw new ArgumentException("O catalogo de servico informado para a regra e invalido.", nameof(catalogoServicoId));
        }

        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria informada para a regra e invalida.", nameof(categoriaId));
        }

        if (subcategoriaId == Guid.Empty)
        {
            throw new ArgumentException("A subcategoria informada para a regra e invalida.", nameof(subcategoriaId));
        }

        if (subcategoriaId.HasValue && !categoriaId.HasValue)
        {
            throw new InvalidOperationException("Uma regra com subcategoria deve informar tambem a categoria.");
        }

        if (naturezaChamado.HasValue && !Enum.IsDefined(naturezaChamado.Value))
        {
            throw new ArgumentException("A natureza ITSM informada para a regra e invalida.", nameof(naturezaChamado));
        }

        if (impactoMinimo.HasValue && !Enum.IsDefined(impactoMinimo.Value))
        {
            throw new ArgumentException("O impacto minimo informado para a regra e invalido.", nameof(impactoMinimo));
        }

        if (urgenciaMinima.HasValue && !Enum.IsDefined(urgenciaMinima.Value))
        {
            throw new ArgumentException("A urgencia minima informada para a regra e invalida.", nameof(urgenciaMinima));
        }

        if (prioridadeMinima.HasValue && !Enum.IsDefined(prioridadeMinima.Value))
        {
            throw new ArgumentException("A prioridade minima informada para a regra e invalida.", nameof(prioridadeMinima));
        }

        if (custoMinimo.HasValue && custoMinimo.Value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(custoMinimo), "O custo minimo da regra nao pode ser negativo.");
        }

        if (nivelRiscoMinimo.HasValue && nivelRiscoMinimo.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(nivelRiscoMinimo), "O nivel de risco minimo deve ser maior que zero.");
        }

        NaturezaChamado = naturezaChamado;
        TipoSolicitacaoId = tipoSolicitacaoId;
        CatalogoServicoId = catalogoServicoId;
        CategoriaId = categoriaId;
        SubcategoriaId = subcategoriaId;
        ImpactoMinimo = impactoMinimo;
        UrgenciaMinima = urgenciaMinima;
        PrioridadeMinima = prioridadeMinima;
        CustoMinimo = custoMinimo;
        NivelRiscoMinimo = nivelRiscoMinimo;
    }

    private void DefinirEfeitoOperacional(
        EfeitoOperacionalRegraAprovacao efeitoOperacional,
        bool exigeAprovacao,
        bool bloqueante)
    {
        if (!Enum.IsDefined(efeitoOperacional))
        {
            throw new ArgumentException("O efeito operacional da regra de aprovacao informado e invalido.", nameof(efeitoOperacional));
        }

        switch (efeitoOperacional)
        {
            case EfeitoOperacionalRegraAprovacao.Permitir:
            case EfeitoOperacionalRegraAprovacao.Sinalizar:
                if (exigeAprovacao || bloqueante)
                {
                    throw new InvalidOperationException("Regras de permitir ou sinalizar nao podem exigir aprovacao nem marcar bloqueio.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.ExigirAprovacao:
                if (!exigeAprovacao || bloqueante)
                {
                    throw new InvalidOperationException("Regras de aprovacao nao bloqueante devem exigir aprovacao e nao marcar bloqueio.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco:
                if (!exigeAprovacao || !bloqueante)
                {
                    throw new InvalidOperationException("Regras de aprovacao bloqueante devem exigir aprovacao e marcar bloqueio.");
                }

                break;
            case EfeitoOperacionalRegraAprovacao.RequerReavaliacao:
                if (bloqueante)
                {
                    throw new InvalidOperationException("Regras de reavaliacao nao devem marcar bloqueio imediato nesta etapa.");
                }

                break;
        }

        EfeitoOperacional = efeitoOperacional;
        ExigeAprovacao = exigeAprovacao;
        Bloqueante = bloqueante;
    }

    private void DefinirComportamentosAdicionais(bool permiteReenvio, bool permiteFallback)
    {
        PermiteReenvio = permiteReenvio;
        PermiteFallback = permiteFallback;
    }

    private void DefinirFluxoEAprovador(
        TipoFluxoAprovacao tipoFluxoAprovacao,
        TipoResolucaoAprovadorRegraAprovacao tipoResolucaoAprovador,
        Guid? aprovadorEspecificoUsuarioId,
        Guid? aprovadorPadraoUsuarioId,
        int? prazoDecisaoHoras)
    {
        if (!Enum.IsDefined(tipoFluxoAprovacao))
        {
            throw new ArgumentException("O tipo de fluxo de aprovacao informado e invalido.", nameof(tipoFluxoAprovacao));
        }

        if (!Enum.IsDefined(tipoResolucaoAprovador))
        {
            throw new ArgumentException("O tipo de resolucao de aprovador informado e invalido.", nameof(tipoResolucaoAprovador));
        }

        if (aprovadorEspecificoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador especifico informado para a regra e invalido.", nameof(aprovadorEspecificoUsuarioId));
        }

        if (aprovadorPadraoUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador padrao informado para a regra e invalido.", nameof(aprovadorPadraoUsuarioId));
        }

        if (prazoDecisaoHoras.HasValue && prazoDecisaoHoras.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(prazoDecisaoHoras), "O prazo de decisao da regra deve ser maior que zero.");
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico && !aprovadorEspecificoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Regras com aprovador especifico devem informar o usuario aprovador.");
        }

        if (tipoResolucaoAprovador == TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao && !aprovadorPadraoUsuarioId.HasValue)
        {
            throw new InvalidOperationException("Regras com aprovador padrao devem informar o usuario padrao.");
        }

        if (aprovadorEspecificoUsuarioId.HasValue && tipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico)
        {
            throw new InvalidOperationException("A referencia de aprovador especifico so pode ser usada em regra com resolucao por aprovador especifico.");
        }

        if (aprovadorPadraoUsuarioId.HasValue && tipoResolucaoAprovador != TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao)
        {
            throw new InvalidOperationException("A referencia de aprovador padrao so pode ser usada em regra com resolucao por aprovador padrao.");
        }

        TipoFluxoAprovacao = tipoFluxoAprovacao;
        TipoResolucaoAprovador = tipoResolucaoAprovador;
        AprovadorEspecificoUsuarioId = aprovadorEspecificoUsuarioId;
        AprovadorPadraoUsuarioId = aprovadorPadraoUsuarioId;
        PrazoDecisaoHoras = prazoDecisaoHoras;
    }

    private void DefinirVigencia(DateTime? vigenteDe, DateTime? vigenteAte)
    {
        if (vigenteDe.HasValue && vigenteAte.HasValue && vigenteAte.Value < vigenteDe.Value)
        {
            throw new InvalidOperationException("A vigencia final da regra de aprovacao nao pode ser anterior ao inicio da vigencia.");
        }

        VigenteDe = vigenteDe;
        VigenteAte = vigenteAte;
    }

    private void RegistrarAtualizacaoUsuario(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da regra de aprovacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }
}
