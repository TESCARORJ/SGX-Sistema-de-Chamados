using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Tests;

public sealed class SelecionarNotificacoesProcessaveisUseCaseTests
{
    [Fact]
    public async Task DeveSelecionarPendentesEAgendadasVencidasRespeitandoLimiteEOrdenacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("selecionar");
        context.Usuarios.Add(usuario);

        var pendente = CriarNotificacao(usuario.Id, "pendente");
        var agendadaVencida = CriarNotificacao(usuario.Id, "agendada-vencida");
        agendadaVencida.Agendar(new DateTime(2026, 6, 21, 11, 0, 0, DateTimeKind.Utc), "teste");

        var agendadaFutura = CriarNotificacao(usuario.Id, "agendada-futura");
        agendadaFutura.Agendar(new DateTime(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc), "teste");

        context.Notificacoes.AddRange(pendente, agendadaVencida, agendadaFutura);
        await context.SaveChangesAsync();

        var useCase = new SelecionarNotificacoesProcessaveisUseCase(PortalUseCasesTestFactory.Repo<Notificacao>(context));
        var resultado = await useCase.ExecutarAsync(new SelecionarNotificacoesProcessaveisRequest(
            2,
            new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc)));

        Assert.Equal(2, resultado.Count);
        Assert.Equal(agendadaVencida.Id, resultado.First().NotificacaoId);
        Assert.Contains(resultado, x => x.NotificacaoId == pendente.Id);
        Assert.DoesNotContain(resultado, x => x.NotificacaoId == agendadaFutura.Id);
    }

    [Fact]
    public async Task DeveIgnorarEnviadasCanceladasEmProcessamentoEFalhou()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("filtrar");
        context.Usuarios.Add(usuario);

        var enviada = CriarNotificacao(usuario.Id, "enviada");
        enviada.IniciarProcessamento(DateTime.UtcNow, "teste");
        enviada.RegistrarEnvio(DateTime.UtcNow.AddMinutes(1), "teste");

        var cancelada = CriarNotificacao(usuario.Id, "cancelada");
        cancelada.Cancelar(DateTime.UtcNow, "teste");

        var emProcessamento = CriarNotificacao(usuario.Id, "processando");
        emProcessamento.IniciarProcessamento(DateTime.UtcNow, "teste");

        var falhou = CriarNotificacao(usuario.Id, "falhou");
        falhou.IniciarProcessamento(DateTime.UtcNow, "teste");
        falhou.RegistrarFalha("falha", DateTime.UtcNow.AddMinutes(1), "teste");

        context.Notificacoes.AddRange(enviada, cancelada, emProcessamento, falhou);
        await context.SaveChangesAsync();

        var useCase = new SelecionarNotificacoesProcessaveisUseCase(PortalUseCasesTestFactory.Repo<Notificacao>(context));
        var resultado = await useCase.ExecutarAsync(new SelecionarNotificacoesProcessaveisRequest(10, DateTime.UtcNow.AddMinutes(2)));

        Assert.Empty(resultado);
    }

    private static Usuario CriarUsuario(string sufixo)
        => new($"Usuario {sufixo}", $"usuario.{sufixo}@teste.local", $"login.{sufixo}", "teste");

    private static Notificacao CriarNotificacao(Guid usuarioId, string chave)
        => new(
            Domain.Enums.TipoEventoNotificacao.EventoChamado,
            Domain.Enums.CanalNotificacao.Email,
            "Conteudo de teste",
            $"notif:{chave}",
            "teste",
            usuarioId,
            null,
            null,
            "Assunto",
            $"corr:{chave}",
            usuarioId);
}
