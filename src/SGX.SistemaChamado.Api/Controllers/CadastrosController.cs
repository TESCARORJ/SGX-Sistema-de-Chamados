using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/cadastros")]
[Authorize]
public sealed class CadastrosController(SGXSistemaChamadoDbContext dbContext) : ControllerBase
{
    [HttpGet("departamentos")]
    public async Task<IActionResult> ListarDepartamentos(CancellationToken cancellationToken)
    {
        var items = await dbContext.Departamentos
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, x.Sigla, x.Descricao, x.Ativo })
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("categorias")]
    public async Task<IActionResult> ListarCategorias(CancellationToken cancellationToken)
    {
        var items = await dbContext.CategoriasChamado
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, x.Descricao, x.DepartamentoId, x.Ativo })
            .OrderBy(x => x.Nome)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("prioridades")]
    public async Task<IActionResult> ListarPrioridades(CancellationToken cancellationToken)
    {
        var items = await dbContext.PrioridadesChamado
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Select(x => new
            {
                x.Id,
                x.Nome,
                x.Nivel,
                x.Descricao,
                x.PrazoPrimeiraRespostaHoras,
                x.PrazoResolucaoHoras,
                x.Ativo
            })
            .OrderBy(x => x.Nivel)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("status")]
    public async Task<IActionResult> ListarStatus(CancellationToken cancellationToken)
    {
        var items = await dbContext.StatusChamado
            .AsNoTracking()
            .Where(x => x.Ativo)
            .Select(x => new { x.Id, x.Nome, x.Codigo, x.Descricao, x.EhStatusFinal, x.PausaSla, x.Ativo })
            .OrderBy(x => x.Codigo)
            .ToListAsync(cancellationToken);

        return Ok(items);
    }
}
