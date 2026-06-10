using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/regras-aprovacao")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminConfiguracoesRegrasAprovacaoController(
    IAdminConfiguracaoRegraAprovacaoUseCases useCases) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> Listar([FromQuery] ListarConfiguracoesRegrasAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.ListarAsync(request, cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.ObterPorIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosGerenciar)]
    public Task<IActionResult> Criar([FromBody] CriarConfiguracaoRegraAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.CriarAsync(request, cancellationToken));

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosGerenciar)]
    public Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarConfiguracaoRegraAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.AtualizarAsync(id, request, cancellationToken));

    [HttpPatch("{id:guid}/status")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosGerenciar)]
    public Task<IActionResult> AtualizarStatus(
        Guid id,
        [FromBody] AlterarStatusConfiguracaoRegraAprovacaoRequest request,
        CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.AlterarStatusAsync(id, request, cancellationToken));

    [HttpPost("validar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosGerenciar)]
    public Task<IActionResult> Validar([FromBody] ValidarConfiguracaoRegraAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.ValidarAsync(request, cancellationToken));

    [HttpPost("candidatas")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> Candidatas([FromBody] ContextoAvaliacaoRegraAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.ListarRegrasCandidatasAsync(request, cancellationToken));

    [HttpPost("avaliar")]
    [Authorize(Policy = PermissionPolicies.AprovacaoChamadosVisualizar)]
    public Task<IActionResult> Avaliar([FromBody] ContextoAvaliacaoRegraAprovacaoRequest request, CancellationToken cancellationToken)
        => ExecutarAsync(() => useCases.AvaliarRegraAsync(request, cancellationToken));

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
