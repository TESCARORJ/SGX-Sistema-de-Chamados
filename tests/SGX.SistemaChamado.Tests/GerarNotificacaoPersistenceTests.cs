using System.Threading;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using SGX.SistemaChamado.Infrastructure.Repositories;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class GerarNotificacaoPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public GerarNotificacaoPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DevePersistirViaServicoERecuperarDadosDaNotificacao()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var chamado = await _fixture.CriarChamadoTemporarioAsync(context);
        var useCase = CriarUseCase(context, usuarioAtualId);
        var request = CriarRequest(
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            chamadoId: chamado.Id,
            destinatarioUsuarioId: destinatarioId,
            destinatarioEndereco: "servico.persistencia@cliente.com",
            agendadaEm: new DateTime(2026, 6, 21, 17, 0, 0, DateTimeKind.Utc),
            usuarioOriginadorId: usuarioAtualId);

        var response = await useCase.ExecutarAsync(request);

        await using var consulta = _fixture.CreateContext();
        var notificacao = await consulta.Notificacoes.SingleAsync(x => x.Id == response.NotificacaoId);

        Assert.True(response.Criada);
        Assert.False(response.JaExistia);
        Assert.Equal(StatusNotificacao.Agendada, notificacao.Status);
        Assert.Equal(chamado.Id, notificacao.ChamadoId);
        Assert.Equal(destinatarioId, notificacao.DestinatarioUsuarioId);
        Assert.Equal("servico.persistencia@cliente.com", notificacao.DestinatarioEndereco);
        Assert.Equal(request.Evento.ChaveCorrelacao, notificacao.ChaveCorrelacao);
        Assert.Equal(request.Evento.ChaveIdempotencia, notificacao.ChaveIdempotencia);
        Assert.Equal(usuarioAtualId, notificacao.CriadoPorUsuarioId);
        Assert.Equal(0, notificacao.QuantidadeTentativas);
    }

    [Fact]
    public async Task DeveRetornarMesmaNotificacaoEmExecucoesSequenciaisComMesmaChave()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var useCase = CriarUseCase(context, usuarioAtualId);
        var chave = _fixture.NovaChaveIdempotencia();

        var primeira = await useCase.ExecutarAsync(CriarRequest(
            chaveIdempotencia: chave,
            destinatarioUsuarioId: destinatarioId,
            usuarioOriginadorId: usuarioAtualId));
        var segunda = await useCase.ExecutarAsync(CriarRequest(
            chaveIdempotencia: chave,
            destinatarioUsuarioId: destinatarioId,
            destinatarioEndereco: "outro@cliente.com",
            conteudo: "conteudo divergente",
            usuarioOriginadorId: usuarioAtualId));

        await using var consulta = _fixture.CreateContext();
        var notificacoes = await consulta.Notificacoes.ToListAsync();

        Assert.Single(notificacoes);
        Assert.True(primeira.Criada);
        Assert.True(segunda.JaExistia);
        Assert.Equal(primeira.NotificacaoId, segunda.NotificacaoId);
        Assert.Equal(chave, notificacoes.Single().ChaveIdempotencia);
        Assert.NotEqual("conteudo divergente", notificacoes.Single().Conteudo);
    }

    [Fact]
    public async Task DeveTratarConcorrenciaComMesmaChaveSemEscaparErroDeUnicidade()
    {
        await _fixture.ResetAsync();
        await using var preparo = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(preparo);
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(preparo);
        var chave = _fixture.NovaChaveIdempotencia();
        using var barreira = new Barrier(2);

        async Task<GerarNotificacaoResponse> ExecutarAsync(string assunto)
        {
            await using var context = _fixture.CreateContext();
            var useCase = new GerarNotificacaoUseCase(
                new Repository<Notificacao>(context),
                new UnitOfWorkSincronizado(context, barreira),
                CriarUsuarioContexto(usuarioAtualId));

            return await useCase.ExecutarAsync(CriarRequest(
                chaveIdempotencia: chave,
                destinatarioUsuarioId: destinatarioId,
                assunto: assunto,
                usuarioOriginadorId: usuarioAtualId));
        }

        var resultados = await Task.WhenAll(
            ExecutarAsync("Primeira tentativa"),
            ExecutarAsync("Segunda tentativa"));

        await using var consulta = _fixture.CreateContext();
        var notificacoes = await consulta.Notificacoes
            .Where(x => x.ChaveIdempotencia == chave)
            .ToListAsync();

        Assert.Single(notificacoes);
        Assert.Equal(resultados[0].NotificacaoId, resultados[1].NotificacaoId);
        Assert.Equal(1, resultados.Count(x => x.Criada));
        Assert.Equal(1, resultados.Count(x => x.JaExistia));
    }

    [Fact]
    public async Task DevePropagarErroNaoRelacionadoAIdempotenciaESemPersistirResiduo()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var useCase = CriarUseCase(context, usuarioAtualId);

        await Assert.ThrowsAsync<DbUpdateException>(() => useCase.ExecutarAsync(CriarRequest(
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            destinatarioUsuarioId: Guid.NewGuid(),
            usuarioOriginadorId: usuarioAtualId)));

        await using var consulta = _fixture.CreateContext();
        Assert.Equal(0, await consulta.Notificacoes.CountAsync());
    }

    [Fact]
    public async Task DeveFalharComValidationExceptionAntesDaPersistencia()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioAtualId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var useCase = CriarUseCase(context, usuarioAtualId);

        await Assert.ThrowsAsync<ValidationException>(() => useCase.ExecutarAsync(CriarRequest(
            chaveIdempotencia: _fixture.NovaChaveIdempotencia(),
            destinatarioUsuarioId: null,
            destinatarioEndereco: null,
            usuarioOriginadorId: usuarioAtualId)));

        Assert.Equal(0, await context.Notificacoes.CountAsync());
    }

    private static GerarNotificacaoUseCase CriarUseCase(SGXSistemaChamadoDbContext context, Guid usuarioAtualId)
    {
        return new GerarNotificacaoUseCase(
            new Repository<Notificacao>(context),
            new UnitOfWork(context),
            CriarUsuarioContexto(usuarioAtualId));
    }

    private static FakeUsuarioContextoAplicacaoService CriarUsuarioContexto(Guid usuarioAtualId)
        => new(new UsuarioContextoAplicacao(
            usuarioAtualId,
            "Usuario Atual",
            "usuario.atual@teste.local",
            "usuario.atual",
            ["Administrador"]));

    private static GerarNotificacaoRequest CriarRequest(
        string chaveIdempotencia,
        Guid? chamadoId = null,
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = "destinatario@teste.local",
        string? assunto = "Assunto persistido",
        string conteudo = "Conteudo persistido da notificacao",
        DateTime? agendadaEm = null,
        Guid? usuarioOriginadorId = null)
    {
        return new GerarNotificacaoRequest(
            new EventoCandidatoNotificacao(
                TipoEventoNotificacao.EventoChamado,
                chamadoId,
                usuarioOriginadorId,
                new DateTime(2026, 6, 21, 18, 0, 0, DateTimeKind.Utc),
                "corr-persistencia-servico",
                chaveIdempotencia,
                new Dictionary<string, string>
                {
                    ["origem"] = "teste-relacional"
                }),
            CanalNotificacao.Email,
            destinatarioUsuarioId,
            destinatarioEndereco,
            assunto,
            conteudo,
            agendadaEm);
    }

    private sealed class UnitOfWorkSincronizado(SGXSistemaChamadoDbContext context, Barrier barreira) : SGX.SistemaChamado.Application.Interfaces.Persistence.IUnitOfWork
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            barreira.SignalAndWait(cancellationToken);
            return context.SaveChangesAsync(cancellationToken);
        }
    }
}
