using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/catalogo-servicos")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminCatalogoServicosController(
    IAdminCatalogoServicosUseCases adminCatalogoServicosUseCases,
    IValidator<FiltroCatalogoServicoRequest> filtroValidator,
    IValidator<CriarCatalogoServicoRequest> criarValidator,
    IValidator<AtualizarCatalogoServicoRequest> atualizarValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroCatalogoServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminCatalogoServicosUseCases.ListarAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminCatalogoServicosUseCases.ObterPorIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarCatalogoServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminCatalogoServicosUseCases.CriarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCatalogoServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminCatalogoServicosUseCases.AtualizarAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/publicar")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosPublicar)]
    public Task<IActionResult> Publicar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminCatalogoServicosUseCases.PublicarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/arquivar")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> Arquivar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminCatalogoServicosUseCases.ArquivarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reativar")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminCatalogoServicosUseCases.ReativarAsync(id, cancellationToken));

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
