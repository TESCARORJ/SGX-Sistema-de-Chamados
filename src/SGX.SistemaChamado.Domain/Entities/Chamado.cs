using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class Chamado : AuditableEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public string Descricao { get; private set; } = string.Empty;
    public Guid SolicitanteId { get; private set; }
    public Guid? ResponsavelId { get; private set; }
    public Guid? GrupoTecnicoId { get; private set; }
    public Guid? FilaAtendimentoId { get; private set; }
    public Guid? DepartamentoId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public Guid PrioridadeId { get; private set; }
    public Guid? TipoSolicitacaoId { get; private set; }
    public Guid? LocalUnidadeId { get; private set; }
    public Guid? CatalogoServicoId { get; private set; }
    public Guid? InventarioAtivoId { get; private set; }
    public Guid StatusId { get; private set; }
    public OrigemChamado Origem { get; private set; }
    public NaturezaChamadoEnum NaturezaChamado { get; private set; } = NaturezaChamadoEnum.Requisicao;
    public ImpactoChamadoEnum ImpactoChamado { get; private set; } = ImpactoChamadoEnum.Baixo;
    public UrgenciaChamadoEnum UrgenciaChamado { get; private set; } = UrgenciaChamadoEnum.Baixa;
    public DateTime AbertoEm { get; private set; }
    public DateTime? EncerradoEm { get; private set; }
    public DateTime? ResolvidoEm { get; private set; }
    public Guid? AceitoPorUsuarioId { get; private set; }
    public string? ObservacaoAceite { get; private set; }
    public DateTime? SolucaoRejeitadaEm { get; private set; }
    public Guid? SolucaoRejeitadaPorUsuarioId { get; private set; }
    public string? MotivoRejeicaoSolucao { get; private set; }
    public DateTime? AceitoEm { get; private set; }
    public Usuario Solicitante { get; private set; } = default!;
    public Usuario? Responsavel { get; private set; }
    public GrupoTecnico? GrupoTecnico { get; private set; }
    public FilaAtendimento? FilaAtendimento { get; private set; }
    public Departamento? Departamento { get; private set; }
    public CategoriaChamado Categoria { get; private set; } = default!;
    public SubcategoriaChamado? Subcategoria { get; private set; }
    public PrioridadeChamado Prioridade { get; private set; } = default!;
    public TipoSolicitacao? TipoSolicitacao { get; private set; }
    public LocalUnidade? LocalUnidade { get; private set; }
    public CatalogoServico? CatalogoServico { get; private set; }
    public InventarioAtivo? InventarioAtivo { get; private set; }
    public StatusChamado Status { get; private set; } = default!;
    public ICollection<HistoricoChamado> Historicos { get; private set; } = [];
    public ICollection<ComentarioChamado> Comentarios { get; private set; } = [];
    public ICollection<AnexoChamado> Anexos { get; private set; } = [];
    public ICollection<ChamadoArtigoConhecimento> ArtigosConhecimento { get; private set; } = [];
    public ICollection<ChamadoRelacionamento> RelacionamentosOrigem { get; private set; } = [];
    public ICollection<ChamadoRelacionamento> RelacionamentosDestino { get; private set; } = [];
    public ICollection<ChamadoTarefa> Tarefas { get; private set; } = [];
    public ICollection<AprovacaoChamado> Aprovacoes { get; private set; } = [];
    public ICollection<InstanciaAprovacaoChamado> InstanciasAprovacao { get; private set; } = [];
    public ICollection<EventoSla> EventosSla { get; private set; } = [];
    public SlaControle? SlaControle { get; private set; }
    public ChamadoSla? ChamadoSla { get; private set; }

    private Chamado()
    {
    }

    public Chamado(
        string codigo,
        string titulo,
        string descricao,
        Guid solicitanteId,
        Guid categoriaId,
        Guid prioridadeId,
        Guid statusId,
        OrigemChamado origem,
        string criadoPor,
        Guid? departamentoId = null,
        Guid? subcategoriaId = null,
        Guid? tipoSolicitacaoId = null,
        Guid? localUnidadeId = null,
        Guid? catalogoServicoId = null,
        Guid? inventarioAtivoId = null,
        NaturezaChamadoEnum naturezaChamado = NaturezaChamadoEnum.Requisicao,
        ImpactoChamadoEnum impactoChamado = ImpactoChamadoEnum.Baixo,
        UrgenciaChamadoEnum urgenciaChamado = UrgenciaChamadoEnum.Baixa)
    {
        DefinirCodigo(codigo);
        DefinirTitulo(titulo);
        DefinirDescricao(descricao);

        if (solicitanteId == Guid.Empty)
        {
            throw new ArgumentException("O solicitante do chamado e obrigatorio.", nameof(solicitanteId));
        }

        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria do chamado e obrigatoria.", nameof(categoriaId));
        }

        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade do chamado e obrigatoria.", nameof(prioridadeId));
        }

        if (statusId == Guid.Empty)
        {
            throw new ArgumentException("O status do chamado e obrigatorio.", nameof(statusId));
        }

        if (catalogoServicoId == Guid.Empty)
        {
            throw new ArgumentException("O servico de catalogo informado e invalido.", nameof(catalogoServicoId));
        }

        if (!Enum.IsDefined(naturezaChamado))
        {
            throw new ArgumentException("A natureza do chamado informada e invalida.", nameof(naturezaChamado));
        }

        if (!Enum.IsDefined(impactoChamado))
        {
            throw new ArgumentException("O impacto do chamado informado e invalido.", nameof(impactoChamado));
        }

        if (!Enum.IsDefined(urgenciaChamado))
        {
            throw new ArgumentException("A urgencia do chamado informada e invalida.", nameof(urgenciaChamado));
        }

        SolicitanteId = solicitanteId;
        CategoriaId = categoriaId;
        PrioridadeId = prioridadeId;
        StatusId = statusId;
        Origem = origem;
        NaturezaChamado = naturezaChamado;
        ImpactoChamado = impactoChamado;
        UrgenciaChamado = urgenciaChamado;
        DepartamentoId = departamentoId;
        SubcategoriaId = subcategoriaId;
        TipoSolicitacaoId = tipoSolicitacaoId;
        LocalUnidadeId = localUnidadeId;
        CatalogoServicoId = catalogoServicoId;
        InventarioAtivoId = inventarioAtivoId;
        AbertoEm = DateTime.UtcNow;
        DefinirCriacao(criadoPor);
    }

    public void DefinirCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O codigo do chamado e obrigatorio.", nameof(codigo));
        }

        Codigo = codigo.Trim().ToUpperInvariant();
    }

    public void DefinirTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O titulo do chamado e obrigatorio.", nameof(titulo));
        }

        Titulo = titulo.Trim();
    }

    public void DefinirDescricao(string descricao)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descricao do chamado e obrigatoria.", nameof(descricao));
        }

        Descricao = descricao.Trim();
    }

    public void AlterarStatus(Guid novoStatusId, string atualizadoPor)
    {
        if (novoStatusId == Guid.Empty)
        {
            throw new ArgumentException("O status informado e invalido.", nameof(novoStatusId));
        }

        StatusId = novoStatusId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AtribuirResponsavel(Guid? responsavelId, string atualizadoPor)
    {
        if (responsavelId == Guid.Empty)
        {
            throw new ArgumentException("O responsavel informado e invalido.", nameof(responsavelId));
        }

        ResponsavelId = responsavelId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirGrupoTecnico(Guid? grupoTecnicoId, string atualizadoPor)
    {
        if (grupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico informado e invalido.", nameof(grupoTecnicoId));
        }

        GrupoTecnicoId = grupoTecnicoId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirFilaAtendimento(Guid? filaAtendimentoId, string atualizadoPor)
    {
        if (filaAtendimentoId == Guid.Empty)
        {
            throw new ArgumentException("A fila de atendimento informada e invalida.", nameof(filaAtendimentoId));
        }

        FilaAtendimentoId = filaAtendimentoId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void TransferirGrupoTecnico(Guid grupoTecnicoId, Guid? filaAtendimentoId, string atualizadoPor)
    {
        if (grupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico de destino e obrigatorio.", nameof(grupoTecnicoId));
        }

        if (filaAtendimentoId == Guid.Empty)
        {
            throw new ArgumentException("A fila de atendimento de destino informada e invalida.", nameof(filaAtendimentoId));
        }

        GrupoTecnicoId = grupoTecnicoId;
        FilaAtendimentoId = filaAtendimentoId;
        ResponsavelId = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DirecionarGrupoTecnico(Guid grupoTecnicoId, Guid? filaAtendimentoId, string atualizadoPor)
    {
        if (grupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico e obrigatorio.", nameof(grupoTecnicoId));
        }

        if (filaAtendimentoId == Guid.Empty)
        {
            throw new ArgumentException("A fila de atendimento informada e invalida.", nameof(filaAtendimentoId));
        }

        GrupoTecnicoId = grupoTecnicoId;
        FilaAtendimentoId = filaAtendimentoId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AlterarPrioridade(Guid prioridadeId, string atualizadoPor)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade informada e invalida.", nameof(prioridadeId));
        }

        PrioridadeId = prioridadeId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AlterarImpactoEUrgencia(ImpactoChamadoEnum impactoChamado, UrgenciaChamadoEnum urgenciaChamado, string atualizadoPor)
    {
        if (!Enum.IsDefined(impactoChamado))
        {
            throw new ArgumentException("O impacto do chamado informado e invalido.", nameof(impactoChamado));
        }

        if (!Enum.IsDefined(urgenciaChamado))
        {
            throw new ArgumentException("A urgencia do chamado informada e invalida.", nameof(urgenciaChamado));
        }

        ImpactoChamado = impactoChamado;
        UrgenciaChamado = urgenciaChamado;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AlterarCategoria(Guid categoriaId, Guid? departamentoId, string atualizadoPor)
    {
        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria informada e invalida.", nameof(categoriaId));
        }

        CategoriaId = categoriaId;
        DepartamentoId = departamentoId;
        SubcategoriaId = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AlterarClassificacaoOperacional(
        Guid? subcategoriaId,
        Guid? tipoSolicitacaoId,
        Guid? localUnidadeId,
        Guid? departamentoId,
        string atualizadoPor)
    {
        if (subcategoriaId == Guid.Empty)
        {
            throw new ArgumentException("A subcategoria informada e invalida.", nameof(subcategoriaId));
        }

        if (tipoSolicitacaoId == Guid.Empty)
        {
            throw new ArgumentException("O tipo de solicitacao informado e invalido.", nameof(tipoSolicitacaoId));
        }

        if (localUnidadeId == Guid.Empty)
        {
            throw new ArgumentException("O local/unidade informado e invalido.", nameof(localUnidadeId));
        }

        if (departamentoId == Guid.Empty)
        {
            throw new ArgumentException("O departamento informado e invalido.", nameof(departamentoId));
        }

        SubcategoriaId = subcategoriaId;
        TipoSolicitacaoId = tipoSolicitacaoId;
        LocalUnidadeId = localUnidadeId;
        DepartamentoId = departamentoId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void VincularInventarioAtivo(Guid inventarioAtivoId, string atualizadoPor)
    {
        if (inventarioAtivoId == Guid.Empty)
        {
            throw new ArgumentException("O ativo informado e invalido.", nameof(inventarioAtivoId));
        }

        InventarioAtivoId = inventarioAtivoId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RemoverVinculoInventarioAtivo(string atualizadoPor)
    {
        InventarioAtivoId = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Encerrar(Guid statusEncerradoId, string atualizadoPor)
    {
        if (statusEncerradoId == Guid.Empty)
        {
            throw new ArgumentException("O status de encerramento e obrigatorio.", nameof(statusEncerradoId));
        }

        StatusId = statusEncerradoId;
        EncerradoEm = DateTime.UtcNow;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Reabrir(Guid statusReabertoId, string atualizadoPor)
    {
        if (statusReabertoId == Guid.Empty)
        {
            throw new ArgumentException("O status de reabertura e obrigatorio.", nameof(statusReabertoId));
        }

        StatusId = statusReabertoId;
        EncerradoEm = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Resolver(Guid statusResolvidoId, string? solucaoTecnica, string atualizadoPor)
    {
        StatusId = statusResolvidoId;
        ResolvidoEm = DateTime.UtcNow;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Cancelar(Guid statusCanceladoId, string motivo, string atualizadoPor)
    {
        StatusId = statusCanceladoId;
        EncerradoEm = DateTime.UtcNow;
        AtualizarAuditoria(atualizadoPor);
    }

    public void AceitarSolucao(Guid statusFechadoId, Guid usuarioId, string? observacao, string atualizadoPor)
    {
        var dataAceite = DateTime.UtcNow;
        StatusId = statusFechadoId;
        AceitoPorUsuarioId = usuarioId;
        ObservacaoAceite = observacao;
        AceitoEm = dataAceite;
        EncerradoEm = dataAceite;
        AtualizarAuditoria(atualizadoPor);
    }

    public void FecharAutomaticamentePorPrazoAceite(Guid statusFechadoId, StatusChamadoEnum statusAnterior, DateTime dataReferencia, TimeSpan prazoAceite, string atualizadoPor)
    {
        StatusId = statusFechadoId;
        EncerradoEm = dataReferencia;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RejeitarSolucao(Guid statusRejeitadoId, Guid usuarioId, string motivo, string atualizadoPor)
    {
        StatusId = statusRejeitadoId;
        SolucaoRejeitadaEm = DateTime.UtcNow;
        SolucaoRejeitadaPorUsuarioId = usuarioId;
        MotivoRejeicaoSolucao = motivo;
        AtualizarAuditoria(atualizadoPor);
    }
}
