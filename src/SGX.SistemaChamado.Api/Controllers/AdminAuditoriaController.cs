using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/auditoria")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminAuditoriaController(
    IListarEventosAuditoriaUseCase listarEventosAuditoriaUseCase,
    IObterEventoAuditoriaUseCase obterEventoAuditoriaUseCase,
    IObterDashboardAuditoriaUseCase obterDashboardAuditoriaUseCase,
    IValidator<FiltroEventosAuditoriaRequest> filtroEventosValidator,
    IValidator<FiltroDashboardAuditoriaRequest> filtroDashboardValidator) : ControllerBase
{
    [HttpGet("eventos")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroEventosAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroEventosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarEventosAuditoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("eventos/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterEventoAuditoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("dashboard")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public async Task<IActionResult> Dashboard([FromQuery] FiltroDashboardAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroDashboardValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => obterDashboardAuditoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    private static async Task<IActionResult?> ValidarAsync<T>(
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (validation.IsValid)
        {
            return null;
        }

        return new BadRequestObjectResult(validation.Errors.Select(e => new
        {
            campo = e.PropertyName,
            mensagem = e.ErrorMessage
        }));
    }

    private async Task<IActionResult> ExecutarAsync<T>(Func<Task<T>> acao)
    {
        try
        {
            var response = await acao();
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
