using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Chamados;

public sealed record ErroCampoObrigatorioChamado(string Campo, string Mensagem);

public sealed class CamposObrigatoriosChamadoInput
{
    public NaturezaChamadoEnum? NaturezaChamado { get; init; }
    public ImpactoChamadoEnum? ImpactoChamado { get; init; }
    public UrgenciaChamadoEnum? UrgenciaChamado { get; init; }
    public string? Titulo { get; init; }
    public string? Descricao { get; init; }
    public Guid? CategoriaId { get; init; }
    public Guid? TipoSolicitacaoId { get; init; }
    public Guid? CatalogoServicoId { get; init; }
    public string? CatalogoServicoSlug { get; init; }
    public string Origem { get; init; } = "Portal";
}
