using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.UseCases.Admin;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Tests;

public sealed class ServicoAplicacaoRegrasAprovacaoTests
{
    [Fact]
    public async Task CriarRegraValidaDevePersistirConfiguracao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);

        var response = await useCases.CriarAsync(CriarRequestBase());

        var persistida = await context.ConfiguracoesRegrasAprovacao.SingleAsync();
        Assert.Equal(response.Id, persistida.Id);
        Assert.Equal("Mudanca critica", persistida.Nome);
        Assert.True(persistida.ExigeAprovacao);
        Assert.True(persistida.Bloqueante);
        Assert.Equal(TipoFluxoAprovacao.Sequencial, persistida.TipoFluxoAprovacao);
    }

    [Fact]
    public async Task CriarRegraBloqueanteSemExigirAprovacaoDeveFalhar()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);
        var request = CriarRequestBase(
            exigeAprovacao: false,
            efeitoOperacional: EfeitoOperacionalRegraAprovacao.Sinalizar);

        var ex = await Assert.ThrowsAsync<ValidationException>(() => useCases.CriarAsync(request));

        Assert.Contains(ex.Errors, x => x.ErrorMessage.Contains("bloqueante", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task AtualizarRegraDevePersistirNovosCriteriosSemGerarEfeitosOperacionais()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);

        var criada = await useCases.CriarAsync(CriarRequestBase());

        var atualizada = await useCases.AtualizarAsync(criada.Id, new AtualizarConfiguracaoRegraAprovacaoRequest
        {
            Nome = "Mudanca critica revisada",
            Descricao = "Atualizada sem gerar aprovacao.",
            TipoRegra = TipoRegraAprovacao.Combinada,
            EscopoRegra = EscopoRegraAprovacao.AtendimentoChamado,
            Ordem = 2,
            Prioridade = 120,
            Versao = 2,
            NaturezaChamado = NaturezaChamadoEnum.Mudanca,
            ImpactoMinimo = ImpactoChamadoEnum.Alto,
            UrgenciaMinima = UrgenciaChamadoEnum.Alta,
            PrioridadeMinima = PrioridadeChamadoEnum.Critica,
            ExigeAprovacao = true,
            Bloqueante = false,
            PermiteReenvio = true,
            PermiteFallback = true,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Paralela,
            TipoResolucaoAprovador = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            PrazoDecisaoHoras = 6,
            VigenteDe = DateTime.UtcNow.Date,
            VigenteAte = DateTime.UtcNow.Date.AddDays(15),
            Ativo = false
        });

        var persistida = await context.ConfiguracoesRegrasAprovacao.SingleAsync();
        Assert.Equal("Mudanca critica revisada", atualizada.Nome);
        Assert.Equal(2, persistida.Versao);
        Assert.Equal(TipoFluxoAprovacao.Paralela, persistida.TipoFluxoAprovacao);
        Assert.False(persistida.Ativo);
        Assert.Empty(context.InstanciasAprovacaoChamado);
        Assert.Empty(context.AprovacoesChamado);
        Assert.Empty(context.EtapasAprovacaoChamado);
        Assert.Empty(context.DecisoesAprovacaoChamado);
    }

    [Fact]
    public async Task ListarRegrasDeveFiltrarPorAtivo()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);

        await useCases.CriarAsync(CriarRequestBase(nome: "Regra ativa"));
        await useCases.CriarAsync(CriarRequestBase(nome: "Regra inativa", versao: 2, ativo: false));

        var response = await useCases.ListarAsync(new ListarConfiguracoesRegrasAprovacaoRequest
        {
            Ativo = true,
            Pagina = 1,
            TamanhoPagina = 20
        });

        Assert.Single(response.Items);
        Assert.Equal("Regra ativa", response.Items.Single().Nome);
    }

    [Fact]
    public async Task AvaliacaoPuraDeveSelecionarRegraAtivaEVigenteSemCriarInstanciaOuAprovacao()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);

        await useCases.CriarAsync(CriarRequestBase(
            nome: "Regra candidata",
            prioridade: 150,
            vigenteDe: DateTime.UtcNow.Date.AddDays(-1),
            vigenteAte: DateTime.UtcNow.Date.AddDays(10)));

        await useCases.CriarAsync(CriarRequestBase(nome: "Regra inativa", versao: 2, ativo: false));

        var instanciasAntes = await context.InstanciasAprovacaoChamado.CountAsync();
        var aprovacoesAntes = await context.AprovacoesChamado.CountAsync();
        var etapasAntes = await context.EtapasAprovacaoChamado.CountAsync();
        var decisoesAntes = await context.DecisoesAprovacaoChamado.CountAsync();

        var avaliacao = await useCases.AvaliarRegraAsync(new ContextoAvaliacaoRegraAprovacaoRequest
        {
            NaturezaChamado = NaturezaChamadoEnum.Mudanca,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta,
            PrioridadeChamado = PrioridadeChamadoEnum.Critica,
            DataReferencia = DateTime.UtcNow
        });

        Assert.True(avaliacao.RegraAplicavel);
        Assert.NotNull(avaliacao.MelhorRegra);
        Assert.Equal("Regra candidata", avaliacao.MelhorRegra!.NomeRegra);
        Assert.True(avaliacao.ExigeAprovacao);
        Assert.Contains(avaliacao.Avisos, x => x.Contains("nao gera aprovacao", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(instanciasAntes, await context.InstanciasAprovacaoChamado.CountAsync());
        Assert.Equal(aprovacoesAntes, await context.AprovacoesChamado.CountAsync());
        Assert.Equal(etapasAntes, await context.EtapasAprovacaoChamado.CountAsync());
        Assert.Equal(decisoesAntes, await context.DecisoesAprovacaoChamado.CountAsync());
    }

    [Fact]
    public async Task RegraInativaNaoDeveSerCandidata()
    {
        using var context = PortalUseCasesTestFactory.CriarContexto();
        var useCases = CriarUseCases(context);

        await useCases.CriarAsync(CriarRequestBase(nome: "Regra desligada", ativo: false));

        var candidatas = await useCases.ListarRegrasCandidatasAsync(new ContextoAvaliacaoRegraAprovacaoRequest
        {
            NaturezaChamado = NaturezaChamadoEnum.Mudanca,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta,
            PrioridadeChamado = PrioridadeChamadoEnum.Critica,
            DataReferencia = DateTime.UtcNow
        });

        Assert.Empty(candidatas);
    }

    private static ConfiguracaoRegraAprovacaoAdminUseCases CriarUseCases(SGXSistemaChamadoDbContext context)
        => new(
            PortalUseCasesTestFactory.Repo<ConfiguracaoRegraAprovacao>(context),
            PortalUseCasesTestFactory.Repo<TipoSolicitacao>(context),
            PortalUseCasesTestFactory.Repo<CatalogoServico>(context),
            PortalUseCasesTestFactory.Repo<CategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<SubcategoriaChamado>(context),
            PortalUseCasesTestFactory.Repo<Usuario>(context),
            new FakeUsuarioContextoAplicacaoService(new UsuarioContextoAplicacao(
                Guid.NewGuid(),
                "Administrador de Regras",
                "admin.regras@sgx.local",
                "admin.regras",
                ["Administrador"])),
            PortalUseCasesTestFactory.Uow(context));

    private static CriarConfiguracaoRegraAprovacaoRequest CriarRequestBase(
        string nome = "Mudanca critica",
        int prioridade = 100,
        int versao = 1,
        bool exigeAprovacao = true,
        bool bloqueante = true,
        EfeitoOperacionalRegraAprovacao efeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
        DateTime? vigenteDe = null,
        DateTime? vigenteAte = null,
        bool ativo = true)
        => new()
        {
            Nome = nome,
            Descricao = "Regra administrativa para avaliacao conceitual.",
            TipoRegra = TipoRegraAprovacao.Combinada,
            EscopoRegra = EscopoRegraAprovacao.AtendimentoChamado,
            Ordem = 1,
            Prioridade = prioridade,
            Versao = versao,
            NaturezaChamado = NaturezaChamadoEnum.Mudanca,
            ImpactoMinimo = ImpactoChamadoEnum.Alto,
            UrgenciaMinima = UrgenciaChamadoEnum.Alta,
            PrioridadeMinima = PrioridadeChamadoEnum.Alta,
            ExigeAprovacao = exigeAprovacao,
            Bloqueante = bloqueante,
            PermiteReenvio = true,
            PermiteFallback = false,
            EfeitoOperacional = efeitoOperacional,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Sequencial,
            TipoResolucaoAprovador = TipoResolucaoAprovadorRegraAprovacao.NaoDefinido,
            PrazoDecisaoHoras = 4,
            VigenteDe = vigenteDe ?? DateTime.UtcNow.Date.AddDays(-2),
            VigenteAte = vigenteAte ?? DateTime.UtcNow.Date.AddDays(20),
            Ativo = ativo
        };
}
