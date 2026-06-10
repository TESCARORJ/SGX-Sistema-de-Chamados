using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class ConfiguracaoRegraAprovacaoContractsTests
{
    [Fact]
    public void CriarConfiguracaoDeveExigirAprovadorEspecificoQuandoResolucaoForEspecifica()
    {
        var validator = new CriarConfiguracaoRegraAprovacaoRequestValidator();

        var resultado = validator.Validate(new CriarConfiguracaoRegraAprovacaoRequest
        {
            Nome = "Mudanca critica",
            TipoRegra = TipoRegraAprovacao.NaturezaItsm,
            EscopoRegra = EscopoRegraAprovacao.AtendimentoChamado,
            Ordem = 1,
            Prioridade = 10,
            Versao = 1,
            ExigeAprovacao = true,
            Bloqueante = false,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Simples,
            TipoResolucaoAprovador = TipoResolucaoAprovadorRegraAprovacao.AprovadorEspecifico
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == string.Empty && x.ErrorMessage.Contains("AprovadorEspecificoUsuarioId", StringComparison.Ordinal));
    }

    [Fact]
    public void CriarConfiguracaoNaoDeveAceitarSubcategoriaSemCategoria()
    {
        var validator = new CriarConfiguracaoRegraAprovacaoRequestValidator();

        var resultado = validator.Validate(new CriarConfiguracaoRegraAprovacaoRequest
        {
            Nome = "Regra por subcategoria",
            TipoRegra = TipoRegraAprovacao.CategoriaSubcategoria,
            EscopoRegra = EscopoRegraAprovacao.AberturaChamado,
            Ordem = 2,
            Prioridade = 20,
            Versao = 1,
            SubcategoriaId = Guid.NewGuid(),
            ExigeAprovacao = true,
            Bloqueante = false,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacao,
            TipoFluxoAprovacao = TipoFluxoAprovacao.Simples,
            TipoResolucaoAprovador = TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            AprovadorPadraoUsuarioId = Guid.NewGuid()
        });

        Assert.Contains(resultado.Errors, x => x.ErrorMessage.Contains("CategoriaId", StringComparison.Ordinal));
    }

    [Fact]
    public void FiltroDeveAceitarPaginacaoEOrdenacaoEsperadas()
    {
        var validator = new ListarConfiguracoesRegrasAprovacaoRequestValidator();

        var resultado = validator.Validate(new ListarConfiguracoesRegrasAprovacaoRequest
        {
            Termo = "mudanca",
            TipoRegra = TipoRegraAprovacao.NaturezaItsm,
            EfeitoOperacional = EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            Pagina = 1,
            TamanhoPagina = 20,
            OrdenarPor = "prioridade",
            DirecaoOrdenacao = "desc"
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void ResponseDevePermitirRepresentarCriteriosEClassificacoesDaRegra()
    {
        var response = new ConfiguracaoRegraAprovacaoResponse(
            Guid.NewGuid(),
            "Mudanca critica",
            "Exige aprovacao tecnica",
            TipoRegraAprovacao.Combinada,
            "Combinada",
            EscopoRegraAprovacao.AtendimentoChamado,
            "AtendimentoChamado",
            1,
            100,
            2,
            NaturezaChamadoEnum.Mudanca,
            Guid.NewGuid(),
            "Mudanca padrao",
            Guid.NewGuid(),
            "Firewall",
            Guid.NewGuid(),
            "Infraestrutura",
            Guid.NewGuid(),
            "Firewall perimetral",
            ImpactoChamadoEnum.Alto,
            UrgenciaChamadoEnum.Alta,
            PrioridadeChamadoEnum.Alta,
            5000m,
            4,
            true,
            true,
            true,
            false,
            EfeitoOperacionalRegraAprovacao.ExigirAprovacaoEBloquearAvanco,
            "ExigirAprovacaoEBloquearAvanco",
            TipoFluxoAprovacao.Sequencial,
            "Sequencial",
            TipoResolucaoAprovadorRegraAprovacao.AprovadorPadrao,
            "AprovadorPadrao",
            null,
            null,
            Guid.NewGuid(),
            "Gestor TI",
            8,
            DateTime.UtcNow.Date,
            DateTime.UtcNow.Date.AddDays(30),
            true,
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow,
            DateTime.UtcNow);

        Assert.Equal("Mudanca critica", response.Nome);
        Assert.Equal(TipoFluxoAprovacao.Sequencial, response.TipoFluxoAprovacao);
        Assert.True(response.Bloqueante);
        Assert.Equal(5000m, response.CustoMinimo);
    }
}
