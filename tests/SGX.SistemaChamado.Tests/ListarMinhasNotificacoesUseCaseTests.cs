using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ListarMinhasNotificacoesUseCaseTests
{
    [Fact]
    public async Task DeveListarApenasNotificacoesEnviadasDoCanalSistemaDoUsuarioAtual()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioA = Guid.NewGuid();
        var usuarioB = Guid.NewGuid();

        var baseData = DateTime.UtcNow.AddMinutes(1);
        var enviadaNaoLida = CriarNotificacaoSistemaEnviada(usuarioA, "notif:a1", baseData);
        var enviadaLida = CriarNotificacaoSistemaEnviada(usuarioA, "notif:a2", baseData.AddMinutes(5));
        enviadaLida.MarcarComoLida(enviadaLida.EnviadaEm!.Value.AddMinutes(1), "teste", usuarioA);
        var email = CriarNotificacaoEmailEnviada(usuarioA, "notif:a3");
        var outroUsuario = CriarNotificacaoSistemaEnviada(usuarioB, "notif:b1", baseData.AddMinutes(8));
        var pendente = CriarNotificacao(usuarioA, CanalNotificacao.Sistema, "notif:a4");

        context.Notificacoes.AddRange(enviadaNaoLida, enviadaLida, email, outroUsuario, pendente);
        context.Entry(pendente).Property(nameof(Notificacao.Ativo)).CurrentValue = false;
        await context.SaveChangesAsync();

        var useCase = new ListarMinhasNotificacoesUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioA));

        var response = await useCase.ExecutarAsync(new ListarMinhasNotificacoesRequest(1, 20, null));

        Assert.Equal(2, response.Total);
        Assert.Equal(1, response.Pagina);
        Assert.Equal(20, response.TamanhoPagina);
        Assert.Equal(1, response.TotalNaoLidas);
        Assert.Equal(1, response.TotalPaginas);
        Assert.Equal(2, response.Itens.Count);
        Assert.Equal(enviadaLida.Id, response.Itens.First().Id);
        Assert.All(response.Itens, x => Assert.NotEqual(Guid.Empty, x.Id));
        Assert.DoesNotContain(response.Itens, x => x.Id == email.Id);
        Assert.DoesNotContain(response.Itens, x => x.Id == outroUsuario.Id);
        Assert.All(response.Itens, x => Assert.True(x.ConteudoResumo.Length <= 200));
    }

    [Fact]
    public async Task DeveFiltrarLidasENaoLidasComPaginacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuarioId = Guid.NewGuid();

        var baseData = DateTime.UtcNow.AddMinutes(1);
        var primeira = CriarNotificacaoSistemaEnviada(usuarioId, "notif:1", baseData);
        var segunda = CriarNotificacaoSistemaEnviada(usuarioId, "notif:2", baseData.AddMinutes(5));
        segunda.MarcarComoLida(segunda.EnviadaEm!.Value.AddMinutes(1), "teste", usuarioId);
        var terceira = CriarNotificacaoSistemaEnviada(usuarioId, "notif:3", baseData.AddMinutes(10));

        context.Notificacoes.AddRange(primeira, segunda, terceira);
        await context.SaveChangesAsync();

        var useCase = new ListarMinhasNotificacoesUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            CriarContexto(usuarioId));

        var lidas = await useCase.ExecutarAsync(new ListarMinhasNotificacoesRequest(1, 10, true));
        var naoLidasPaginadas = await useCase.ExecutarAsync(new ListarMinhasNotificacoesRequest(2, 1, false));

        Assert.Single(lidas.Itens);
        Assert.Equal(segunda.Id, lidas.Itens.Single().Id);
        Assert.Equal(2, naoLidasPaginadas.Total);
        Assert.Equal(2, naoLidasPaginadas.TotalPaginas);
        Assert.Single(naoLidasPaginadas.Itens);
    }

    private static FakeUsuarioContextoAplicacaoService CriarContexto(Guid usuarioId)
        => new(new UsuarioContextoAplicacao(
            usuarioId,
            "Solicitante",
            "solicitante@sgx.local",
            "solicitante",
            ["Solicitante"]));

    private static Notificacao CriarNotificacao(Guid usuarioId, CanalNotificacao canal, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            canal,
            new string('x', 250),
            chave,
            "teste",
            usuarioId,
            canal == CanalNotificacao.Email ? "destinatario@sgx.local" : null,
            null,
            "Assunto",
            $"corr:{chave}",
            usuarioId);

    private static Notificacao CriarNotificacaoSistemaEnviada(Guid usuarioId, string chave, DateTime enviadaEm)
    {
        var notificacao = CriarNotificacao(usuarioId, CanalNotificacao.Sistema, chave);
        notificacao.IniciarProcessamento(enviadaEm.AddMinutes(-1), "teste", usuarioId);
        notificacao.RegistrarEnvio(enviadaEm, "teste", usuarioId);
        return notificacao;
    }

    private static Notificacao CriarNotificacaoEmailEnviada(Guid usuarioId, string chave)
    {
        var notificacao = CriarNotificacao(usuarioId, CanalNotificacao.Email, chave);
        notificacao.IniciarProcessamento(DateTime.UtcNow.AddMinutes(-2), "teste", usuarioId);
        notificacao.RegistrarEnvio(DateTime.UtcNow.AddMinutes(-1), "teste", usuarioId);
        return notificacao;
    }
}
