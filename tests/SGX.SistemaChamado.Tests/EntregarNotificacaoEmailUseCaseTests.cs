using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class EntregarNotificacaoEmailUseCaseTests
{
    [Fact]
    public async Task DeveEntregarNotificacaoEmailValidaEPreservarPayload()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-valida");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacaoEmail(usuario.Id, "destinatario@cliente.com", "notif:email:ok");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(true, false, "<msg-1@sgx>", null));
        var useCase = CriarUseCase(context, transportador);

        var response = await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.True(response.Entregue);
        Assert.False(response.JaEstavaEntregue);
        Assert.False(response.Reagendada);
        Assert.Equal(StatusNotificacao.Enviada, response.Status);
        Assert.Equal("destinatario@cliente.com", response.Destinatario);
        Assert.Equal(DataEntregaUtc, response.EnviadaEm);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(DataEntregaUtc, persistida.EnviadaEm);
        Assert.Equal("Assunto Email", persistida.Assunto);
        Assert.Equal("<p>Conteudo materializado do canal Email.</p>", persistida.Conteudo);
        Assert.Equal("destinatario@cliente.com", transportador.Mensagens.Single().Destinatario);
        Assert.Equal("Assunto Email", transportador.Mensagens.Single().Assunto);
        Assert.Equal("<p>Conteudo materializado do canal Email.</p>", transportador.Mensagens.Single().Conteudo);
        Assert.True(transportador.Mensagens.Single().ConteudoHtml);
    }

    [Fact]
    public async Task DeveRetornarIdempotenciaSemReenviarQuandoNotificacaoJaEstiverEnviada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-idempotente");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacaoEmail(usuario.Id, "snapshot@cliente.com", "notif:email:idempotente");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-2), "teste");
        notificacao.RegistrarEnvio(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(true, false, "<msg-2@sgx>", null));
        var useCase = CriarUseCase(context, transportador);

        var response = await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc));

        Assert.False(response.Entregue);
        Assert.True(response.JaEstavaEntregue);
        Assert.Equal(DataEntregaUtc.AddMinutes(-1), response.EnviadaEm);
        Assert.Empty(transportador.Mensagens);
    }

    [Fact]
    public async Task DeveRegistrarFalhaDefinitivaQuandoAssuntoEstiverAusenteSemChamarTransporte()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-sem-assunto");
        context.Usuarios.Add(usuario);
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "<p>Conteudo materializado do canal Email.</p>",
            "notif:email:sem-assunto",
            "teste",
            usuario.Id,
            "destinatario@cliente.com",
            null,
            null,
            "corr-sem-assunto",
            usuario.Id);
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(true, false, "<msg-3@sgx>", null));
        var useCase = CriarUseCase(context, transportador);

        var response = await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.False(response.Entregue);
        Assert.False(response.JaEstavaEntregue);
        Assert.False(response.Reagendada);
        Assert.Equal(StatusNotificacao.Falhou, response.Status);
        Assert.Contains("assunto", response.Erro!, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(StatusNotificacao.Falhou, persistida.Status);
        Assert.Empty(transportador.Mensagens);
    }

    [Fact]
    public async Task DeveReagendarQuandoTransporteRetornarFalhaTransitoria()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-transitoria");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacaoEmail(usuario.Id, "transiente@cliente.com", "notif:email:transitoria");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(false, true, null, "SMTP 450 mailbox busy"));
        var useCase = CriarUseCase(context, transportador);

        var response = await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.False(response.Entregue);
        Assert.True(response.Reagendada);
        Assert.Equal(StatusNotificacao.Agendada, response.Status);
        Assert.NotNull(response.ProximaTentativaEm);
        Assert.Contains("450", response.Erro!);
        Assert.Equal(StatusNotificacao.Agendada, persistida.Status);
        Assert.NotNull(persistida.AgendadaEm);
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveEncerrarQuandoTransporteRetornarFalhaDefinitiva()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-definitiva");
        context.Usuarios.Add(usuario);
        var notificacao = CriarNotificacaoEmail(usuario.Id, "definitiva@cliente.com", "notif:email:definitiva");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(false, false, null, "SMTP 550 mailbox unavailable"));
        var useCase = CriarUseCase(context, transportador);

        var response = await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.False(response.Entregue);
        Assert.False(response.Reagendada);
        Assert.Equal(StatusNotificacao.Falhou, response.Status);
        Assert.Equal(StatusNotificacao.Falhou, persistida.Status);
        Assert.Contains("550", persistida.UltimoErro!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeveRejeitarCanalSistemaSemAfetarNotificacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = CriarUsuario("email-sistema");
        context.Usuarios.Add(usuario);
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo sistema",
            "notif:sistema:nao-email",
            "teste",
            usuario.Id,
            null,
            null,
            "Assunto Sistema",
            "corr-sistema",
            usuario.Id);
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var transportador = new FakeTransportadorEmailNotificacao(
            new ResultadoTransporteEmailNotificacao(true, false, "<msg-4@sgx>", null));
        var useCase = CriarUseCase(context, transportador);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacao.Id, DataEntregaUtc)));

        Assert.Empty(transportador.Mensagens);
        Assert.Equal(StatusNotificacao.EmProcessamento, (await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id)).Status);
    }

    private static EntregarNotificacaoEmailUseCase CriarUseCase(
        SGXSistemaChamadoDbContext context,
        ITransportadorEmailNotificacao transportadorEmailNotificacao)
    {
        var usuarioContexto = new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "Processador Email",
            "processador.email@sgx.local",
            "processador.email",
            ["Sistema"]));

        return new EntregarNotificacaoEmailUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            transportadorEmailNotificacao,
            new RegistrarSucessoEntregaNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<Notificacao>(context),
                usuarioContexto,
                PortalUseCasesTestFactory.Uow(context)),
            new RegistrarFalhaEntregaNotificacaoUseCase(
                PortalUseCasesTestFactory.Repo<Notificacao>(context),
                usuarioContexto,
                PortalUseCasesTestFactory.Uow(context)),
            NullLogger<EntregarNotificacaoEmailUseCase>.Instance);
    }

    private static Usuario CriarUsuario(string sufixo)
        => new($"Usuario {sufixo}", $"usuario.{sufixo}@teste.local", $"login.{sufixo}", "teste");

    private static Notificacao CriarNotificacaoEmail(Guid usuarioId, string endereco, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "<p>Conteudo materializado do canal Email.</p>",
            chave,
            "teste",
            usuarioId,
            endereco,
            null,
            "Assunto Email",
            $"corr:{chave}",
            usuarioId);

    private static readonly DateTime DataEntregaUtc = new(2026, 6, 23, 15, 0, 0, DateTimeKind.Utc);

    private sealed class FakeTransportadorEmailNotificacao(ResultadoTransporteEmailNotificacao resultado) : ITransportadorEmailNotificacao
    {
        public List<MensagemEmailNotificacao> Mensagens { get; } = [];

        public Task<ResultadoTransporteEmailNotificacao> EnviarAsync(
            MensagemEmailNotificacao mensagem,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Mensagens.Add(mensagem);
            return Task.FromResult(resultado);
        }
    }
}
