using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Tests;

public sealed class AuthorizationTests
{
    [Fact]
    public async Task AdministradorAcessaPoliticaAdministrador()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Admin",
            "admin@empresa.com",
            "admin",
            "Ativo",
            null,
            "AzureAd",
            [PerfisInternos.Administrador],
            [PermissoesInternas.AdminAcessar]));

        var handler = new PerfilRequirementHandler(service);
        var requirement = new PerfilRequirement(PerfisInternos.Administrador);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task AtendenteAcessaPoliticaAdminOuAtendente()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Atendente",
            "atendente@empresa.com",
            "atendente",
            "Ativo",
            null,
            "AzureAd",
            [PerfisInternos.Atendente],
            [PermissoesInternas.ChamadosAtender]));

        var handler = new PerfilRequirementHandler(service);
        var requirement = new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task SolicitanteNaoAcessaPoliticaAdministrador()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Solicitante",
            "solicitante@empresa.com",
            "solicitante",
            "Ativo",
            null,
            "AzureAd",
            [PerfisInternos.Solicitante],
            [PermissoesInternas.ChamadosCriar]));

        var handler = new PerfilRequirementHandler(service);
        var requirement = new PerfilRequirement(PerfisInternos.Administrador);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task UsuarioSemPerfilNaoAcessaAreaRestrita()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Sem Perfil",
            "semperfil@empresa.com",
            "semperfil",
            "Ativo",
            null,
            "AzureAd",
            [],
            []));

        var handler = new PerfilRequirementHandler(service);
        var requirement = new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task PolicyDePermissaoLiberaUsuarioComCodigo()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Atendente",
            "atendente@empresa.com",
            "atendente",
            "Ativo",
            null,
            "AzureAd",
            [PerfisInternos.Atendente],
            [PermissoesConstants.ChamadosAssumir]));

        var handler = new PermissionAuthorizationHandler(service, CriarHttpContextAccessor());
        var requirement = new PermissionRequirement(PermissoesConstants.ChamadosAssumir);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task PolicyDePermissaoNaoLiberaSemCodigo()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(),
            "Atendente",
            "atendente@empresa.com",
            "atendente",
            "Ativo",
            null,
            "AzureAd",
            [PerfisInternos.Atendente],
            [PermissoesConstants.ChamadosAtribuir]));

        var handler = new PermissionAuthorizationHandler(service, CriarHttpContextAccessor());
        var requirement = new PermissionRequirement(PermissoesConstants.ChamadosAssumir);
        var context = CriarContexto(requirement);

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    private static AuthorizationHandlerContext CriarContexto(IAuthorizationRequirement requirement)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "Teste")],
            "Bearer"));

        return new AuthorizationHandlerContext([requirement], user, resource: null);
    }

    private static IHttpContextAccessor CriarHttpContextAccessor()
        => new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };

    private sealed class StubUsuarioAtualService(UsuarioAutenticadoContexto contexto) : IUsuarioAtualService
    {
        public Task<UsuarioAutenticadoContexto> ObterAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(contexto);
    }
}
