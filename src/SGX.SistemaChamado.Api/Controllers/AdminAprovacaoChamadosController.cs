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
public sealed class AdminAprovacaoChamadosController(
    IAdminAprovacaoChamadosUseCases adminAprovacaoChamadosUseCases,
    IValidator<FiltroAprovacaoChamadoRequest> filtroValidator,
    IValidator<SolicitarAprovacaoChamadoRequest> solicitarValidator,
    IValidator<DecidirAprovacaoChamadoRequest> decidirValidator,
    IValidator<CancelarAprovacaoChamadoRequest> cancelarValidator) : ControllerBase
{
    [HttpGet("aprovacao-chamados")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroAprovacaoChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminAprovacaoChamadosUseCases.ListarAsync(request, cancellationToken));
    }

    [HttpGet("aprovacao-chamados/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminAprovacaoChamadosUseCases.ObterPorIdAsync(id, cancellationToken));

    [HttpPost("chamados/{chamadoId:guid}/aprovacao/solicitar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosGerenciar)]
    public async Task<IActionResult> Solicitar(Guid chamadoId, [FromBody] SolicitarAprovacaoChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(solicitarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminAprovacaoChamadosUseCases.SolicitarAsync(chamadoId, request, cancellationToken));
    }

    [HttpPost("aprovacao-chamados/{id:guid}/aprovar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosAprovar)]
    public async Task<IActionResult> Aprovar(Guid id, [FromBody] DecidirAprovacaoChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(decidirValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminAprovacaoChamadosUseCases.AprovarAsync(id, request, cancellationToken));
    }

    [HttpPost("aprovacao-chamados/{id:guid}/reprovar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosReprovar)]
    public async Task<IActionResult> Reprovar(Guid id, [FromBody] DecidirAprovacaoChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(decidirValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminAprovacaoChamadosUseCases.ReprovarAsync(id, request, cancellationToken));
    }

    [HttpPost("aprovacao-chamados/{id:guid}/cancelar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosCancelar)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] CancelarAprovacaoChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(cancelarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminAprovacaoChamadosUseCases.CancelarAsync(id, request, cancellationToken));
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

        return new BadRequestObjectResult(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
    }

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
