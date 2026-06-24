using Microsoft.EntityFrameworkCore;
using Npgsql;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

[Collection("NotificacoesPersistenceRelational")]
public sealed class LeituraNotificacaoPersistenceTests : IClassFixture<NotificacaoPersistenceDatabaseFixture>
{
    private readonly NotificacaoPersistenceDatabaseFixture _fixture;

    public LeituraNotificacaoPersistenceTests(NotificacaoPersistenceDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DevePersistirLeituraENaoLeituraDeNotificacaoSistema()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var dataLeitura = TruncarParaMicrosegundos(notificacao.EnviadaEm!.Value.AddMinutes(1));
        notificacao.MarcarComoLida(dataLeitura, "teste", usuarioId);
        await context.SaveChangesAsync();

        await using var consulta = _fixture.CreateContext();
        var persistida = await consulta.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Equal(dataLeitura, persistida.LidaEm);

        persistida.MarcarComoNaoLida("teste", usuarioId);
        await consulta.SaveChangesAsync();

        await using var verificacao = _fixture.CreateContext();
        var final = await verificacao.Notificacoes.SingleAsync(x => x.Id == notificacao.Id);
        Assert.Null(final.LidaEm);
        Assert.Equal(StatusNotificacao.Enviada, final.Status);
    }

    [Fact]
    public async Task DeveRejeitarPersistenciaDeLeituraAnteriorAoEnvio()
    {
        await _fixture.ResetAsync();
        await using var context = _fixture.CreateContext();
        var usuarioId = await _fixture.CriarUsuarioTemporarioAsync(context);
        var notificacao = CriarNotificacaoSistemaEnviada(usuarioId, _fixture.NovaChaveIdempotencia());
        context.Notificacoes.Add(notificacao);
        await context.SaveChangesAsync();

        var ex = await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE notificacoes SET lida_em = {notificacao.EnviadaEm!.Value.AddMinutes(-1)} WHERE id = {notificacao.Id}"));

        Assert.Equal(PostgresErrorCodes.CheckViolation, ex.SqlState);
        Assert.Equal("ck_notificacoes_lida_em_maior_ou_igual_enviada_em", ex.ConstraintName);
    }

    private static Notificacao CriarNotificacaoSistemaEnviada(Guid usuarioId, string chave)
    {
        var notificacao = new Notificacao(
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            "Conteudo Sistema",
            chave,
            "teste",
            usuarioId,
            null,
            null,
            "Assunto Sistema",
            $"corr:{chave}",
            usuarioId);
        notificacao.IniciarProcessamento(DateTime.UtcNow.AddMinutes(-2), "teste", usuarioId);
        notificacao.RegistrarEnvio(DateTime.UtcNow.AddMinutes(-1), "teste", usuarioId);
        return notificacao;
    }

    private static DateTime TruncarParaMicrosegundos(DateTime dataUtc)
        => new(dataUtc.Ticks - (dataUtc.Ticks % 10), DateTimeKind.Utc);
}
