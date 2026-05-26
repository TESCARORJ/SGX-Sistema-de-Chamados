using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ConfiguracaoIntegracaoActiveDirectoryServiceTests
{
    [Fact]
    public async Task DeveConsultarConfiguracaoComStatusInvalidoQuandoCamposMinimosNaoEstaoPreenchidos()
    {
        await using var context = CriarDbContext();
        var service = CriarService(context, new ActiveDirectoryOptions
        {
            Ativo = true,
            Servidor = "",
            Porta = 636,
            UsarLdaps = true,
            BaseDn = "",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))"
        });

        var response = await service.ObterConfiguracaoAsync();

        Assert.Equal("ConfiguracaoInvalida", response.StatusConfiguracao);
        Assert.False(response.TecnicamenteConfigurado);
        Assert.NotEmpty(response.PendenciasConfiguracao);
    }

    [Fact]
    public async Task DeveSalvarConfiguracaoValida()
    {
        await using var context = CriarDbContext();
        var service = CriarService(context);

        var response = await service.AtualizarConfiguracaoAsync(new AtualizarActiveDirectoryIntegracaoRequest
        {
            Ativo = true,
            Servidor = "ldaps://dc01.empresa.local",
            Porta = 636,
            UsarLdaps = true,
            PermitirLdapSemTls = false,
            ConfirmacaoPermitirLdapSemTls = false,
            Dominio = "EMPRESA",
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
            PermitirAutoProvisionamento = true,
            PerfilPadrao = "Solicitante",
            TimeoutConexaoSegundos = 8
        });

        Assert.Equal("Configurado", response.StatusConfiguracao);
        Assert.True(response.TecnicamenteConfigurado);
        Assert.Equal("ldaps://dc01.empresa.local", response.Servidor);
        Assert.Equal(8, response.TimeoutConexaoSegundos);
    }

    [Fact]
    public async Task DeveRejeitarConfiguracaoInvalida()
    {
        await using var context = CriarDbContext();
        var service = CriarService(context);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AtualizarConfiguracaoAsync(
            new AtualizarActiveDirectoryIntegracaoRequest
            {
                Ativo = true,
                Servidor = "ldaps://dc01.empresa.local",
                Porta = 636,
                UsarLdaps = true,
                BaseDn = "",
                UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
                PermitirAutoProvisionamento = false,
                PerfilPadrao = "Solicitante",
                TimeoutConexaoSegundos = 10
            }));

        Assert.Contains("Base DN", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task LdapSemTlsExigeConfirmacaoExplicita()
    {
        await using var context = CriarDbContext();
        var service = CriarService(context);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AtualizarConfiguracaoAsync(
            new AtualizarActiveDirectoryIntegracaoRequest
            {
                Ativo = true,
                Servidor = "ldap://dc01.empresa.local",
                Porta = 389,
                UsarLdaps = false,
                PermitirLdapSemTls = true,
                ConfirmacaoPermitirLdapSemTls = false,
                Dominio = "EMPRESA",
                BaseDn = "DC=empresa,DC=local",
                UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
                PermitirAutoProvisionamento = false,
                PerfilPadrao = "Solicitante",
                TimeoutConexaoSegundos = 10
            }));

        Assert.Contains("Confirmacao explicita", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TestarAutenticacaoControladaNaoPersisteSenha()
    {
        await using var context = CriarDbContext();
        var service = CriarService(context);

        var response = await service.TestarAutenticacaoAsync(new TestarAutenticacaoActiveDirectoryRequest
        {
            Usuario = "thiago",
            Senha = "Senha@123456",
            Dominio = "EMPRESA",
            Ativo = true,
            Servidor = "ldaps://dc01.empresa.local",
            Porta = 636,
            UsarLdaps = true,
            PermitirLdapSemTls = false,
            BaseDn = "DC=empresa,DC=local",
            UserSearchFilter = "(&(objectClass=user)(sAMAccountName={0}))",
            TimeoutConexaoSegundos = 10
        });

        Assert.True(response.Sucesso);

        var valores = await context.ParametrosSistema.Select(x => x.Valor).ToListAsync();
        Assert.DoesNotContain(valores, x => x.Contains("Senha@123456", StringComparison.Ordinal));
    }

    private static SGXSistemaChamadoDbContext CriarDbContext()
    {
        var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
            .UseInMemoryDatabase($"sgx-ad-config-{Guid.NewGuid():N}")
            .Options;

        return new SGXSistemaChamadoDbContext(options);
    }

    private static ConfiguracaoIntegracaoActiveDirectoryService CriarService(
        SGXSistemaChamadoDbContext context,
        ActiveDirectoryOptions? fallbackOptions = null)
    {
        return new ConfiguracaoIntegracaoActiveDirectoryService(
            context,
            Options.Create(fallbackOptions ?? new ActiveDirectoryOptions()),
            new FakeConnectivityTester(),
            new FakeCredentialValidator());
    }

    private sealed class FakeConnectivityTester : IActiveDirectoryConnectivityTester
    {
        public Task<(bool Sucesso, string Mensagem)> TestarConexaoTcpAsync(
            ActiveDirectoryOptions options,
            CancellationToken cancellationToken = default)
            => Task.FromResult((true, $"Conexao TCP estabelecida com {options.Servidor}:{options.Porta}."));
    }

    private sealed class FakeCredentialValidator : IActiveDirectoryCredentialValidator
    {
        public Task<ActiveDirectoryValidacaoResultado> ValidarCredenciaisAsync(
            string usuario,
            string senha,
            string dominio,
            ActiveDirectoryOptions options,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new ActiveDirectoryValidacaoResultado(
                Sucesso: true,
                UsuarioSamAccountName: usuario,
                NomeCompleto: "Usuario Teste",
                Email: "usuario.teste@empresa.local",
                UserPrincipalName: "usuario.teste@empresa.local",
                DistinguishedName: "CN=Usuario Teste,OU=Usuarios,DC=empresa,DC=local"));
    }
}
