using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Repositories;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class EntregaNotificacaoEmailPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public EntregaNotificacaoEmailPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeveConcluirEntregaEmailEPermanecerIdempotenteAposSucessoPersistido()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var processadorId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoEmail(usuarioId, "email@cliente.com", _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacao.Id, processadorId);
        var transportador = new FakeTransportadorEmailNotificacao(new ResultadoTransporteEmailNotificacao(true, false, "<msg@sgx>", null));

        var primeira = await EntregarAsync(notificacao.Id, processadorId, transportador);
        var segunda = await EntregarAsync(notificacao.Id, processadorId, transportador);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.True(primeira.Entregue);
        Assert.False(segunda.Entregue);
        Assert.True(segunda.JaEstavaEntregue);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(1, persistida.QuantidadeTentativas);
        Assert.Equal(1, transportador.Chamadas);
        Assert.Equal(1, await consulta.Notificacoes.CountAsync(x => x.Id == notificacao.Id));
    }

    [Fact]
    public async Task DeveReagendarQuandoTransporteFalharDeFormaTransitoria()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var processadorId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoEmail(usuarioId, "temporario@cliente.com", _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacao.Id, processadorId);
        var transportador = new FakeTransportadorEmailNotificacao(new ResultadoTransporteEmailNotificacao(false, true, null, "SMTP 450 timeout"));

        var response = await EntregarAsync(notificacao.Id, processadorId, transportador);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.False(response.Entregue);
        Assert.True(response.Reagendada);
        Assert.Equal(StatusNotificacao.Agendada, persistida.Status);
        Assert.NotNull(persistida.AgendadaEm);
        Assert.Equal(1, persistida.QuantidadeTentativas);
        Assert.Equal("SMTP 450 timeout", persistida.UltimoErro);
        Assert.Equal(1, transportador.Chamadas);
    }

    [Fact]
    public async Task DeveEncerrarQuandoTransporteFalharDeFormaDefinitivaSemAfetarCanalSistema()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioEmailId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var usuarioSistemaId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var processadorId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacaoEmail = CriarNotificacaoEmail(usuarioEmailId, "definitivo@cliente.com", _fixture.NovaChaveIdempotencia());
        var notificacaoSistema = CriarNotificacaoSistema(usuarioSistemaId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.AddRange(notificacaoEmail, notificacaoSistema);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacaoEmail.Id, processadorId);
        var transportador = new FakeTransportadorEmailNotificacao(new ResultadoTransporteEmailNotificacao(false, false, null, "SMTP 550 mailbox unavailable"));

        var response = await EntregarAsync(notificacaoEmail.Id, processadorId, transportador);

        await using var consulta = _fixture.CreateContext();
        var persistidaEmail = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacaoEmail.Id);
        var persistidaSistema = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacaoSistema.Id);
        Assert.False(response.Entregue);
        Assert.False(response.Reagendada);
        Assert.Equal(StatusNotificacao.Falhou, persistidaEmail.Status);
        Assert.Equal("SMTP 550 mailbox unavailable", persistidaEmail.UltimoErro);
        Assert.Equal(StatusNotificacao.Pendente, persistidaSistema.Status);
        Assert.Null(persistidaSistema.EnviadaEm);
        Assert.Equal(0, await consulta.Notificacoes.CountAsync(x => x.Status == StatusNotificacao.EmProcessamento));
    }

    [Fact]
    public async Task NaoDeveChamarTransporteEmChamadasConcorrentesQuandoSucessoJaEstiverPersistido()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var processadorId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoEmail(usuarioId, "concorrente@cliente.com", _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacao.Id, processadorId);
        var transportador = new FakeTransportadorEmailNotificacao(new ResultadoTransporteEmailNotificacao(true, false, "<msg@sgx>", null));
        await EntregarAsync(notificacao.Id, processadorId, transportador);

        var task1 = EntregarAsync(notificacao.Id, processadorId, transportador);
        var task2 = EntregarAsync(notificacao.Id, processadorId, transportador);
        await Task.WhenAll(task1, task2);

        Assert.True((await task1).JaEstavaEntregue);
        Assert.True((await task2).JaEstavaEntregue);
        Assert.Equal(1, transportador.Chamadas);
    }

    private async Task<EntregarNotificacaoEmailResponse> EntregarAsync(
        Guid notificacaoId,
        Guid processadorId,
        FakeTransportadorEmailNotificacao transportador)
    {
        await using var context = _fixture.CreateContext();
        var usuarioContexto = new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
            processadorId,
            "Processador Email",
            "processador.email@sgx.local",
            "processador.email",
            ["Sistema"]));

        var useCase = new EntregarNotificacaoEmailUseCase(
            new Repository<Notificacao>(context),
            transportador,
            new RegistrarSucessoEntregaNotificacaoUseCase(
                new Repository<Notificacao>(context),
                usuarioContexto,
                new UnitOfWork(context)),
            new RegistrarFalhaEntregaNotificacaoUseCase(
                new Repository<Notificacao>(context),
                usuarioContexto,
                new UnitOfWork(context)),
            NullLogger<EntregarNotificacaoEmailUseCase>.Instance);

        return await useCase.ExecutarAsync(new EntregarNotificacaoEmailRequest(notificacaoId, DataEntregaUtc));
    }

    private async Task IniciarProcessamentoAsync(Guid notificacaoId, Guid usuarioAtualId)
    {
        await using var context = _fixture.CreateContext();
        var useCase = new IniciarProcessamentoNotificacaoUseCase(
            new Repository<Notificacao>(context),
            new NotificacaoProcessamentoRepository(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                usuarioAtualId,
                "Processador",
                "processador@sgx.local",
                "processador",
                ["Sistema"])),
            new UnitOfWork(context));

        await useCase.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(notificacaoId, DataEntregaUtc.AddMinutes(-1)));
    }

    private static Notificacao CriarNotificacaoEmail(Guid usuarioId, string endereco, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "<p>Conteudo materializado do canal Email.</p>",
            chave,
            "test.notificacao.persistence",
            usuarioId,
            endereco,
            null,
            "Assunto Email",
            $"corr:{chave}",
            usuarioId);

    private static Notificacao CriarNotificacaoSistema(Guid usuarioId, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo sistema",
            chave,
            "test.notificacao.persistence",
            usuarioId,
            null,
            null,
            "Assunto Sistema",
            $"corr:{chave}",
            usuarioId);

    private static readonly DateTime DataEntregaUtc = new(2026, 6, 23, 16, 0, 0, DateTimeKind.Utc);

    private sealed class FakeTransportadorEmailNotificacao(ResultadoTransporteEmailNotificacao resultado) : ITransportadorEmailNotificacao
    {
        public int Chamadas { get; private set; }

        public Task<ResultadoTransporteEmailNotificacao> EnviarAsync(
            MensagemEmailNotificacao mensagem,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Chamadas++;
            return Task.FromResult(resultado);
        }
    }
}
