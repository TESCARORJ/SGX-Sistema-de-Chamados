using Microsoft.AspNetCore.Authorization;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Authorization;

public sealed class PerfilRequirementHandler(IUsuarioAtualService usuarioAtualService)
    : AuthorizationHandler<PerfilRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PerfilRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
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

        if (requirement.PerfisAceitos.Any(usuarioAtual.PossuiPerfil))
        {
            context.Succeed(requirement);
        }
    }
}
