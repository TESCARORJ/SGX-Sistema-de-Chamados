namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class RespostaFormularioAberturaRequest
{
    public Guid CampoFormularioServicoId { get; set; }
    public string? Valor { get; set; }
    public List<string>? Valores { get; set; }
}
