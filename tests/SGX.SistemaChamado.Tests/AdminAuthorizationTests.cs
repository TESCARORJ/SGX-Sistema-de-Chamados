using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Services;

namespace SGX.SistemaChamado.Tests;

public sealed class AdminAuthorizationTests
{
    [Fact]
    public async Task SolicitanteNaoAcessaEndpointsAdministrativos()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(), "Solicitante", "sol@empresa.com", "sol", "Ativo", null, "AzureAd", [PerfisInternos.Solicitante], []));

        var handler = new PerfilRequirementHandler(service);
        var context = CriarContexto(new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente));

        await handler.HandleAsync(context);

        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task AtendenteAcessaFilaAdministrativa()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(), "Atendente", "aten@empresa.com", "aten", "Ativo", null, "AzureAd", [PerfisInternos.Atendente], []));

        var handler = new PerfilRequirementHandler(service);
        var context = CriarContexto(new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task AdministradorAcessaFilaAdministrativa()
    {
        var service = new StubUsuarioAtualService(new UsuarioAutenticadoContexto(
            Guid.NewGuid(), "Administrador", "admin@empresa.com", "admin", "Ativo", null, "AzureAd", [PerfisInternos.Administrador], []));

        var handler = new PerfilRequirementHandler(service);
        var context = CriarContexto(new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente));

        await handler.HandleAsync(context);

        Assert.True(context.HasSucceeded);
    }

    private static AuthorizationHandlerContext CriarContexto(IAuthorizationRequirement requirement)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "Teste")], "Bearer"));
        return new AuthorizationHandlerContext([requirement], user, null);
    }

    private sealed class StubUsuarioAtualService(UsuarioAutenticadoContexto contexto) : IUsuarioAtualService
    {
        public Task<UsuarioAutenticadoContexto> ObterAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(contexto);
    }
}
