using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
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

public sealed class ActiveDirectoryAuthenticationServiceTests
{
    [Fact]
    public async Task DeveFalharQuandoProvedorActiveDirectoryEstiverDesabilitado()
    {
        await using var context = CriarContexto();
        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: false),
            CriarActiveDirectoryOptions(),
            new FakeCredentialValidator(_ => SucessoPadrao()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        }));
    }

    [Fact]
    public async Task DeveFalharQuandoSenhaForInvalidaNoActiveDirectory()
    {
        await using var context = CriarContexto();
        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(),
            new FakeCredentialValidator(_ => new ActiveDirectoryValidacaoResultado(
                Sucesso: false,
                UsuarioSamAccountName: "thiago",
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DistinguishedName: null)));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "SenhaErrada",
            Dominio = "EMPRESA"
        }));
    }

    [Fact]
    public async Task DeveFalharQuandoUsuarioNaoExisteEAutoProvisionamentoEstaDesativado()
    {
        await using var context = CriarContexto();
        await GarantirPerfilSolicitanteAsync(context);

        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(permitirAutoProvisionamento: false),
            new FakeCredentialValidator(_ => SucessoPadrao()));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        }));
    }

    [Fact]
    public async Task DeveAutoProvisionarQuandoConfigurado()
    {
        await using var context = CriarContexto();
        await GarantirPerfilSolicitanteAsync(context);

        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(permitirAutoProvisionamento: true, perfilPadrao: PerfisInternos.Solicitante),
            new FakeCredentialValidator(_ => SucessoPadrao()));

        var response = await service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        });

        var usuario = await context.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .SingleAsync(x => x.Login == "thiago");

        Assert.Equal("ActiveDirectory", response.AutenticadoPor);
        Assert.True(usuario.Ativo);
        Assert.Null(usuario.SenhaHashLocal);
        Assert.Contains(usuario.UsuarioPerfis, x => x.PerfilAcesso.Nome == PerfisInternos.Solicitante);
    }

    [Fact]
    public async Task LoginAdBemSucedidoDeveGerarEventoAuditoriaAutenticacao()
    {
        await using var context = CriarContexto();
        await GarantirPerfilSolicitanteAsync(context);

        var auditoria = new FakeAuditoriaService();
        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(permitirAutoProvisionamento: true, perfilPadrao: PerfisInternos.Solicitante),
            new FakeCredentialValidator(_ => SucessoPadrao()),
            auditoria);

        _ = await service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        });

        Assert.Contains(auditoria.Eventos, x =>
            x.Modulo == "Autenticacao" &&
            x.Descricao.Contains("Active Directory bem-sucedido", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task LoginAdNegadoDeveGerarEventoAuditoriaAutenticacao()
    {
        await using var context = CriarContexto();
        var auditoria = new FakeAuditoriaService();
        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(),
            new FakeCredentialValidator(_ => new ActiveDirectoryValidacaoResultado(
                Sucesso: false,
                UsuarioSamAccountName: "thiago",
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DistinguishedName: null)),
            auditoria);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "SenhaErrada",
            Dominio = "EMPRESA"
        }));

        Assert.Contains(auditoria.Eventos, x =>
            x.Modulo == "Autenticacao" &&
            x.Descricao.Contains("Login Active Directory negado", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task DeveBloquearUsuarioInativoMesmoComCredencialAdValida()
    {
        await using var context = CriarContexto();
        await GarantirPerfilSolicitanteAsync(context);

        var usuario = new Usuario("Thiago Teste", "thiago@empresa.com", "thiago", "teste");
        usuario.AlterarSituacao(SituacaoUsuario.Inativo, "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            CriarActiveDirectoryOptions(permitirAutoProvisionamento: false),
            new FakeCredentialValidator(_ => SucessoPadrao()));

        await Assert.ThrowsAsync<AcessoNegadoException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        }));
    }

    [Fact]
    public async Task DeveFalharQuandoConfiguracaoAdForInvalida()
    {
        await using var context = CriarContexto();
        var service = CriarService(
            context,
            CriarAuthOptions(habilitarAd: true),
            new ActiveDirectoryOptions(),
            new FakeCredentialValidator(_ => SucessoPadrao()));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoginAsync(new LoginActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123",
            Dominio = "EMPRESA"
        }));
    }

    private static ActiveDirectoryAuthenticationService CriarService(
        SGXSistemaChamadoDbContext context,
        AuthOptions authOptions,
        ActiveDirectoryOptions adOptions,
        IActiveDirectoryCredentialValidator validator,
        IAuditoriaService? auditoriaService = null)
    {
        return new ActiveDirectoryAuthenticationService(
            context,
            Options.Create(authOptions),
            Options.Create(adOptions),
            new FakeMetodosLoginAdminService(authOptions, adOptions),
            validator,
            NullLogger<ActiveDirectoryAuthenticationService>.Instance,
            auditoriaService);
    }

    private static ActiveDirectoryOptions CriarActiveDirectoryOptions(
        bool permitirAutoProvisionamento = false,
        string perfilPadrao = "Solicitante")
    {
        return new ActiveDirectoryOptions
        {
            Servidor = "ldaps://dc01.empresa.local",
            Porta = 636,
            UsarLdaps = true,
            Dominio = "EMPRESA",
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
            PermitirAutoProvisionamento = permitirAutoProvisionamento,
            PerfilPadrao = perfilPadrao
        };
    }

    private static AuthOptions CriarAuthOptions(bool habilitarAd)
    {
        return new AuthOptions
        {
            JwtLocalIssuer = "SGX.Local.Testes",
            JwtLocalAudience = "SGX.SistemaChamado.Api",
            JwtLocalChaveAssinatura = "sgx-testes-login-local-chave-com-minimo-32-caracteres",
            JwtLocalExpiracaoMinutos = 120,
            Provedores = new ProvedoresAutenticacaoOptions
            {
                Configurados = habilitarAd
                    ? [CodigoProvedorAutenticacao.ActiveDirectory]
                    : [CodigoProvedorAutenticacao.LocalSgx],
                Habilitados = habilitarAd
                    ? [CodigoProvedorAutenticacao.ActiveDirectory]
                    : [CodigoProvedorAutenticacao.LocalSgx],
                Principal = habilitarAd
                    ? CodigoProvedorAutenticacao.ActiveDirectory
                    : CodigoProvedorAutenticacao.LocalSgx
            }
        };
    }

    private static ActiveDirectoryValidacaoResultado SucessoPadrao()
    {
        return new ActiveDirectoryValidacaoResultado(
            Sucesso: true,
            UsuarioSamAccountName: "thiago",
            NomeCompleto: "Thiago Teste",
            Email: "thiago@empresa.com",
            UserPrincipalName: "thiago@empresa.com",
            DistinguishedName: "CN=Thiago Teste,OU=Usuarios,DC=empresa,DC=local");
    }

    private static SGXSistemaChamadoDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        var context = new SGXSistemaChamadoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static async Task GarantirPerfilSolicitanteAsync(SGXSistemaChamadoDbContext context)
    {
        if (await context.PerfisAcesso.AnyAsync(x => x.Nome == PerfisInternos.Solicitante))
        {
            return;
        }

        context.PerfisAcesso.Add(new PerfilAcesso(PerfisInternos.Solicitante, TipoPerfil.Solicitante, "Perfil padrao", "teste"));
        await context.SaveChangesAsync();
    }

    private sealed class FakeCredentialValidator(Func<LoginActiveDirectoryRequest, ActiveDirectoryValidacaoResultado> factory) : IActiveDirectoryCredentialValidator
    {
        public Task<ActiveDirectoryValidacaoResultado> ValidarCredenciaisAsync(
            string usuario,
            string senha,
            string dominio,
            ActiveDirectoryOptions options,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(factory(new LoginActiveDirectoryRequest
            {
                Usuario = usuario,
                Senha = senha,
                Dominio = dominio
            }));
        }
    }

    private sealed class FakeMetodosLoginAdminService(
        AuthOptions authOptions,
        ActiveDirectoryOptions adOptions) : IMetodosLoginAdminService
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
            => Task.FromResult(string.Equals(codigoProvedor, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase));

        public Task<MetodoLoginEfetivo?> ObterMetodoEfetivoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
        {
            var habilitado = authOptions.ObterCodigosProvedoresHabilitadosNormalizados()
                .Contains(CodigoProvedorAutenticacao.ActiveDirectory, StringComparer.OrdinalIgnoreCase);
            var funcional = adOptions.EstaConfigurado();

            return Task.FromResult<MetodoLoginEfetivo?>(new MetodoLoginEfetivo(
                Codigo: CodigoProvedorAutenticacao.ActiveDirectory,
                Nome: "Active Directory",
                Descricao: "AD",
                Configurado: true,
                Habilitado: habilitado,
                Principal: habilitado,
                Ordem: 20,
                PermiteAutoProvisionamento: adOptions.PermitirAutoProvisionamento,
                PerfilPadraoAutoProvisionamento: adOptions.PerfilPadrao,
                RotuloExibicao: "Active Directory",
                Funcional: funcional,
                PodeHabilitar: funcional,
                MotivoBloqueioHabilitar: funcional ? null : "Active Directory nao esta tecnicamente configurado."));
        }
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
