using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/formulario-servicos")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminFormularioServicosController(
    IAdminFormularioServicosUseCases adminFormularioServicosUseCases,
    IValidator<CriarFormularioServicoRequest> criarFormularioValidator,
    IValidator<AtualizarFormularioServicoRequest> atualizarFormularioValidator,
    IValidator<CriarFormularioServicoVersaoRequest> criarVersaoValidator,
    IValidator<AtualizarFormularioServicoVersaoRequest> atualizarVersaoValidator,
    IValidator<CriarCampoFormularioServicoRequest> criarCampoValidator,
    IValidator<AtualizarCampoFormularioServicoRequest> atualizarCampoValidator,
    IValidator<CriarOpcaoCampoFormularioServicoRequest> criarOpcaoValidator,
    IValidator<AtualizarOpcaoCampoFormularioServicoRequest> atualizarOpcaoValidator) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> Listar([FromQuery] Guid? catalogoServicoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ListarAsync(catalogoServicoId, cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> Obter(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ObterPorIdAsync(id, cancellationToken));

    [HttpPost]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> Criar([FromBody] CriarFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarFormularioValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.CriarAsync(request, cancellationToken));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarFormularioValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.AtualizarAsync(id, request, cancellationToken));
    }

    [HttpPost("{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> Inativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.InativarAsync(id, cancellationToken));

    [HttpPost("{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> Reativar(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ReativarAsync(id, cancellationToken));

    [HttpGet("{formularioId:guid}/versoes")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> ListarVersoes(Guid formularioId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ListarVersoesAsync(formularioId, cancellationToken));

    [HttpPost("{formularioId:guid}/versoes")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> CriarVersao(Guid formularioId, [FromBody] CriarFormularioServicoVersaoRequest request, CancellationToken cancellationToken)
    {
        if (request.FormularioServicoId != Guid.Empty && request.FormularioServicoId != formularioId)
        {
            return BadRequest(new { mensagem = "FormularioServicoId do corpo diverge do formulario informado na rota." });
        }

        var requestNormalizado = new CriarFormularioServicoVersaoRequest
        {
            FormularioServicoId = formularioId,
            Numero = request.Numero,
            Publicada = request.Publicada,
            PublicadoEm = request.PublicadoEm,
            Ativo = request.Ativo
        };

        var badRequest = await ValidarAsync(criarVersaoValidator, requestNormalizado, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.CriarVersaoAsync(requestNormalizado, cancellationToken));
    }

    [HttpPut("versoes/{versaoId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> AtualizarVersao(Guid versaoId, [FromBody] AtualizarFormularioServicoVersaoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarVersaoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.AtualizarVersaoAsync(versaoId, request, cancellationToken));
    }

    [HttpPost("versoes/{versaoId:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> InativarVersao(Guid versaoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.InativarVersaoAsync(versaoId, cancellationToken));

    [HttpPost("versoes/{versaoId:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> ReativarVersao(Guid versaoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ReativarVersaoAsync(versaoId, cancellationToken));

    [HttpGet("versoes/{versaoId:guid}/campos")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> ListarCampos(Guid versaoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ListarCamposAsync(versaoId, cancellationToken));

    [HttpPost("versoes/{versaoId:guid}/campos")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> CriarCampo(Guid versaoId, [FromBody] CriarCampoFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        if (request.FormularioServicoVersaoId != Guid.Empty && request.FormularioServicoVersaoId != versaoId)
        {
            return BadRequest(new { mensagem = "FormularioServicoVersaoId do corpo diverge da versao informada na rota." });
        }

        var requestNormalizado = new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = versaoId,
            Nome = request.Nome,
            Rotulo = request.Rotulo,
            Tipo = request.Tipo,
            Obrigatorio = request.Obrigatorio,
            Ordem = request.Ordem,
            TextoAjuda = request.TextoAjuda,
            Visivel = request.Visivel,
            Ativo = request.Ativo
        };

        var badRequest = await ValidarAsync(criarCampoValidator, requestNormalizado, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.CriarCampoAsync(requestNormalizado, cancellationToken));
    }

    [HttpPut("campos/{campoId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> AtualizarCampo(Guid campoId, [FromBody] AtualizarCampoFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarCampoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.AtualizarCampoAsync(campoId, request, cancellationToken));
    }

    [HttpPost("campos/{campoId:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> InativarCampo(Guid campoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.InativarCampoAsync(campoId, cancellationToken));

    [HttpPost("campos/{campoId:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> ReativarCampo(Guid campoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ReativarCampoAsync(campoId, cancellationToken));

    [HttpGet("campos/{campoId:guid}/opcoes")]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosVisualizar)]
    public Task<IActionResult> ListarOpcoes(Guid campoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ListarOpcoesAsync(campoId, cancellationToken));

    [HttpPost("campos/{campoId:guid}/opcoes")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> CriarOpcao(Guid campoId, [FromBody] CriarOpcaoCampoFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        if (request.CampoFormularioServicoId != Guid.Empty && request.CampoFormularioServicoId != campoId)
        {
            return BadRequest(new { mensagem = "CampoFormularioServicoId do corpo diverge do campo informado na rota." });
        }

        var requestNormalizado = new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = campoId,
            Valor = request.Valor,
            Rotulo = request.Rotulo,
            Ordem = request.Ordem,
            Ativo = request.Ativo
        };

        var badRequest = await ValidarAsync(criarOpcaoValidator, requestNormalizado, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.CriarOpcaoAsync(requestNormalizado, cancellationToken));
    }

    [HttpPut("opcoes/{opcaoId:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosGerenciar)]
    public async Task<IActionResult> AtualizarOpcao(Guid opcaoId, [FromBody] AtualizarOpcaoCampoFormularioServicoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarOpcaoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => adminFormularioServicosUseCases.AtualizarOpcaoAsync(opcaoId, request, cancellationToken));
    }

    [HttpPost("opcoes/{opcaoId:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> InativarOpcao(Guid opcaoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.InativarOpcaoAsync(opcaoId, cancellationToken));

    [HttpPost("opcoes/{opcaoId:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.CatalogoServicosArquivar)]
    public Task<IActionResult> ReativarOpcao(Guid opcaoId, CancellationToken cancellationToken)
        => ExecutarAsync(() => adminFormularioServicosUseCases.ReativarOpcaoAsync(opcaoId, cancellationToken));

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
        catch (ValidationException ex)
        {
            return new BadRequestObjectResult(ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage }));
        }
    }
}
