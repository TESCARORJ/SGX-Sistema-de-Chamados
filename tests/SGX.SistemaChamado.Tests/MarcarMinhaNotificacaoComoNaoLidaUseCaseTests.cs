using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class MarcarMinhaNotificacaoComoNaoLidaUseCaseTests
{
    [Fact]
    public async Task DeveMarcarMinhaNotificacaoComoNaoLidaComIdempotencia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioId = Guid.NewGuid();
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioId, "notif:naolida");
        notificacao.MarcarComoLida(DateTime.UtcNow, "teste", usuarioId);
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new MarcarMinhaNotificacaoComoNaoLidaUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioId),
            PortalUseCasesTestFactory.Uow(context));

        var primeira = await useCase.ExecutarAsync(notificacao.Id);
        var segunda = await useCase.ExecutarAsync(notificacao.Id);

        Assert.False(primeira.Lida);
        Assert.True(primeira.EstadoAlterado);
        Assert.Null(primeira.LidaEm);
        Assert.False(segunda.Lida);
        Assert.False(segunda.EstadoAlterado);
    }

    [Fact]
    public async Task DeveOcultarNotificacaoDeOutroUsuario()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioB, "notif:outro");
        notificacao.MarcarComoLida(DateTime.UtcNow, "teste", usuarioB);
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new MarcarMinhaNotificacaoComoNaoLidaUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioA),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(notificacao.Id));
    }

    private static FakeUsuarioContextoAplicacaoService CriarContexto(Guid usuarioId)
        => new(new UsuarioContextoAplicacao(
            usuarioId,
            "Solicitante",
            "solicitante@sgx.local",
            "solicitante",
            ["Solicitante"]));

    private static Notificacao CriarNotificacaoSistemaEnviada(Guid usuarioId, string chave)
    {
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo",
            chave,
            "teste",
            usuarioId,
            null,
            null,
            "Assunto",
            $"corr:{chave}",
            usuarioId);
        notificacao.IniciarProcessamento(DateTime.UtcNow.AddMinutes(-2), "teste", usuarioId);
        notificacao.RegistrarEnvio(DateTime.UtcNow.AddMinutes(-1), "teste", usuarioId);
        return notificacao;
    }
}
