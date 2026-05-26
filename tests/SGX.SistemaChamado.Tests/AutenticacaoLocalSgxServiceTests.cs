using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class AutenticacaoLocalSgxServiceTests
{
    [Fact]
    public async Task DeveEmitirTokenQuandoCredenciaisSaoValidas()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "local@empresa.com", "local@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher);
        var response = await service.LoginAsync(new LocalLoginRequest
        {
            Email = "local@empresa.com",
            Senha = "Senha@123456"
        });

        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.Equal("LocalSgx", response.AutenticadoPor);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        Assert.Equal("LocalSgx", jwt.Claims.FirstOrDefault(x => x.Type == "auth_provider")?.Value);
    }

    [Fact]
    public async Task LoginLocalBemSucedidoDeveGerarEventoAuditoriaAutenticacao()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "audit.local@empresa.com", "audit.local@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var service = CriarService(context, hasher, auditoriaService: auditoria);
        _ = await service.LoginAsync(new LocalLoginRequest
        {
            Email = "audit.local@empresa.com",
            Senha = "Senha@123456"
        });

        var evento = Assert.Single(auditoria.Eventos.Where(x =>
            x.Modulo == "Autenticacao" &&
            x.Descricao.Contains("bem-sucedido", StringComparison.OrdinalIgnoreCase)));
        Assert.Equal(TipoAcaoAuditoria.Login, evento.Acao);
        Assert.DoesNotContain("Senha@123456", evento.Descricao, StringComparison.Ordinal);
        Assert.DoesNotContain("Senha@123456", evento.Metadados ?? string.Empty, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeveFalharQuandoSenhaForInvalida()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "local@empresa.com", "local@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LocalLoginRequest
        {
            Email = "local@empresa.com",
            Senha = "SenhaErrada"
        }));
    }

    [Fact]
    public async Task LoginLocalNegadoDeveGerarEventoAuditoriaAutenticacao()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "negado.local@empresa.com", "negado.local@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var auditoria = new FakeAuditoriaService();
        var service = CriarService(context, hasher, auditoriaService: auditoria);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LocalLoginRequest
        {
            Email = "negado.local@empresa.com",
            Senha = "invalida"
        }));

        Assert.Contains(auditoria.Eventos, x =>
            x.Modulo == "Autenticacao" &&
            x.Descricao.Contains("Falha de login local SGX", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task FalhaDeAuditoriaNaoDeveImpedirLoginLocal()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "resiliente.local@empresa.com", "resiliente.local@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher, auditoriaService: new ThrowingAuditoriaService());
        var response = await service.LoginAsync(new LocalLoginRequest
        {
            Email = "resiliente.local@empresa.com",
            Senha = "Senha@123456"
        });

        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
    }

    [Fact]
    public async Task DeveBloquearUsuarioInativo()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Inativo", "inativo@empresa.com", "inativo@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher);

        await Assert.ThrowsAsync<AcessoNegadoException>(() => service.LoginAsync(new LocalLoginRequest
        {
            Email = "inativo@empresa.com",
            Senha = "Senha@123456"
        }));
    }

    [Fact]
    public async Task IncrementaTentativasInvalidasEAplicaLockoutAoAtingirLimite()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "lockout@empresa.com", "lockout@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher, tentativasMaximas: 2, minutosBloqueio: 15);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LocalLoginRequest
        {
            Email = "lockout@empresa.com",
            Senha = "SenhaErrada"
        }));

        var usuarioDepoisPrimeiraFalha = await context.Usuarios.SingleAsync(x => x.Email == "lockout@empresa.com");
        Assert.Equal(1, usuarioDepoisPrimeiraFalha.TentativasInvalidas);
        Assert.Null(usuarioDepoisPrimeiraFalha.BloqueadoAte);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LocalLoginRequest
        {
            Email = "lockout@empresa.com",
            Senha = "SenhaErrada2"
        }));

        var usuarioBloqueado = await context.Usuarios.SingleAsync(x => x.Email == "lockout@empresa.com");
        Assert.Equal(0, usuarioBloqueado.TentativasInvalidas);
        Assert.True(usuarioBloqueado.BloqueadoAte.HasValue);
        Assert.True(usuarioBloqueado.BloqueadoAte.Value > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginComSucessoZeraTentativasERegistraUltimoLogin()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = new Usuario("Usuario Local", "sucesso@empresa.com", "sucesso@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, "Senha@123456"), "teste");
        usuario.RegistrarFalhaLoginLocal(5, TimeSpan.FromMinutes(15), DateTime.UtcNow, "teste");
        await context.SaveChangesAsync();

        var service = CriarService(context, hasher);

        var resposta = await service.LoginAsync(new LocalLoginRequest
        {
            Email = "sucesso@empresa.com",
            Senha = "Senha@123456"
        });

        var usuarioAtualizado = await context.Usuarios.SingleAsync(x => x.Email == "sucesso@empresa.com");
        Assert.NotNull(resposta.AccessToken);
        Assert.Equal(0, usuarioAtualizado.TentativasInvalidas);
        Assert.Null(usuarioAtualizado.BloqueadoAte);
        Assert.True(usuarioAtualizado.UltimoLoginEm.HasValue);
    }

    private static AutenticacaoLocalSgxService CriarService(
        SGXSistemaChamadoDbContext context,
        IPasswordHasher<Usuario> hasher,
        int tentativasMaximas = 5,
        int minutosBloqueio = 15,
        IMetodosLoginAdminService? metodosLoginAdminService = null,
        IAuditoriaService? auditoriaService = null)
    {
        return new AutenticacaoLocalSgxService(
            context,
            hasher,
            Options.Create(new AuthOptions
            {
                ProvedorPrincipal = ProvedorAutenticacao.Local,
                LoginLocalHabilitado = true,
                JwtLocalIssuer = "SGX.Local.Testes",
                JwtLocalAudience = "SGX.SistemaChamado.Api",
                JwtLocalChaveAssinatura = "sgx-testes-login-local-chave-com-minimo-32-caracteres",
                JwtLocalExpiracaoMinutos = 120,
                Lockout = new SGX.SistemaChamado.Api.Options.LockoutOptions
                {
                    TentativasMaximas = tentativasMaximas,
                    MinutosBloqueio = minutosBloqueio
                }
            }),
            metodosLoginAdminService ?? new FakeMetodosLoginAdminService(),
            NullLogger<AutenticacaoLocalSgxService>.Instance,
            auditoriaService);
    }

    private static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new SGXSistemaChamadoDbContext(options);
    }

    private sealed class FakeMetodosLoginAdminService : IMetodosLoginAdminService
    {
        public Task<MetodosLoginAdminResponse> ObterConfiguracaoAdminAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<MetodosLoginAdminResponse> AtualizarConfiguracaoAdminAsync(
            AtualizarMetodosLoginAdminRequest request,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<ProvedoresAutenticacaoResponse> ObterProvedoresPublicosAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(new ProvedoresAutenticacaoResponse([]));

        public Task<bool> ProvedorHabilitadoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
            => Task.FromResult(string.Equals(codigoProvedor, CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase));

        public Task<MetodoLoginEfetivo?> ObterMetodoEfetivoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
            => Task.FromResult<MetodoLoginEfetivo?>(null);
    }

    private sealed class FakeAuditoriaService : IAuditoriaService
    {
        public List<RegistrarEventoAuditoriaRequest> Eventos { get; } = [];

        public Task RegistrarAsync(RegistrarEventoAuditoriaRequest request, CancellationToken cancellationToken = default)
        {
            Eventos.Add(request);
            return Task.CompletedTask;
        }

        public Task RegistrarCriacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarEdicaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarExclusaoLogicaAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarAtivacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarInativacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarLoginAsync(bool sucesso, string descricao, string? mensagemErro = null, Guid? usuarioId = null, string? usuarioNome = null, string? usuarioEmail = null, string? usuarioLogin = null, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarLogoutAsync(string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
        public Task RegistrarErroAsync(string modulo, string entidade, string descricao, string? entidadeId = null, Exception? exception = null, string? metadados = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }

    private sealed class ThrowingAuditoriaService : IAuditoriaService
    {
        public Task RegistrarAsync(RegistrarEventoAuditoriaRequest request, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");

        public Task RegistrarCriacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarEdicaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? dadosDepois = null, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarExclusaoLogicaAsync(string modulo, string entidade, string entidadeId, string descricao, string? dadosAntes = null, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarAtivacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarInativacaoAsync(string modulo, string entidade, string entidadeId, string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarLoginAsync(bool sucesso, string descricao, string? mensagemErro = null, Guid? usuarioId = null, string? usuarioNome = null, string? usuarioEmail = null, string? usuarioLogin = null, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarLogoutAsync(string descricao, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
        public Task RegistrarErroAsync(string modulo, string entidade, string descricao, string? entidadeId = null, Exception? exception = null, string? metadados = null, CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("Falha proposital de auditoria.");
    }
}
