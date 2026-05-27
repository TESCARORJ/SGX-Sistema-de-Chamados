using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroIndicadoresRequest
{
    public DateTime? DataInicio { get; init; }
    public DateTime? DataFim { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? ResponsavelId { get; init; }
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
}

public sealed record IndicadorCardResponse(string Chave, string Titulo, int Valor);

public sealed record ChamadosPorStatusResponse(string Status, int Total);
public sealed record ChamadosPorPrioridadeResponse(string Prioridade, int Total);
public sealed record ChamadosPorCategoriaResponse(string Categoria, int Total);
public sealed record ChamadosPorNaturezaResponse(int Codigo, string Natureza, int Total);

public sealed class IndicadoresSlaResponse
{
    public int TotalChamados { get; init; }
    public int TotalDentroDoPrazo { get; init; }
    public int TotalVencidos { get; init; }
    public decimal PercentualCumprimento { get; init; }
    public int TotalProximosDoVencimento { get; init; }
    public double? MediaHorasResolucao { get; init; }
    public double? MediaHorasPrimeiraResposta { get; init; }
}

public sealed class ProdutividadeAtendenteResponse
{
    public Guid ResponsavelId { get; init; }
    public string ResponsavelNome { get; init; } = string.Empty;
    public int TotalAtendidos { get; init; }
    public int TotalEncerrados { get; init; }
    public int TotalVencidos { get; init; }
    public double? MediaHorasResolucao { get; init; }
}

public sealed class DashboardAdminResponse
{
    public int TotalAbertos { get; init; }
    public int TotalEmAtendimento { get; init; }
    public int TotalAguardandoSolicitante { get; init; }
    public int TotalResolvidosPeriodo { get; init; }
    public int TotalEncerradosPeriodo { get; init; }
    public int TotalVencidos { get; init; }
    public int TotalProximosDoVencimento { get; init; }
    public int TotalSemResponsavel { get; init; }
    public IReadOnlyCollection<IndicadorCardResponse> Cards { get; init; } = [];
    public IReadOnlyCollection<ChamadosPorStatusResponse> ChamadosPorStatus { get; init; } = [];
    public IReadOnlyCollection<ChamadosPorPrioridadeResponse> ChamadosPorPrioridade { get; init; } = [];
    public IReadOnlyCollection<ChamadosPorCategoriaResponse> ChamadosPorCategoria { get; init; } = [];
    public IReadOnlyCollection<ChamadosPorNaturezaResponse> ChamadosPorNatureza { get; init; } = [];
    public IndicadoresSlaResponse IndicadoresSla { get; init; } = new();
    public IReadOnlyCollection<ProdutividadeAtendenteResponse> ProdutividadePorAtendente { get; init; } = [];
}
