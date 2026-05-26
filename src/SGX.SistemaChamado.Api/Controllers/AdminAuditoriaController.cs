using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.DTOs.Auditoria;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Domain.Enums;
using System.Text.Json;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/auditoria")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminAuditoriaController(
    IListarEventosAuditoriaUseCase listarEventosAuditoriaUseCase,
    IObterEventoAuditoriaUseCase obterEventoAuditoriaUseCase,
    IObterDashboardAuditoriaUseCase obterDashboardAuditoriaUseCase,
    IValidator<FiltroEventosAuditoriaRequest> filtroEventosValidator,
    IValidator<FiltroDashboardAuditoriaRequest> filtroDashboardValidator) : ControllerBase
{
    [HttpGet("eventos")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public async Task<IActionResult> Listar([FromQuery] FiltroEventosAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroEventosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarEventosAuditoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("autenticacao")]
    [Authorize(Policy = PermissionPolicies.AuditoriaAutenticacaoVisualizar)]
    public async Task<IActionResult> ListarAutenticacao([FromQuery] FiltroEventosAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var requestAutenticacao = new FiltroEventosAuditoriaRequest
        {
            DataInicio = request.DataInicio,
            DataFim = request.DataFim,
            UsuarioId = request.UsuarioId,
            UsuarioEmail = request.UsuarioEmail,
            Modulo = AuditoriaAutenticacaoHelper.ModuloAutenticacao,
            Entidade = request.Entidade,
            EntidadeId = request.EntidadeId,
            Acao = request.Acao,
            Nivel = request.Nivel,
            Sucesso = request.Sucesso,
            IpOrigem = request.IpOrigem,
            CorrelacaoId = request.CorrelacaoId,
            Texto = request.Texto,
            Provedor = request.Provedor,
            TipoEventoAutenticacao = request.TipoEventoAutenticacao,
            ResultadoAutenticacao = request.ResultadoAutenticacao,
            Pagina = request.Pagina,
            TamanhoPagina = request.TamanhoPagina
        };

        var badRequest = await ValidarAsync(filtroEventosValidator, requestAutenticacao, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(async () =>
        {
            var response = await listarEventosAuditoriaUseCase.ExecutarAsync(requestAutenticacao, cancellationToken);

            return new ListaEventosAuditoriaAutenticacaoResponse
            {
                Total = response.Total,
                Pagina = response.Pagina,
                TamanhoPagina = response.TamanhoPagina,
                Items = response.Items.Select(item =>
                {
                    var (provedor, tipoEvento, resultado) = ExtrairDadosAutenticacao(item.Metadados, item.Sucesso);

                    return new EventoAuditoriaAutenticacaoResumoResponse(
                        item.Id,
                        item.DataEvento,
                        item.UsuarioNome,
                        item.UsuarioEmail,
                        provedor,
                        tipoEvento,
                        resultado,
                        item.IpOrigem,
                        item.Descricao);
                }).ToArray()
            };
        });
    }

    [HttpGet("eventos/{id:guid}")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterEventoAuditoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("dashboard")]
    [Authorize(Policy = PermissionPolicies.AuditoriaVisualizar)]
    public async Task<IActionResult> Dashboard([FromQuery] FiltroDashboardAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroDashboardValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => obterDashboardAuditoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    private static async Task<IActionResult?> ValidarAsync<T>(
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (validation.IsValid)
        {
            return null;
        }

        return new BadRequestObjectResult(validation.Errors.Select(e => new
        {
            campo = e.PropertyName,
            mensagem = e.ErrorMessage
        }));
    }

    private async Task<IActionResult> ExecutarAsync<T>(Func<Task<T>> acao)
    {
        try
        {
            var response = await acao();
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    private static (string Provedor, string TipoEvento, string Resultado) ExtrairDadosAutenticacao(
        string? metadados,
        bool sucesso)
    {
        if (string.IsNullOrWhiteSpace(metadados))
        {
            return ("NaoInformado", "NaoInformado", sucesso ? ResultadoEventoAutenticacao.Sucesso.ToString() : ResultadoEventoAutenticacao.Falha.ToString());
        }

        try
        {
            using var json = JsonDocument.Parse(metadados);
            var root = json.RootElement;

            var provedor = ObterCampo(root, "provedor") ?? "NaoInformado";
            var tipoEvento = ObterCampo(root, "tipoEventoAutenticacao") ?? "NaoInformado";
            var resultado = ObterCampo(root, "resultadoAutenticacao")
                ?? (sucesso ? ResultadoEventoAutenticacao.Sucesso.ToString() : ResultadoEventoAutenticacao.Falha.ToString());

            return (provedor, tipoEvento, resultado);
        }
        catch
        {
            return ("NaoInformado", "NaoInformado", sucesso ? ResultadoEventoAutenticacao.Sucesso.ToString() : ResultadoEventoAutenticacao.Falha.ToString());
        }
    }

    private static string? ObterCampo(JsonElement root, string campo)
    {
        if (!root.TryGetProperty(campo, out var valor) || valor.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        var texto = valor.GetString();
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }
}
