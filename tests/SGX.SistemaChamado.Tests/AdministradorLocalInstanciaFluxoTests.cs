using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

[Collection("EnvironmentVariables")]
public sealed class AdministradorLocalInstanciaFluxoTests
{
    private const string VariavelEmail = "SGX_ADMIN_INICIAL_EMAIL";
    private const string VariavelSenha = "SGX_ADMIN_INICIAL_SENHA";
    private const string VariavelNome = "SGX_ADMIN_INICIAL_NOME";

    [Fact]
    public async Task AdministradorInicialPrimeiroLoginExigeTrocaESegundoLoginNaoExige()
    {
        using var contexto = PortalUseCasesTestFactory.CriarContexto();
        var options = CriarAuthOptions();
        var adminService = CriarAdministradorInicialService(contexto);

        const string email = "admin.instancia@empresa.com";
        const string senhaInicial = "Senha@Inicial123";
        const string senhaNova = "Senha@Nova123456";

        using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
        {
            [VariavelEmail] = email,
            [VariavelSenha] = senhaInicial,
            [VariavelNome] = "Administrador da Instancia"
        });

        await adminService.SeedAsync();

        var autenticacaoService = CriarAutenticacaoLocalService(contexto, options);
        var loginInicial = await autenticacaoService.LoginAsync(new LocalLoginRequest
        {
            Email = email,
            Senha = senhaInicial
        });

        Assert.True(loginInicial.DeveAlterarSenha);
        Assert.Equal("LocalSgx", loginInicial.AutenticadoPor);

        var usuario = await contexto.Usuarios.SingleAsync(x => x.Email == email);
        var gestaoSenhaService = CriarGestaoSenhaService(contexto, options);
        await gestaoSenhaService.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = senhaInicial,
            NovaSenha = senhaNova,
            ConfirmacaoNovaSenha = senhaNova
        });

        var loginNovo = await autenticacaoService.LoginAsync(new LocalLoginRequest
        {
            Email = email,
            Senha = senhaNova
        });

        Assert.False(loginNovo.DeveAlterarSenha);
        var usuarioAtualizado = await contexto.Usuarios.SingleAsync(x => x.Id == usuario.Id);
        Assert.False(usuarioAtualizado.DeveAlterarSenha);
    }

    [Fact]
    public async Task AdministradorInicialInativoNaoConsegueLoginLocalSgx()
    {
        using var contexto = PortalUseCasesTestFactory.CriarContexto();
        var options = CriarAuthOptions();
        var adminService = CriarAdministradorInicialService(contexto);

        const string email = "admin.inativo.instancia@empresa.com";
        const string senha = "Senha@Inicial123";

        using var _ = new EscopoVariaveisAmbiente(new Dictionary<string, string?>
        {
            [VariavelEmail] = email,
            [VariavelSenha] = senha,
            [VariavelNome] = "Administrador Inativo"
        });

        await adminService.SeedAsync();

        var usuario = await contexto.Usuarios.SingleAsync(x => x.Email == email);
        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        await contexto.SaveChangesAsync();

        var autenticacaoService = CriarAutenticacaoLocalService(contexto, options);

        await Assert.ThrowsAsync<AcessoNegadoException>(() => autenticacaoService.LoginAsync(new LocalLoginRequest
        {
            Email = email,
            Senha = senha
        }));
    }

    private static AdministradorInicialService CriarAdministradorInicialService(SGXSistemaChamadoDbContext contexto)
    {
        return new AdministradorInicialService(
            contexto,
            new PasswordHasher<Usuario>(),
            new PoliticaSenhaService(
                Options.Create(CriarAuthOptions()),
                new PasswordHasher<Usuario>()),
            NullLogger<AdministradorInicialService>.Instance);
    }

    private static AutenticacaoLocalSgxService CriarAutenticacaoLocalService(
        SGXSistemaChamadoDbContext contexto,
        AuthOptions options)
    {
        return new AutenticacaoLocalSgxService(
            contexto,
            new PasswordHasher<Usuario>(),
            Options.Create(options),
            new FakeMetodosLoginAdminService(),
            NullLogger<AutenticacaoLocalSgxService>.Instance);
    }

    private static GestaoSenhaLocalSgxService CriarGestaoSenhaService(
        SGXSistemaChamadoDbContext contexto,
        AuthOptions options)
    {
        var hasher = new PasswordHasher<Usuario>();
        return new GestaoSenhaLocalSgxService(
            contexto,
            hasher,
            new PoliticaSenhaService(Options.Create(options), hasher),
            Options.Create(options),
            new TokenRecuperacaoSenhaService(),
            NullLogger<GestaoSenhaLocalSgxService>.Instance);
    }

    private static AuthOptions CriarAuthOptions()
    {
        return new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = true,
            ModoLocalHabilitado = false,
            JwtLocalIssuer = "SGX.Local.Testes",
            JwtLocalAudience = "SGX.SistemaChamado.Api",
            JwtLocalChaveAssinatura = "sgx-testes-login-local-chave-com-minimo-32-caracteres",
            JwtLocalExpiracaoMinutos = 120,
            Provedores = new ProvedoresAutenticacaoOptions
            {
                Configurados = [CodigoProvedorAutenticacao.LocalSgx],
                Habilitados = [CodigoProvedorAutenticacao.LocalSgx],
                Principal = CodigoProvedorAutenticacao.LocalSgx,
                Ordem = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    [CodigoProvedorAutenticacao.LocalSgx] = 10
                }
            }
        };
    }

    private sealed class EscopoVariaveisAmbiente : IDisposable
    {
        private readonly Dictionary<string, string?> _anteriores = [];

        public EscopoVariaveisAmbiente(IDictionary<string, string?> valores)
        {
            foreach (var item in valores)
            {
                _anteriores[item.Key] = Environment.GetEnvironmentVariable(item.Key);
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        }

        public void Dispose()
        {
            foreach (var item in _anteriores)
            {
                Environment.SetEnvironmentVariable(item.Key, item.Value);
            }
        }
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
}
