using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class PreferenciaNotificacaoUsuarioTests
{
    [Fact]
    public void DeveCriarPreferenciaHabilitada()
    {
        var usuarioId = Guid.NewGuid();
        var criadoPorUsuarioId = Guid.NewGuid();

        var preferencia = new PreferenciaNotificacaoUsuario(
            usuarioId,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Sistema,
            true,
            criadoPorUsuarioId,
            "teste");

        Assert.Equal(usuarioId, preferencia.UsuarioId);
        Assert.Equal(TipoEventoNotificacao.EventoChamado, preferencia.TipoEvento);
        Assert.Equal(CanalNotificacao.Sistema, preferencia.Canal);
        Assert.True(preferencia.Habilitada);
        Assert.Equal(criadoPorUsuarioId, preferencia.CriadoPorUsuarioId);
        Assert.True(preferencia.Ativo);
    }

    [Fact]
    public void DeveCriarPreferenciaDesabilitada()
    {
        var preferencia = new PreferenciaNotificacaoUsuario(
            Guid.NewGuid(),
            TipoEventoNotificacao.EventoAprovacao,
            CanalNotificacao.Email,
            false,
            Guid.NewGuid(),
            "teste");

        Assert.False(preferencia.Habilitada);
    }

    [Fact]
    public void DeveExigirUsuario()
    {
        Assert.Throws<ArgumentException>(() => new PreferenciaNotificacaoUsuario(
            Guid.Empty,
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            true,
            Guid.NewGuid(),
            "teste"));
    }

    [Fact]
    public void DeveExigirEventoValido()
    {
        Assert.Throws<ArgumentException>(() => new PreferenciaNotificacaoUsuario(
            Guid.NewGuid(),
            (TipoEventoNotificacao)0,
            CanalNotificacao.Email,
            true,
            Guid.NewGuid(),
            "teste"));
    }

    [Fact]
    public void DeveExigirCanalValido()
    {
        Assert.Throws<ArgumentException>(() => new PreferenciaNotificacaoUsuario(
            Guid.NewGuid(),
            TipoEventoNotificacao.EventoChamado,
            (CanalNotificacao)0,
            true,
            Guid.NewGuid(),
            "teste"));
    }

    [Fact]
    public void DeveHabilitarPreferenciaEDefinirAuditoria()
    {
        var atualizadoPorUsuarioId = Guid.NewGuid();
        var preferencia = CriarPreferencia(habilitada: false);

        preferencia.Habilitar(atualizadoPorUsuarioId, "atualizador");

        Assert.True(preferencia.Habilitada);
        Assert.Equal(atualizadoPorUsuarioId, preferencia.AtualizadoPorUsuarioId);
        Assert.Equal("atualizador", preferencia.AtualizadoPor);
        Assert.NotNull(preferencia.AtualizadoEm);
    }

    [Fact]
    public void DeveDesabilitarPreferenciaEDefinirAuditoria()
    {
        var atualizadoPorUsuarioId = Guid.NewGuid();
        var preferencia = CriarPreferencia(habilitada: true);

        preferencia.Desabilitar(atualizadoPorUsuarioId, "atualizador");

        Assert.False(preferencia.Habilitada);
        Assert.Equal(atualizadoPorUsuarioId, preferencia.AtualizadoPorUsuarioId);
        Assert.Equal("atualizador", preferencia.AtualizadoPor);
        Assert.NotNull(preferencia.AtualizadoEm);
    }

    [Fact]
    public void DeveSerIdempotenteAoHabilitar()
    {
        var preferencia = CriarPreferencia(habilitada: true);

        preferencia.Habilitar(Guid.NewGuid(), "atualizador");

        Assert.True(preferencia.Habilitada);
    }

    [Fact]
    public void DeveSerIdempotenteAoDesabilitar()
    {
        var preferencia = CriarPreferencia(habilitada: false);

        preferencia.Desabilitar(Guid.NewGuid(), "atualizador");

        Assert.False(preferencia.Habilitada);
    }

    [Fact]
    public void DevePreservarChaveLogicaImutavel()
    {
        var usuarioId = Guid.NewGuid();
        var preferencia = new PreferenciaNotificacaoUsuario(
            usuarioId,
            TipoEventoNotificacao.EventoSla,
            CanalNotificacao.Email,
            true,
            Guid.NewGuid(),
            "teste");

        preferencia.Desabilitar(Guid.NewGuid(), "atualizador");

        Assert.Equal(usuarioId, preferencia.UsuarioId);
        Assert.Equal(TipoEventoNotificacao.EventoSla, preferencia.TipoEvento);
        Assert.Equal(CanalNotificacao.Email, preferencia.Canal);
    }

    private static PreferenciaNotificacaoUsuario CriarPreferencia(bool habilitada)
        => new(
            Guid.NewGuid(),
            TipoEventoNotificacao.EventoChamado,
            CanalNotificacao.Email,
            habilitada,
            Guid.NewGuid(),
            "teste");
}
