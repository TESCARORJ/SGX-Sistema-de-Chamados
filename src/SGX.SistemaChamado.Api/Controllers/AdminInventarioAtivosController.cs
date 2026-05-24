using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/inventario-ativos")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminInventarioAtivosController(
    IAdminInventarioAtivosUseCases adminInventarioAtivosUseCases,
    IValidator<FiltroInventarioAtivoRequest> filtroValidator,
    IValidator<FiltroHistoricoInventarioAtivoRequest> filtroHistoricoValidator,
    IValidator<FiltroChamadosRelacionadosInventarioAtivoRequest> filtroChamadosValidator,
    IValidator<CriarInventarioAtivoRequest> criarValidator,
    IValidator<AtualizarInventarioAtivoRequest> atualizarValidator,
    IValidator<MovimentarInventarioAtivoRequest> movimentarValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.ListarAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInventarioAtivosUseCases.ObterPorIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.CriarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.AtualizarAsync(id, request, cancellationToken));
    }

    [HttpGet("{id:guid}/historico")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVisualizar)]
    public async Task<IActionResult> ListarHistorico(Guid id, [FromQuery] FiltroHistoricoInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroHistoricoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.ListarHistoricoAsync(id, request, cancellationToken));
    }

    [HttpGet("{id:guid}/chamados")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVisualizar)]
    public async Task<IActionResult> ListarChamadosRelacionados(Guid id, [FromQuery] FiltroChamadosRelacionadosInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroChamadosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.ListarChamadosAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/movimentar")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosMovimentar)]
    public async Task<IActionResult> Movimentar(Guid id, [FromBody] MovimentarInventarioAtivoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(movimentarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminInventarioAtivosUseCases.MovimentarAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/inativar")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosInativar)]
    public Task<IActionResult> Inativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInventarioAtivosUseCases.InativarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reativar")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosInativar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInventarioAtivosUseCases.ReativarAsync(id, cancellationToken));

    [HttpGet("tipos")]
    [Authorize(Policy = PermissionPolicies.InventarioAtivosVisualizar)]
    public Task<IActionResult> ListarTipos(CancellationToken cancellationToken)
        => ExecutarAsync(() => adminInventarioAtivosUseCases.ListarTiposAtivoAsync(cancellationToken));

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
