using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces.Portal;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/portal")]
[Authorize]
public sealed class PortalController(
    IObterPortalContextoUseCase obterPortalContextoUseCase,
    IListarMeusChamadosUseCase listarMeusChamadosUseCase,
    IDetalharMeuChamadoUseCase detalharMeuChamadoUseCase,
    IObterStatusAprovacaoChamadoPortalUseCase obterStatusAprovacaoChamadoPortalUseCase,
    IAbrirChamadoUseCase abrirChamadoUseCase,
    IComentarChamadoUseCase comentarChamadoUseCase,
    IAnexarArquivoChamadoUseCase anexarArquivoChamadoUseCase,
    IValidator<CriarChamadoRequest> criarChamadoValidator,
    IValidator<ComentarioChamadoRequest> comentarioValidator) : ControllerBase
{
    [HttpGet("contexto")]
    public async Task<IActionResult> ObterContexto(CancellationToken cancellationToken)
    {
        var contexto = await obterPortalContextoUseCase.ExecutarAsync(cancellationToken);
        return Ok(contexto);
    }

    [HttpGet("chamados")]
    public async Task<IActionResult> ListarChamados(
        [FromQuery] Guid? statusId,
        [FromQuery] Guid? prioridadeId,
        [FromQuery] Guid? categoriaId,
        [FromQuery] DateTime? dataInicial,
        [FromQuery] DateTime? dataFinal,
        [FromQuery] string? texto,
        [FromQuery] bool visaoAmpliada = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var filtro = new FiltroChamadosPortalRequest
        {
            StatusId = statusId,
            PrioridadeId = prioridadeId,
            CategoriaId = categoriaId,
            DataInicial = dataInicial,
            DataFinal = dataFinal,
            Texto = texto,
            VisaoAmpliada = visaoAmpliada,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };

        try
        {
            var response = await listarMeusChamadosUseCase.ExecutarAsync(filtro, cancellationToken);
            return Ok(response);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || HttpContext.RequestAborted.IsCancellationRequested)
        {
            return StatusCode(499);
        }
    }

    [HttpGet("chamados/{id:guid}")]
    public async Task<IActionResult> ObterChamado(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var detalhe = await detalharMeuChamadoUseCase.ExecutarAsync(id, cancellationToken);
            return Ok(detalhe);
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

    [HttpGet("chamados/{chamadoId:guid}/aprovacao")]
    public async Task<IActionResult> ObterStatusAprovacaoChamado(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await obterStatusAprovacaoChamadoPortalUseCase.ExecutarAsync(chamadoId, cancellationToken);
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

    [HttpPost("chamados")]
    public async Task<IActionResult> CriarChamado([FromBody] CriarChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await criarChamadoValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await abrirChamadoUseCase.ExecutarAsync(request, cancellationToken);
            return CreatedAtAction(nameof(ObterChamado), new { id = response.Id }, response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("chamados/{id:guid}/comentarios")]
    public async Task<IActionResult> AdicionarComentario(Guid id, [FromBody] ComentarioChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await comentarioValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await comentarChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
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

    [HttpPost("chamados/{id:guid}/anexos")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> AnexarArquivo(Guid id, [FromForm] IFormFile? arquivo, CancellationToken cancellationToken)
    {
        if (arquivo is null || arquivo.Length <= 0)
        {
            return BadRequest(new { mensagem = "Arquivo obrigatorio." });
        }

        await using var stream = arquivo.OpenReadStream();
        var request = new UploadAnexoChamadoRequest
        {
            NomeArquivo = arquivo.FileName,
            ContentType = arquivo.ContentType,
            TamanhoBytes = arquivo.Length,
            Conteudo = stream
        };

        try
        {
            var response = await anexarArquivoChamadoUseCase.ExecutarAsync(id, request, cancellationToken);
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
