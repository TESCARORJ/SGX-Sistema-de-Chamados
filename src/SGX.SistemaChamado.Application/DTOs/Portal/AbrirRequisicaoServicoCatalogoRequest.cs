namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class AbrirRequisicaoServicoCatalogoRequest
{
    public Guid CatalogoServicoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public List<RespostaFormularioAberturaRequest>? RespostasFormulario { get; set; }
}
