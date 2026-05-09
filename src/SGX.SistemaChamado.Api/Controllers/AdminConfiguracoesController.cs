using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/configuracoes")]
[Authorize(Policy = Policies.Administrador)]
public sealed class AdminConfiguracoesController(
    IListarParametrosSistemaUseCase listarParametrosUseCase,
    IObterParametroSistemaUseCase obterParametroUseCase,
    ICriarParametroSistemaUseCase criarParametroUseCase,
    IAtualizarParametroSistemaUseCase atualizarParametroUseCase,
    IInativarParametroSistemaUseCase inativarParametroUseCase,
    IReativarParametroSistemaUseCase reativarParametroUseCase,
    IValidator<FiltroCadastroRequest> filtroValidator,
    IValidator<CriarParametroSistemaRequest> criarParametroValidator,
    IValidator<AtualizarParametroSistemaRequest> atualizarParametroValidator) : ControllerBase
{
    [HttpGet("parametros")]
    public async Task<IActionResult> Listar([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarParametrosUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("parametros/{id:guid}")]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterParametroUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("parametros")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarParametroSistemaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarParametroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarParametroUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("parametros/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarParametroSistemaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarParametroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarParametroUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("parametros/{id:guid}/inativar")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public Task<IActionResult> Inativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarParametroUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("parametros/{id:guid}/reativar")]
    [Authorize(Policy = PermissionPolicies.ParametrosGerenciar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarParametroUseCase.ExecutarAsync(id, cancellationToken));

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
