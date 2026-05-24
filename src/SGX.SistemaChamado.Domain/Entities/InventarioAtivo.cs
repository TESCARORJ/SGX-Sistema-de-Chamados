using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class InventarioAtivo : AuditableEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? NumeroPatrimonio { get; private set; }
    public string? NumeroSerie { get; private set; }
    public Guid TipoAtivoInventarioId { get; private set; }
    public string? Fabricante { get; private set; }
    public string? Modelo { get; private set; }
    public Guid? DepartamentoId { get; private set; }
    public Guid? LocalUnidadeId { get; private set; }
    public Guid? UsuarioResponsavelId { get; private set; }
    public StatusOperacionalAtivo StatusOperacional { get; private set; } = StatusOperacionalAtivo.Operacional;
    public StatusPatrimonialAtivo StatusPatrimonial { get; private set; } = StatusPatrimonialAtivo.EmUso;
    public CriticidadeAtivo Criticidade { get; private set; } = CriticidadeAtivo.Media;
    public DateTime? DataAquisicao { get; private set; }
    public DateTime? DataFimGarantia { get; private set; }
    public decimal? ValorAquisicao { get; private set; }
    public string? Fornecedor { get; private set; }
    public string? Observacoes { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }
    public DateTime? InativadoEm { get; private set; }
    public Guid? InativadoPorUsuarioId { get; private set; }

    public TipoAtivoInventario TipoAtivoInventario { get; private set; } = default!;
    public Departamento? Departamento { get; private set; }
    public LocalUnidade? LocalUnidade { get; private set; }
    public Usuario? UsuarioResponsavel { get; private set; }
    public ICollection<HistoricoInventarioAtivo> Historicos { get; private set; } = [];
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private InventarioAtivo()
    {
    }

    public InventarioAtivo(
        string codigo,
        string nome,
        Guid tipoAtivoInventarioId,
        Guid criadoPorUsuarioId,
        string criadoPor)
    {
        DefinirCodigo(codigo);
        DefinirNome(nome);
        DefinirTipoAtivoInventario(tipoAtivoInventarioId);
        DefinirCriadoPorUsuario(criadoPorUsuarioId);
        DefinirCriacao(criadoPor);
    }

    public void DefinirCodigo(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new ArgumentException("O codigo do ativo e obrigatorio.", nameof(codigo));
        }

        Codigo = codigo.Trim().ToUpperInvariant();
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do ativo e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao) => Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();

    public void DefinirNumeroPatrimonio(string? numeroPatrimonio) => NumeroPatrimonio = string.IsNullOrWhiteSpace(numeroPatrimonio) ? null : numeroPatrimonio.Trim();

    public void DefinirNumeroSerie(string? numeroSerie) => NumeroSerie = string.IsNullOrWhiteSpace(numeroSerie) ? null : numeroSerie.Trim();

    public void DefinirTipoAtivoInventario(Guid tipoAtivoInventarioId)
    {
        if (tipoAtivoInventarioId == Guid.Empty)
        {
            throw new ArgumentException("O tipo de ativo do inventario e obrigatorio.", nameof(tipoAtivoInventarioId));
        }

        TipoAtivoInventarioId = tipoAtivoInventarioId;
    }

    public void DefinirFabricante(string? fabricante) => Fabricante = string.IsNullOrWhiteSpace(fabricante) ? null : fabricante.Trim();

    public void DefinirModelo(string? modelo) => Modelo = string.IsNullOrWhiteSpace(modelo) ? null : modelo.Trim();

    public void DefinirDepartamento(Guid? departamentoId)
    {
        if (departamentoId == Guid.Empty)
        {
            throw new ArgumentException("O departamento informado e invalido.", nameof(departamentoId));
        }

        DepartamentoId = departamentoId;
    }

    public void DefinirLocalUnidade(Guid? localUnidadeId)
    {
        if (localUnidadeId == Guid.Empty)
        {
            throw new ArgumentException("O local/unidade informado e invalido.", nameof(localUnidadeId));
        }

        LocalUnidadeId = localUnidadeId;
    }

    public void DefinirUsuarioResponsavel(Guid? usuarioResponsavelId)
    {
        if (usuarioResponsavelId == Guid.Empty)
        {
            throw new ArgumentException("O usuario responsavel informado e invalido.", nameof(usuarioResponsavelId));
        }

        UsuarioResponsavelId = usuarioResponsavelId;
    }

    public void DefinirStatusOperacional(StatusOperacionalAtivo statusOperacional)
    {
        if (!Enum.IsDefined(statusOperacional))
        {
            throw new ArgumentException("Status operacional invalido.", nameof(statusOperacional));
        }

        StatusOperacional = statusOperacional;
    }

    public void DefinirStatusPatrimonial(StatusPatrimonialAtivo statusPatrimonial)
    {
        if (!Enum.IsDefined(statusPatrimonial))
        {
            throw new ArgumentException("Status patrimonial invalido.", nameof(statusPatrimonial));
        }

        StatusPatrimonial = statusPatrimonial;
    }

    public void DefinirCriticidade(CriticidadeAtivo criticidade)
    {
        if (!Enum.IsDefined(criticidade))
        {
            throw new ArgumentException("Criticidade invalida.", nameof(criticidade));
        }

        Criticidade = criticidade;
    }

    public void DefinirDataAquisicao(DateTime? dataAquisicao) => DataAquisicao = dataAquisicao?.Date;

    public void DefinirDataFimGarantia(DateTime? dataFimGarantia) => DataFimGarantia = dataFimGarantia?.Date;

    public void DefinirValorAquisicao(decimal? valorAquisicao)
    {
        if (valorAquisicao is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorAquisicao), "O valor de aquisicao nao pode ser negativo.");
        }

        ValorAquisicao = valorAquisicao;
    }

    public void DefinirFornecedor(string? fornecedor) => Fornecedor = string.IsNullOrWhiteSpace(fornecedor) ? null : fornecedor.Trim();

    public void DefinirObservacoes(string? observacoes) => Observacoes = string.IsNullOrWhiteSpace(observacoes) ? null : observacoes.Trim();

    public void DefinirCriadoPorUsuario(Guid criadoPorUsuarioId)
    {
        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        CriadoPorUsuarioId = criadoPorUsuarioId;
    }

    public void AtualizarAuditoriaUsuario(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Inativar(Guid inativadoPorUsuarioId, string atualizadoPor)
    {
        if (inativadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de inativacao e obrigatorio.", nameof(inativadoPorUsuarioId));
        }

        InativadoEm = DateTime.UtcNow;
        InativadoPorUsuarioId = inativadoPorUsuarioId;
        AtualizadoPorUsuarioId = inativadoPorUsuarioId;
        Desativar(atualizadoPor);
    }

    public void Reativar(Guid reativadoPorUsuarioId, string atualizadoPor)
    {
        if (reativadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de reativacao e obrigatorio.", nameof(reativadoPorUsuarioId));
        }

        // Preserva ultima inativacao para rastreabilidade historica.
        AtualizadoPorUsuarioId = reativadoPorUsuarioId;
        Ativar(atualizadoPor);
    }
}
