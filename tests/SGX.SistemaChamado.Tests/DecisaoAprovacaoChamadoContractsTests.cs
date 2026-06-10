using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DecisaoAprovacaoChamadoContractsTests
{
    [Fact]
    public void RegistrarDecisaoNaoDeveAceitarResultadoIncompativelComTipo()
    {
        var validator = new RegistrarDecisaoAprovacaoChamadoRequestValidator();

        var resultado = validator.Validate(new RegistrarDecisaoAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = Guid.NewGuid(),
            TipoDecisao = TipoDecisaoAprovacaoChamado.Aprovacao,
            Resultado = ResultadoDecisaoAprovacaoChamado.Reprovada,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            StatusInstanciaAnterior = StatusInstanciaAprovacaoChamado.Pendente,
            StatusInstanciaNovo = StatusInstanciaAprovacaoChamado.Aprovada
        });

        Assert.Contains(resultado.Errors, x => x.ErrorMessage.Contains("TipoDecisao", StringComparison.Ordinal));
    }

    [Fact]
    public void RegistrarDecisaoNaoDevePermitirLiberacaoEBloqueioAoMesmoTempo()
    {
        var validator = new RegistrarDecisaoAprovacaoChamadoRequestValidator();

        var resultado = validator.Validate(new RegistrarDecisaoAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = Guid.NewGuid(),
            TipoDecisao = TipoDecisaoAprovacaoChamado.Aprovacao,
            Resultado = ResultadoDecisaoAprovacaoChamado.Aprovada,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            StatusInstanciaAnterior = StatusInstanciaAprovacaoChamado.Pendente,
            StatusInstanciaNovo = StatusInstanciaAprovacaoChamado.Aprovada,
            LiberaAvanco = true,
            MantemBloqueio = true
        });

        Assert.Contains(resultado.Errors, x => x.ErrorMessage.Contains("LiberaAvanco", StringComparison.Ordinal));
    }

    [Fact]
    public void ReprovarDeveExigirJustificativa()
    {
        var validator = new ReprovarAprovacaoChamadoRequestValidator();

        var resultado = validator.Validate(new ReprovarAprovacaoChamadoRequest
        {
            InstanciaAprovacaoChamadoId = Guid.NewGuid()
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(ReprovarAprovacaoChamadoRequest.Justificativa));
    }

    [Fact]
    public void ResponseDeveRepresentarDecisaoPorEtapaEContextoDeFluxo()
    {
        var response = new DecisaoAprovacaoChamadoResponse(
            Id: Guid.NewGuid(),
            InstanciaAprovacaoChamadoId: Guid.NewGuid(),
            EtapaAprovacaoChamadoId: Guid.NewGuid(),
            ChamadoId: Guid.NewGuid(),
            TipoDecisao: TipoDecisaoAprovacaoChamado.Rejeicao,
            TipoDecisaoDescricao: "Rejeicao",
            Resultado: ResultadoDecisaoAprovacaoChamado.RequerAjuste,
            ResultadoDescricao: "RequerAjuste",
            DataDecisao: DateTime.UtcNow,
            DecisorUsuarioId: Guid.NewGuid(),
            DecisorNome: "Gestor TI",
            PapelDecisorSnapshot: "Gestor",
            AutoridadeDecisorSnapshot: "Aprovador padrao",
            DecisorEhAprovadorEspecifico: false,
            DecisorEhAprovadorPadrao: true,
            DecisorEhMembroGrupo: false,
            DecisorPorDelegacao: false,
            GrupoAprovadorSnapshot: null,
            QuorumEsperado: null,
            QuorumAtingido: null,
            Justificativa: "Necessita ajuste",
            Observacao: "Detalhar risco",
            EscopoDecididoSnapshot: "Escopo tecnico",
            EfeitoOperacional: EfeitoOperacionalRegraAprovacao.RequerReavaliacao,
            EfeitoOperacionalDescricao: "RequerReavaliacao",
            DecisaoParcial: true,
            DecisaoFinal: false,
            LiberaAvanco: false,
            MantemBloqueio: true,
            ExigeReavaliacao: true,
            PermiteNovaSolicitacao: true,
            CancelaFluxo: false,
            StatusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            StatusInstanciaAnteriorDescricao: "Pendente",
            StatusInstanciaNovo: StatusInstanciaAprovacaoChamado.EmReavaliacao,
            StatusInstanciaNovoDescricao: "EmReavaliacao",
            StatusEtapaAnterior: StatusEtapaAprovacaoChamado.Pendente,
            StatusEtapaAnteriorDescricao: "Pendente",
            StatusEtapaNovo: StatusEtapaAprovacaoChamado.Reprovada,
            StatusEtapaNovoDescricao: "Reprovada",
            StatusChamadoAnteriorId: Guid.NewGuid(),
            StatusChamadoAnteriorNome: "Aguardando aprovacao",
            StatusChamadoNovoId: Guid.NewGuid(),
            StatusChamadoNovoNome: "Em analise",
            NivelSnapshot: 2,
            OrdemSnapshot: 1,
            RamoSnapshot: "infra",
            RegraNomeSnapshot: "Mudanca critica",
            RegraVersaoSnapshot: 3,
            RegraCriterioSnapshot: "Impacto alto",
            CriadoPorUsuarioId: Guid.NewGuid(),
            AtualizadoPorUsuarioId: Guid.NewGuid(),
            CriadoEm: DateTime.UtcNow,
            AtualizadoEm: DateTime.UtcNow);

        Assert.Equal(TipoDecisaoAprovacaoChamado.Rejeicao, response.TipoDecisao);
        Assert.True(response.DecisaoParcial);
        Assert.True(response.ExigeReavaliacao);
        Assert.Equal("infra", response.RamoSnapshot);
    }
}
