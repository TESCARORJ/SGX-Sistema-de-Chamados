using Microsoft.AspNetCore.Authorization;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PerfilRequirement(params string[] perfisAceitos) : IAuthorizationRequirement
{
    public IReadOnlyCollection<string> PerfisAceitos { get; } = perfisAceitos
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Select(x => x.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();
}
