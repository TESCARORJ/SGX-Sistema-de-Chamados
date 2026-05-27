using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class CamposObrigatoriosChamadoServiceTests
{
    private readonly CamposObrigatoriosChamadoService _service = new();

    [Theory]
    [InlineData(NaturezaChamadoEnum.Incidente)]
    [InlineData(NaturezaChamadoEnum.Requisicao)]
    [InlineData(NaturezaChamadoEnum.Mudanca)]
    [InlineData(NaturezaChamadoEnum.Problema)]
    [InlineData(NaturezaChamadoEnum.EventoAlerta)]
    [InlineData(NaturezaChamadoEnum.TarefaOperacional)]
    public void DeveValidarCamposMinimosPorNatureza(NaturezaChamadoEnum natureza)
    {
        var erros = _service.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = natureza,
            Titulo = "Titulo valido",
            Descricao = natureza is NaturezaChamadoEnum.Mudanca or NaturezaChamadoEnum.Problema
                ? "Descricao com detalhamento suficiente para validacao desta natureza."
                : "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            TipoSolicitacaoId = Guid.NewGuid(),
            ImpactoChamado = ImpactoChamadoEnum.Medio,
            UrgenciaChamado = UrgenciaChamadoEnum.Media,
            Origem = "Portal"
        });

        Assert.Empty(erros);
    }

    [Fact]
    public void DeveBloquearCriacaoSemNatureza()
    {
        var erros = _service.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            Titulo = "Titulo",
            Descricao = "Descricao",
            CategoriaId = Guid.NewGuid()
        });

        Assert.Contains(erros, x => x.Campo == "NaturezaChamado");
    }

    [Fact]
    public void DeveBloquearCriacaoSemTituloEDescricao()
    {
        var erros = _service.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            CategoriaId = Guid.NewGuid()
        });

        Assert.Contains(erros, x => x.Campo == "Titulo");
        Assert.Contains(erros, x => x.Campo == "Descricao");
    }

    [Fact]
    public void DeveBloquearIncidenteSemImpactoEUrgencia()
    {
        var erros = _service.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = NaturezaChamadoEnum.Incidente,
            Titulo = "Incidente",
            Descricao = "Descricao do incidente",
            CategoriaId = Guid.NewGuid(),
            Origem = "Portal"
        });

        Assert.Contains(erros, x => x.Campo == "ImpactoChamado");
        Assert.Contains(erros, x => x.Campo == "UrgenciaChamado");
    }

    [Fact]
    public void DevePermitirFallbackEmailSemImpactoEUrgencia()
    {
        var erros = _service.ValidarCriacao(new CamposObrigatoriosChamadoInput
        {
            NaturezaChamado = NaturezaChamadoEnum.EventoAlerta,
            Titulo = "Alerta",
            Descricao = "Alerta recebido por integracao de e-mail com conteudo valido.",
            CategoriaId = Guid.NewGuid(),
            Origem = "Email"
        });

        Assert.DoesNotContain(erros, x => x.Campo == "ImpactoChamado");
        Assert.DoesNotContain(erros, x => x.Campo == "UrgenciaChamado");
    }
}
