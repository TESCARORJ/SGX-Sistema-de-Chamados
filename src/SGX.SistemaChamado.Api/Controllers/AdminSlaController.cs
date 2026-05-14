using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/sla")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminSlaController(
    IObterConfiguracaoAlertaSlaUseCase obterConfiguracaoAlertaSlaUseCase,
    IAtualizarConfiguracaoAlertaSlaUseCase atualizarConfiguracaoAlertaSlaUseCase,
    IObterDashboardSlaUseCase obterDashboardSlaUseCase,
    IListarRelatorioSlaUseCase listarRelatorioSlaUseCase) : ControllerBase
{
    [HttpGet("alert-config")]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> ObterConfiguracaoAlertas(CancellationToken cancellationToken)
        => ExecutarAsync(() => obterConfiguracaoAlertaSlaUseCase.ExecutarAsync(cancellationToken));

    [HttpPut("alert-config")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> AtualizarConfiguracaoAlertas(
        [FromBody] AtualizarConfiguracaoAlertaSlaRequest request,
        CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarConfiguracaoAlertaSlaUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("dashboard")]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> ObterDashboard([FromQuery] FiltroDashboardSlaRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterDashboardSlaUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("report")]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> ListarRelatorio([FromQuery] FiltroDashboardSlaRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarRelatorioSlaUseCase.ExecutarAsync(request, cancellationToken));

    private static async Task<IActionResult> ExecutarAsync<T>(Func<Task<T>> action)
    {
        try
        {
            var response = await action();
            return new OkObjectResult(response);
        }
        catch (UnauthorizedAccessException)
        {
            return new ForbidResult();
        }
        catch (KeyNotFoundException ex)
        {
            return new NotFoundObjectResult(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return new BadRequestObjectResult(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return new BadRequestObjectResult(new { mensagem = ex.Message });
        }
    }
}
