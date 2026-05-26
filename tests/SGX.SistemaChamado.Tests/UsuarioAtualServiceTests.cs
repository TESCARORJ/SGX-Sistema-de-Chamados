using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authentication;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class UsuarioAtualServiceTests
{
    [Fact]
    public async Task DeveIdentificarUsuarioPorPreferredUsernameComoPrioridade()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuarioPrincipal = await CriarUsuarioComPerfilAsync(
            context,
            "Maria Principal",
            "maria.principal@empresa.com",
            "maria.principal@empresa.com",
            TipoPerfil.Solicitante);

        await CriarUsuarioComPerfilAsync(
            context,
            "Maria Fallback",
            "maria.fallback@empresa.com",
            "maria.fallback@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "maria.principal@empresa.com"),
                new Claim("email", "maria.fallback@empresa.com"),
                new Claim("name", "Maria Principal")
            ]);

        var resultado = await service.ObterAsync();

        Assert.Equal(usuarioPrincipal.Id, resultado.Id);
        Assert.Equal("maria.principal@empresa.com", resultado.Email);
    }

    [Fact]
    public async Task DeveUsarEmailComoFallbackQuandoPreferredUsernameNaoExiste()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = await CriarUsuarioComPerfilAsync(
            context,
            "Email Fallback",
            "email.fallback@empresa.com",
            "email.fallback@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("email", "email.fallback@empresa.com"),
                new Claim("name", "Email Fallback")
            ]);

        var resultado = await service.ObterAsync();

        Assert.Equal(usuario.Id, resultado.Id);
        Assert.Equal("email.fallback@empresa.com", resultado.Login);
    }

    [Fact]
    public async Task DeveUsarUpnComoFallbackQuandoEmailNaoExiste()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = await CriarUsuarioComPerfilAsync(
            context,
            "Upn Fallback",
            "upn.fallback@empresa.com",
            "upn.fallback@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("upn", "upn.fallback@empresa.com"),
                new Claim("name", "Upn Fallback")
            ]);

        var resultado = await service.ObterAsync();

        Assert.Equal(usuario.Id, resultado.Id);
        Assert.Equal("upn.fallback@empresa.com", resultado.Login);
    }

    [Fact]
    public async Task DeveRetornarErroControladoQuandoNaoHaIdentificadorConfiavel()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("name", "Sem Identificador"),
                new Claim("sub", Guid.NewGuid().ToString())
            ]);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterAsync());

        Assert.Contains("Nao foi possivel identificar o usuario autenticado", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task DeveCriarUsuarioSolicitanteSeNaoExistirQuandoCriacaoAutomaticaHabilitada()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "novo.usuario@empresa.com"),
                new Claim("name", "Novo Usuario")
            ],
            options: CriarAuthOptions());

        var resultado = await service.ObterAsync();

        Assert.Equal("novo.usuario@empresa.com", resultado.Email);
        Assert.Contains(PerfisInternos.Solicitante, resultado.Perfis);

        var usuarioCriado = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .SingleAsync(x => x.Email == "novo.usuario@empresa.com");

        Assert.Contains(usuarioCriado.UsuarioPerfis, p => p.PerfilAcesso.Nome == PerfisInternos.Solicitante);
    }

    [Fact]
    public async Task DeveBloquearUsuarioNovoQuandoCriacaoAutomaticaDesabilitada()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "bloqueado@empresa.com"),
                new Claim("name", "Usuario Bloqueado")
            ],
            options: CriarAuthOptions(criarUsuarioAutomaticamente: false));

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());

        Assert.Equal("Usuário não provisionado no SGX Sistema de Chamados.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearUsuarioInternoInativoMesmoComTokenValido()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var usuario = await CriarUsuarioComPerfilAsync(
            context,
            "Usuario Inativo",
            "inativo@empresa.com",
            "inativo@empresa.com",
            TipoPerfil.Solicitante);

        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        await context.SaveChangesAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "inativo@empresa.com"),
                new Claim("name", "Usuario Inativo")
            ]);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());

        Assert.Equal("Usuário inativo no SGX Sistema de Chamados.", ex.Message);
    }

    [Fact]
    public async Task DeveAceitarUsuarioQuandoDominioPermitidoConfigurado()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        await CriarUsuarioComPerfilAsync(
            context,
            "Dominio Permitido",
            "permitido@sgxdigital.com",
            "permitido@sgxdigital.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "permitido@sgxdigital.com"),
                new Claim("name", "Dominio Permitido")
            ],
            options: CriarAuthOptions(dominiosPermitidos: ["sgxdigital.com", "crea-rj.org.br"]));

        var resultado = await service.ObterAsync();

        Assert.Equal("permitido@sgxdigital.com", resultado.Email);
    }

    [Fact]
    public async Task DeveBloquearUsuarioQuandoDominioNaoPermitido()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "fora@dominio.com"),
                new Claim("name", "Dominio Bloqueado")
            ],
            options: CriarAuthOptions(dominiosPermitidos: ["sgxdigital.com", "crea-rj.org.br"]));

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());

        Assert.Equal("Domínio do usuário não permitido para acesso ao sistema.", ex.Message);
    }

    [Fact]
    public async Task DeveRetornarPerfisInternosEPermissoesSemDuplicidade()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var perfilAdmin = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Administrador);
        var perfilAtendente = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == TipoPerfil.Atendente);
        var usuario = new Usuario("Admin User", "admin@empresa.com", "admin@empresa.com", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAdmin.Id, "teste"));
        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfilAtendente.Id, "teste"));
        await context.SaveChangesAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "admin@empresa.com"),
                new Claim("name", "Admin User")
            ]);

        var resultado = await service.ObterAsync();

        Assert.Contains(PerfisInternos.Administrador, resultado.Perfis);
        Assert.Contains(PerfisInternos.Atendente, resultado.Perfis);
        Assert.Contains("Usuarios.Gerenciar", resultado.Permissoes);
        Assert.Contains("Chamados.Assumir", resultado.Permissoes);

        var duplicadas = resultado.Permissoes
            .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToArray();

        Assert.Empty(duplicadas);
    }

    [Fact]
    public async Task DeveRetornarAutenticadoPorMicrosoftEntraIdNoFluxoBearer()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        await CriarUsuarioComPerfilAsync(
            context,
            "Autenticacao Microsoft",
            "microsoft@empresa.com",
            "microsoft@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "microsoft@empresa.com"),
                new Claim("name", "Autenticacao Microsoft")
            ],
            authType: "Bearer");

        var resultado = await service.ObterAsync();

        Assert.Equal("MicrosoftEntraId", resultado.AutenticadoPor);
    }

    [Fact]
    public async Task DeveBloquearTokenMicrosoftComTidDiferenteDoTenantConfigurado()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "usuario@empresa.com"),
                new Claim("name", "Usuario Externo"),
                new Claim("tid", "33333333-3333-3333-3333-333333333333")
            ]);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Tenant Microsoft não autorizado.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearTokenMicrosoftSemTid()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "usuario@empresa.com"),
                new Claim("name", "Usuario Sem Tid")
            ],
            claimsMicrosoftPadrao: false);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Token Microsoft inválido para este ambiente.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearContaMicrosoftPessoalNoFluxoCorporativo()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "usuario@outlook.com"),
                new Claim("name", "Usuario Pessoal"),
                new Claim("tid", "9188040d-6c67-4c5b-b112-36a304b66dad")
            ]);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Conta Microsoft não permitida para este ambiente.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearTokenMicrosoftComIssuerInvalido()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "usuario@empresa.com"),
                new Claim("name", "Usuario Issuer Invalido"),
                new Claim("iss", "https://login.microsoftonline.com/outro-tenant/v2.0")
            ]);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Token Microsoft inválido para este ambiente.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearTokenMicrosoftComAudienceInvalida()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "usuario@empresa.com"),
                new Claim("name", "Usuario Audience Invalida"),
                new Claim("aud", "api://audience-invalida")
            ]);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Token Microsoft inválido para este ambiente.", ex.Message);
    }

    [Fact]
    public async Task DeveBloquearTokenMicrosoftSemOid()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var claims = new List<Claim>
        {
            new("preferred_username", "usuario@empresa.com"),
            new("name", "Usuario Sem Oid"),
            new("tid", "11111111-1111-1111-1111-111111111111"),
            new("iss", "https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111/v2.0"),
            new("aud", "api://sgx.sistema.chamado")
        };

        var service = CriarService(context, claims, claimsMicrosoftPadrao: false);

        var ex = await Assert.ThrowsAsync<AcessoNegadoException>(() => service.ObterAsync());
        Assert.Equal("Token Microsoft inválido para este ambiente.", ex.Message);
    }

    [Fact]
    public async Task DeveRetornarAutenticadoPorLocalSgxNoFluxoBearerLocal()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        await CriarUsuarioComPerfilAsync(
            context,
            "Autenticacao Local",
            "local@empresa.com",
            "local@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "local@empresa.com"),
                new Claim("name", "Autenticacao Local"),
                new Claim("auth_provider", "LocalSgx")
            ],
            authType: AuthSchemes.BearerLocalSgx);

        var resultado = await service.ObterAsync();

        Assert.Equal("LocalSgx", resultado.AutenticadoPor);
    }

    [Fact]
    public async Task DeveManterAutorizacaoInternaMesmoComRolesEGroupsDoAzure()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        await CriarUsuarioComPerfilAsync(
            context,
            "Usuario Solicitante",
            "solicitante@empresa.com",
            "solicitante@empresa.com",
            TipoPerfil.Solicitante);

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "solicitante@empresa.com"),
                new Claim("name", "Usuario Solicitante"),
                new Claim("roles", "Administrador"),
                new Claim("groups", Guid.NewGuid().ToString())
            ]);

        var resultado = await service.ObterAsync();

        Assert.Contains(PerfisInternos.Solicitante, resultado.Perfis);
        Assert.DoesNotContain(PerfisInternos.Administrador, resultado.Perfis);
    }

    [Fact]
    public async Task DeveManterLoginLocalDevelopmentEEmulacaoDePerfil()
    {
        using var context = CriarContexto();
        await context.Database.EnsureCreatedAsync();

        var service = CriarService(
            context,
            [
                new Claim("preferred_username", "admin@sgxdigital.com"),
                new Claim("email", "admin@sgxdigital.com"),
                new Claim("name", "Administrador SGX"),
                new Claim("sgx_dev_role", PerfisInternos.Administrador),
                new Claim(ClaimTypes.Role, PerfisInternos.Administrador)
            ],
            authType: AuthSchemes.LocalDevelopment,
            options: CriarAuthOptions(modoLocalHabilitado: true));

        var resultado = await service.ObterAsync();

        Assert.Equal(AuthSchemes.LocalDevelopment, resultado.AutenticadoPor);
        Assert.Contains(PerfisInternos.Administrador, resultado.Perfis);
    }

    private static AuthOptions CriarAuthOptions(
        bool modoLocalHabilitado = false,
        bool criarUsuarioAutomaticamente = true,
        string[]? dominiosPermitidos = null,
        string perfilPadraoUsuarioMicrosoft = PerfisInternos.Solicitante)
    {
        return new AuthOptions
        {
            ModoLocalHabilitado = modoLocalHabilitado,
            AdminLocalEmail = "admin.local@sgx.local",
            AdminLocalNome = "Administrador Local",
            DominiosPermitidos = dominiosPermitidos ?? [],
            CriarUsuarioAutomaticamente = criarUsuarioAutomaticamente,
            PerfilPadraoUsuarioMicrosoft = perfilPadraoUsuarioMicrosoft
        };
    }

    private static UsuarioAtualService CriarService(
        SGXSistemaChamadoDbContext context,
        IEnumerable<Claim> claims,
        string authType = "Bearer",
        AuthOptions? options = null,
        string environmentName = "Development",
        AzureAdOptions? azureOptions = null,
        bool claimsMicrosoftPadrao = true)
    {
        var claimsLista = claims.ToList();

        if (claimsMicrosoftPadrao && authType == "Bearer")
        {
            AdicionarClaimSeAusente(claimsLista, "tid", "11111111-1111-1111-1111-111111111111");
            AdicionarClaimSeAusente(claimsLista, "iss", "https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111/v2.0");
            AdicionarClaimSeAusente(claimsLista, "aud", "api://sgx.sistema.chamado");
            AdicionarClaimSeAusente(claimsLista, "oid", Guid.NewGuid().ToString());
        }

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claimsLista, authType));

        var accessor = new HttpContextAccessor
        {
            HttpContext = httpContext
        };

        return new UsuarioAtualService(
            accessor,
            context,
            new FakeEnvironment { EnvironmentName = environmentName },
            Options.Create(options ?? CriarAuthOptions()),
            Options.Create(azureOptions ?? CriarAzureAdOptions()),
            new FakeMetodosLoginAdminService(options ?? CriarAuthOptions(), environmentName),
            new FakeConfiguracaoIntegracaoMicrosoftService(
                options ?? CriarAuthOptions(),
                azureOptions ?? CriarAzureAdOptions(),
                environmentName));
    }

    private static AzureAdOptions CriarAzureAdOptions()
    {
        return new AzureAdOptions
        {
            Instance = "https://login.microsoftonline.com/",
            TenantId = "11111111-1111-1111-1111-111111111111",
            ClientId = "22222222-2222-2222-2222-222222222222",
            Audience = "api://sgx.sistema.chamado",
            Issuer = "https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111/v2.0"
        };
    }

    private static void AdicionarClaimSeAusente(List<Claim> claims, string tipo, string valor)
    {
        if (!claims.Any(x => string.Equals(x.Type, tipo, StringComparison.Ordinal)))
        {
            claims.Add(new Claim(tipo, valor));
        }
    }

    private static async Task<Usuario> CriarUsuarioComPerfilAsync(
        SGXSistemaChamadoDbContext context,
        string nome,
        string email,
        string login,
        TipoPerfil tipoPerfil)
    {
        var perfil = await context.PerfisAcesso.FirstAsync(x => x.TipoPerfil == tipoPerfil);
        var usuario = new Usuario(nome, email, login, "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        context.UsuariosPerfisAcesso.Add(new UsuarioPerfilAcesso(usuario.Id, perfil.Id, "teste"));
        await context.SaveChangesAsync();

        return usuario;
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

    private sealed class FakeConfiguracaoIntegracaoMicrosoftService(
        AuthOptions authOptions,
        AzureAdOptions azureAdOptions,
        string environmentName) : IConfiguracaoIntegracaoMicrosoftService
    {
        public Task<MicrosoftEntraIdIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default)
        {
            var efetiva = CriarEfetiva();
            return Task.FromResult(new MicrosoftEntraIdIntegracaoResponse(
                Habilitado: efetiva.MicrosoftHabilitado,
                ProvedorPrincipal: efetiva.ProvedorPrincipal,
                LoginLocalHabilitado: efetiva.LoginLocalSgxHabilitado,
                TenantId: efetiva.TenantId,
                ClientId: efetiva.ClientId,
                Audience: efetiva.Audience,
                Issuer: efetiva.Issuer,
                Authority: efetiva.Authority,
                ApiScope: efetiva.ApiScope,
                RedirectUri: efetiva.RedirectUri,
                DominiosPermitidos: efetiva.DominiosPermitidos,
                CriarUsuarioAutomaticamente: efetiva.CriarUsuarioAutomaticamente,
                PerfilPadraoUsuarioMicrosoft: efetiva.PerfilPadraoUsuarioMicrosoft,
                StatusConfiguracao: "Configurado",
                PendenciasConfiguracao: []));
        }

        public Task<MicrosoftEntraIdIntegracaoResponse> AtualizarConfiguracaoAsync(
            AtualizarMicrosoftEntraIdIntegracaoRequest request,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<ProvedoresAutenticacaoResponse> ObterProvedoresAutenticacaoAsync(CancellationToken cancellationToken = default)
        {
            var efetiva = CriarEfetiva();
            var provedores = new List<ProvedorAutenticacaoDto>();
            if (efetiva.MicrosoftHabilitado)
            {
                provedores.Add(new ProvedorAutenticacaoDto(
                    Codigo: CodigoProvedorAutenticacao.MicrosoftEntraId,
                    Nome: "Microsoft Entra ID",
                    Descricao: string.Empty,
                    Habilitado: true,
                    Principal: true,
                    Ordem: 10));
            }

            if (efetiva.LoginLocalSgxHabilitado)
            {
                provedores.Add(new ProvedorAutenticacaoDto(
                    Codigo: CodigoProvedorAutenticacao.LocalSgx,
                    Nome: "Local SGX",
                    Descricao: string.Empty,
                    Habilitado: true,
                    Principal: !efetiva.MicrosoftHabilitado,
                    Ordem: 30));
            }

            return Task.FromResult(new ProvedoresAutenticacaoResponse(
                Provedores: provedores));
        }

        public Task<ConfiguracaoAutenticacaoEfetiva> ObterConfiguracaoAutenticacaoEfetivaAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(CriarEfetiva());

        private ConfiguracaoAutenticacaoEfetiva CriarEfetiva()
        {
            var provedorPrincipal = authOptions.ObterProvedorPrincipalNormalizado();
            var usaMicrosoft = authOptions.UsaMicrosoftComoPrincipalOuHibrido();
            var usaLocal = authOptions.UsaLoginLocalSgxComoPrincipalOuHibrido();
            var microsoftConfigurado = azureAdOptions.EstaConfigurado();

            return new ConfiguracaoAutenticacaoEfetiva(
                MicrosoftHabilitado: usaMicrosoft && microsoftConfigurado,
                LoginLocalSgxHabilitado: usaLocal && authOptions.LoginLocalHabilitado,
                LoginLocalDevelopmentHabilitado: string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase) && authOptions.ModoLocalHabilitado,
                ProvedorPrincipal: provedorPrincipal,
                CriarUsuarioAutomaticamente: authOptions.CriarUsuarioAutomaticamente,
                PerfilPadraoUsuarioMicrosoft: authOptions.PerfilPadraoUsuarioMicrosoft,
                DominiosPermitidos: authOptions.DominiosPermitidos,
                TenantId: azureAdOptions.TenantId,
                ClientId: azureAdOptions.ClientId,
                Audience: azureAdOptions.Audience,
                Issuer: azureAdOptions.Issuer,
                Authority: azureAdOptions.BuildAuthority(),
                ApiScope: string.Empty,
                RedirectUri: string.Empty);
        }
    }

    private sealed class FakeMetodosLoginAdminService(
        AuthOptions authOptions,
        string environmentName) : IMetodosLoginAdminService
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
        {
            var habilitado = authOptions.ObterCodigosProvedoresHabilitadosNormalizados()
                .Contains(codigoProvedor, StringComparer.OrdinalIgnoreCase);
            return Task.FromResult(habilitado);
        }

        public Task<MetodoLoginEfetivo?> ObterMetodoEfetivoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
        {
            var habilitado = authOptions.ObterCodigosProvedoresHabilitadosNormalizados()
                .Contains(codigoProvedor, StringComparer.OrdinalIgnoreCase);
            var funcional = !string.Equals(codigoProvedor, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase)
                || string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase);

            var metodo = new MetodoLoginEfetivo(
                Codigo: codigoProvedor,
                Nome: codigoProvedor,
                Descricao: codigoProvedor,
                Configurado: true,
                Habilitado: habilitado,
                Principal: string.Equals(authOptions.ObterCodigoProvedorPrincipalNormalizado(), codigoProvedor, StringComparison.OrdinalIgnoreCase),
                Ordem: 10,
                PermiteAutoProvisionamento: authOptions.CriarUsuarioAutomaticamente,
                PerfilPadraoAutoProvisionamento: authOptions.PerfilPadraoUsuarioMicrosoft,
                RotuloExibicao: codigoProvedor,
                Funcional: funcional,
                PodeHabilitar: funcional,
                MotivoBloqueioHabilitar: funcional ? null : "Provedor indisponivel no ambiente atual.");

            return Task.FromResult<MetodoLoginEfetivo?>(metodo);
        }
    }
}
