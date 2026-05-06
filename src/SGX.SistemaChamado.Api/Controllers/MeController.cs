using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Contracts;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/me")]
[Authorize]
public sealed class MeController(IUsuarioAtualService usuarioAtualService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var usuario = await usuarioAtualService.ObterAsync(cancellationToken);

        return Ok(new MeResponse(
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.Login,
            usuario.Situacao,
            usuario.Perfis,
            usuario.Permissoes,
            usuario.DepartamentoId,
            usuario.AutenticadoPor));
    }
}
