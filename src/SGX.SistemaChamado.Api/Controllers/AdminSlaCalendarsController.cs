using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/sla/calendars")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminSlaCalendarsController(
    IListarCalendariosCorporativosUseCase listarCalendariosCorporativosUseCase,
    IObterCalendarioCorporativoUseCase obterCalendarioCorporativoUseCase,
    ICriarCalendarioCorporativoUseCase criarCalendarioCorporativoUseCase,
    IAtualizarCalendarioCorporativoUseCase atualizarCalendarioCorporativoUseCase,
    IAtualizarStatusCalendarioCorporativoUseCase atualizarStatusCalendarioCorporativoUseCase,
    IDefinirCalendarioCorporativoPadraoUseCase definirCalendarioCorporativoPadraoUseCase,
    ICriarHorarioAtendimentoCalendarioUseCase criarHorarioAtendimentoCalendarioUseCase,
    IAtualizarHorarioAtendimentoCalendarioUseCase atualizarHorarioAtendimentoCalendarioUseCase,
    IExcluirHorarioAtendimentoCalendarioUseCase excluirHorarioAtendimentoCalendarioUseCase,
    ICriarExcecaoCalendarioCorporativoUseCase criarExcecaoCalendarioCorporativoUseCase,
    IAtualizarExcecaoCalendarioCorporativoUseCase atualizarExcecaoCalendarioCorporativoUseCase,
    IExcluirExcecaoCalendarioCorporativoUseCase excluirExcecaoCalendarioCorporativoUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> Listar(CancellationToken cancellationToken)
        => ExecutarAsync(() => listarCalendariosCorporativosUseCase.ExecutarAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.SlaVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterCalendarioCorporativoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaCriar)]
    public Task<IActionResult> Criar([FromBody] CriarCalendarioCorporativoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => criarCalendarioCorporativoUseCase.ExecutarAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarCalendarioCorporativoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarCalendarioCorporativoUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaAtivarDesativar)]
    public Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AtualizarStatusCalendarioCorporativoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarStatusCalendarioCorporativoUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpPatch("{id:guid}/default")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> DefinirPadrao(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => definirCalendarioCorporativoPadraoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/schedules")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> CriarHorario(Guid id, [FromBody] HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => criarHorarioAtendimentoCalendarioUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpPut("{id:guid}/schedules/{scheduleId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> AtualizarHorario(Guid id, Guid scheduleId, [FromBody] HorarioAtendimentoCalendarioRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarHorarioAtendimentoCalendarioUseCase.ExecutarAsync(id, scheduleId, request, cancellationToken));

    [HttpDelete("{id:guid}/schedules/{scheduleId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> ExcluirHorario(Guid id, Guid scheduleId, CancellationToken cancellationToken)
        => ExecutarAsync(() => excluirHorarioAtendimentoCalendarioUseCase.ExecutarAsync(id, scheduleId, cancellationToken));

    [HttpPost("{id:guid}/exceptions")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> CriarExcecao(Guid id, [FromBody] ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => criarExcecaoCalendarioCorporativoUseCase.ExecutarAsync(id, request, cancellationToken));

    [HttpPut("{id:guid}/exceptions/{exceptionId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> AtualizarExcecao(Guid id, Guid exceptionId, [FromBody] ExcecaoCalendarioCorporativoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => atualizarExcecaoCalendarioCorporativoUseCase.ExecutarAsync(id, exceptionId, request, cancellationToken));

    [HttpDelete("{id:guid}/exceptions/{exceptionId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.SlaEditar)]
    public Task<IActionResult> ExcluirExcecao(Guid id, Guid exceptionId, CancellationToken cancellationToken)
        => ExecutarAsync(() => excluirExcecaoCalendarioCorporativoUseCase.ExecutarAsync(id, exceptionId, cancellationToken));

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
