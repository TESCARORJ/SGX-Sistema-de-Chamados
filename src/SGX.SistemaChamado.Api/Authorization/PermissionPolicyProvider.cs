using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PermissionPolicies.Prefixo, StringComparison.OrdinalIgnoreCase))
        {
            var codigoPermissao = policyName[PermissionPolicies.Prefixo.Length..];
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(codigoPermissao))
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return base.GetPolicyAsync(policyName);
    }
}
