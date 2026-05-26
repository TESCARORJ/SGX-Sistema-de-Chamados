using SGX.SistemaChamado.Api.Authorization;

namespace SGX.SistemaChamado.Api.Contracts.Admin;

public sealed record MetodosLoginAdminResponse(
    IReadOnlyCollection<MetodoLoginAdminDto> Provedores);

public sealed record MetodoLoginAdminDto(
    string Codigo,
    string Nome,
    string Descricao,
    bool Configurado,
    bool Habilitado,
    bool Principal,
    int Ordem,
    bool PermiteAutoProvisionamento,
    string PerfilPadraoAutoProvisionamento,
    string RotuloExibicao,
    bool Funcional,
    bool PodeHabilitar,
    string? MotivoBloqueioHabilitar,
    bool PodeDesabilitar,
    string? MotivoBloqueioDesabilitar);

public sealed class AtualizarMetodosLoginAdminRequest
{
    public IReadOnlyCollection<MetodoLoginAdminAtualizacaoDto> Provedores { get; init; } = [];
}

public sealed class MetodoLoginAdminAtualizacaoDto
{
    public string Codigo { get; init; } = string.Empty;
    public bool Habilitado { get; init; }
    public bool Principal { get; init; }
    public int Ordem { get; init; }
    public bool PermiteAutoProvisionamento { get; init; }
    public string PerfilPadraoAutoProvisionamento { get; init; } = PerfisInternos.Solicitante;
    public string RotuloExibicao { get; init; } = string.Empty;
}
