namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class FiltroChamadosPortalRequest
{
    public Guid? StatusId { get; set; }
    public Guid? PrioridadeId { get; set; }
    public Guid? CategoriaId { get; set; }
    public DateTime? DataInicial { get; set; }
    public DateTime? DataFinal { get; set; }
    public string? Texto { get; set; }
    public bool VisaoAmpliada { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}
