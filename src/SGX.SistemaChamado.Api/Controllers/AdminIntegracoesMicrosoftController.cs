using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/integracoes/microsoft-entra-id")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminIntegracoesMicrosoftController(
    IConfiguracaoIntegracaoMicrosoftService configuracaoIntegracaoMicrosoftService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.IntegracoesMicrosoftVisualizar)]
    [ProducesResponseType(typeof(MicrosoftEntraIdIntegracaoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter(CancellationToken cancellationToken)
    {
        var response = await configuracaoIntegracaoMicrosoftService.ObterConfiguracaoAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    [Authorize(Policy = PermissionPolicies.IntegracoesMicrosoftGerenciar)]
    [ProducesResponseType(typeof(MicrosoftEntraIdIntegracaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarMicrosoftEntraIdIntegracaoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await configuracaoIntegracaoMicrosoftService.AtualizarConfiguracaoAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}

