using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        try
        {
            var validation = await filtroValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
            }

            var response = await listarLogsIntegracaoEmailUseCase.ExecutarAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || HttpContext.RequestAborted.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest, new { mensagem = "Requisicao cancelada pelo cliente." });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Nao foi possivel carregar os logs de integracao de e-mail." });
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
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || HttpContext.RequestAborted.IsCancellationRequested)
        {
            return StatusCode(StatusCodes.Status499ClientClosedRequest, new { mensagem = "Requisicao cancelada pelo cliente." });
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { mensagem = "Nao foi possivel carregar o detalhe do log de integracao de e-mail." });
        }
    }
}
