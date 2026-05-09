using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/integracoes/email/logs")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminIntegracoesEmailController(
    IListarLogsIntegracaoEmailUseCase listarLogsIntegracaoEmailUseCase,
    IObterLogIntegracaoEmailUseCase obterLogIntegracaoEmailUseCase,
    IValidator<FiltroLogsEmailRequest> filtroValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.IntegracoesEmailVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroLogsEmailRequest request, CancellationToken cancellationToken)
    {
        var validation = await filtroValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await listarLogsIntegracaoEmailUseCase.ExecutarAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.IntegracoesEmailVisualizar)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await obterLogIntegracaoEmailUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
