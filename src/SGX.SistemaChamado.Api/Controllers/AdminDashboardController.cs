using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminDashboardController(
    IObterDashboardAdminUseCase obterDashboardAdminUseCase,
    IObterIndicadoresChamadosPorStatusUseCase obterChamadosPorStatusUseCase,
    IObterIndicadoresChamadosPorPrioridadeUseCase obterChamadosPorPrioridadeUseCase,
    IObterIndicadoresChamadosPorCategoriaUseCase obterChamadosPorCategoriaUseCase,
    IObterIndicadoresSlaUseCase obterIndicadoresSlaUseCase,
    IObterIndicadoresProdutividadeUseCase obterProdutividadeUseCase,
    IValidator<FiltroIndicadoresRequest> filtroValidator) : ControllerBase
{
    [HttpGet("dashboard")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterDashboard([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterDashboardAdminUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("indicadores/chamados-por-status")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterChamadosPorStatus([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterChamadosPorStatusUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("indicadores/chamados-por-prioridade")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterChamadosPorPrioridade([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterChamadosPorPrioridadeUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("indicadores/chamados-por-categoria")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterChamadosPorCategoria([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterChamadosPorCategoriaUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("indicadores/sla")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterIndicadoresSla([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterIndicadoresSlaUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("indicadores/produtividade")]
    [Authorize(Policy = PermissionPolicies.DashboardVisualizar)]
    public Task<IActionResult> ObterProdutividade([FromQuery] FiltroIndicadoresRequest request, CancellationToken cancellationToken)
        => ExecutarComValidacaoAsync(request, cancellationToken, () => obterProdutividadeUseCase.ExecutarAsync(request, cancellationToken));

    private async Task<IActionResult> ExecutarComValidacaoAsync<T>(
        FiltroIndicadoresRequest request,
        CancellationToken cancellationToken,
        Func<Task<T>> executor)
    {
        var validation = await filtroValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await executor();
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}
