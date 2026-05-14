using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/admin/cadastros")]
[Authorize(Policy = Policies.AdminOuAtendente)]
public sealed class AdminCadastrosController(
    IListarUsuariosAdminUseCase listarUsuariosUseCase,
    IObterUsuarioAdminUseCase obterUsuarioUseCase,
    ICriarUsuarioAdminUseCase criarUsuarioUseCase,
    IAtualizarUsuarioAdminUseCase atualizarUsuarioUseCase,
    IInativarUsuarioAdminUseCase inativarUsuarioUseCase,
    IReativarUsuarioAdminUseCase reativarUsuarioUseCase,
    IAlterarPerfisUsuarioUseCase alterarPerfisUsuarioUseCase,
    IGestaoSenhaLocalSgxService gestaoSenhaLocalSgxService,
    IUsuarioAtualService usuarioAtualService,
    IListarPerfisAcessoUseCase listarPerfisUseCase,
    IObterPerfilAcessoUseCase obterPerfilUseCase,
    IListarPermissoesSistemaUseCase listarPermissoesSistemaUseCase,
    IObterPermissoesPerfilUseCase obterPermissoesPerfilUseCase,
    IAtualizarPermissoesPerfilUseCase atualizarPermissoesPerfilUseCase,
    ICriarPerfilAcessoUseCase criarPerfilUseCase,
    IAtualizarPerfilAcessoUseCase atualizarPerfilUseCase,
    IInativarPerfilAcessoUseCase inativarPerfilUseCase,
    IReativarPerfilAcessoUseCase reativarPerfilUseCase,
    IListarDepartamentosAdminUseCase listarDepartamentosUseCase,
    IObterDepartamentoAdminUseCase obterDepartamentoUseCase,
    ICriarDepartamentoUseCase criarDepartamentoUseCase,
    IAtualizarDepartamentoUseCase atualizarDepartamentoUseCase,
    IInativarDepartamentoUseCase inativarDepartamentoUseCase,
    IReativarDepartamentoUseCase reativarDepartamentoUseCase,
    IListarCategoriasAdminUseCase listarCategoriasUseCase,
    IObterCategoriaAdminUseCase obterCategoriaUseCase,
    ICriarCategoriaUseCase criarCategoriaUseCase,
    IAtualizarCategoriaUseCase atualizarCategoriaUseCase,
    IInativarCategoriaUseCase inativarCategoriaUseCase,
    IReativarCategoriaUseCase reativarCategoriaUseCase,
    IListarPrioridadesAdminUseCase listarPrioridadesUseCase,
    IObterPrioridadeAdminUseCase obterPrioridadeUseCase,
    ICriarPrioridadeUseCase criarPrioridadeUseCase,
    IAtualizarPrioridadeUseCase atualizarPrioridadeUseCase,
    IInativarPrioridadeUseCase inativarPrioridadeUseCase,
    IReativarPrioridadeUseCase reativarPrioridadeUseCase,
    IListarStatusAdminUseCase listarStatusUseCase,
    IObterStatusAdminUseCase obterStatusUseCase,
    ICriarStatusUseCase criarStatusUseCase,
    IAtualizarStatusUseCase atualizarStatusUseCase,
    IInativarStatusUseCase inativarStatusUseCase,
    IReativarStatusUseCase reativarStatusUseCase,
    IValidator<FiltroCadastroRequest> filtroValidator,
    IValidator<CriarUsuarioAdminRequest> criarUsuarioValidator,
    IValidator<AtualizarUsuarioAdminRequest> atualizarUsuarioValidator,
    IValidator<AlterarPerfisUsuarioRequest> alterarPerfisValidator,
    IValidator<CriarPerfilAcessoRequest> criarPerfilValidator,
    IValidator<AtualizarPerfilAcessoRequest> atualizarPerfilValidator,
    IValidator<AtualizarPermissoesPerfilRequest> atualizarPermissoesPerfilValidator,
    IValidator<CriarDepartamentoRequest> criarDepartamentoValidator,
    IValidator<AtualizarDepartamentoRequest> atualizarDepartamentoValidator,
    IValidator<CriarCategoriaChamadoRequest> criarCategoriaValidator,
    IValidator<AtualizarCategoriaChamadoRequest> atualizarCategoriaValidator,
    IValidator<CriarPrioridadeChamadoRequest> criarPrioridadeValidator,
    IValidator<AtualizarPrioridadeChamadoRequest> atualizarPrioridadeValidator,
    IValidator<CriarStatusChamadoRequest> criarStatusValidator,
    IValidator<AtualizarStatusChamadoRequest> atualizarStatusValidator) : ControllerBase
{
    [HttpGet("usuarios")]
    public async Task<IActionResult> ListarUsuarios([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarUsuariosUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("usuarios/{id:guid}")]
    public Task<IActionResult> ObterUsuario(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterUsuarioUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("usuarios")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosGerenciar)]
    public async Task<IActionResult> CriarUsuario([FromBody] CriarUsuarioAdminRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarUsuarioValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarUsuarioUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("usuarios/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosGerenciar)]
    public async Task<IActionResult> AtualizarUsuario(Guid id, [FromBody] AtualizarUsuarioAdminRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarUsuarioValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarUsuarioUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPut("usuarios/{id:guid}/perfis")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosAlterarPerfis)]
    public async Task<IActionResult> AlterarPerfisUsuario(Guid id, [FromBody] AlterarPerfisUsuarioRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(alterarPerfisValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => alterarPerfisUsuarioUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("usuarios/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosGerenciar)]
    public Task<IActionResult> InativarUsuario(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarUsuarioUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("usuarios/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosGerenciar)]
    public Task<IActionResult> ReativarUsuario(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarUsuarioUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("usuarios/{id:guid}/redefinir-senha")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.UsuariosRedefinirSenha)]
    public async Task<IActionResult> RedefinirSenhaUsuario(
        Guid id,
        [FromBody] RedefinirSenhaUsuarioAdminRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var usuarioAutenticado = await usuarioAtualService.ObterAsync(cancellationToken);
            var response = await gestaoSenhaLocalSgxService.RedefinirSenhaPorAdministradorAsync(
                id,
                request,
                usuarioAutenticado.Email,
                cancellationToken);
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("perfis")]
    public async Task<IActionResult> ListarPerfis([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarPerfisUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("perfis/{id:guid}")]
    public Task<IActionResult> ObterPerfil(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterPerfilUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("permissoes")]
    public Task<IActionResult> ListarPermissoes(CancellationToken cancellationToken)
        => ExecutarAsync(() => listarPermissoesSistemaUseCase.ExecutarAsync(cancellationToken));

    [HttpGet("perfis/{id:guid}/permissoes")]
    public Task<IActionResult> ObterPermissoesPerfil(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterPermissoesPerfilUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPut("perfis/{id:guid}/permissoes")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.PerfisAlterarPermissoes)]
    public async Task<IActionResult> AtualizarPermissoesPerfil(
        Guid id,
        [FromBody] AtualizarPermissoesPerfilRequest request,
        CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarPermissoesPerfilValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarPermissoesPerfilUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("perfis")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.PerfisGerenciar)]
    public async Task<IActionResult> CriarPerfil([FromBody] CriarPerfilAcessoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarPerfilValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarPerfilUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("perfis/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.PerfisGerenciar)]
    public async Task<IActionResult> AtualizarPerfil(Guid id, [FromBody] AtualizarPerfilAcessoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarPerfilValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarPerfilUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("perfis/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.PerfisGerenciar)]
    public Task<IActionResult> InativarPerfil(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarPerfilUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("perfis/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    [Authorize(Policy = PermissionPolicies.PerfisGerenciar)]
    public Task<IActionResult> ReativarPerfil(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarPerfilUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("departamentos")]
    public async Task<IActionResult> ListarDepartamentos([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarDepartamentosUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("departamentos/{id:guid}")]
    public Task<IActionResult> ObterDepartamento(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterDepartamentoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("departamentos")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> CriarDepartamento([FromBody] CriarDepartamentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarDepartamentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarDepartamentoUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("departamentos/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> AtualizarDepartamento(Guid id, [FromBody] AtualizarDepartamentoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarDepartamentoValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarDepartamentoUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("departamentos/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> InativarDepartamento(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarDepartamentoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("departamentos/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> ReativarDepartamento(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarDepartamentoUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("categorias")]
    public async Task<IActionResult> ListarCategorias([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarCategoriasUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("categorias/{id:guid}")]
    public Task<IActionResult> ObterCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("categorias")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarCategoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarCategoriaUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("categorias/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> AtualizarCategoria(Guid id, [FromBody] AtualizarCategoriaChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarCategoriaValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarCategoriaUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("categorias/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> InativarCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("categorias/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> ReativarCategoria(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarCategoriaUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("prioridades")]
    public async Task<IActionResult> ListarPrioridades([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarPrioridadesUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("prioridades/{id:guid}")]
    public Task<IActionResult> ObterPrioridade(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterPrioridadeUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("prioridades")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> CriarPrioridade([FromBody] CriarPrioridadeChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarPrioridadeValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarPrioridadeUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("prioridades/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> AtualizarPrioridade(Guid id, [FromBody] AtualizarPrioridadeChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarPrioridadeValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarPrioridadeUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("prioridades/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> InativarPrioridade(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarPrioridadeUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("prioridades/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> ReativarPrioridade(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarPrioridadeUseCase.ExecutarAsync(id, cancellationToken));

    [HttpGet("status")]
    public async Task<IActionResult> ListarStatus([FromQuery] FiltroCadastroRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(filtroValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => listarStatusUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpGet("status/{id:guid}")]
    public Task<IActionResult> ObterStatus(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => obterStatusUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("status")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> CriarStatus([FromBody] CriarStatusChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(criarStatusValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => criarStatusUseCase.ExecutarAsync(request, cancellationToken));
    }

    [HttpPut("status/{id:guid}")]
    [Authorize(Policy = Policies.Administrador)]
    public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] AtualizarStatusChamadoRequest request, CancellationToken cancellationToken)
    {
        var badRequest = await ValidarAsync(atualizarStatusValidator, request, cancellationToken);
        if (badRequest is not null)
        {
            return badRequest;
        }

        return await ExecutarAsync(() => atualizarStatusUseCase.ExecutarAsync(id, request, cancellationToken));
    }

    [HttpPost("status/{id:guid}/inativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> InativarStatus(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => inativarStatusUseCase.ExecutarAsync(id, cancellationToken));

    [HttpPost("status/{id:guid}/reativar")]
    [Authorize(Policy = Policies.Administrador)]
    public Task<IActionResult> ReativarStatus(Guid id, CancellationToken cancellationToken)
        => ExecutarAsync(() => reativarStatusUseCase.ExecutarAsync(id, cancellationToken));

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
    }
}
