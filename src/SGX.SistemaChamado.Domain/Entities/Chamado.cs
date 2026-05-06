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
    public Guid? DepartamentoId { get; private set; }
    public Guid CategoriaId { get; private set; }
    public Guid PrioridadeId { get; private set; }
    public Guid StatusId { get; private set; }
    public OrigemChamado Origem { get; private set; }
    public DateTime AbertoEm { get; private set; }
    public DateTime? EncerradoEm { get; private set; }

    public Usuario Solicitante { get; private set; } = default!;
    public Usuario? Responsavel { get; private set; }
    public Departamento? Departamento { get; private set; }
    public CategoriaChamado Categoria { get; private set; } = default!;
    public PrioridadeChamado Prioridade { get; private set; } = default!;
    public StatusChamado Status { get; private set; } = default!;
    public ICollection<HistoricoChamado> Historicos { get; private set; } = [];
    public ICollection<ComentarioChamado> Comentarios { get; private set; } = [];
    public ICollection<AnexoChamado> Anexos { get; private set; } = [];
    public SlaControle? SlaControle { get; private set; }

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
        Guid? departamentoId = null)
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

        SolicitanteId = solicitanteId;
        CategoriaId = categoriaId;
        PrioridadeId = prioridadeId;
        StatusId = statusId;
        Origem = origem;
        DepartamentoId = departamentoId;
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

    public void AlterarPrioridade(Guid prioridadeId, string atualizadoPor)
    {
        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade informada e invalida.", nameof(prioridadeId));
        }

        PrioridadeId = prioridadeId;
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
}
