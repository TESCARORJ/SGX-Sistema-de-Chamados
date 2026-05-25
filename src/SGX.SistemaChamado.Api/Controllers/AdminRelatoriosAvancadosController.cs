using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/relatorios-avancados")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminRelatoriosAvancadosController(
    IAdminRelatoriosAvancadosUseCases adminRelatoriosAvancadosUseCases,
    IValidator<FiltroRelatorioChamadosRequest> filtroRelatorioChamadosValidator,
    IValidator<FiltroRelatorioAtendimentoRequest> filtroRelatorioAtendimentoValidator,
    IValidator<FiltroRelatorioSlaRequest> filtroRelatorioSlaValidator,
    IValidator<FiltroRelatorioAprovacoesRequest> filtroRelatorioAprovacoesValidator,
    IValidator<FiltroRelatorioCatalogoServicosRequest> filtroRelatorioCatalogoServicosValidator,
    IValidator<FiltroRelatorioInventarioAtivosRequest> filtroRelatorioInventarioAtivosValidator,
    IValidator<FiltroRelatorioBaseConhecimentoRequest> filtroRelatorioBaseConhecimentoValidator,
    IValidator<FiltroRelatorioAuditoriaRequest> filtroRelatorioAuditoriaValidator) : ControllerBase
{
    [HttpGet("metadados")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    public Task<IActionResult> ObterMetadados(CancellationToken cancellationToken)
        => ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterMetadadosAsync(cancellationToken));

    [HttpGet("chamados/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    public async Task<IActionResult> ObterResumoChamados([FromQuery] FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioChamadosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoChamadosAsync(request, cancellationToken));
    }

    [HttpGet("chamados/serie-temporal")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    public async Task<IActionResult> ObterSerieTemporalChamados([FromQuery] FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioChamadosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterSerieTemporalChamadosAsync(request, cancellationToken));
    }

    [HttpGet("chamados/distribuicao")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    public async Task<IActionResult> ObterDistribuicaoChamados([FromQuery] FiltroRelatorioChamadosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioChamadosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterDistribuicaoChamadosAsync(request, cancellationToken));
    }

    [HttpGet("atendimento/produtividade")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosOperacional)]
    public async Task<IActionResult> ObterProdutividadeAtendimento([FromQuery] FiltroRelatorioAtendimentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAtendimentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterProdutividadeAtendimentoAsync(request, cancellationToken));
    }

    [HttpGet("sla/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterResumoSla([FromQuery] FiltroRelatorioSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioSlaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoSlaAsync(request, cancellationToken));
    }

    [HttpGet("sla/violacoes")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosOperacional)]
    public async Task<IActionResult> ObterViolacoesSla([FromQuery] FiltroRelatorioSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioSlaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterViolacoesSlaAsync(request, cancellationToken));
    }

    [HttpGet("sla/por-departamento")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterSlaPorDepartamento([FromQuery] FiltroRelatorioSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioSlaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterSlaPorDepartamentoAsync(request, cancellationToken));
    }

    [HttpGet("sla/por-prioridade")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosOperacional)]
    public async Task<IActionResult> ObterSlaPorPrioridade([FromQuery] FiltroRelatorioSlaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioSlaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterSlaPorPrioridadeAsync(request, cancellationToken));
    }

    [HttpGet("aprovacoes/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterResumoAprovacoes([FromQuery] FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAprovacoesValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoAprovacoesAsync(request, cancellationToken));
    }

    [HttpGet("aprovacoes/tempo-medio")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterTempoMedioAprovacoes([FromQuery] FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAprovacoesValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterTempoMedioAprovacoesAsync(request, cancellationToken));
    }

    [HttpGet("aprovacoes/por-origem")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosOperacional)]
    public async Task<IActionResult> ObterAprovacoesPorOrigem([FromQuery] FiltroRelatorioAprovacoesRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAprovacoesValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterAprovacoesPorOrigemAsync(request, cancellationToken));
    }

    [HttpGet("catalogo-servicos/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterResumoCatalogoServicos([FromQuery] FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioCatalogoServicosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoCatalogoServicosAsync(request, cancellationToken));
    }

    [HttpGet("catalogo-servicos/mais-solicitados")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterCatalogoServicosMaisSolicitados([FromQuery] FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioCatalogoServicosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterCatalogoServicosMaisSolicitadosAsync(request, cancellationToken));
    }

    [HttpGet("catalogo-servicos/por-departamento")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    public async Task<IActionResult> ObterCatalogoServicosPorDepartamento([FromQuery] FiltroRelatorioCatalogoServicosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioCatalogoServicosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterCatalogoServicosPorDepartamentoAsync(request, cancellationToken));
    }

    [HttpGet("inventario-ativos/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterResumoInventarioAtivos([FromQuery] FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioInventarioAtivosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoInventarioAtivosAsync(request, cancellationToken));
    }

    [HttpGet("inventario-ativos/por-status")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterInventarioAtivosPorStatus([FromQuery] FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioInventarioAtivosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterInventarioAtivosPorStatusAsync(request, cancellationToken));
    }

    [HttpGet("inventario-ativos/chamados-recorrentes")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterInventarioAtivosChamadosRecorrentes([FromQuery] FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioInventarioAtivosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterInventarioAtivosChamadosRecorrentesAsync(request, cancellationToken));
    }

    [HttpGet("inventario-ativos/por-departamento")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterInventarioAtivosPorDepartamento([FromQuery] FiltroRelatorioInventarioAtivosRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioInventarioAtivosValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterInventarioAtivosPorDepartamentoAsync(request, cancellationToken));
    }

    [HttpGet("base-conhecimento/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterResumoBaseConhecimento([FromQuery] FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioBaseConhecimentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoBaseConhecimentoAsync(request, cancellationToken));
    }

    [HttpGet("base-conhecimento/por-status")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterBaseConhecimentoPorStatus([FromQuery] FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioBaseConhecimentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterBaseConhecimentoPorStatusAsync(request, cancellationToken));
    }

    [HttpGet("base-conhecimento/vinculos-chamados")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosGerencial)]
    public async Task<IActionResult> ObterBaseConhecimentoVinculosChamados([FromQuery] FiltroRelatorioBaseConhecimentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioBaseConhecimentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterBaseConhecimentoVinculosChamadosAsync(request, cancellationToken));
    }

    [HttpGet("auditoria/resumo")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosAuditoria)]
    public async Task<IActionResult> ObterResumoAuditoria([FromQuery] FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAuditoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterResumoAuditoriaAsync(request, cancellationToken));
    }

    [HttpGet("auditoria/por-usuario")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosAuditoria)]
    public async Task<IActionResult> ObterAuditoriaPorUsuario([FromQuery] FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAuditoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterAuditoriaPorUsuarioAsync(request, cancellationToken));
    }

    [HttpGet("auditoria/por-entidade")]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosVisualizar)]
    [Authorize(Policy = PermissionPolicies.RelatoriosAvancadosAuditoria)]
    public async Task<IActionResult> ObterAuditoriaPorEntidade([FromQuery] FiltroRelatorioAuditoriaRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroRelatorioAuditoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminRelatoriosAvancadosUseCases.ObterAuditoriaPorEntidadeAsync(request, cancellationToken));
    }

    private static async Task<IActionResult?> ValidarAsync<T>(
        IValidator<T> validator,
        T request,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return new ObjectResult(new { mensagem = "Requisicao cancelada pelo cliente." })
            {
                StatusCode = StatusCodes.Status499ClientClosedRequest
            };
        }

        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (validation.IsValid)
        {
            return null;
        }

        return new BadRequestObjectResult(validation.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
    }

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
