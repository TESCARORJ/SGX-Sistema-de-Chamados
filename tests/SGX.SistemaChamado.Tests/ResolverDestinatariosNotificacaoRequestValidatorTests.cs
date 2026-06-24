using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ResolverDestinatariosNotificacaoRequestValidatorTests
{
    private readonly ResolverDestinatariosNotificacaoRequestValidator _validator = new();

    [Fact]
    public void DeveAceitarRequestValido()
    {
        var resultado = _validator.Validate(CriarRequest());

        Assert.True(resultado.IsValid);
    }

    [Fact]
    public void DeveRejeitarQuandoNaoHouverParticipacoes()
    {
        var resultado = _validator.Validate(CriarRequest(participacoes: []));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.Participacoes));
    }

    [Fact]
    public void DeveRejeitarParticipacoesDuplicadas()
    {
        var resultado = _validator.Validate(CriarRequest(participacoes:
        [
            TipoParticipacaoDestinatarioNotificacao.Solicitante,
            TipoParticipacaoDestinatarioNotificacao.Solicitante
        ]));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.Participacoes));
    }

    [Fact]
    public void DeveExigirChamadoQuandoParticipacaoDependerDoChamado()
    {
        var request = CriarRequest(
            evento: CriarEvento(chamadoId: null, manterChamadoNulo: true),
            participacoes: [TipoParticipacaoDestinatarioNotificacao.Solicitante]);

        var resultado = _validator.Validate(request);

        Assert.Contains(resultado.Errors, x => x.PropertyName == "Evento.ChamadoId");
    }

    [Fact]
    public void DeveExigirAprovacaoLegadaQuandoSolicitada()
    {
        var resultado = _validator.Validate(CriarRequest(
            participacoes: [TipoParticipacaoDestinatarioNotificacao.AprovadorLegado],
            aprovacaoChamadoId: null));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.AprovacaoChamadoId));
    }

    [Fact]
    public void DeveExigirInstanciaQuandoSolicitada()
    {
        var resultado = _validator.Validate(CriarRequest(
            participacoes: [TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia],
            instanciaAprovacaoChamadoId: null));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.InstanciaAprovacaoChamadoId));
    }

    [Fact]
    public void DeveExigirGrupoOuChamadoQuandoGrupoTecnicoForSolicitado()
    {
        var resultado = _validator.Validate(CriarRequest(
            evento: CriarEvento(chamadoId: null, manterChamadoNulo: true),
            participacoes: [TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico],
            grupoTecnicoId: null));

        Assert.Contains(resultado.Errors, x => x.ErrorMessage.Contains("grupo tecnico", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DeveExigirPerfilQuandoParticipacaoPorPerfilForSolicitada()
    {
        var resultado = _validator.Validate(CriarRequest(
            participacoes: [TipoParticipacaoDestinatarioNotificacao.PerfilAcesso],
            perfilAcessoId: null));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.PerfilAcessoId));
    }

    [Fact]
    public void DeveRejeitarGuidVazioQuandoInformado()
    {
        var resultado = _validator.Validate(CriarRequest(
            aprovacaoChamadoId: Guid.Empty,
            instanciaAprovacaoChamadoId: Guid.Empty,
            grupoTecnicoId: Guid.Empty,
            perfilAcessoId: Guid.Empty));

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.AprovacaoChamadoId));
        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.InstanciaAprovacaoChamadoId));
        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.GrupoTecnicoId));
        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ResolverDestinatariosNotificacaoRequest.PerfilAcessoId));
    }

    private static ResolverDestinatariosNotificacaoRequest CriarRequest(
        EventoCandidatoNotificacao? evento = null,
        IReadOnlyCollection<TipoParticipacaoDestinatarioNotificacao>? participacoes = null,
        Guid? aprovacaoChamadoId = null,
        Guid? instanciaAprovacaoChamadoId = null,
        Guid? grupoTecnicoId = null,
        Guid? perfilAcessoId = null,
        bool excluirUsuarioOriginador = false)
        => new(
            evento ?? CriarEvento(),
            participacoes ?? [TipoParticipacaoDestinatarioNotificacao.Solicitante],
            aprovacaoChamadoId,
            instanciaAprovacaoChamadoId,
            grupoTecnicoId,
            perfilAcessoId,
            excluirUsuarioOriginador);

    private static EventoCandidatoNotificacao CriarEvento(Guid? chamadoId = null, bool manterChamadoNulo = false)
        => new(
            TipoEventoNotificacao.EventoChamado,
            manterChamadoNulo ? chamadoId : chamadoId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2026, 6, 21, 12, 0, 0, DateTimeKind.Utc),
            "corr-dest-001",
            "idem-dest-001",
            new Dictionary<string, string>());
}
