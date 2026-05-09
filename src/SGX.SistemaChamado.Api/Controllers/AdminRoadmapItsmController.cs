using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/roadmap")]
[Route("api/admin/roadmap-itsm")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminRoadmapItsmController(
    IListarRoadmapItsmUseCase listarRoadmapItsmUseCase,
    IObterRoadmapItsmItemUseCase obterRoadmapItsmItemUseCase,
    ICriarRoadmapItsmItemUseCase criarRoadmapItsmItemUseCase,
    IAtualizarRoadmapItsmItemUseCase atualizarRoadmapItsmItemUseCase,
    IAtualizarStatusRoadmapItsmUseCase atualizarStatusRoadmapItsmUseCase,
    IInativarRoadmapItsmItemUseCase inativarRoadmapItsmItemUseCase,
    IReativarRoadmapItsmItemUseCase reativarRoadmapItsmItemUseCase,
    IListarRoadmapImplementacoesFuturasUseCase listarRoadmapImplementacoesFuturasUseCase,
    IListarRoadmapImplementacoesPorItemUseCase listarRoadmapImplementacoesPorItemUseCase,
    IObterRoadmapImplementacaoFuturaUseCase obterRoadmapImplementacaoFuturaUseCase,
    ICriarRoadmapImplementacaoFuturaUseCase criarRoadmapImplementacaoFuturaUseCase,
    IAtualizarRoadmapImplementacaoFuturaUseCase atualizarRoadmapImplementacaoFuturaUseCase,
    IConcluirRoadmapImplementacaoFuturaUseCase concluirRoadmapImplementacaoFuturaUseCase,
    IInativarRoadmapImplementacaoFuturaUseCase inativarRoadmapImplementacaoFuturaUseCase,
    IReativarRoadmapImplementacaoFuturaUseCase reativarRoadmapImplementacaoFuturaUseCase,
    IListarRoadmapCategoriasUseCase listarRoadmapCategoriasUseCase,
    IObterRoadmapCategoriaUseCase obterRoadmapCategoriaUseCase,
    ICriarRoadmapCategoriaUseCase criarRoadmapCategoriaUseCase,
    IAtualizarRoadmapCategoriaUseCase atualizarRoadmapCategoriaUseCase,
    IInativarRoadmapCategoriaUseCase inativarRoadmapCategoriaUseCase,
    IReativarRoadmapCategoriaUseCase reativarRoadmapCategoriaUseCase,
    IListarRoadmapChecklistPorItemUseCase listarRoadmapChecklistPorItemUseCase,
    ICriarRoadmapChecklistItemUseCase criarRoadmapChecklistItemUseCase,
    IAtualizarRoadmapChecklistItemUseCase atualizarRoadmapChecklistItemUseCase,
    IConcluirRoadmapChecklistItemUseCase concluirRoadmapChecklistItemUseCase,
    IReabrirRoadmapChecklistItemUseCase reabrirRoadmapChecklistItemUseCase,
    IInativarRoadmapChecklistItemUseCase inativarRoadmapChecklistItemUseCase,
    IExcluirRoadmapChecklistItemUseCase excluirRoadmapChecklistItemUseCase,
    IReativarRoadmapChecklistItemUseCase reativarRoadmapChecklistItemUseCase,
    IValidator<FiltroRoadmapItsmRequest> filtroValidator,
    IValidator<CriarRoadmapItsmItemRequest> criarValidator,
    IValidator<AtualizarRoadmapItsmItemRequest> atualizarValidator,
    IValidator<AtualizarStatusRoadmapItsmRequest> atualizarStatusValidator,
    IValidator<FiltroRoadmapImplementacaoFuturaRequest> filtroImplementacoesValidator,
    IValidator<CriarRoadmapImplementacaoFuturaRequest> criarImplementacaoValidator,
    IValidator<AtualizarRoadmapImplementacaoFuturaRequest> atualizarImplementacaoValidator,
    IValidator<FiltroRoadmapCategoriaRequest> filtroCategoriasValidator,
    IValidator<CriarRoadmapCategoriaRequest> criarCategoriaValidator,
    IValidator<AtualizarRoadmapCategoriaRequest> atualizarCategoriaValidator,
    IValidator<CriarRoadmapChecklistItemRequest> criarChecklistValidator,
    IValidator<AtualizarRoadmapChecklistItemRequest> atualizarChecklistValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.RoadmapVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroRoadmapItsmRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarRoadmapItsmUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.RoadmapVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterRoadmapItsmItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarRoadmapItsmItemRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarRoadmapItsmItemUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarRoadmapItsmItemRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarRoadmapItsmItemUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AtualizarStatusRoadmapItsmRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarStatusValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarStatusRoadmapItsmUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> Inativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarRoadmapItsmItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarRoadmapItsmItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("categorias")]
    [Authorize(Policy = PermissionPolicies.RoadmapVisualizar)]
    public async Task<IActionResult> ListarCategorias([FromQuery] FiltroRoadmapCategoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroCategoriasValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarRoadmapCategoriasUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("categorias/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.RoadmapVisualizar)]
    public Task<IActionResult> ObterCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterRoadmapCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("categorias")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> CriarCategoria([FromBody] CriarRoadmapCategoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarCategoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarRoadmapCategoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("categorias/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> AtualizarCategoria(Guid id, [FromBody] AtualizarRoadmapCategoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarCategoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarRoadmapCategoriaUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("categorias/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> InativarCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarRoadmapCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("categorias/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> ReativarCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarRoadmapCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("{roadmapItemId:guid}/checklist")]
    [Authorize(Policy = PermissionPolicies.RoadmapVisualizar)]
    public Task<IActionResult> ListarChecklist(Guid roadmapItemId, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarRoadmapChecklistPorItemUseCase.ExecutarAsync(roadmapItemId, cancellationToken));

    [HttpPost("{roadmapItemId:guid}/checklist")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> CriarChecklist(Guid roadmapItemId, [FromBody] CriarRoadmapChecklistItemRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarChecklistValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarRoadmapChecklistItemUseCase.ExecutarAsync(roadmapItemId, request, cancellationToken));
    }

    [HttpPut("checklist/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public async Task<IActionResult> AtualizarChecklist(Guid id, [FromBody] AtualizarRoadmapChecklistItemRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarChecklistValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarRoadmapChecklistItemUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("checklist/{id:guid}/concluir")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> ConcluirChecklist(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => concluirRoadmapChecklistItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("checklist/{id:guid}/reabrir")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> ReabrirChecklist(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reabrirRoadmapChecklistItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("checklist/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> InativarChecklist(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarRoadmapChecklistItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpDelete("checklist/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> ExcluirChecklist(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => excluirRoadmapChecklistItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("checklist/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapGerenciar)]
    public Task<IActionResult> ReativarChecklist(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarRoadmapChecklistItemUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("implementacoes")]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesVisualizar)]
    public async Task<IActionResult> ListarImplementacoes(
        [FromQuery] FiltroRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroImplementacoesValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarRoadmapImplementacoesFuturasUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("{roadmapItemId:guid}/implementacoes")]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesVisualizar)]
    public Task<IActionResult> ListarImplementacoesPorItem(Guid roadmapItemId, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarRoadmapImplementacoesPorItemUseCase.ExecutarAsync(roadmapItemId, cancellationToken));

    [HttpGet("implementacoes/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesVisualizar)]
    public Task<IActionResult> ObterImplementacao(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterRoadmapImplementacaoFuturaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("implementacoes")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesGerenciar)]
    public async Task<IActionResult> CriarImplementacao([FromBody] CriarRoadmapImplementacaoFuturaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarImplementacaoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarRoadmapImplementacaoFuturaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("implementacoes/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesGerenciar)]
    public async Task<IActionResult> AtualizarImplementacao(
        Guid id,
        [FromBody] AtualizarRoadmapImplementacaoFuturaRequest request,
        CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarImplementacaoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarRoadmapImplementacaoFuturaUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("implementacoes/{id:guid}/concluir")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesGerenciar)]
    public Task<IActionResult> ConcluirImplementacao(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => concluirRoadmapImplementacaoFuturaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("implementacoes/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesGerenciar)]
    public Task<IActionResult> InativarImplementacao(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarRoadmapImplementacaoFuturaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("implementacoes/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.RoadmapImplementacoesGerenciar)]
    public Task<IActionResult> ReativarImplementacao(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarRoadmapImplementacaoFuturaUseCase.ExecutarAsync(id, cancellationToken));

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
