using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/integracoes/active-directory")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminIntegracoesActiveDirectoryController(
    IConfiguracaoIntegracaoActiveDirectoryService configuracaoIntegracaoActiveDirectoryService) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.IntegracoesActiveDirectoryVisualizar)]
    [ProducesResponseType(typeof(ActiveDirectoryIntegracaoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Obter(CancellationToken cancellationToken)
    {
        var response = await configuracaoIntegracaoActiveDirectoryService.ObterConfiguracaoAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPut]
    [Authorize(Policy = PermissionPolicies.IntegracoesActiveDirectoryGerenciar)]
    [ProducesResponseType(typeof(ActiveDirectoryIntegracaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Atualizar(
        [FromBody] AtualizarActiveDirectoryIntegracaoRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await configuracaoIntegracaoActiveDirectoryService.AtualizarConfiguracaoAsync(request, cancellationToken);
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

    [HttpPost("testar-conexao")]
    [Authorize(Policy = PermissionPolicies.IntegracoesActiveDirectoryGerenciar)]
    [ProducesResponseType(typeof(TestarConexaoActiveDirectoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestarConexao(
        [FromBody] TestarConexaoActiveDirectoryRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await configuracaoIntegracaoActiveDirectoryService.TestarConexaoAsync(request, cancellationToken);
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

    [HttpPost("testar-autenticacao")]
    [Authorize(Policy = PermissionPolicies.IntegracoesActiveDirectoryGerenciar)]
    [ProducesResponseType(typeof(TestarAutenticacaoActiveDirectoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TestarAutenticacao(
        [FromBody] TestarAutenticacaoActiveDirectoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await configuracaoIntegracaoActiveDirectoryService.TestarAutenticacaoAsync(request, cancellationToken);
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
