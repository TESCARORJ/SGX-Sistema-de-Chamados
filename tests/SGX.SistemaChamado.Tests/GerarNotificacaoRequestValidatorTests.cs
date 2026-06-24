using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class GerarNotificacaoRequestValidatorTests
{
    private readonly GerarNotificacaoRequestValidator _validator = new();

    [Fact]
    public void DeveAceitarRequestValido()
    {
        var resultado = _validator.Validate(CriarRequest());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void DeveRejeitarSemDestinatario()
    {
        var resultado = _validator.Validate(CriarRequest(destinatarioUsuarioId: null, destinatarioEndereco: null));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.DestinatarioEndereco));
    }

    [Fact]
    public void DeveRejeitarCanalInvalido()
    {
        var resultado = _validator.Validate(CriarRequest(canal: (CanalNotificacao)999));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.Canal));
    }

    [Fact]
    public void DeveRejeitarChaveIdempotenciaVazia()
    {
        var evento = new EventoCandidatoNotificacao(
            TipoEventoNotificacao.EventoChamado,
            null,
            Guid.NewGuid(),
            new DateTime(2026, 6, 21, 10, 0, 0, DateTimeKind.Utc),
            "corr-001",
            "idem-001",
            new Dictionary<string, string>());
        var request = CriarRequest(evento: evento with { ChaveIdempotencia = string.Empty });

        var resultado = _validator.Validate(request);

        Assert.Contains(resultado.Errors, x => x.PropertyName == "Evento.ChaveIdempotencia");
    }

    [Fact]
    public void DeveRejeitarConteudoAcimaDoLimite()
    {
        var resultado = _validator.Validate(CriarRequest(conteudo: new string('c', 10001)));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.Conteudo));
    }

    [Fact]
    public void DeveRejeitarAssuntoAcimaDoLimite()
    {
        var resultado = _validator.Validate(CriarRequest(assunto: new string('a', 301)));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.Assunto));
    }

    [Fact]
    public void DeveRejeitarDestinatarioUsuarioIdVazio()
    {
        var resultado = _validator.Validate(CriarRequest(destinatarioUsuarioId: Guid.Empty));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.DestinatarioUsuarioId));
    }

    [Fact]
    public void DeveRejeitarDataAgendamentoDefault()
    {
        var resultado = _validator.Validate(CriarRequest(agendadaEm: new DateTime()));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(GerarNotificacaoRequest.AgendadaEm));
    }

    private static GerarNotificacaoRequest CriarRequest(
        EventoCandidatoNotificacao? evento = null,
        CanalNotificacao canal = CanalNotificacao.Email,
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = "destinatario@teste.local",
        string? assunto = "Assunto valido",
        string conteudo = "Conteudo valido",
        DateTime? agendadaEm = null)
    {
        return new GerarNotificacaoRequest(
            evento ?? new EventoCandidatoNotificacao(
                TipoEventoNotificacao.EventoChamado,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTime(2026, 6, 21, 10, 0, 0, DateTimeKind.Utc),
                "corr-001",
                "idem-001",
                new Dictionary<string, string>()),
            canal,
            destinatarioUsuarioId,
            destinatarioEndereco,
            assunto,
            conteudo,
            agendadaEm);
    }
}
