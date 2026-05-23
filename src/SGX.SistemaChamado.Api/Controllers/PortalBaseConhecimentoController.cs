using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces.Portal;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/portal/base-conhecimento")]
[Authorize]
public sealed class PortalBaseConhecimentoController(
    IListarArtigosPortalBaseConhecimentoUseCase listarArtigosUseCase,
    IObterArtigoPortalBaseConhecimentoPorSlugUseCase obterArtigoPorSlugUseCase) : ControllerBase
{
    [HttpGet("artigos")]
    public async Task<IActionResult> ListarArtigos(
        [FromQuery] string? termo,
        [FromQuery] Guid? categoriaId,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var request = new PortalFiltroBaseConhecimentoRequest
        {
            Termo = termo,
            CategoriaId = categoriaId,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };

        var response = await listarArtigosUseCase.ExecutarAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("artigos/{slug}")]
    public async Task<IActionResult> ObterArtigoPorSlug(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await obterArtigoPorSlugUseCase.ExecutarAsync(slug, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}