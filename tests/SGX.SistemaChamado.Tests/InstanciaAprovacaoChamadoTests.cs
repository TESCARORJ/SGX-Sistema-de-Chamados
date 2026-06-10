using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class InstanciaAprovacaoChamadoTests
{
    [Fact]
    public void DeveCriarInstanciaPendenteComSnapshotDaRegra()
    {
        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: Guid.NewGuid(),
            solicitanteId: Guid.NewGuid(),
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.Combinada,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste",
            configuracaoRegraAprovacaoId: Guid.NewGuid(),
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            catalogoServicoId: Guid.NewGuid(),
            impactoAvaliado: ImpactoChamadoEnum.Alto,
            urgenciaAvaliada: UrgenciaChamadoEnum.Alta,
            prioridadeAvaliada: PrioridadeChamadoEnum.Alta,
            permiteFallback: true,
            aprovadorPadraoUsuarioId: Guid.NewGuid(),
            prazoDecisaoHoras: 6,
            deveExpirarEm: DateTime.UtcNow.AddHours(6),
            regraNomeSnapshot: "Mudanca critica",
            regraVersaoSnapshot: 1,
            regraCriterioSnapshot: "Natureza=Mudanca; Catalogo=Servico Critico");

        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, instancia.Status);
        Assert.Equal(OrigemInstanciaAprovacaoChamado.RegraMotor, instancia.Origem);
        Assert.Equal(TipoFluxoAprovacao.Simples, instancia.TipoFluxoAprovacao);
        Assert.True(instancia.ExigeAprovacao);
        Assert.True(instancia.Bloqueante);
        Assert.Equal("Mudanca critica", instancia.RegraNomeSnapshot);
        Assert.Equal(1, instancia.RegraVersaoSnapshot);
        Assert.NotNull(instancia.RegraCriterioSnapshot);
    }

    [Fact]
    public void DeveExigirChamadoValido()
    {
        var ex = Assert.Throws<ArgumentException>(() => new InstanciaAprovacaoChamado(
            chamadoId: Guid.Empty,
            solicitanteId: Guid.NewGuid(),
            origem: OrigemInstanciaAprovacaoChamado.Manual,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            escopoRegra: EscopoRegraAprovacao.EscopoGeralChamado,
            tipoRegra: TipoRegraAprovacao.Geral,
            exigeAprovacao: true,
            bloqueante: false,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste"));

        Assert.Contains("chamado", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DbContextDevePersistirInstanciaAprovacaoChamado()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();

        var usuario = new Usuario("Solicitante", "solicitante.instancia@sgx.local", "solicitante.instancia", "teste");
        var configurador = new Usuario("Configurador Instancia", "config.instancia@sgx.local", "config.instancia", "teste");
        context.Usuarios.AddRange(usuario, configurador);

        var categoria = new CategoriaChamado("Categoria instancia", null, null, "teste");
        var prioridade = new PrioridadeChamado("Alta", PrioridadeChamadoEnum.Alta, null, 4, 8, "teste");
        var status = new StatusChamado("Aberto", StatusChamadoEnum.Aberto, null, false, false, "teste");
        context.CategoriasChamado.Add(categoria);
        context.PrioridadesChamado.Add(prioridade);
        context.StatusChamado.Add(status);
        await context.SaveChangesAsync();

        var chamado = new Chamado(
            codigo: "CH-TESTE-001",
            titulo: "Chamado com instancia",
            descricao: "Descricao",
            solicitanteId: usuario.Id,
            categoriaId: categoria.Id,
            prioridadeId: prioridade.Id,
            statusId: status.Id,
            origem: OrigemChamado.Portal,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoChamado: ImpactoChamadoEnum.Alto,
            urgenciaChamado: UrgenciaChamadoEnum.Alta);
        context.Chamados.Add(chamado);

        var configuracao = new ConfiguracaoRegraAprovacao(
            nome: "Regra de instancia",
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            ordem: 1,
            prioridade: 10,
            versao: 1,
            criadoPorUsuarioId: configurador.Id,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            exigeAprovacao: true,
            bloqueante: true,
            aprovadorPadraoUsuarioId: configurador.Id);
        context.ConfiguracoesRegrasAprovacao.Add(configuracao);
        await context.SaveChangesAsync();

        var instancia = new InstanciaAprovacaoChamado(
            chamadoId: chamado.Id,
            solicitanteId: usuario.Id,
            origem: OrigemInstanciaAprovacaoChamado.RegraMotor,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            exigeAprovacao: true,
            bloqueante: true,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            criadoPorUsuarioId: configurador.Id,
            criadoPor: "teste",
            configuracaoRegraAprovacaoId: configuracao.Id,
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            impactoAvaliado: ImpactoChamadoEnum.Alto,
            urgenciaAvaliada: UrgenciaChamadoEnum.Alta,
            aprovadorPadraoUsuarioId: configurador.Id,
            regraNomeSnapshot: configuracao.Nome,
            regraVersaoSnapshot: configuracao.Versao,
            regraCriterioSnapshot: "Natureza=Mudanca");

        context.InstanciasAprovacaoChamado.Add(instancia);
        await context.SaveChangesAsync();

        var persistida = context.InstanciasAprovacaoChamado.Single();
        Assert.Equal(chamado.Id, persistida.ChamadoId);
        Assert.Equal(configuracao.Id, persistida.ConfiguracaoRegraAprovacaoId);
        Assert.Equal(StatusInstanciaAprovacaoChamado.Pendente, persistida.Status);
        Assert.True(persistida.Bloqueante);
        Assert.Equal("Regra de instancia", persistida.RegraNomeSnapshot);
    }
}
