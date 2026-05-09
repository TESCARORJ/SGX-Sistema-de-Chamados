using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class UsuarioAtualServiceTests
{
    [Fact]
    public async Task DeveIdentificarUsuarioPorEmail()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilSolicitante = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante);
        var usuario = new Usuario("Maria Silva", "maria@empresa.com", "maria", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilSolicitante.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(context, "maria@empresa.com", "Maria Silva");

        var resultado = await service.ObterAsync();

        Assert.Equal(usuario.Id, resultado.Id);
        Assert.Equal("maria@empresa.com", resultado.Email);
    }

    [Fact]
    public async Task DeveCriarUsuarioSolicitanteSeNaoExistir()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(context, "novo.usuario@empresa.com", "Novo Usuario");

        var resultado = await service.ObterAsync();

        Assert.Equal("novo.usuario@empresa.com", resultado.Email);
        Assert.Contains(PerfisInternos.Solicitante, resultado.Perfis);

        var usuarioCriado = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .SingleAsync(x => x.Email == "novo.usuario@empresa.com");

        Assert.Contains(usuarioCriado.UsuarioPerfis, p => p.PerfilAcesso.TipoPerfil == TipoPerfil.Solicitante);
    }

    [Fact]
    public async Task DeveAtualizarUltimoAcesso()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilSolicitante = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Solicitante);
        var usuario = new Usuario("Joao Tester", "joao@empresa.com", "joao", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilSolicitante.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(context, "joao@empresa.com", "Joao Tester");
        var antes = usuario.UltimoAcessoEm;

        var resultado = await service.ObterAsync();

        Assert.NotNull(resultado);
        Assert.NotNull(usuario.UltimoAcessoEm);
        Assert.NotEqual(antes, usuario.UltimoAcessoEm);
    }

    [Fact]
    public async Task DeveRetornarPerfisInternosEPermissoes()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilAdmin = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        var usuario = new Usuario("Admin User", "admin@empresa.com", "admin", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAdmin.Id, "teste"));
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAtendente.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(context, "admin@empresa.com", "Admin User");
        var resultado = await service.ObterAsync();

        Assert.Contains(PerfisInternos.Administrador, resultado.Perfis);
        Assert.Contains(PerfisInternos.Atendente, resultado.Perfis);
        Assert.Contains("Usuarios.Gerenciar", resultado.Permissoes);
        Assert.Contains("Chamados.Assumir", resultado.Permissoes);
    }

    [Fact]
    public async Task DeveRetornarPermissoesSemDuplicidade()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilAdmin = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        var usuario = new Usuario("User Multi Perfil", "multi@empresa.com", "multi", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAdmin.Id, "teste"));
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAtendente.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(context, "multi@empresa.com", "User Multi Perfil");
        var resultado = await service.ObterAsync();

        var duplicadas = resultado.Permissoes
            .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToArray();

        Assert.Empty(duplicadas);
    }

    [Fact]
    public async Task DeveIgnorarPerfilEPermissaoInativosNoRetorno()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilAdmin = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        var permissaoVisualizarTodos = await context.PermissoesSistema.FirstAsync(x => x.Codigo == "Chamados.VisualizarTodos");

        perfilAtendente.Desativar("teste");
        permissaoVisualizarTodos.Desativar("teste");
        await context.SaveChangesAsync();

        var usuario = new Usuario("Usuario Filtro", "filtro@empresa.com", "filtro", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAdmin.Id, "teste"));
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAtendente.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(context, "filtro@empresa.com", "Usuario Filtro");
        var resultado = await service.ObterAsync();

        Assert.DoesNotContain("Atendente", resultado.Perfis);
        Assert.DoesNotContain("Chamados.VisualizarTodos", resultado.Permissoes);
    }

    private static UsuarioAtualService CriarService(
        SGXSistemaChamadoDbContext context,
        string email,
        string nome)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
            [
                new Claim("preferred_username", email),
                new Claim("email", email),
                new Claim("name", nome),
                new Claim("oid", Guid.NewGuid().ToString())
            ],
            "Bearer"));

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        return new UsuarioAtualService(
            accessor,
            context,
            new FakeEnvironment(),
            Options.Create(new AuthOptions
            {
                ModoLocalHabilitado = false,
                AdminLocalEmail = "admin.local@sgx.local",
                AdminLocalNome = "Administrador Local"
            }));
    }

    private static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new SGXSistemaChamadoDbContext(options);
    }

    private sealed class FakeEnvironment : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
