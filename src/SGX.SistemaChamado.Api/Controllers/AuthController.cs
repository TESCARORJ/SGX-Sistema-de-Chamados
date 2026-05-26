using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    IAutenticacaoLocalSgxService autenticacaoLocalSgxService,
    IActiveDirectoryAuthenticationService activeDirectoryAuthenticationService,
    IGestaoSenhaLocalSgxService gestaoSenhaLocalSgxService,
    IMetodosLoginAdminService metodosLoginAdminService,
    IUsuarioAtualService usuarioAtualService) : ControllerBase
{
    [HttpGet("provedores")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProvedoresAutenticacaoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterProvedores(CancellationToken cancellationToken)
    {
        var response = await metodosLoginAdminService.ObterProvedoresPublicosAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPost("ad/login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LocalLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LoginActiveDirectory([FromBody] LoginActiveDirectoryRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { mensagem = "Payload de login Active Directory inválido." });
        }

        try
        {
            var response = await activeDirectoryAuthenticationService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AcessoNegadoException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("local/login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LocalLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> LoginLocal([FromBody] LocalLoginRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { mensagem = "Payload de login local inválido." });
        }

        try
        {
            var response = await autenticacaoLocalSgxService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (AcessoNegadoException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { mensagem = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("local/alterar-senha")]
    [Authorize]
    [ProducesResponseType(typeof(MensagemAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AlterarSenhaLocal([FromBody] AlterarSenhaLocalRequest request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { mensagem = "Payload de alteração de senha inválido." });
        }

        try
        {
            var usuarioAtual = await usuarioAtualService.ObterAsync(cancellationToken);
            var response = await gestaoSenhaLocalSgxService.AlterarSenhaAsync(usuarioAtual.Id, request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("local/recuperar-senha/solicitar")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MensagemAuthResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> SolicitarRecuperacaoSenha(
        [FromBody] RecuperarSenhaSolicitacaoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await gestaoSenhaLocalSgxService.SolicitarRecuperacaoSenhaAsync(
            request,
            HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpContext.Request.Headers.UserAgent.ToString(),
            cancellationToken);

        return Ok(response);
    }

    [HttpPost("local/recuperar-senha/redefinir")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(MensagemAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RedefinirSenhaLocal(
        [FromBody] RecuperarSenhaRedefinicaoRequest request,
        CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { mensagem = "Payload de redefinição de senha inválido." });
        }

        try
        {
            var response = await gestaoSenhaLocalSgxService.RedefinirSenhaAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}


