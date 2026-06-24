using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/notificacoes/minhas")]
[Authorize]
public sealed class MinhasNotificacoesController(
    IListarMinhasNotificacoesUseCase listarMinhasNotificacoesUseCase,
    IObterMinhaNotificacaoUseCase obterMinhaNotificacaoUseCase,
    IMarcarMinhaNotificacaoComoLidaUseCase marcarMinhaNotificacaoComoLidaUseCase,
    IMarcarMinhaNotificacaoComoNaoLidaUseCase marcarMinhaNotificacaoComoNaoLidaUseCase,
    IContarMinhasNotificacoesNaoLidasUseCase contarMinhasNotificacoesNaoLidasUseCase) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ListarMinhasNotificacoesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        [FromQuery] bool? lida = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await listarMinhasNotificacoesUseCase.ExecutarAsync(
                new ListarMinhasNotificacoesRequest(pagina, tamanhoPagina, lida),
                cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || HttpContext.RequestAborted.IsCancellationRequested)
        {
            return StatusCode(499);
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(MinhaNotificacaoDetalheResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await obterMinhaNotificacaoUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("nao-lidas/contagem")]
    [ProducesResponseType(typeof(ContagemMinhasNotificacoesNaoLidasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ContarNaoLidas(CancellationToken cancellationToken)
    {
        var response = await contarMinhasNotificacoesNaoLidasUseCase.ExecutarAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPatch("{id:guid}/lida")]
    [ProducesResponseType(typeof(AlterarLeituraNotificacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarcarComoLida(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await marcarMinhaNotificacaoComoLidaUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/nao-lida")]
    [ProducesResponseType(typeof(AlterarLeituraNotificacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> MarcarComoNaoLida(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await marcarMinhaNotificacaoComoNaoLidaUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
