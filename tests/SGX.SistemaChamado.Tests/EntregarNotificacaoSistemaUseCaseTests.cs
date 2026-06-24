using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class EntregarNotificacaoSistemaUseCaseTests
{
    [Fact]
    public async Task DeveEntregarNotificacaoSistemaValidaSemAlterarPayload()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var destinatario = CriarUsuario("sistema-entrega");
        context.Usuarios.Add(destinatario);

        var notificacao = CriarNotificacaoSistema(destinatario.Id, "notif:sistema:valida");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.True(response.Entregue);
        Assert.False(response.JaEstavaEntregue);
        Assert.Equal(StatusNotificacao.Enviada, response.Status);
        Assert.Equal(destinatario.Id, response.DestinatarioUsuarioId);
        Assert.Equal(DataEntregaUtc, response.EntregueEm);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(DataEntregaUtc, persistida.EnviadaEm);
        Assert.Equal("Assunto Sistema", persistida.Assunto);
        Assert.Equal("Conteudo materializado do canal Sistema.", persistida.Conteudo);
        Assert.Equal("notif:sistema:valida", persistida.ChaveIdempotencia);
        Assert.Equal(destinatario.Id, persistida.DestinatarioUsuarioId);
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveRetornarSucessoIdempotenteQuandoNotificacaoJaEstiverEnviada()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var destinatario = CriarUsuario("sistema-idempotente");
        context.Usuarios.Add(destinatario);

        var notificacao = CriarNotificacaoSistema(destinatario.Id, "notif:sistema:idempotente");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-2), "teste");
        notificacao.RegistrarEnvio(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        var response = await useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc));

        var persistida = await context.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.False(response.Entregue);
        Assert.True(response.JaEstavaEntregue);
        Assert.Equal(DataEntregaUtc.AddMinutes(-1), response.EntregueEm);
        Assert.Equal(DataEntregaUtc.AddMinutes(-1), persistida.EnviadaEm);
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveRejeitarQuandoCanalNaoForSistema()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var destinatario = CriarUsuario("sistema-canal");
        context.Usuarios.Add(destinatario);

        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo por email",
            "notif:email:nao-entregar",
            "teste",
            destinatario.Id,
            null,
            null,
            "Assunto Email",
            "corr-email",
            destinatario.Id);
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc)));
    }

    [Fact]
    public async Task DeveRejeitarQuandoDestinatarioInternoNaoEstiverPreenchido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo materializado do canal Sistema.",
            "notif:sistema:sem-destinatario",
            "teste",
            null,
            "externo@cliente.com",
            null,
            "Assunto Sistema",
            "corr-sistema-sem-destinatario",
            null);
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc)));
    }

    [Fact]
    public async Task DeveRejeitarQuandoDestinatarioInternoNaoEstiverElegivel()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var destinatario = CriarUsuario("sistema-bloqueado");
        destinatario.AlterarSituacao(SituacaoUsuario.Bloqueado, "teste");
        context.Usuarios.Add(destinatario);

        var notificacao = CriarNotificacaoSistema(destinatario.Id, "notif:sistema:bloqueado");
        notificacao.IniciarProcessamento(DataEntregaUtc.AddMinutes(-1), "teste");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc)));
    }

    [Fact]
    public async Task DeveRejeitarQuandoNotificacaoNaoEstiverEmProcessamento()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var destinatario = CriarUsuario("sistema-pendente");
        context.Usuarios.Add(destinatario);

        var notificacao = CriarNotificacaoSistema(destinatario.Id, "notif:sistema:pendente");
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var useCase = CriarUseCase(context);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacao.Id, DataEntregaUtc)));
    }

    [Fact]
    public async Task DeveRespeitarCancellationTokenCancelado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(Guid.NewGuid(), DataEntregaUtc), cts.Token));
    }

    private static EntregarNotificacaoSistemaUseCase CriarUseCase(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeNotificacaoProcessamentoRepository(context),
            PortalUseCasesTestFactory.Uow(context));

    private static Usuario CriarUsuario(string sufixo)
        => new($"Usuario {sufixo}", $"usuario.{sufixo}@teste.local", $"login.{sufixo}", "teste");

    private static Notificacao CriarNotificacaoSistema(Guid destinatarioUsuarioId, string chaveIdempotencia)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo materializado do canal Sistema.",
            chaveIdempotencia,
            "teste",
            destinatarioUsuarioId,
            null,
            null,
            "Assunto Sistema",
            $"corr:{chaveIdempotencia}",
            destinatarioUsuarioId);

    private static readonly DateTime DataEntregaUtc = new(2026, 6, 21, 15, 0, 0, DateTimeKind.Utc);

    private sealed class FakeNotificacaoProcessamentoRepository(SGXSistemaChamadoDbContext context) : INotificacaoProcessamentoRepository
    {
        public Task<bool> TentarIniciarProcessamentoAsync(
            Guid notificacaoId,
            DateTime processadaEm,
            string atualizadoPor,
            Guid? atualizadoPorUsuarioId,
            int limiteTentativas,
            CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<bool> TentarRegistrarSucessoAsync(
            Guid notificacaoId,
            DateTime enviadaEm,
            string atualizadoPor,
            Guid? atualizadoPorUsuarioId,
            CancellationToken cancellationToken = default)
        {
            var notificacao = context.Notificacoes.SingleOrDefault(x => x.Id == notificacaoId);
            if (notificacao is null || notificacao.Status != StatusNotificacao.EmProcessamento)
            {
                return Task.FromResult(false);
            }

            notificacao.RegistrarEnvio(enviadaEm, atualizadoPor, atualizadoPorUsuarioId);
            return Task.FromResult(true);
        }
    }
}
