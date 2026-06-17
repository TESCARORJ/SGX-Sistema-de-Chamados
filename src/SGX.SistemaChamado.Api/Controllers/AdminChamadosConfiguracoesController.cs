using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/chamados/configuracoes")]
[Authorize(Policy = Policies.Administrador)]
public sealed class AdminChamadosConfiguracoesController(
    IObterConfiguracaoAutoFechamentoChamadoUseCase obterConfiguracaoAutoFechamentoChamadoUseCase,
    IAtualizarConfiguracaoAutoFechamentoChamadoUseCase atualizarConfiguracaoAutoFechamentoChamadoUseCase,
    IValidator<AtualizarConfiguracaoAutoFechamentoChamadoRequest> atualizarValidator) : ControllerBase
{
    [HttpGet("auto-fechamento")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public Task<IActionResult> ObterAutoFechamento(CancellationToken cancellationToken)
        => ExecutarAsync(() => obterConfiguracaoAutoFechamentoChamadoUseCase.ExecutarAsync(cancellationToken));

    [HttpPut("auto-fechamento")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public async Task<IActionResult> AtualizarAutoFechamento(
        [FromBody] AtualizarConfiguracaoAutoFechamentoChamadoRequest request,
        CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarConfiguracaoAutoFechamentoChamadoUseCase.ExecutarAsync(request, cancellationToken));
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
