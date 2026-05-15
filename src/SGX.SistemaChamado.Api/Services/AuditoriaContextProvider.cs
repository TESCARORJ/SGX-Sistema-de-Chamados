using System.Security.Claims;
using SGX.SistemaChamado.Api.Middlewares;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;

namespace SGX.SistemaChamado.Api.Services;

public sealed class AuditoriaContextProvider(IHttpContextAccessor httpContextAccessor) : IAuditoriaContextProvider
{
    public ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var principal = httpContext?.User;

        var usuarioId = ObterUsuarioId(principal);
        var usuarioNome = ObterClaim(principal, ClaimTypes.Name, "name");
        var usuarioEmail = ObterClaim(principal, ClaimTypes.Email, "email", "upn", "preferred_username");
        var usuarioLogin = ObterClaim(principal, "preferred_username", "upn", "email");
        var ipOrigem = httpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = httpContext?.Request.Headers.UserAgent.ToString();
        var correlacaoId = ObterCorrelacaoId(httpContext);

        var contexto = new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            usuarioId,
            usuarioNome,
            usuarioEmail,
            usuarioLogin,
            ipOrigem,
            userAgent,
            correlacaoId);

        return ValueTask.FromResult(contexto);
    }

    private static Guid? ObterUsuarioId(ClaimsPrincipal? principal)
    {
        if (principal is null)
        {
            return null;
        }

        var valor = ObterClaim(principal, ClaimTypes.NameIdentifier, "sub");
        return Guid.TryParse(valor, out var usuarioId) ? usuarioId : null;
    }

    private static string? ObterCorrelacaoId(HttpContext? context)
    {
        if (context is null)
        {
            return null;
        }

        if (context.Items.TryGetValue(CorrelationIdMiddleware.HeaderName, out var valorItem)
            && valorItem is string correlacaoItem
            && !string.IsNullOrWhiteSpace(correlacaoItem))
        {
            return correlacaoItem.Trim();
        }

        if (context.Request.Headers.TryGetValue(CorrelationIdMiddleware.HeaderName, out var valorHeader)
            && !string.IsNullOrWhiteSpace(valorHeader))
        {
            return valorHeader.ToString().Trim();
        }

        return string.IsNullOrWhiteSpace(context.TraceIdentifier)
            ? null
            : context.TraceIdentifier.Trim();
    }

    private static string? ObterClaim(ClaimsPrincipal? principal, params string[] tiposClaim)
    {
        if (principal is null)
        {
            return null;
        }

        foreach (var tipoClaim in tiposClaim)
        {
            var valor = principal.FindFirst(tipoClaim)?.Value;
            if (!string.IsNullOrWhiteSpace(valor))
            {
                return valor.Trim();
            }
        }

        return null;
    }
}
