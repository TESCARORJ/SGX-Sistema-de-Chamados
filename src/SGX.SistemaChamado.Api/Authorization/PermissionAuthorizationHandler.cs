using Microsoft.AspNetCore.Authorization;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PermissionAuthorizationHandler(IUsuarioAtualService usuarioAtualService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true || string.IsNullOrWhiteSpace(requirement.CodigoPermissao))
        {
            return;
        }

        UsuarioAutenticadoContexto usuarioAtual;
        try
        {
            usuarioAtual = await usuarioAtualService.ObterAsync();
        }
        catch
        {
            return;
        }

        if (usuarioAtual.PossuiPerfil(PerfisInternos.Administrador))
        {
            context.Succeed(requirement);
            return;
        }

        if (usuarioAtual.Permissoes.Contains(requirement.CodigoPermissao, StringComparer.OrdinalIgnoreCase))
        {
            context.Succeed(requirement);
        }
    }
}
