using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ConfiguracaoRegraAprovacaoTests
{
    [Fact]
    public void DeveCriarRegraSimplesBloqueantePorNaturezaECatalogo()
    {
        var criadoPorUsuarioId = Guid.NewGuid();
        var tipoSolicitacaoId = Guid.NewGuid();
        var catalogoServicoId = Guid.NewGuid();

        var regra = new ConfiguracaoRegraAprovacao(
            nome: "Mudanca critica por catalogo",
            tipoRegra: TipoRegraAprovacao.Combinada,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            ordem: 10,
            prioridade: 100,
            versao: 1,
            criadoPorUsuarioId: criadoPorUsuarioId,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Mudanca,
            tipoSolicitacaoId: tipoSolicitacaoId,
            catalogoServicoId: catalogoServicoId,
            impactoMinimo: ImpactoChamadoEnum.Alto,
            urgenciaMinima: UrgenciaChamadoEnum.Alta,
            prioridadeMinima: PrioridadeChamadoEnum.Alta,
            exigeAprovacao: true,
            bloqueante: true,
            permiteFallback: true,
            aprovadorPadraoUsuarioId: Guid.NewGuid(),
            prazoDecisaoHoras: 8);

        Assert.Equal("Mudanca critica por catalogo", regra.Nome);
        Assert.Equal(TipoRegraAprovacao.Combinada, regra.TipoRegra);
        Assert.Equal(EscopoRegraAprovacao.AtendimentoChamado, regra.EscopoRegra);
        Assert.Equal(NaturezaChamadoEnum.Mudanca, regra.NaturezaChamado);
        Assert.Equal(tipoSolicitacaoId, regra.TipoSolicitacaoId);
        Assert.Equal(catalogoServicoId, regra.CatalogoServicoId);
        Assert.True(regra.ExigeAprovacao);
        Assert.True(regra.Bloqueante);
        Assert.True(regra.Ativo);
        Assert.True(regra.EstaVigenteEm(DateTime.UtcNow));
    }

    [Fact]
    public void NaoDevePermitirRegraSinalizadaComoBloqueanteSemAprovacao()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => new ConfiguracaoRegraAprovacao(
            nome: "Sinalizacao inconsistente",
            tipoRegra: TipoRegraAprovacao.ImpactoUrgencia,
            escopoRegra: EscopoRegraAprovacao.AberturaChamado,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.Sinalizar,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            ordem: 1,
            prioridade: 1,
            versao: 1,
            criadoPorUsuarioId: Guid.NewGuid(),
            criadoPor: "teste",
            exigeAprovacao: false,
            bloqueante: true));

        Assert.Contains("sinalizar", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DbContextDevePersistirConfiguracaoRegraAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var usuario = new Usuario("Configurador", "configurador@sgx.local", "configurador", "teste");
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var regra = new ConfiguracaoRegraAprovacao(
            nome: "Regra por natureza",
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            escopoRegra: EscopoRegraAprovacao.EscopoGeralChamado,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.ResolucaoDinamicaFutura,
            ordem: 5,
            prioridade: 50,
            versao: 1,
            criadoPorUsuarioId: usuario.Id,
            criadoPor: "teste",
            naturezaChamado: NaturezaChamadoEnum.Problema,
            impactoMinimo: ImpactoChamadoEnum.Medio,
            urgenciaMinima: UrgenciaChamadoEnum.Media,
            exigeAprovacao: true,
            bloqueante: false,
            permiteReenvio: true,
            vigenteDe: DateTime.UtcNow.Date);

        context.ConfiguracoesRegrasAprovacao.Add(regra);
        await context.SaveChangesAsync();

        var persistida = context.ConfiguracoesRegrasAprovacao.Single();
        Assert.Equal("Regra por natureza", persistida.Nome);
        Assert.Equal(NaturezaChamadoEnum.Problema, persistida.NaturezaChamado);
        Assert.True(persistida.ExigeAprovacao);
        Assert.False(persistida.Bloqueante);
    }

    [Fact]
    public void DeveCriarRegraSinalizandoGrupoAprovadorFuturo()
    {
        var criadoPorUsuarioId = Guid.NewGuid();

        var regra = new ConfiguracaoRegraAprovacao(
            nome: "Regra Grupo Aprovador Futuro",
            tipoRegra: TipoRegraAprovacao.NaturezaItsm,
            escopoRegra: EscopoRegraAprovacao.AtendimentoChamado,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            tipoFluxoAprovacao: TipoFluxoAprovacao.Simples,
            tipoResolucaoAprovador: TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo,
            ordem: 1,
            prioridade: 1,
            versao: 1,
            criadoPorUsuarioId: criadoPorUsuarioId,
            criadoPor: "teste",
            exigeAprovacao: true,
            bloqueante: true);

        Assert.Equal(TipoResolucaoAprovadorRegraAprovacao.GrupoAprovadorFuturo, regra.TipoResolucaoAprovador);
    }
}
