using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class MetodosLoginAdminServiceTests
{
    [Fact]
    public async Task DeveRespeitarOrdemDeExibicaoQuandoAtualizada()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Development");

        await service.AtualizarConfiguracaoAdminAsync(new AtualizarMetodosLoginAdminRequest
        {
            Provedores =
            [
                Novo(CodigoProvedorAutenticacao.LocalSgx, true, false, 30),
                Novo(CodigoProvedorAutenticacao.ActiveDirectory, true, true, 10),
                Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, true, false, 20),
                Novo(CodigoProvedorAutenticacao.LocalDevelopment, false, false, 40)
            ]
        });

        var response = await service.ObterProvedoresPublicosAsync();
        var codigos = response.Provedores.Select(x => x.Codigo).ToArray();

        Assert.Equal(
        [
            CodigoProvedorAutenticacao.ActiveDirectory,
            CodigoProvedorAutenticacao.MicrosoftEntraId,
            CodigoProvedorAutenticacao.LocalSgx
        ], codigos);
    }

    [Fact]
    public async Task DeveRemoverProvedorDesabilitadoDaListaPublica()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Development");

        await service.AtualizarConfiguracaoAdminAsync(new AtualizarMetodosLoginAdminRequest
        {
            Provedores =
            [
                Novo(CodigoProvedorAutenticacao.LocalSgx, true, true, 10),
                Novo(CodigoProvedorAutenticacao.ActiveDirectory, false, false, 20),
                Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, false, false, 30),
                Novo(CodigoProvedorAutenticacao.LocalDevelopment, false, false, 40)
            ]
        });

        var response = await service.ObterProvedoresPublicosAsync();

        Assert.Single(response.Provedores);
        Assert.Equal(CodigoProvedorAutenticacao.LocalSgx, response.Provedores.Single().Codigo);
    }

    [Fact]
    public async Task NaoDevePermitirSalvarSemMetodoViavel()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(
            dbContext,
            "Production",
            adOptions: new ActiveDirectoryOptions(),
            microsoftHabilitado: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AtualizarConfiguracaoAdminAsync(
            new AtualizarMetodosLoginAdminRequest
            {
                Provedores =
                [
                    Novo(CodigoProvedorAutenticacao.LocalSgx, false, false, 10),
                    Novo(CodigoProvedorAutenticacao.ActiveDirectory, true, true, 20),
                    Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, false, false, 30)
                ]
            }));

        Assert.True(
            ex.Message.Contains("método de login viável", StringComparison.OrdinalIgnoreCase)
            || ex.Message.Contains("LocalSgx", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task NaoDevePermitirDesabilitarLocalSgxSemAlternativaAdministrativa()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Production", microsoftHabilitado: false);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AtualizarConfiguracaoAdminAsync(
            new AtualizarMetodosLoginAdminRequest
            {
                Provedores =
                [
                    Novo(CodigoProvedorAutenticacao.LocalSgx, false, false, 10),
                    Novo(CodigoProvedorAutenticacao.ActiveDirectory, false, false, 20),
                    Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, true, true, 30)
                ]
            }));

        Assert.Contains("LocalSgx", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveBloquearPrincipalDuplicado()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Development");

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AtualizarConfiguracaoAdminAsync(
            new AtualizarMetodosLoginAdminRequest
            {
                Provedores =
                [
                    Novo(CodigoProvedorAutenticacao.LocalSgx, true, true, 10),
                    Novo(CodigoProvedorAutenticacao.ActiveDirectory, true, true, 20),
                    Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, false, false, 30),
                    Novo(CodigoProvedorAutenticacao.LocalDevelopment, false, false, 40)
                ]
            }));

        Assert.Contains("principal", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveOcultarLocalDevelopmentForaDeDevelopment()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Production");

        var response = await service.ObterConfiguracaoAdminAsync();

        Assert.DoesNotContain(response.Provedores, x => x.Codigo == CodigoProvedorAutenticacao.LocalDevelopment);
    }

    [Fact]
    public async Task DevePersistirAutoProvisionamentoEPerfilPadrao()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(dbContext, "Development");

        await service.AtualizarConfiguracaoAdminAsync(new AtualizarMetodosLoginAdminRequest
        {
            Provedores =
            [
                Novo(CodigoProvedorAutenticacao.LocalSgx, true, true, 10),
                Novo(CodigoProvedorAutenticacao.ActiveDirectory, true, false, 20, true, "Atendente"),
                Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, false, false, 30),
                Novo(CodigoProvedorAutenticacao.LocalDevelopment, false, false, 40)
            ]
        });

        var ad = await service.ObterMetodoEfetivoAsync(CodigoProvedorAutenticacao.ActiveDirectory);

        Assert.NotNull(ad);
        Assert.True(ad!.PermiteAutoProvisionamento);
        Assert.Equal("Atendente", ad.PerfilPadraoAutoProvisionamento);
    }

    [Fact]
    public async Task AlteracaoMetodosLoginDeveGerarAuditoriaAdministrativa()
    {
        await using var dbContext = CriarDbContext();
        var auditoria = new FakeAuditoriaService();
        var service = CriarService(dbContext, "Development", auditoriaService: auditoria);

        await service.AtualizarConfiguracaoAdminAsync(new AtualizarMetodosLoginAdminRequest
        {
            Provedores =
            [
                Novo(CodigoProvedorAutenticacao.LocalSgx, true, true, 10),
                Novo(CodigoProvedorAutenticacao.ActiveDirectory, false, false, 20),
                Novo(CodigoProvedorAutenticacao.MicrosoftEntraId, false, false, 30),
                Novo(CodigoProvedorAutenticacao.LocalDevelopment, false, false, 40)
            ]
        });

        Assert.Contains(auditoria.Eventos, x =>
            x.Modulo == "Autenticacao" &&
            x.Entidade == "MetodosLogin");
    }

    [Fact]
    public async Task ActiveDirectoryNaoDeveFicarViavelSemConfiguracaoTecnica()
    {
        await using var dbContext = CriarDbContext();
        var service = CriarService(
            dbContext,
            "Production",
            adOptions: new ActiveDirectoryOptions
            {
                Ativo = true,
                Servidor = "",
                Porta = 636,
                UsarLdaps = true,
                BaseDn = "",
                UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
            });

        var config = await service.ObterConfiguracaoAdminAsync();
        var ad = config.Provedores.Single(x => x.Codigo == CodigoProvedorAutenticacao.ActiveDirectory);

        Assert.False(ad.PodeHabilitar);
        Assert.False(ad.Funcional);
        Assert.False(string.IsNullOrWhiteSpace(ad.MotivoBloqueioHabilitar));
    }

    private static MetodoLoginAdminAtualizacaoDto Novo(
        string codigo,
        bool habilitado,
        bool principal,
        int ordem,
        bool permiteAutoProvisionamento = false,
        string perfilPadrao = "Solicitante")
    {
        return new MetodoLoginAdminAtualizacaoDto
        {
            Codigo = codigo,
            Habilitado = habilitado,
            Principal = principal,
            Ordem = ordem,
            PermiteAutoProvisionamento = permiteAutoProvisionamento,
            PerfilPadraoAutoProvisionamento = perfilPadrao,
            RotuloExibicao = codigo
        };
    }

    private static MetodosLoginAdminService CriarService(
        SGXSistemaChamadoDbContext dbContext,
        string environmentName,
        AuthOptions? authOptions = null,
        ActiveDirectoryOptions? adOptions = null,
        bool microsoftHabilitado = true,
        IAuditoriaService? auditoriaService = null)
    {
        return new MetodosLoginAdminService(
            dbContext,
            new FakeEnvironment(environmentName),
            Options.Create(authOptions ?? CriarAuthOptions()),
            new FakeConfiguracaoIntegracaoActiveDirectoryService(adOptions ?? CriarAdOptions()),
            new FakeConfiguracaoIntegracaoMicrosoftService(environmentName, microsoftHabilitado),
            Microsoft.Extensions.Logging.Abstractions.NullLogger<MetodosLoginAdminService>.Instance,
            auditoriaService);
    }

    private static SGXSistemaChamadoDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase($"sgx-metodos-login-{Guid.NewGuid():N}")
            .Options;

        return new SGXSistemaChamadoDbContext(options);
    }

    private static AuthOptions CriarAuthOptions()
    {
        return new AuthOptions
        {
            ProvedorPrincipal = ProvedorAutenticacao.Local,
            LoginLocalHabilitado = true,
            ModoLocalHabilitado = true,
            Provedores = new ProvedoresAutenticacaoOptions
            {
                Configurados =
                [
                    CodigoProvedorAutenticacao.MicrosoftEntraId,
                    CodigoProvedorAutenticacao.ActiveDirectory,
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.LocalDevelopment
                ],
                Habilitados =
                [
                    CodigoProvedorAutenticacao.LocalSgx,
                    CodigoProvedorAutenticacao.ActiveDirectory
                ],
                Principal = CodigoProvedorAutenticacao.LocalSgx,
                Ordem = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    [CodigoProvedorAutenticacao.MicrosoftEntraId] = 10,
                    [CodigoProvedorAutenticacao.ActiveDirectory] = 20,
                    [CodigoProvedorAutenticacao.LocalSgx] = 30,
                    [CodigoProvedorAutenticacao.LocalDevelopment] = 40
                }
            },
            JwtLocalIssuer = "SGX.Local",
            JwtLocalAudience = "SGX.Api",
            JwtLocalChaveAssinatura = "sgx-chave-local-super-segura-com-32-caracteres",
            JwtLocalExpiracaoMinutos = 120
        };
    }

    private static ActiveDirectoryOptions CriarAdOptions()
    {
        return new ActiveDirectoryOptions
        {
            Servidor = "ldaps://dc01.empresa.local",
            Porta = 636,
            UsarLdaps = true,
            PermitirLdapSemTls = false,
            Dominio = "EMPRESA",
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
            PermitirAutoProvisionamento = false,
            PerfilPadrao = "Solicitante"
        };
    }

    private sealed class FakeEnvironment(string environmentName) : Microsoft.Extensions.Hosting.IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "SGX.SistemaChamado.Tests";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } =
            new Microsoft.Extensions.FileProviders.NullFileProvider();
    }

    private sealed class FakeConfiguracaoIntegracaoMicrosoftService(
        string environmentName,
        bool microsoftHabilitado) : IConfiguracaoIntegracaoMicrosoftService
    {
        public Task<SGX.SistemaChamado.Api.Contracts.Admin.MicrosoftEntraIdIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<SGX.SistemaChamado.Api.Contracts.Admin.MicrosoftEntraIdIntegracaoResponse> AtualizarConfiguracaoAsync(
            SGX.SistemaChamado.Api.Contracts.Admin.AtualizarMicrosoftEntraIdIntegracaoRequest request,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<SGX.SistemaChamado.Api.Contracts.Auth.ProvedoresAutenticacaoResponse> ObterProvedoresAutenticacaoAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<ConfiguracaoAutenticacaoEfetiva> ObterConfiguracaoAutenticacaoEfetivaAsync(CancellationToken cancellationToken = default)
        {
            var dev = string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase);
            return Task.FromResult(new ConfiguracaoAutenticacaoEfetiva(
                MicrosoftHabilitado: microsoftHabilitado,
                LoginLocalSgxHabilitado: true,
                LoginLocalDevelopmentHabilitado: dev,
                ProvedorPrincipal: ProvedorAutenticacao.Local,
                CriarUsuarioAutomaticamente: true,
                PerfilPadraoUsuarioMicrosoft: "Solicitante",
                DominiosPermitidos: [],
                TenantId: "tenant",
                ClientId: "client",
                Audience: "aud",
                Issuer: "iss",
                Authority: "auth",
                ApiScope: "scope",
                RedirectUri: "http://localhost:5173"));
        }
    }

    private sealed class FakeConfiguracaoIntegracaoActiveDirectoryService(
        ActiveDirectoryOptions options) : IConfiguracaoIntegracaoActiveDirectoryService
    {
        public Task<SGX.SistemaChamado.Api.Contracts.Admin.ActiveDirectoryIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<SGX.SistemaChamado.Api.Contracts.Admin.ActiveDirectoryIntegracaoResponse> AtualizarConfiguracaoAsync(
            SGX.SistemaChamado.Api.Contracts.Admin.AtualizarActiveDirectoryIntegracaoRequest request,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<SGX.SistemaChamado.Api.Contracts.Admin.TestarConexaoActiveDirectoryResponse> TestarConexaoAsync(
            SGX.SistemaChamado.Api.Contracts.Admin.TestarConexaoActiveDirectoryRequest? request,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<SGX.SistemaChamado.Api.Contracts.Admin.TestarAutenticacaoActiveDirectoryResponse> TestarAutenticacaoAsync(
            SGX.SistemaChamado.Api.Contracts.Admin.TestarAutenticacaoActiveDirectoryRequest request,
            CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<ActiveDirectoryOptions> ObterConfiguracaoEfetivaAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(options);
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
}
