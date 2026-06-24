using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.UseCases.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Repositories;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class EntregaNotificacaoSistemaPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public EntregaNotificacaoSistemaPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeveEntregarNotificacaoSistemaEPermitirConsultaSomenteAoDestinatario()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var outroUsuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);

        var notificacaoSistema = CriarNotificacaoSistema(destinatarioId, _fixture.NovaChaveIdempotencia());
        var notificacaoEmail = CriarNotificacaoEmail(destinatarioId, _fixture.NovaChaveIdempotencia());
        var notificacaoOutroUsuario = CriarNotificacaoSistema(outroUsuarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.AddRange(notificacaoSistema, notificacaoEmail, notificacaoOutroUsuario);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacaoSistema.Id, destinatarioId, DataEntregaUtc.AddMinutes(-2));
        await IniciarProcessamentoAsync(notificacaoOutroUsuario.Id, outroUsuarioId, DataEntregaUtc.AddMinutes(-2));

        await using var contextoEntrega = _fixture.CreateContext();
        var entregar = new EntregarNotificacaoSistemaUseCase(
            new Repository<Notificacao>(contextoEntrega),
            new Repository<Usuario>(contextoEntrega),
            new NotificacaoProcessamentoRepository(contextoEntrega),
            new UnitOfWork(contextoEntrega));

        var response = await entregar.ExecutarAsync(new EntregarNotificacaoSistemaRequest(
            notificacaoSistema.Id,
            DataEntregaUtc));

        Assert.True(response.Entregue);
        Assert.False(response.JaEstavaEntregue);
        Assert.Equal(destinatarioId, response.DestinatarioUsuarioId);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacaoSistema.Id);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(DataEntregaUtc, persistida.EnviadaEm);
        Assert.Equal(destinatarioId, persistida.DestinatarioUsuarioId);
        Assert.Equal("Assunto Sistema", persistida.Assunto);
        Assert.Equal("Conteudo materializado do canal Sistema.", persistida.Conteudo);
        Assert.Equal(1, persistida.QuantidadeTentativas);

        await using var contextoListagem = _fixture.CreateContext();
        var listar = new ListarNotificacoesSistemaUsuarioUseCase(new Repository<Notificacao>(contextoListagem));
        var doDestinatario = await listar.ExecutarAsync(new ListarNotificacoesSistemaUsuarioRequest(destinatarioId));
        var doOutroUsuario = await listar.ExecutarAsync(new ListarNotificacoesSistemaUsuarioRequest(outroUsuarioId));

        Assert.Single(doDestinatario);
        Assert.Equal(notificacaoSistema.Id, doDestinatario.Single().NotificacaoId);
        Assert.DoesNotContain(doDestinatario, x => x.NotificacaoId == notificacaoEmail.Id);
        Assert.Empty(doOutroUsuario);
    }

    [Fact]
    public async Task DeveManterIdempotenciaQuandoEntregaForExecutadaDuasVezes()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoSistema(destinatarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacao.Id, destinatarioId, DataEntregaUtc.AddMinutes(-1));

        var primeira = await EntregarAsync(notificacao.Id, DataEntregaUtc);
        var segunda = await EntregarAsync(notificacao.Id, DataEntregaUtc.AddMinutes(5));

        Assert.True(primeira.Entregue);
        Assert.False(primeira.JaEstavaEntregue);
        Assert.False(segunda.Entregue);
        Assert.True(segunda.JaEstavaEntregue);
        Assert.Equal(DataEntregaUtc, segunda.EntregueEm);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(DataEntregaUtc, persistida.EnviadaEm);
        Assert.Equal(1, await consulta.Notificacoes.CountAsync(x => x.ChaveIdempotencia == notificacao.ChaveIdempotencia));
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    [Fact]
    public async Task DevePermitirApenasUmaConclusaoEfetivaEmEntregasConcorrentesDaMesmaNotificacao()
    {
        await _fixture.ResetAsync();
        await using var contextoBase = _fixture.CreateContext();
        var destinatarioId = await _fixture.CriarUsuarioTemporarioAsync(contextoBase);
        var notificacao = CriarNotificacaoSistema(destinatarioId, _fixture.NovaChaveIdempotencia());
        contextoBase.Notificacoes.Add(notificacao);
        await contextoBase.SaveChangesAsync();

        await IniciarProcessamentoAsync(notificacao.Id, destinatarioId, DataEntregaUtc.AddMinutes(-1));

        var task1 = EntregarAsync(notificacao.Id, DataEntregaUtc);
        var task2 = EntregarAsync(notificacao.Id, DataEntregaUtc.AddMinutes(1));

        await Task.WhenAll(task1, task2);

        var resultados = new[] { await task1, await task2 };
        Assert.Single(resultados, x => x.Entregue);
        Assert.Single(resultados, x => x.JaEstavaEntregue);
        var entregaEfetiva = resultados.Single(x => x.Entregue);

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(StatusNotificacao.Enviada, persistida.Status);
        Assert.Equal(entregaEfetiva.EntregueEm, persistida.EnviadaEm);
        Assert.Equal(1, persistida.QuantidadeTentativas);
    }

    private async Task<EntregarNotificacaoSistemaResponse> EntregarAsync(Guid notificacaoId, DateTime entregueEm)
    {
        await using var context = _fixture.CreateContext();
        var useCase = new EntregarNotificacaoSistemaUseCase(
            new Repository<Notificacao>(context),
            new Repository<Usuario>(context),
            new NotificacaoProcessamentoRepository(context),
            new UnitOfWork(context));

        return await useCase.ExecutarAsync(new EntregarNotificacaoSistemaRequest(notificacaoId, entregueEm));
    }

    private async Task IniciarProcessamentoAsync(Guid notificacaoId, Guid usuarioAtualId, DateTime iniciadaEm)
    {
        await using var context = _fixture.CreateContext();
        var useCase = new IniciarProcessamentoNotificacaoUseCase(
            new Repository<Notificacao>(context),
            new NotificacaoProcessamentoRepository(context),
            CriarUsuarioContexto(usuarioAtualId),
            new UnitOfWork(context));

        await useCase.ExecutarAsync(new IniciarProcessamentoNotificacaoRequest(notificacaoId, iniciadaEm));
    }

    private static FakeUsuarioContextoAplicacaoService CriarUsuarioContexto(Guid usuarioAtualId)
        => new(new(
            usuarioAtualId,
            "Processador Sistema",
            "processador.sistema@sgx.local",
            "processador.sistema",
            ["Sistema"]));

    private static Notificacao CriarNotificacaoSistema(Guid destinatarioUsuarioId, string chaveIdempotencia)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo materializado do canal Sistema.",
            chaveIdempotencia,
            "test.notificacao.persistence",
            destinatarioUsuarioId,
            null,
            null,
            "Assunto Sistema",
            $"corr:{chaveIdempotencia}",
            destinatarioUsuarioId);

    private static Notificacao CriarNotificacaoEmail(Guid destinatarioUsuarioId, string chaveIdempotencia)
        => new(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            "Conteudo email",
            chaveIdempotencia,
            "test.notificacao.persistence",
            destinatarioUsuarioId,
            null,
            null,
            "Assunto Email",
            $"corr:{chaveIdempotencia}",
            destinatarioUsuarioId);

    private static readonly DateTime DataEntregaUtc = new(2026, 6, 21, 18, 0, 0, DateTimeKind.Utc);
}
