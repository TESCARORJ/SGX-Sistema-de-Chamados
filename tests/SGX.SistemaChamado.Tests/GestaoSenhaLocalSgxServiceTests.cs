using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class GestaoSenhaLocalSgxServiceTests
{
    private const string SenhaInicial = "Senha@Inicial123";
    private const string SenhaNova = "Senha@Nova123456";

    [Fact]
    public async Task AlterarSenhaComSenhaAtualCorretaAtualizaHashEDesmarcaTrocaObrigatoria()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "alterar@empresa.com", SenhaInicial, deveAlterarSenha: true);

        var logger = new TestLogger<GestaoSenhaLocalSgxService>();
        var service = CriarService(context, logger);

        var response = await service.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = SenhaInicial,
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = SenhaNova
        });

        var usuarioAtualizado = await context.Usuarios.SingleAsync(x => x.Id == usuario.Id);
        var hasher = new PasswordHasher<Usuario>();
        var verificacao = hasher.VerifyHashedPassword(usuarioAtualizado, usuarioAtualizado.SenhaHashLocal!, SenhaNova);

        Assert.Equal("Senha alterada com sucesso.", response.Mensagem);
        Assert.NotEqual(PasswordVerificationResult.Failed, verificacao);
        Assert.False(usuarioAtualizado.DeveAlterarSenha);
        Assert.True(usuarioAtualizado.UltimaAlteracaoSenhaEm.HasValue);
    }

    [Fact]
    public async Task AlterarSenhaRejeitaSenhaAtualIncorreta()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "senha.incorreta@empresa.com", SenhaInicial);

        var service = CriarService(context, new TestLogger<GestaoSenhaLocalSgxService>());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = "SenhaErrada",
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = SenhaNova
        }));
    }

    [Fact]
    public async Task AlterarSenhaRejeitaConfirmacaoDivergente()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "confirmacao@empresa.com", SenhaInicial);

        var service = CriarService(context, new TestLogger<GestaoSenhaLocalSgxService>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = SenhaInicial,
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = "OutraSenha@123"
        }));
    }

    [Fact]
    public async Task AlterarSenhaRejeitaSenhaFraca()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "fraca@empresa.com", SenhaInicial);

        var service = CriarService(context, new TestLogger<GestaoSenhaLocalSgxService>());

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = SenhaInicial,
            NovaSenha = "123456",
            ConfirmacaoNovaSenha = "123456"
        }));
    }

    [Fact]
    public async Task SolicitarRecuperacaoNaoRevelaSeEmailExiste()
    {
        using var context = CriarContexto();
        _ = await CriarUsuarioComSenhaAsync(context, "recuperacao@empresa.com", SenhaInicial);

        var service = CriarService(context, new TestLogger<GestaoSenhaLocalSgxService>());

        var responseExistente = await service.SolicitarRecuperacaoSenhaAsync(
            new RecuperarSenhaSolicitacaoRequest { Email = "recuperacao@empresa.com" },
            "127.0.0.1",
            "xunit");

        var responseInexistente = await service.SolicitarRecuperacaoSenhaAsync(
            new RecuperarSenhaSolicitacaoRequest { Email = "naoexiste@empresa.com" },
            "127.0.0.1",
            "xunit");

        Assert.Equal(responseExistente.Mensagem, responseInexistente.Mensagem);
        Assert.Equal("Se o e-mail estiver cadastrado, enviaremos as instruções para redefinição de senha.", responseExistente.Mensagem);
    }

    [Fact]
    public async Task RedefinicaoComTokenValidoAlteraSenhaEImpedeReutilizacao()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "token.valido@empresa.com", SenhaInicial);

        var logger = new TestLogger<GestaoSenhaLocalSgxService>();
        var tokenService = new TokenRecuperacaoSenhaFakeService("TOKEN_VALIDO");
        var service = CriarService(context, logger, tokenService);

        await service.SolicitarRecuperacaoSenhaAsync(
            new RecuperarSenhaSolicitacaoRequest { Email = usuario.Email },
            "127.0.0.1",
            "xunit");

        var response = await service.RedefinirSenhaAsync(new RecuperarSenhaRedefinicaoRequest
        {
            Token = "TOKEN_VALIDO",
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = SenhaNova
        });

        Assert.Equal("Senha redefinida com sucesso.", response.Mensagem);

        var usuarioAtualizado = await context.Usuarios.SingleAsync(x => x.Id == usuario.Id);
        var hasher = new PasswordHasher<Usuario>();
        var verificacao = hasher.VerifyHashedPassword(usuarioAtualizado, usuarioAtualizado.SenhaHashLocal!, SenhaNova);
        Assert.NotEqual(PasswordVerificationResult.Failed, verificacao);

        var tokenPersistido = await context.TokensRecuperacaoSenha.SingleAsync(x => x.UsuarioId == usuario.Id);
        Assert.True(tokenPersistido.UtilizadoEm.HasValue);
        Assert.False(tokenPersistido.Ativo);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.RedefinirSenhaAsync(new RecuperarSenhaRedefinicaoRequest
        {
            Token = "TOKEN_VALIDO",
            NovaSenha = "OutraSenha@12345",
            ConfirmacaoNovaSenha = "OutraSenha@12345"
        }));
    }

    [Fact]
    public async Task RedefinicaoComTokenExpiradoEhRejeitada()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "token.expirado@empresa.com", SenhaInicial);

        var tokenService = new TokenRecuperacaoSenhaFakeService("TOKEN_EXPIRADO");
        var service = CriarService(context, new TestLogger<GestaoSenhaLocalSgxService>(), tokenService);

        await context.TokensRecuperacaoSenha.AddAsync(new TokenRecuperacaoSenha(
            usuario.Id,
            tokenService.CalcularHash("TOKEN_EXPIRADO"),
            DateTime.UtcNow.AddMinutes(-5),
            "teste"));
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.RedefinirSenhaAsync(new RecuperarSenhaRedefinicaoRequest
        {
            Token = "TOKEN_EXPIRADO",
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = SenhaNova
        }));
    }

    [Fact]
    public async Task LogsNaoDevemConterSenha()
    {
        using var context = CriarContexto();
        var usuario = await CriarUsuarioComSenhaAsync(context, "log.senha@empresa.com", SenhaInicial);

        var logger = new TestLogger<GestaoSenhaLocalSgxService>();
        var service = CriarService(context, logger);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.AlterarSenhaAsync(usuario.Id, new AlterarSenhaLocalRequest
        {
            SenhaAtual = "Errada@123",
            NovaSenha = SenhaNova,
            ConfirmacaoNovaSenha = SenhaNova
        }));

        var mensagens = string.Join("\n", logger.Messages);
        Assert.DoesNotContain("Errada@123", mensagens, StringComparison.Ordinal);
        Assert.DoesNotContain(SenhaNova, mensagens, StringComparison.Ordinal);
        Assert.DoesNotContain(SenhaInicial, mensagens, StringComparison.Ordinal);
    }

    private static GestaoSenhaLocalSgxService CriarService(
        SGXSistemaChamadoDbContext context,
        TestLogger<GestaoSenhaLocalSgxService> logger,
        ITokenRecuperacaoSenhaService? tokenService = null)
    {
        var hasher = new PasswordHasher<Usuario>();

        return new GestaoSenhaLocalSgxService(
            context,
            hasher,
            new PoliticaSenhaService(Options.Create(new AuthOptions()), hasher),
            Options.Create(new AuthOptions()),
            tokenService ?? new TokenRecuperacaoSenhaFakeService("TOKEN_PADRAO"),
            logger);
    }

    private static async Task<Usuario> CriarUsuarioComSenhaAsync(
        SGXSistemaChamadoDbContext context,
        string email,
        string senha,
        bool deveAlterarSenha = false)
    {
        var usuario = new Usuario("Usuario Local", email, email, "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var hasher = new PasswordHasher<Usuario>();
        usuario.DefinirSenhaHashLocal(hasher.HashPassword(usuario, senha), "teste");
        usuario.DefinirDeveAlterarSenha(deveAlterarSenha, "teste");
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

    private sealed class TokenRecuperacaoSenhaFakeService(string tokenFixo) : ITokenRecuperacaoSenhaService
    {
        private readonly TokenRecuperacaoSenhaService _real = new();

        public string GerarToken() => tokenFixo;

        public string CalcularHash(string valor) => _real.CalcularHash(valor);
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = [];

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }
}
