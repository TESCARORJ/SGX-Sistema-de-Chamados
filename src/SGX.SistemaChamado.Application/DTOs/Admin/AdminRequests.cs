using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroChamadosAdminRequest
{
    public Guid? StatusId { get; init; }
    public Guid? PrioridadeId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? ResponsavelId { get; init; }
    public Guid? SolicitanteId { get; init; }
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public bool? SlaVencido { get; init; }
    public SituacaoSlaChamadoEnum? SlaSituacao { get; init; }
    public string? Texto { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "atualizadoEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed class AtribuirChamadoRequest
{
    public Guid ResponsavelId { get; init; }
}

public sealed class AlterarStatusChamadoRequest
{
    public Guid StatusId { get; init; }
}

public sealed class AlterarPrioridadeChamadoRequest
{
    public Guid PrioridadeId { get; init; }
}

public sealed class AlterarCategoriaChamadoRequest
{
    public Guid CategoriaId { get; init; }
}

public sealed class ComentarioAdminChamadoRequest
{
    public string Mensagem { get; init; } = string.Empty;
    public bool Interno { get; init; }
}

public sealed class EncerrarChamadoRequest
{
    public string Solucao { get; init; } = string.Empty;
    public bool ComentarioInterno { get; init; }
}

public sealed class ReabrirChamadoRequest
{
    public string Mensagem { get; init; } = string.Empty;
}
