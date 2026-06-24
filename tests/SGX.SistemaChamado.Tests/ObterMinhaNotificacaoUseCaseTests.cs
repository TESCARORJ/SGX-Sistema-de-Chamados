using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ObterMinhaNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveObterMinhaNotificacaoSemMarcarAutomaticamenteComoLida()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioId = Guid.NewGuid();
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioId, "notif:detalhe");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = new ObterMinhaNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioId));

        var detalhe = await useCase.ExecutarAsync(notificacao.Id);

        Assert.Equal(notificacao.Id, detalhe.Id);
        Assert.Equal("Conteudo completo do detalhe.", detalhe.Conteudo);
        Assert.False(detalhe.Lida);
        Assert.Null(detalhe.LidaEm);
        Assert.Equal(notificacao.ChaveCorrelacao, detalhe.ChaveCorrelacao);

        var persistida = context.Notificacoes.Single(x => x.Id == notificacao.Id);
        Assert.False(persistida.Lida);
    }

    [Fact]
    public async Task DeveOcultarNotificacaoAlheiaOuDeCanalIncompativel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();
        var outroUsuario = CriarNotificacaoSistemaEnviada(usuarioB, "notif:outro");
        var email = CriarNotificacaoEmailEnviada(usuarioA, "notif:email");
        context.Notificacoes.AddRange(outroUsuario, email);
        await context.SaveChangesAsync();

        var useCase = new ObterMinhaNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioA));

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
            "Conteudo completo do detalhe.",
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
            "Conteudo email",
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
