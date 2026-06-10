using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class DecisaoAprovacaoChamadoTests
{
    [Fact]
    public void DeveCriarDecisaoDiretaNaInstanciaComLiberacaoDeAvanco()
    {
        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: Guid.NewGuid(),
            tipoDecisao: TipoDecisaoAprovacaoChamado.Aprovacao,
            resultado: ResultadoDecisaoAprovacaoChamado.Aprovada,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.Aprovada,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste",
            decisorUsuarioId: Guid.NewGuid(),
            papelDecisorSnapshot: "Gestor de TI",
            autoridadeDecisorSnapshot: "Aprovador padrao da instancia",
            justificativa: "Aprovado para execucao.",
            escopoDecididoSnapshot: "Mudanca critica do servico X",
            decisaoFinal: true,
            liberaAvanco: true,
            regraNomeSnapshot: "Mudanca critica",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca");

        Assert.Equal(TipoDecisaoAprovacaoChamado.Aprovacao, decisao.TipoDecisao);
        Assert.Equal(ResultadoDecisaoAprovacaoChamado.Aprovada, decisao.Resultado);
        Assert.Null(decisao.EtapaAprovacaoChamadoId);
        Assert.True(decisao.DecisaoFinal);
        Assert.True(decisao.LiberaAvanco);
        Assert.False(decisao.MantemBloqueio);
        Assert.NotNull(decisao.Justificativa);
    }

    [Fact]
    public void DeveCriarDecisaoDeEtapaComEscopoParcial()
    {
        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: Guid.NewGuid(),
            etapaAprovacaoChamadoId: Guid.NewGuid(),
            tipoDecisao: TipoDecisaoAprovacaoChamado.Rejeicao,
            resultado: ResultadoDecisaoAprovacaoChamado.RequerAjuste,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.RequerReavaliacao,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.EmReavaliacao,
            statusEtapaAnterior: StatusEtapaAprovacaoChamado.Pendente,
            statusEtapaNovo: StatusEtapaAprovacaoChamado.Reprovada,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste",
            decisorUsuarioId: Guid.NewGuid(),
            decisorEhMembroGrupo: true,
            grupoAprovadorSnapshot: "CAB tecnico",
            justificativa: "Necessita ajuste de risco.",
            escopoDecididoSnapshot: "Ramo tecnico / infraestrutura",
            decisaoParcial: true,
            mantemBloqueio: true,
            exigeReavaliacao: true,
            nivelEtapaSnapshot: 1,
            ordemEtapaSnapshot: 2,
            ramoEtapaSnapshot: "infra");

        Assert.NotNull(decisao.EtapaAprovacaoChamadoId);
        Assert.True(decisao.DecisaoParcial);
        Assert.True(decisao.MantemBloqueio);
        Assert.True(decisao.ExigeReavaliacao);
        Assert.Equal(StatusEtapaAprovacaoChamado.Reprovada, decisao.StatusEtapaNovo);
    }

    [Fact]
    public void DeveExigirStatusDeEtapaQuandoDecisaoForVinculadaAEtapa()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: Guid.NewGuid(),
            etapaAprovacaoChamadoId: Guid.NewGuid(),
            tipoDecisao: TipoDecisaoAprovacaoChamado.Aprovacao,
            resultado: ResultadoDecisaoAprovacaoChamado.Aprovada,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.Aprovada,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste"));

        Assert.Contains("etapa", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DbContextDevePersistirDecisaoAprovacaoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var solicitante = new Usuario("Solicitante decisao", "solicitante.decisao@sgx.local", "solicitante.decisao", "teste");
        var aprovador = new Usuario("Aprovador decisao", "aprovador.decisao@sgx.local", "aprovador.decisao", "teste");
        context.Usuarios.AddRange(solicitante, aprovador);

        var categoria = new CategoriaChamado("Categoria decisao", null, null, "teste");
        var prioridade = new PrioridadeChamado("Alta", PrioridadeChamadoEnum.Alta, null, 4, 8, "teste");
        var statusAberto = new StatusChamado("Aberto", StatusChamadoEnum.Aberto, null, false, false, "teste");
        var statusAguardando = new StatusChamado("Aguardando aprovacao", StatusChamadoEnum.AguardandoAprovacao, null, false, true, "teste");
        context.CategoriasChamado.Add(categoria);
        context.PrioridadesChamado.Add(prioridade);
        context.StatusChamado.AddRange(statusAberto, statusAguardando);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            codigo: "CH-DECISAO-001",
            titulo: "Chamado com decisao",
            descricao: "Descricao",
            solicitanteId: solicitante.Id,
            categoriaId: categoria.Id,
            prioridadeId: prioridade.Id,
            statusId: statusAberto.Id,
            origem: OrigemChamado.Portal,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoChamado: ImpactoChamadoEnum.Alto,
            urgenciaChamado: UrgenciaChamadoEnum.Alta);
        context.Chamados.Add(chamado);
        await context.SaveChangesAsync();

        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: chamado.Id,
            solicitanteId: solicitante.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            criadoPorUsuarioId: aprovador.Id,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoAvaliado: ImpactoChamadoEnum.Alto,
            urgenciaAvaliada: UrgenciaChamadoEnum.Alta,
            aprovadorEspecificoUsuarioId: aprovador.Id,
            aprovadorResolvidoUsuarioId: aprovador.Id,
            regraNomeSnapshot: "Regra da decisao",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca");
        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var etapa = new EtapaAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            solicitanteId: solicitante.Id,
            tipoEtapa: TipoEtapaAprovacaoChamado.Simples,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico,
            ordem: 0,
            nivel: 1,
            criadoPorUsuarioId: aprovador.Id,
            criadoPor: "teste",
            aprovadorEspecificoUsuarioId: aprovador.Id,
            escopoResumoSnapshot: "Chamado inteiro",
            regraNomeSnapshot: "Regra da decisao",
            regraVersaoSnapshot: 1);
        context.EtapasAprovacaoChamado.Add(etapa);
        await context.SaveChangesAsync();

        var decisao = new DecisaoAprovacaoChamado(
            instanciaAprovacaoChamadoId: instancia.Id,
            etapaAprovacaoChamadoId: etapa.Id,
            tipoDecisao: TipoDecisaoAprovacaoChamado.Aprovacao,
            resultado: ResultadoDecisaoAprovacaoChamado.Aprovada,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            statusInstanciaAnterior: StatusInstanciaAprovacaoChamado.Pendente,
            statusInstanciaNovo: StatusInstanciaAprovacaoChamado.Aprovada,
            statusEtapaAnterior: StatusEtapaAprovacaoChamado.Pendente,
            statusEtapaNovo: StatusEtapaAprovacaoChamado.Aprovada,
            criadoPorUsuarioId: aprovador.Id,
            criadoPor: "teste",
            decisorUsuarioId: aprovador.Id,
            justificativa: "Aprovado.",
            decisaoFinal: true,
            liberaAvanco: true,
            statusChamadoAnteriorId: statusAguardando.Id,
            statusChamadoNovoId: statusAberto.Id,
            nivelEtapaSnapshot: etapa.Nivel,
            ordemEtapaSnapshot: etapa.Ordem,
            regraNomeSnapshot: etapa.RegraNomeSnapshot,
            regraVersaoSnapshot: etapa.RegraVersaoSnapshot);

        context.DecisoesAprovacaoChamado.Add(decisao);
        await context.SaveChangesAsync();

        var persistida = context.DecisoesAprovacaoChamado.Single();
        Assert.Equal(instancia.Id, persistida.InstanciaAprovacaoChamadoId);
        Assert.Equal(etapa.Id, persistida.EtapaAprovacaoChamadoId);
        Assert.Equal(ResultadoDecisaoAprovacaoChamado.Aprovada, persistida.Resultado);
        Assert.True(persistida.DecisaoFinal);
        Assert.True(persistida.LiberaAvanco);
    }
}
