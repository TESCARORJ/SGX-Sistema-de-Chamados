using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/sla/policies")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminSlaPoliciesController(
    IListarPoliticasSlaUseCase listarPoliticasSlaUseCase,
    IObterPoliticaSlaUseCase obterPoliticaSlaUseCase,
    ICriarPoliticaSlaUseCase criarPoliticaSlaUseCase,
    IAtualizarPoliticaSlaUseCase atualizarPoliticaSlaUseCase,
    IAtualizarStatusPoliticaSlaUseCase atualizarStatusPoliticaSlaUseCase,
    IInativarPoliticaSlaUseCase inativarPoliticaSlaUseCase,
    IValidator<FiltroPoliticaSlaRequest> filtroValidator,
    IValidator<CriarPoliticaSlaRequest> criarValidator,
    IValidator<AtualizarPoliticaSlaRequest> atualizarValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroPoliticaSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarPoliticasSlaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterPoliticaSlaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaCriar)]
    public async Task<IActionResult> Criar([FromBody] CriarPoliticaSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarPoliticaSlaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPoliticaSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarPoliticaSlaUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaAtivarDesativar)]
    public Task<IActionResult> AtualizarStatus(
        Guid id,
        [FromBody] AtualizarStatusPoliticaSlaRequest request,
        CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarStatusPoliticaSlaUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaExcluir)]
    public Task<IActionResult> Inativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarPoliticaSlaUseCase.ExecutarAsync(id, cancellationToken));

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
