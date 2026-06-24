using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class MarcarMinhaNotificacaoComoLidaUseCaseTests
{
    [Fact]
    public async Task DeveMarcarMinhaNotificacaoComoLidaComIdempotencia()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioId = Guid.NewGuid();
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioId, "notif:lida");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new MarcarMinhaNotificacaoComoLidaUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioId),
            PortalUseCasesTestFactory.Uow(context));

        var primeira = await useCase.ExecutarAsync(notificacao.Id);
        var segunda = await useCase.ExecutarAsync(notificacao.Id);

        Assert.True(primeira.Lida);
        Assert.True(primeira.EstadoAlterado);
        Assert.NotNull(primeira.LidaEm);
        Assert.True(segunda.Lida);
        Assert.False(segunda.EstadoAlterado);
        Assert.Equal(primeira.LidaEm, segunda.LidaEm);
    }

    [Fact]
    public async Task DeveRetornarNaoEncontradaParaNotificacaoAlheiaOuEmail()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();
        var outroUsuario = CriarNotificacaoSistemaEnviada(usuarioB, "notif:b");
        var email = CriarNotificacaoEmailEnviada(usuarioA, "notif:email");
        context.Notificacoes.AddRange(outroUsuario, email);
        await context.SaveChangesAsync();

        var useCase = new MarcarMinhaNotificacaoComoLidaUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioA),
            PortalUseCasesTestFactory.Uow(context));

        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(outroUsuario.Id));
        await Assert.ThrowsAsync<KeyNotFoundException>(() => useCase.ExecutarAsync(email.Id));
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

    private static Notificacao CriarNotificacaoEmailEnviada(Guid usuarioId, string chave)
    {
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo",
            chave,
            "teste",
            usuarioId,
            "email@sgx.local",
            null,
            "Assunto",
            $"corr:{chave}",
            usuarioId);
        notificacao.IniciarProcessamento(DateTime.UtcNow.AddMinutes(-2), "teste", usuarioId);
        notificacao.RegistrarEnvio(DateTime.UtcNow.AddMinutes(-1), "teste", usuarioId);
        return notificacao;
    }
}
