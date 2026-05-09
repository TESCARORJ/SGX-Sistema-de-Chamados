using Microsoft.AspNetCore.Authorization;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PermissionRequirement(string codigoPermissao) : IAuthorizationRequirement
{
    public string CodigoPermissao { get; } = string.IsNullOrWhiteSpace(codigoPermissao)
        ? string.Empty
        : codigoPermissao.Trim();
}
