using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminChamadosController(
    IObterAdminContextoUseCase obterAdminContextoUseCase,
    IListarChamadosAdminUseCase listarChamadosAdminUseCase,
    IDetalharChamadoAdminUseCase detalharChamadoAdminUseCase,
    IAssumirChamadoUseCase assumirChamadoUseCase,
    IAtribuirChamadoUseCase atribuirChamadoUseCase,
    IAlterarStatusChamadoUseCase alterarStatusChamadoUseCase,
    IAlterarPrioridadeChamadoUseCase alterarPrioridadeChamadoUseCase,
    IAlterarCategoriaChamadoUseCase alterarCategoriaChamadoUseCase,
    IComentarChamadoAdminUseCase comentarChamadoAdminUseCase,
    IEncerrarChamadoUseCase encerrarChamadoUseCase,
    IReabrirChamadoUseCase reabrirChamadoUseCase,
    IValidator<FiltroChamadosAdminRequest> filtroValidator,
    IValidator<AtribuirChamadoRequest> atribuirValidator,
    IValidator<AlterarStatusChamadoRequest> alterarStatusValidator,
    IValidator<AlterarPrioridadeChamadoRequest> alterarPrioridadeValidator,
    IValidator<AlterarCategoriaChamadoRequest> alterarCategoriaValidator,
    IValidator<ComentarioAdminChamadoRequest> comentarioValidator,
    IValidator<EncerrarChamadoRequest> encerrarValidator,
    IValidator<ReabrirChamadoRequest> reabrirValidator) : ControllerBase
{
    [HttpGet("contexto")]
    public async Task<IActionResult> ObterContexto(CancellationToken cancellationToken)
    {
        try
        {
            var contexto = await obterAdminContextoUseCase.ExecutarAsync(cancellationToken);
            return Ok(contexto);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("chamados")]
    public async Task<IActionResult> ListarChamados([FromQuery] FiltroChamadosAdminRequest request, CancellationToken cancellationToken)
    {
        var validation = await filtroValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await listarChamadosAdminUseCase.ExecutarAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpGet("chamados/{id:guid}")]
    public async Task<IActionResult> ObterChamado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await detalharChamadoAdminUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/assumir")]
    [Authorize(Policy = PermissionPolicies.ChamadosAssumir)]
    public async Task<IActionResult> AssumirChamado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await assumirChamadoUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/atribuir")]
    [Authorize(Policy = PermissionPolicies.ChamadosAtribuir)]
    public async Task<IActionResult> AtribuirChamado(Guid id, [FromBody] AtribuirChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await atribuirValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await atribuirChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-status")]
    public async Task<IActionResult> AlterarStatus(Guid id, [FromBody] AlterarStatusChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarStatusValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarStatusChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-prioridade")]
    public async Task<IActionResult> AlterarPrioridade(Guid id, [FromBody] AlterarPrioridadeChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarPrioridadeValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarPrioridadeChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/alterar-categoria")]
    public async Task<IActionResult> AlterarCategoria(Guid id, [FromBody] AlterarCategoriaChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await alterarCategoriaValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await alterarCategoriaChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/comentarios")]
    public async Task<IActionResult> Comentar(Guid id, [FromBody] ComentarioAdminChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await comentarioValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await comentarChamadoAdminUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/encerrar")]
    [Authorize(Policy = PermissionPolicies.ChamadosEncerrar)]
    public async Task<IActionResult> Encerrar(Guid id, [FromBody] EncerrarChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await encerrarValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await encerrarChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/reabrir")]
    public async Task<IActionResult> Reabrir(Guid id, [FromBody] ReabrirChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await reabrirValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await reabrirChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
