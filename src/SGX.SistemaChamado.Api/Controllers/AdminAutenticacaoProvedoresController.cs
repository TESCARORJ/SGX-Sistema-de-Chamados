using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/autenticacao/provedores")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminAutenticacaoProvedoresController(
    IMetodosLoginAdminService metodosLoginAdminService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.AutenticacaoProvedoresVisualizar)]
    [ProducesResponseType(typeof(MetodosLoginAdminResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter(CancellationToken cancellationToken)
    {
        var response = await metodosLoginAdminService.ObterConfiguracaoAdminAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    [Authorize(Policy = PermissionPolicies.AutenticacaoProvedoresGerenciar)]
    [ProducesResponseType(typeof(MetodosLoginAdminResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarMetodosLoginAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await metodosLoginAdminService.AtualizarConfiguracaoAdminAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
