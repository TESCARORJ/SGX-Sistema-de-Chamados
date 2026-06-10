using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class EtapaAprovacaoChamadoTests
{
    [Fact]
    public void DeveCriarEtapaPendenteComOrdemNivelERamo()
    {
        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: Guid.NewGuid(),
            solicitanteId: Guid.NewGuid(),
            tipoEtapa: TipoEtapaAprovacaoChamado.Tecnica,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Paralela,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            ordem: 2,
            nivel: 1,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste",
            ramo: "infra",
            obrigatoria: true,
            criticaParaConsolidacao: true,
            aprovadorEspecificoUsuarioId: Guid.NewGuid(),
            prazoDecisaoHoras: 4,
            deveExpirarEm: DateTime.UtcNow.AddHours(4),
            escopoResumoSnapshot: "Servico critico",
            regraNomeSnapshot: "Regra tecnica",
            regraVersaoSnapshot: 1);

        Assert.Equal(StatusEtapaAprovacaoChamado.Pendente, etapa.Status);
        Assert.Equal(2, etapa.Ordem);
        Assert.Equal(1, etapa.Nivel);
        Assert.Equal("infra", etapa.Ramo);
        Assert.True(etapa.Obrigatoria);
        Assert.True(etapa.CriticaParaConsolidacao);
        Assert.Equal(TipoEtapaAprovacaoChamado.Tecnica, etapa.TipoEtapa);
    }

    [Fact]
    public void DeveExigirInstanciaValida()
    {
        var ex = Assert.Throws<ArgumentException>(() => new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: Guid.Empty,
            solicitanteId: Guid.NewGuid(),
            tipoEtapa: TipoEtapaAprovacaoChamado.Simples,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            ordem: 0,
            nivel: 1,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste"));

        Assert.Contains("instancia", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DbContextDevePersistirEtapaAprovacaoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var solicitante = new Usuario("Solicitante etapa", "solicitante.etapa@sgx.local", "solicitante.etapa", "teste");
        var configurador = new Usuario("Configurador etapa", "configurador.etapa@sgx.local", "configurador.etapa", "teste");
        context.Usuarios.AddRange(solicitante, configurador);

        var categoria = new CategoriaChamado("Categoria etapa", null, null, "teste");
        var prioridade = new PrioridadeChamado("Alta", PrioridadeChamadoEnum.Alta, null, 4, 8, "teste");
        var status = new StatusChamado("Aberto", StatusChamadoEnum.Aberto, null, false, false, "teste");
        context.CategoriasChamado.Add(categoria);
        context.PrioridadesChamado.Add(prioridade);
        context.StatusChamado.Add(status);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            codigo: "CH-ETAPA-001",
            titulo: "Chamado com etapa",
            descricao: "Descricao",
            solicitanteId: solicitante.Id,
            categoriaId: categoria.Id,
            prioridadeId: prioridade.Id,
            statusId: status.Id,
            origem: OrigemChamado.Portal,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoChamado: ImpactoChamadoEnum.Alto,
            urgenciaChamado: UrgenciaChamadoEnum.Alta);
        context.Chamados.Add(chamado);

        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: chamado.Id,
            solicitanteId: solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Sequencial,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            criadoPorUsuarioId: configurador.Id,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoAvaliado: ImpactoChamadoEnum.Alto,
            urgenciaAvaliada: UrgenciaChamadoEnum.Alta,
            aprovadorPadraoUsuarioId: configurador.Id,
            regraNomeSnapshot: "Regra sequencial",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca");
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Gerencial,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Sequencial,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            ordem: 1,
            nivel: 1,
            criadoPorUsuarioId: configurador.Id,
            criadoPor: "teste",
            obrigatoria: true,
            criticaParaConsolidacao: true,
            aprovadorPadraoUsuarioId: configurador.Id,
            escopoResumoSnapshot: "Mudanca critica",
            regraNomeSnapshot: "Regra sequencial",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Nivel 1");

        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();

        var persistida = context.EtapasAprovacaoChamado.Single();
        Assert.Equal(instancia.Id, persistida.InstanciaAprovacaoChamadoId);
        Assert.Equal(StatusEtapaAprovacaoChamado.Pendente, persistida.Status);
        Assert.Equal(1, persistida.Ordem);
        Assert.Equal(1, persistida.Nivel);
        Assert.Equal(TipoEtapaAprovacaoChamado.Gerencial, persistida.TipoEtapa);
    }
}
