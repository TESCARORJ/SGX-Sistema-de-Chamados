using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Services;

namespace SGX.SistemaChamado.Tests;

public sealed class AuditoriaServiceTests
{
    [Fact]
    public async Task RegistrarEventoSimplesPersisteEventoAuditoria()
    {
        using var context = CriarContexto();
        var service = CriarService(context, ContextoPadrao());

        await service.RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Admin.Cadastros",
            Entidade = "Usuario",
            EntidadeId = Guid.NewGuid().ToString(),
            Acao = TipoAcaoAuditoria.Criacao,
            Descricao = "Teste de criacao de usuario.",
            Sucesso = true
        });

        var evento = await context.EventosAuditoria.SingleAsync();
        Assert.Equal("Admin.Cadastros", evento.Modulo);
        Assert.Equal("Usuario", evento.Entidade);
        Assert.Equal(TipoAcaoAuditoria.Criacao, evento.Acao);
        Assert.True(evento.Sucesso);
    }

    [Fact]
    public async Task RegistrarEventoComUsuarioUsaContextoDoProvider()
    {
        using var context = CriarContexto();
        var usuarioId = Guid.NewGuid();
        var provider = new StubAuditoriaContextProvider(new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            usuarioId,
            "Admin SGX",
            "admin@sgx.local",
            "admin.sgx",
            "10.10.10.10",
            "TesteAgent",
            "correlacao-1"));

        var service = CriarService(context, provider);
        await service.RegistrarCriacaoAsync(
            "Admin.Cadastros",
            "PerfilAcesso",
            Guid.NewGuid().ToString(),
            "Criacao de perfil para teste.");

        var evento = await context.EventosAuditoria.SingleAsync();
        Assert.Equal(usuarioId, evento.UsuarioId);
        Assert.Equal("Admin SGX", evento.UsuarioNome);
        Assert.Equal("admin@sgx.local", evento.UsuarioEmail);
        Assert.Equal("admin.sgx", evento.UsuarioLogin);
        Assert.Equal("10.10.10.10", evento.IpOrigem);
        Assert.Equal("TesteAgent", evento.UserAgent);
        Assert.Equal("correlacao-1", evento.CorrelacaoId);
    }

    [Fact]
    public async Task RegistrarEventoSemUsuarioPermaneceComUsuarioNulo()
    {
        using var context = CriarContexto();
        var provider = new StubAuditoriaContextProvider(new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            null,
            null,
            null,
            null,
            "127.0.0.1",
            "SemUsuario",
            "correlacao-2"));

        var service = CriarService(context, provider);
        await service.RegistrarAtivacaoAsync(
            "Admin.Cadastros",
            "Usuario",
            Guid.NewGuid().ToString(),
            "Reativacao sem usuario autenticado.");

        var evento = await context.EventosAuditoria.SingleAsync();
        Assert.Null(evento.UsuarioId);
        Assert.Null(evento.UsuarioNome);
        Assert.Null(evento.UsuarioEmail);
        Assert.Null(evento.UsuarioLogin);
        Assert.Equal("127.0.0.1", evento.IpOrigem);
    }

    [Fact]
    public async Task RegistrarEdicaoComDadosAntesEDepoisPersisteJson()
    {
        using var context = CriarContexto();
        var service = CriarService(context, ContextoPadrao());

        await service.RegistrarEdicaoAsync(
            "Admin.Cadastros",
            "Usuario",
            Guid.NewGuid().ToString(),
            "Atualizacao de dados cadastrais.",
            dadosAntes: "{\"nome\":\"Antes\"}",
            dadosDepois: "{\"nome\":\"Depois\"}");

        var evento = await context.EventosAuditoria.SingleAsync();
        Assert.Equal("{\"nome\":\"Antes\"}", evento.DadosAntes);
        Assert.Equal("{\"nome\":\"Depois\"}", evento.DadosDepois);
        Assert.Equal(TipoAcaoAuditoria.Edicao, evento.Acao);
    }

    [Fact]
    public async Task RegistrarErroPersisteEventoCritico()
    {
        using var context = CriarContexto();
        var service = CriarService(context, ContextoPadrao());

        await service.RegistrarErroAsync(
            "Autenticacao",
            "Usuario",
            "Falha inesperada no fluxo de login.",
            exception: new InvalidOperationException("erro de teste"));

        var evento = await context.EventosAuditoria.SingleAsync();
        Assert.Equal(TipoAcaoAuditoria.Erro, evento.Acao);
        Assert.Equal(NivelAuditoria.Critico, evento.Nivel);
        Assert.False(evento.Sucesso);
        Assert.Equal("erro de teste", evento.MensagemErro);
    }

    [Fact]
    public async Task NaoDeveQuebrarFluxoQuandoProviderFalha()
    {
        using var context = CriarContexto();
        var service = CriarService(context, new StubAuditoriaContextProviderComFalha());

        await service.RegistrarCriacaoAsync(
            "Admin.Cadastros",
            "Usuario",
            Guid.NewGuid().ToString(),
            "Tentativa de auditoria com falha do provider.");

        Assert.Equal(0, await context.EventosAuditoria.CountAsync());
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

    private static AuditoriaService CriarService(
        SGXSistemaChamadoDbContext context,
        IAuditoriaContextProvider contextoProvider)
        => new(context, contextoProvider, NullLogger<AuditoriaService>.Instance);

    private static IAuditoriaContextProvider ContextoPadrao()
        => new StubAuditoriaContextProvider(new ContextoAuditoriaAtual(
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Usuario Teste",
            "usuario.teste@sgx.local",
            "usuario.teste",
            "192.168.0.15",
            "Xunit",
            Guid.NewGuid().ToString("N")));

    private sealed class StubAuditoriaContextProvider(ContextoAuditoriaAtual contexto) : IAuditoriaContextProvider
    {
        public ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default)
            => ValueTask.FromResult(contexto);
    }

    private sealed class StubAuditoriaContextProviderComFalha : IAuditoriaContextProvider
    {
        public ValueTask<ContextoAuditoriaAtual> ObterContextoAsync(CancellationToken cancellationToken = default)
            => throw new InvalidOperationException("falha proposital no provider");
    }
}
