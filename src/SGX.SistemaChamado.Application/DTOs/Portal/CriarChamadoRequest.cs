namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class CriarChamadoRequest
{
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public Guid? DepartamentoId { get; set; }
    public Guid CategoriaId { get; set; }
    public Guid PrioridadeId { get; set; }
}
