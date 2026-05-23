using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/base-conhecimento/artigos")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminBaseConhecimentoController(
    IListarArtigosBaseConhecimentoUseCase listarArtigosUseCase,
    IObterArtigoBaseConhecimentoUseCase obterArtigoUseCase,
    ICriarArtigoBaseConhecimentoUseCase criarArtigoUseCase,
    IAtualizarArtigoBaseConhecimentoUseCase atualizarArtigoUseCase,
    IPublicarArtigoBaseConhecimentoUseCase publicarArtigoUseCase,
    IArquivarArtigoBaseConhecimentoUseCase arquivarArtigoUseCase,
    IReativarArtigoBaseConhecimentoUseCase reativarArtigoUseCase,
    IValidator<FiltroBaseConhecimentoArtigoRequest> filtroValidator,
    IValidator<CriarBaseConhecimentoArtigoRequest> criarValidator,
    IValidator<AtualizarBaseConhecimentoArtigoRequest> atualizarValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarArtigosUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterArtigoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarArtigoUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarBaseConhecimentoArtigoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarArtigoUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/publicar")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoPublicar)]
    public Task<IActionResult> Publicar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => publicarArtigoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/arquivar")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoArquivar)]
    public Task<IActionResult> Arquivar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => arquivarArtigoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reativar")]
    [Authorize(Policy = PermissionPolicies.BaseConhecimentoArquivar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarArtigoUseCase.ExecutarAsync(id, cancellationToken));

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