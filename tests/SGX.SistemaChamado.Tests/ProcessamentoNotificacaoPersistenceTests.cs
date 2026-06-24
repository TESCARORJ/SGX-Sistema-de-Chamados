using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Repositories;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class ProcessamentoNotificacaoPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public ProcessamentoNotificacaoPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeveExecutarCicloCompletoComFalhaTransitoriaESucessoPosterior()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacao(usuarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await using var contextoSelecao1 = _fixture.CreateContext();
        var selecionar = new SelecionarNotificacoesProcessaveisUseCase(new Infrastructure.Repositories.Repository<Notificacao>(contextoSelecao1));
        var processaveis = await selecionar.ExecutarAsync(new SelecionarNotificacoesProcessaveisRequest(10, DateTime.UtcNow));
        Assert.Contains(processaveis, x => x.NotificacaoId == notificacao.Id);

        await using var contextoInicio1 = _fixture.CreateContext();
        var inicio = new IniciarProcessamentoNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contextoInicio1),
            new NotificacaoProcessamentoRepository(contextoInicio1),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contextoInicio1));
        await inicio.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(
            notificacao.Id,
            new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc)));

        await using var contextoFalha = _fixture.CreateContext();
        var falha = new RegistrarFalhaEntregaNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contextoFalha),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contextoFalha));
        var falhaResposta = await falha.ExecutarAsync(new RegistrarFalhaEntregaNotificacaoRequest(
            notificacao.Id,
            "timeout",
            true,
            new DateTime(2026, 6, 21, 12, 5, 0, DateTimeKind.Utc)));

        Assert.Equal(StatusNotificacao.Agendada, falhaResposta.Status);

        await using var contextoSelecao2 = _fixture.CreateContext();
        selecionar = new SelecionarNotificacoesProcessaveisUseCase(new Infrastructure.Repositories.Repository<Notificacao>(contextoSelecao2));
        var novamente = await selecionar.ExecutarAsync(new SelecionarNotificacoesProcessaveisRequest(
            10,
            new DateTime(2026, 6, 21, 12, 6, 0, DateTimeKind.Utc)));
        Assert.Contains(novamente, x => x.NotificacaoId == notificacao.Id);

        await using var contextoInicio2 = _fixture.CreateContext();
        inicio = new IniciarProcessamentoNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contextoInicio2),
            new NotificacaoProcessamentoRepository(contextoInicio2),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contextoInicio2));
        await inicio.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(
            notificacao.Id,
            new DateTime(2026, 6, 21, 12, 6, 0, DateTimeKind.Utc)));

        await using var contextoSucesso = _fixture.CreateContext();
        var sucesso = new RegistrarSucessoEntregaNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contextoSucesso),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contextoSucesso));
        await sucesso.ExecutarAsync(new RegistrarSucessoEntregaNotificacaoRequest(
            notificacao.Id,
            new DateTime(2026, 6, 21, 12, 7, 0, DateTimeKind.Utc)));

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(2, persistida.QuantidadeTentativas);
        Assert.Equal(new DateTime(2026, 6, 21, 12, 7, 0, DateTimeKind.Utc), persistida.EnviadaEm);
    }

    [Fact]
    public async Task DevePermitirApenasUmaAquisicaoConcorrenteDaMesmaNotificacao()
    {
        await _fixture.ResetAsync();
        await using var contextoBase = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(contextoBase);
        var notificacao = CriarNotificacao(usuarioId, _fixture.NovaChaveIdempotencia());
        contextoBase.Notificacoes.Add(notificacao);
        await contextoBase.SaveChangesAsync();

        await using var contexto1 = _fixture.CreateContext();
        await using var contexto2 = _fixture.CreateContext();

        var useCase1 = new IniciarProcessamentoNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contexto1),
            new NotificacaoProcessamentoRepository(contexto1),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contexto1));
        var useCase2 = new IniciarProcessamentoNotificacaoUseCase(
            new Infrastructure.Repositories.Repository<Notificacao>(contexto2),
            new NotificacaoProcessamentoRepository(contexto2),
            CriarUsuarioContexto(usuarioId),
            new Infrastructure.Repositories.UnitOfWork(contexto2));

        var task1 = ExecutarInicioCapturandoResultadoAsync(useCase1, notificacao.Id);
        var task2 = ExecutarInicioCapturandoResultadoAsync(useCase2, notificacao.Id);

        await Task.WhenAll(task1, task2);

        var resultados = new[] { await task1, await task2 };
        Assert.Single(resultados, x => x);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.EmProcessamento, persistida.Status);
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    [Fact]
    public async Task NaoDeveSelecionarNovamenteQuandoLimiteForAtingido()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacao(usuarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var proximaExecucao = new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc);
        for (var tentativa = 0; tentativa < 5; tentativa++)
        {
            await using var contextoInicio = _fixture.CreateContext();
            var inicio = new IniciarProcessamentoNotificacaoUseCase(
                new Infrastructure.Repositories.Repository<Notificacao>(contextoInicio),
                new NotificacaoProcessamentoRepository(contextoInicio),
                CriarUsuarioContexto(usuarioId),
                new Infrastructure.Repositories.UnitOfWork(contextoInicio));
            await inicio.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(
                notificacao.Id,
                proximaExecucao));

            await using var contextoFalha = _fixture.CreateContext();
            var falha = new RegistrarFalhaEntregaNotificacaoUseCase(
                new Infrastructure.Repositories.Repository<Notificacao>(contextoFalha),
                CriarUsuarioContexto(usuarioId),
                new Infrastructure.Repositories.UnitOfWork(contextoFalha));
            var respostaFalha = await falha.ExecutarAsync(new RegistrarFalhaEntregaNotificacaoRequest(
                notificacao.Id,
                $"falha-{tentativa}",
                tentativa < 4,
                proximaExecucao.AddSeconds(30)));

            if (respostaFalha.AgendadaEm.HasValue)
            {
                proximaExecucao = respostaFalha.AgendadaEm.Value;
            }
        }

        await using var contextoSelecao = _fixture.CreateContext();
        var selecionar = new SelecionarNotificacoesProcessaveisUseCase(new Infrastructure.Repositories.Repository<Notificacao>(contextoSelecao));
        var processaveis = await selecionar.ExecutarAsync(new SelecionarNotificacoesProcessaveisRequest(
            10,
            new DateTime(2026, 6, 21, 13, 0, 0, DateTimeKind.Utc)));

        Assert.DoesNotContain(processaveis, x => x.NotificacaoId == notificacao.Id);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.Falhou, persistida.Status);
        Assert.Equal(5, persistida.QuantidadeTentativas);
    }

    private static async Task<bool> ExecutarInicioCapturandoResultadoAsync(
        IniciarProcessamentoNotificacaoUseCase useCase,
        Guid notificacaoId)
    {
        try
        {
            await useCase.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(
                notificacaoId,
                new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc)));
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static FakeUsuarioContextoAplicacaoService CriarUsuarioContexto(Guid usuarioAtualId)
        => new(new(
            usuarioAtualId,
            "Processador",
            "processador@sgx.local",
            "processador",
            ["Sistema"]));

    private static Notificacao CriarNotificacao(Guid usuarioId, string chave)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo de teste",
            chave,
            "test.processamento",
            usuarioId,
            null,
            null,
            "Assunto",
            $"corr:{Guid.NewGuid():N}",
            usuarioId);
}
