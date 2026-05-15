using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.DTOs;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Persistence.Seed;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/chamados")]
[Authorize]
public sealed class ChamadosController(
    SGXSistemaChamadoDbContext dbContext,
    IRepository<Chamado> chamadoRepository,
    IUnitOfWork unitOfWork,
    SGX.SistemaChamado.Application.Interfaces.ICodigoChamadoService codigoChamadoService,
    IValidator<CriarChamadoDto> validator,
    IUsuarioAtualService usuarioAtualService,
    IValidator<CriarComentarioChamadoRequest> comentarioValidator,
    IListarComentariosChamadoUseCase listarComentariosChamadoUseCase,
    IAdicionarComentarioChamadoUseCase adicionarComentarioChamadoUseCase,
    IListarLinhaTempoChamadoUseCase listarLinhaTempoChamadoUseCase,
    IListarAnexosChamadoUseCase listarAnexosChamadoUseCase,
    IAdicionarAnexoChamadoUseCase adicionarAnexoChamadoUseCase,
    IBaixarAnexoChamadoUseCase baixarAnexoChamadoUseCase) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Policies.AdminOuAtendente)]
    public async Task<IActionResult> Listar(CancellationToken cancellationToken)
    {
        var items = await dbContext.Chamados
            .AsNoTracking()
            .OrderByDescending(x => x.CriadoEm)
            .Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Titulo,
                x.Descricao,
                x.SolicitanteId,
                x.ResponsavelId,
                x.DepartamentoId,
                x.CategoriaId,
                x.PrioridadeId,
                x.StatusId,
                x.Origem,
                x.AbertoEm,
                x.EncerradoEm,
                x.CriadoEm,
                x.CriadoPor,
                x.AtualizadoEm,
                x.AtualizadoPor,
                x.Ativo
            })
            .ToListAsync(cancellationToken);

        return Ok(new { items, total = items.Count });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(new { mensagem = "Id do chamado invalido." });
        }

        var usuarioAtual = await usuarioAtualService.ObterAsync(cancellationToken);
        var consultaChamado = dbContext.Chamados
            .AsNoTracking()
            .Where(x => x.Id == id);

        if (!usuarioAtual.PossuiQualquerPerfil(PerfisInternos.Administrador, PerfisInternos.Atendente))
        {
            consultaChamado = consultaChamado.Where(x => x.SolicitanteId == usuarioAtual.Id);
        }

        var chamado = await consultaChamado
            .Select(x => new
            {
                x.Id,
                x.Codigo,
                x.Titulo,
                x.Descricao,
                x.SolicitanteId,
                x.ResponsavelId,
                x.DepartamentoId,
                x.CategoriaId,
                x.PrioridadeId,
                x.StatusId,
                x.Origem,
                x.AbertoEm,
                x.EncerradoEm,
                x.CriadoEm,
                x.CriadoPor,
                x.AtualizadoEm,
                x.AtualizadoPor,
                x.Ativo
            })
            .FirstOrDefaultAsync(cancellationToken);

        return chamado is null ? NotFound() : Ok(chamado);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarChamadoDto request, CancellationToken cancellationToken)
    {
        var usuarioAtual = await usuarioAtualService.ObterAsync(cancellationToken);
        var podeCriar = usuarioAtual.PossuiQualquerPerfil(
            PerfisInternos.Solicitante,
            PerfisInternos.Atendente,
            PerfisInternos.Administrador);

        if (!podeCriar)
        {
            return Forbid();
        }

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        var solicitanteId = request.SolicitanteId;
        if (usuarioAtual.PossuiPerfil(PerfisInternos.Solicitante))
        {
            solicitanteId = usuarioAtual.Id;
        }

        var existeSolicitante = await dbContext.Usuarios.AnyAsync(x => x.Id == solicitanteId && x.Ativo, cancellationToken);
        if (!existeSolicitante)
        {
            return BadRequest(new { mensagem = "Solicitante nao encontrado ou inativo." });
        }

        var existeCategoria = await dbContext.CategoriasChamado.AnyAsync(x => x.Id == request.CategoriaId && x.Ativo, cancellationToken);
        if (!existeCategoria)
        {
            return BadRequest(new { mensagem = "Categoria nao encontrada ou inativa." });
        }

        var existePrioridade = await dbContext.PrioridadesChamado.AnyAsync(x => x.Id == request.PrioridadeId && x.Ativo, cancellationToken);
        if (!existePrioridade)
        {
            return BadRequest(new { mensagem = "Prioridade nao encontrada ou inativa." });
        }

        var origem = request.Origem switch
        {
            "Portal" => OrigemChamado.Portal,
            "Email" => OrigemChamado.Email,
            "Admin" => OrigemChamado.Admin,
            _ => OrigemChamado.Portal
        };

        var codigo = await codigoChamadoService.GerarAsync(cancellationToken);
        var chamado = new Chamado(
            codigo,
            request.Titulo,
            request.Descricao,
            solicitanteId,
            request.CategoriaId,
            request.PrioridadeId,
            SeedData.StatusAbertoId,
            origem,
            usuarioAtual.Login,
            request.DepartamentoId);

        await chamadoRepository.AddAsync(chamado, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, new
        {
            chamado.Id,
            chamado.Codigo,
            chamado.Titulo,
            chamado.Descricao,
            chamado.SolicitanteId,
            chamado.ResponsavelId,
            chamado.DepartamentoId,
            chamado.CategoriaId,
            chamado.PrioridadeId,
            chamado.StatusId,
            chamado.Origem,
            chamado.AbertoEm,
            chamado.EncerradoEm,
            chamado.CriadoEm,
            chamado.CriadoPor,
            chamado.AtualizadoEm,
            chamado.AtualizadoPor,
            chamado.Ativo
        });
    }

    [HttpGet("{chamadoId:guid}/comentarios")]
    public async Task<IActionResult> ListarComentarios(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await listarComentariosChamadoUseCase.ExecutarAsync(chamadoId, cancellationToken);
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

    [HttpPost("{chamadoId:guid}/comentarios")]
    public async Task<IActionResult> AdicionarComentario(Guid chamadoId, [FromBody] CriarComentarioChamadoRequest request, CancellationToken cancellationToken)
    {
        var validation = await comentarioValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return BadRequest(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }

        try
        {
            var response = await adicionarComentarioChamadoUseCase.ExecutarAsync(chamadoId, request, cancellationToken);
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

    [HttpGet("{chamadoId:guid}/linha-do-tempo")]
    public async Task<IActionResult> ListarLinhaDoTempo(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await listarLinhaTempoChamadoUseCase.ExecutarAsync(chamadoId, cancellationToken);
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

    [HttpGet("{chamadoId:guid}/anexos")]
    public async Task<IActionResult> ListarAnexos(Guid chamadoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await listarAnexosChamadoUseCase.ExecutarAsync(chamadoId, cancellationToken);
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

    [HttpPost("{chamadoId:guid}/anexos")]
    public async Task<IActionResult> AdicionarAnexo(Guid chamadoId, [FromForm] IFormFile? arquivo, CancellationToken cancellationToken)
    {
        if (arquivo is null)
        {
            return BadRequest(new { mensagem = "Arquivo obrigatorio." });
        }

        if (arquivo.Length <= 0)
        {
            return BadRequest(new { mensagem = "Arquivo invalido: tamanho deve ser maior que zero." });
        }

        await using var stream = arquivo.OpenReadStream();
        var request = new CriarAnexoChamadoRequest
        {
            NomeArquivo = arquivo.FileName,
            ContentType = arquivo.ContentType,
            TamanhoBytes = arquivo.Length,
            Conteudo = stream
        };

        try
        {
            var response = await adicionarAnexoChamadoUseCase.ExecutarAsync(chamadoId, request, cancellationToken);
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

    [HttpGet("{chamadoId:guid}/anexos/{anexoId:guid}/download")]
    public async Task<IActionResult> BaixarAnexo(Guid chamadoId, Guid anexoId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await baixarAnexoChamadoUseCase.ExecutarAsync(chamadoId, anexoId, cancellationToken);
            return File(response.Conteudo, response.ContentType, response.NomeArquivo);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { mensagem = "Arquivo fisico nao encontrado." });
        }
    }
}
