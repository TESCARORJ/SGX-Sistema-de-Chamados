using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Options;

namespace SGX.SistemaChamado.Api.Authentication;

public sealed class DevLocalAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!environment.IsDevelopment() || !authOptions.Value.ModoLocalHabilitado)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var email = Request.Headers.TryGetValue("X-Dev-User-Email", out var emailHeader)
            ? emailHeader.ToString()
            : authOptions.Value.AdminLocalEmail;

        var nome = Request.Headers.TryGetValue("X-Dev-User-Name", out var nomeHeader)
            ? nomeHeader.ToString()
            : authOptions.Value.AdminLocalNome;

        var perfil = Request.Headers.TryGetValue("X-Dev-User-Role", out var perfilHeader)
            ? perfilHeader.ToString()
            : PerfisInternos.Administrador;

        email = (email ?? string.Empty).Trim().ToLowerInvariant();
        nome = string.IsNullOrWhiteSpace(nome) ? authOptions.Value.AdminLocalNome : nome.Trim();
        perfil = string.IsNullOrWhiteSpace(perfil) ? PerfisInternos.Administrador : perfil.Trim();
        perfil = NormalizarPerfil(perfil);

        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(AuthenticateResult.Fail(
                "X-Dev-User-Email (ou Authentication__AdminLocalEmail) nao foi informado."));
        }

        if (!PerfisInternos.EhPerfilValido(perfil))
        {
            return Task.FromResult(AuthenticateResult.Fail(
                "Perfil de desenvolvimento invalido. Use: Administrador, Atendente ou Solicitante."));
        }

        Logger.LogWarning(
            "Modo local de autenticacao ativo em Development para o usuario {Email} com perfil {Perfil}.",
            email,
            perfil);

        var login = email.Contains('@', StringComparison.Ordinal)
            ? email[..email.IndexOf('@')]
            : email;

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, nome),
            new(ClaimTypes.Email, email),
            new("name", nome),
            new("email", email),
            new("preferred_username", email),
            new("upn", email),
            new("sgx_dev_role", perfil),
            new(ClaimTypes.Role, perfil)
        };

        var identity = new ClaimsIdentity(claims, AuthSchemes.LocalDevelopment);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthSchemes.LocalDevelopment);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
    private static string NormalizarPerfil(string perfil)
    {
        if (string.IsNullOrWhiteSpace(perfil))
        {
            return PerfisInternos.Administrador;
        }

        var normalized = perfil.Trim().Replace(" ", "").ToLowerInvariant();

        switch (normalized)
        {
            case "administrador":
                return "Administrador";
            case "solicitante":
                return "Solicitante";
            case "atendente":
                return "Atendente";
            case "atendenten1":
            case "atendenten1homologacao":
                return "Atendente N1";
            case "tecnicon2":
            case "tecnicon2homologacao":
                return "Técnico N2";
            case "coordenadorservicedesk":
            case "coordenadorservicedeskhomologacao":
                return "Coordenador Service Desk";
            case "gestorti":
            case "gestortihomologacao":
                return "Gestor TI";
            case "auditorgovernanca":
            case "auditorgovernancahomologacao":
                return "Auditor Governança";
            default:
                var normalizedWithSpaces = perfil.Trim().ToLowerInvariant();
                if (normalizedWithSpaces.Contains("atendente n1")) return "Atendente N1";
                if (normalizedWithSpaces.Contains("tecnico n2") || normalizedWithSpaces.Contains("têcnico n2")) return "Técnico N2";
                if (normalizedWithSpaces.Contains("coordenador service desk")) return "Coordenador Service Desk";
                if (normalizedWithSpaces.Contains("gestor ti")) return "Gestor TI";
                if (normalizedWithSpaces.Contains("auditor governanca") || normalizedWithSpaces.Contains("auditor governança")) return "Auditor Governança";
                
                return perfil;
        }
    }
}
