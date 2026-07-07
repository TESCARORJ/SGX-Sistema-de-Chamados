using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Services;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;
using FluentValidation.TestHelper;

namespace SGX.SistemaChamado.Tests;

public sealed class CriarChamadoRequestValidatorTests
{
    private readonly CriarChamadoRequestValidator _validator = new(new CamposObrigatoriosChamadoService());

    [Fact]
    public void DeveRejeitarTituloObrigatorio()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = string.Empty,
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.Titulo));
    }

    [Fact]
    public void DeveRejeitarDescricaoObrigatoria()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = string.Empty,
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.Descricao));
    }

    [Fact]
    public void DeveRejeitarCategoriaObrigatoria()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CategoriaId = Guid.Empty,
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.CategoriaId));
    }

    [Fact]
    public void DeveRejeitarPrioridadeObrigatoria()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.Empty,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.PrioridadeId));
    }

    [Fact]
    public void DeveRejeitarSubcategoriaInvalidaQuandoGuidVazio()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa,
            SubcategoriaId = Guid.Empty
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.SubcategoriaId));
    }

    [Fact]
    public void DevePermitirSemCategoriaEPrioridadeQuandoCatalogoServicoInformado()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CatalogoServicoId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.CategoriaId));
        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.PrioridadeId));
    }

    [Fact]
    public void ContratoPublicoLegadoNaoDeveExporGrupoSlaOuCamposDeAprovacao()
    {
        var propriedades = typeof(CriarChamadoRequest)
            .GetProperties()
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("GrupoTecnicoId", propriedades);
        Assert.DoesNotContain("SlaId", propriedades);
        Assert.DoesNotContain("RequerAprovacao", propriedades);
        Assert.DoesNotContain("AprovacaoPendente", propriedades);
        Assert.DoesNotContain("AprovacaoChamadoId", propriedades);
    }

    [Fact]
    public void DeveRejeitarCatalogoServicoIdVazio()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CatalogoServicoId = Guid.Empty,
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.CatalogoServicoId));
    }

    [Fact]
    public void DeveRejeitarNaturezaObrigatoria()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid()
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.NaturezaChamado));
    }

    [Fact]
    public void DeveRejeitarIncidenteSemImpactoEUrgencia()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Incidente
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.ImpactoChamado));
        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.UrgenciaChamado));
    }

    [Fact]
    public void DeveAceitarRequisicaoSemImpactoEUrgenciaComCategoria()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao
        });

        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.ImpactoChamado));
        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.UrgenciaChamado));
    }

    [Fact]
    public void DeveRejeitarMudancaSemDetalhamentoMinimo()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Mudanca sem detalhe",
            Descricao = "curta",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Mudanca,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Media
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.Descricao));
    }

    [Fact]
    public void DeveRejeitarProblemaSemDetalhamentoMinimo()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Problema sem detalhe",
            Descricao = "curta",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Problema,
            ImpactoChamado = ImpactoChamadoEnum.Medio,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.Descricao));
    }

    [Fact]
    public void DeveAceitarEventoAlertaComCamposMinimos()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Alerta de monitoramento",
            Descricao = "Falha recorrente detectada pelo monitoramento na aplicacao X.",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.EventoAlerta,
            ImpactoChamado = ImpactoChamadoEnum.Alto,
            UrgenciaChamado = UrgenciaChamadoEnum.Alta
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void DeveAceitarTarefaOperacionalComCamposMinimos()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Rotina operacional noturna",
            Descricao = "Executar procedimento operacional padrao na janela noturna.",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.TarefaOperacional,
            ImpactoChamado = ImpactoChamadoEnum.Baixo,
            UrgenciaChamado = UrgenciaChamadoEnum.Baixa
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void DeveAceitarAusenciaDeRespostasFormulario()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario = null
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void DeveAceitarListaVaziaDeRespostasFormulario()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario = []
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void DeveAceitarRespostaFormularioComValor()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valor = "vpn"
                }
            ]
        });

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeveAceitarRespostaFormularioComValores()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valores = ["vpn", "email"]
                }
            ]
        });

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeveRejeitarRespostaFormularioSemCampoFormularioServicoId()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.Empty,
                    Valor = "vpn"
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor("RespostasFormulario[0].CampoFormularioServicoId");
    }

    [Fact]
    public void DeveRejeitarRespostaFormularioSemValorESemValores()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid()
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor("RespostasFormulario[0]");
    }

    [Fact]
    public void DeveRejeitarRespostaFormularioComValorEValoresAoMesmoTempo()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valor = "vpn",
                    Valores = ["email"]
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor("RespostasFormulario[0]");
    }

    [Fact]
    public void DeveRejeitarRespostaFormularioComValorAcimaDoLimite()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valor = new string('A', RespostaFormularioAberturaRequestValidator.TamanhoMaximoValor + 1)
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor("RespostasFormulario[0].Valor");
    }

    [Fact]
    public void DeveRejeitarRespostaFormularioComItemVazioEmValores()
    {
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = Guid.NewGuid(),
                    Valores = ["vpn", ""]
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor("RespostasFormulario[0].Valores[1]");
    }

    [Fact]
    public void DeveRejeitarRespostasDuplicadasParaOMesmoCampo()
    {
        var campoId = Guid.NewGuid();
        var resultado = _validator.TestValidate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida para requisicao",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid(),
            NaturezaChamado = NaturezaChamadoEnum.Requisicao,
            RespostasFormulario =
            [
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoId,
                    Valor = "vpn"
                },
                new RespostaFormularioAberturaRequest
                {
                    CampoFormularioServicoId = campoId,
                    Valor = "email"
                }
            ]
        });

        resultado.ShouldHaveValidationErrorFor(x => x.RespostasFormulario);
    }
}
