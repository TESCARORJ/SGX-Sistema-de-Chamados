using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class GerarNotificacaoUseCaseTests
{
    [Fact]
    public async Task DeveCriarNotificacaoPendenteNaPrimeiraExecucao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        var usuarioId = Guid.NewGuid();
        var request = CriarRequest(destinatarioUsuarioId: usuarioId, destinatarioEndereco: null);

        var response = await useCase.ExecutarAsync(request);
        var notificacao = await context.Notificacoes.SingleAsync(x => x.Id == response.NotificacaoId);

        Assert.True(response.Criada);
        Assert.False(response.JaExistia);
        Assert.Equal(StatusNotificacao.Pendente, response.Status);
        Assert.Equal(StatusNotificacao.Pendente, notificacao.Status);
        Assert.Equal(0, notificacao.QuantidadeTentativas);
        Assert.Equal(usuarioId, notificacao.DestinatarioUsuarioId);
        Assert.Null(notificacao.DestinatarioEndereco);
        Assert.Equal(request.Conteudo, notificacao.Conteudo);
        Assert.Equal(request.Assunto, notificacao.Assunto);
        Assert.Equal(request.Evento.ChamadoId, notificacao.ChamadoId);
        Assert.Equal(request.Evento.ChaveCorrelacao, notificacao.ChaveCorrelacao);
        Assert.Equal(request.Evento.ChaveIdempotencia, notificacao.ChaveIdempotencia);
    }

    [Fact]
    public async Task DeveCriarNotificacaoAgendadaQuandoRequestInformarData()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        var agendadaEm = new DateTime(2026, 6, 21, 15, 0, 0, DateTimeKind.Utc);

        var response = await useCase.ExecutarAsync(CriarRequest(agendadaEm: agendadaEm));
        var notificacao = await context.Notificacoes.SingleAsync(x => x.Id == response.NotificacaoId);

        Assert.True(response.Criada);
        Assert.Equal(StatusNotificacao.Agendada, response.Status);
        Assert.Equal(StatusNotificacao.Agendada, notificacao.Status);
        Assert.Equal(agendadaEm, notificacao.AgendadaEm);
        Assert.Null(notificacao.ProcessadaEm);
        Assert.Null(notificacao.EnviadaEm);
        Assert.Equal(0, notificacao.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveRetornarNotificacaoExistenteSemDuplicarNemAlterarDados()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        var requestInicial = CriarRequest(
            assunto: "Assunto original",
            conteudo: "Conteudo original",
            destinatarioEndereco: "primeiro@teste.local",
            chaveIdempotencia: "notif:usecase:duplicada");

        var primeira = await useCase.ExecutarAsync(requestInicial);
        var segunda = await useCase.ExecutarAsync(CriarRequest(
            assunto: "Assunto divergente",
            conteudo: "Conteudo divergente",
            destinatarioEndereco: "segundo@teste.local",
            chaveIdempotencia: "notif:usecase:duplicada"));

        var notificacao = await context.Notificacoes.SingleAsync();

        Assert.True(primeira.Criada);
        Assert.False(primeira.JaExistia);
        Assert.False(segunda.Criada);
        Assert.True(segunda.JaExistia);
        Assert.Equal(primeira.NotificacaoId, segunda.NotificacaoId);
        Assert.Single(context.Notificacoes);
        Assert.Equal("Assunto original", notificacao.Assunto);
        Assert.Equal("Conteudo original", notificacao.Conteudo);
        Assert.Equal("primeiro@teste.local", notificacao.DestinatarioEndereco);
        Assert.Equal(StatusNotificacao.Pendente, notificacao.Status);
        Assert.Equal(0, notificacao.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveAceitarChamadoNulo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);

        var response = await useCase.ExecutarAsync(CriarRequest(chamadoId: null));
        var notificacao = await context.Notificacoes.SingleAsync(x => x.Id == response.NotificacaoId);

        Assert.Null(notificacao.ChamadoId);
    }

    [Fact]
    public async Task DeveLancarValidationExceptionQuandoRequestInvalido()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);

        await Assert.ThrowsAsync<ValidationException>(() =>
            useCase.ExecutarAsync(CriarRequest(destinatarioUsuarioId: null, destinatarioEndereco: null)));
    }

    [Fact]
    public async Task DeveRespeitarCancellationTokenCancelado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCase = CriarUseCase(context);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            useCase.ExecutarAsync(CriarRequest(), cts.Token));
    }

    private static GerarNotificacaoUseCase CriarUseCase(
        SGX.SistemaChamado.Infrastructure.Persistence.SGXSistemaChamadoDbContext context,
        Guid? usuarioAtualId = null)
    {
        var usuarioId = usuarioAtualId ?? Guid.NewGuid();
        return new GerarNotificacaoUseCase(
            PortalUseCasesTestFactory.Repo<Notificacao>(context),
            PortalUseCasesTestFactory.Uow(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioId,
                "Usuario Atual",
                "usuario.atual@sgx.local",
                "usuario.atual",
                ["Administrador"])));
    }

    private static GerarNotificacaoRequest CriarRequest(
        Guid? chamadoId = null,
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = "destinatario@teste.local",
        string? assunto = "Assunto da notificacao",
        string conteudo = "Conteudo materializado da notificacao.",
        DateTime? agendadaEm = null,
        string chaveIdempotencia = "notif:usecase:001")
    {
        return new GerarNotificacaoRequest(
            new EventoCandidatoNotificacao(
                TipoEventoNotificacao.EventoChamado,
                chamadoId,
                Guid.NewGuid(),
                new DateTime(2026, 6, 21, 16, 0, 0, DateTimeKind.Utc),
                "corr-usecase-001",
                chaveIdempotencia,
                new Dictionary<string, string>
                {
                    ["origem"] = "teste"
                }),
            CanalNotificacao.Email,
            destinatarioUsuarioId,
            destinatarioEndereco,
            assunto,
            conteudo,
            agendadaEm);
    }
}
