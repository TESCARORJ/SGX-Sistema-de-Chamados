using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/grupos-tecnicos")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminGruposTecnicosController(
    IListarGruposTecnicosAdminUseCase listarGruposTecnicosUseCase,
    IObterGrupoTecnicoAdminUseCase obterGrupoTecnicoUseCase,
    ICriarGrupoTecnicoAdminUseCase criarGrupoTecnicoUseCase,
    IAtualizarGrupoTecnicoAdminUseCase atualizarGrupoTecnicoUseCase,
    IAtualizarStatusGrupoTecnicoAdminUseCase atualizarStatusGrupoTecnicoUseCase,
    IListarFilasAtendimentoGrupoTecnicoAdminUseCase listarFilasAtendimentoGrupoTecnicoUseCase,
    IListarMembrosGrupoTecnicoAdminUseCase listarMembrosGrupoTecnicoUseCase,
    IAdicionarMembroGrupoTecnicoAdminUseCase adicionarMembroGrupoTecnicoUseCase,
    IAtualizarStatusMembroGrupoTecnicoAdminUseCase atualizarStatusMembroGrupoTecnicoUseCase,
    IListarGruposTecnicosDoUsuarioAdminUseCase listarGruposTecnicosDoUsuarioUseCase) : ControllerBase
{
    [HttpGet]
    public Task<IActionResult> Listar([FromQuery] ListarGruposTecnicosRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarGruposTecnicosUseCase.ExecutarAsync(request, cancellationToken));

    [HttpGet("{id:guid}")]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterGrupoTecnicoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> Criar([FromBody] CriarGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => criarGrupoTecnicoUseCase.ExecutarAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarGrupoTecnicoUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AlterarStatusGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarStatusGrupoTecnicoUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpGet("{grupoTecnicoId:guid}/filas")]
    public Task<IActionResult> ListarFilas(Guid grupoTecnicoId, [FromQuery] ListarFilasAtendimentoGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarFilasAtendimentoGrupoTecnicoUseCase.ExecutarAsync(grupoTecnicoId, request, cancellationToken));

    [HttpGet("{grupoTecnicoId:guid}/membros")]
    public Task<IActionResult> ListarMembros(Guid grupoTecnicoId, [FromQuery] ListarMembrosGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarMembrosGrupoTecnicoUseCase.ExecutarAsync(grupoTecnicoId, request, cancellationToken));

    [HttpPost("{grupoTecnicoId:guid}/membros")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> AdicionarMembro(Guid grupoTecnicoId, [FromBody] AdicionarMembroGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => adicionarMembroGrupoTecnicoUseCase.ExecutarAsync(grupoTecnicoId, request, cancellationToken));

    [HttpPatch("{grupoTecnicoId:guid}/membros/{membroId:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> AtualizarStatusMembro(Guid grupoTecnicoId, Guid membroId, [FromBody] AlterarStatusMembroGrupoTecnicoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarStatusMembroGrupoTecnicoUseCase.ExecutarAsync(membroId, request, cancellationToken));

    [HttpGet("~/api/admin/usuarios/{usuarioId:guid}/grupos-tecnicos")]
    public Task<IActionResult> ListarGruposTecnicosDoUsuario(Guid usuarioId, [FromQuery] bool? ativo, CancellationToken cancellationToken)
        => ExecutarAsync(() => listarGruposTecnicosDoUsuarioUseCase.ExecutarAsync(usuarioId, ativo, cancellationToken));

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
