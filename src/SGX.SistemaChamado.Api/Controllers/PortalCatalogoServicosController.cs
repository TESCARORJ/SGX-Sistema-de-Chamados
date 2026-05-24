using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Interfaces.Portal;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/portal/catalogo-servicos")]
[Authorize]
public sealed class PortalCatalogoServicosController(
    IPortalCatalogoServicosUseCases portalCatalogoServicosUseCases) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] string? termo,
        [FromQuery] Guid? departamentoResponsavelId,
        [FromQuery] Guid? categoriaId,
        [FromQuery] Guid? subcategoriaId,
        [FromQuery] bool? permiteAberturaChamado,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        var request = new PortalFiltroCatalogoServicoRequest
        {
            Termo = termo,
            DepartamentoResponsavelId = departamentoResponsavelId,
            CategoriaId = categoriaId,
            SubcategoriaId = subcategoriaId,
            PermiteAberturaChamado = permiteAberturaChamado,
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina
        };

        var response = await portalCatalogoServicosUseCases.ListarAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> ObterPorSlug(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await portalCatalogoServicosUseCases.ObterPorSlugAsync(slug, cancellationToken);
            return Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{slug}/preparar-chamado")]
    public async Task<IActionResult> PrepararAberturaChamado(string slug, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await portalCatalogoServicosUseCases.PrepararAberturaChamadoAsync(slug, cancellationToken);
            return Ok(response);
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
