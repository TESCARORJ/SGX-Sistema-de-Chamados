using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class EventoCandidatoNotificacaoTests
{
    [Fact]
    public void DeveCriarEventoCandidatoValido()
    {
        var ocorridoEm = new DateTime(2026, 6, 21, 11, 0, 0, DateTimeKind.Utc);
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            Guid.NewGuid(),
            Guid.NewGuid(),
            ocorridoEm,
            "corr-001",
            "idem-001",
            new Dictionary<string, string> { ["status"] = "Aberto" });

        Assert.Equal(TipoEventoNotificacao.EventoChamado, evento.TipoEvento);
        Assert.Equal(ocorridoEm, evento.OcorridoEm);
        Assert.Equal("corr-001", evento.ChaveCorrelacao);
        Assert.Equal("idem-001", evento.ChaveIdempotencia);
        Assert.Equal("Aberto", evento.Metadados["status"]);
    }

    [Fact]
    public void DeveAceitarChamadoOpcional()
    {
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoAdministrativo,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow,
            "corr-002",
            "idem-002");

        Assert.Null(evento.ChamadoId);
    }

    [Fact]
    public void DeveAceitarUsuarioOriginadorOpcional()
    {
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoAprovacao,
            Guid.NewGuid(),
            null,
            DateTime.UtcNow,
            "corr-003",
            "idem-003");

        Assert.Null(evento.UsuarioOriginadorId);
    }

    [Fact]
    public void DevePreservarTipoEventoEDadosDoEvento()
    {
        var ocorridoEm = new DateTime(2026, 6, 21, 8, 30, 0, DateTimeKind.Local);
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoSla,
            Guid.NewGuid(),
            Guid.NewGuid(),
            ocorridoEm,
            " corr-004 ",
            " idem-004 ");

        Assert.Equal(TipoEventoNotificacao.EventoSla, evento.TipoEvento);
        Assert.Equal(ocorridoEm.ToUniversalTime(), evento.OcorridoEm);
        Assert.Equal("corr-004", evento.ChaveCorrelacao);
        Assert.Equal("idem-004", evento.ChaveIdempotencia);
    }

    [Fact]
    public void DevePreservarMetadadosComoSomenteLeitura()
    {
        var origem = new Dictionary<string, string> { ["chave"] = "valor" };
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoSla,
            null,
            null,
            DateTime.UtcNow,
            "corr-005",
            "idem-005",
            origem);

        var metadadosMutaveis = Assert.IsAssignableFrom<IDictionary<string, string>>(evento.Metadados);
        Assert.Throws<NotSupportedException>(() => metadadosMutaveis.Add("nova", "entrada"));

        origem["chave"] = "alterado";
        Assert.Equal("valor", evento.Metadados["chave"]);
    }

    [Fact]
    public void DeveNormalizarMetadadosEPermitirValoresSimples()
    {
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            "corr-006",
            "idem-006",
            new Dictionary<string, string> { [" status "] = " Aberto " });

        Assert.Equal("Aberto", evento.Metadados["status"]);
    }

    [Fact]
    public void NaoDevePermitirChaveCorrelacaoVazia()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            " ",
            "idem-007"));

        Assert.Contains("correlacao", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirChaveIdempotenciaVazia()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            "corr-007",
            " "));

        Assert.Contains("idempotencia", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirTipoEventoInvalido()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EventoCandidatoNotificacao(
            (TipoEventoNotificacao)999,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            "corr-008",
            "idem-008"));

        Assert.Contains("tipo de evento", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDevePermitirMetadadoComChaveVazia()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            "corr-009",
            "idem-009",
            new Dictionary<string, string> { [" "] = "valor" }));

        Assert.Contains("metadados", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void NaoDeveExporDependenciaDeEfOuEntidadeDeDominioNoPayload()
    {
        var propertyTypes = typeof(EventoCandidatoNotificacao)
            .GetProperties()
            .Select(x => x.PropertyType)
            .ToArray();

        Assert.DoesNotContain(propertyTypes, x => typeof(Chamado).IsAssignableFrom(x));
        Assert.DoesNotContain(propertyTypes, x => typeof(Notificacao).IsAssignableFrom(x));
        Assert.DoesNotContain(propertyTypes, x => x.Name.Contains("DbContext", StringComparison.OrdinalIgnoreCase));
    }
}
