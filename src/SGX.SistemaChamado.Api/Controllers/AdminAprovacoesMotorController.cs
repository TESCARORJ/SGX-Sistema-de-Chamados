using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/aprovacoes-motor")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminAprovacoesMotorController(
    IAprovarAprovacaoChamadoUseCase aprovarAprovacaoChamadoUseCase,
    IReprovarAprovacaoChamadoUseCase reprovarAprovacaoChamadoUseCase,
    IAdminInstanciaAprovacaoChamadoUseCases adminInstanciaAprovacaoChamadoUseCases,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : ControllerBase
{
    [HttpGet("pendencias")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> ListarPendencias([FromQuery] ListarInstanciasAprovacaoChamadoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ListarAsync(request, cancellationToken));

    [HttpGet("pendencias/minhas")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public async Task<IActionResult> ListarMinhasPendencias(CancellationToken cancellationToken)
    {
        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        return await ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ListarPendentesAsync(usuario.Id, cancellationToken));
    }

    [HttpGet("pendencias/aprovador/{usuarioId:guid}")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> ListarPendenciasPorAprovador([FromRoute] Guid usuarioId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ListarPendentesAsync(usuarioId, cancellationToken));

    [HttpGet("chamados/{chamadoId:guid}/pendencias")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> ListarPendenciasPorChamado([FromRoute] Guid chamadoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ListarPorChamadoAsync(chamadoId, cancellationToken));

    [HttpGet("instancias/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> ObterInstancia([FromRoute] Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ObterPorIdAsync(id, cancellationToken));

    [HttpGet("status/{status}")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> ListarPendenciasPorStatus([FromRoute] StatusInstanciaAprovacaoChamado status, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInstanciaAprovacaoChamadoUseCases.ListarPorStatusAsync(status, cancellationToken));

    [HttpPost("aprovar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosAprovar)]
    public Task<IActionResult> Aprovar([FromBody] AprovarAprovacaoChamadoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => aprovarAprovacaoChamadoUseCase.ExecutarAsync(request, cancellationToken));

    [HttpPost("reprovar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosReprovar)]
    public Task<IActionResult> Reprovar([FromBody] ReprovarAprovacaoChamadoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => reprovarAprovacaoChamadoUseCase.ExecutarAsync(request, cancellationToken));

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
