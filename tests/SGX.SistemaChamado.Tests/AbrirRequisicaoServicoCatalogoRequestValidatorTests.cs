using FluentValidation.TestHelper;
using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Validators;

namespace SGX.SistemaChamado.Tests;

public sealed class AbrirRequisicaoServicoCatalogoRequestValidatorTests
{
    private readonly AbrirRequisicaoServicoCatalogoRequestValidator _validator = new();

    [Fact]
    public void DeveExigirCatalogoServicoId()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.Empty,
            Titulo = "Solicitar VPN"
        });

        resultado.ShouldHaveValidationErrorFor(x => x.CatalogoServicoId);
    }

    [Fact]
    public void DeveExigirTitulo()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = string.Empty
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void DevePermitirDescricaoOpcional()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
            Descricao = null
        });

        resultado.ShouldNotHaveValidationErrorFor(x => x.Descricao);
    }

    [Fact]
    public void DeveRejeitarTituloComApenasEspacos()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "   "
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void DeveRejeitarTituloAcimaDoLimite()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = new string('A', 181)
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Titulo);
    }

    [Fact]
    public void DeveRejeitarDescricaoAcimaDoLimite()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
            Descricao = new string('B', 4001)
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Descricao);
    }

    [Fact]
    public void DeveAceitarRequestValido()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
            Descricao = "Preciso de acesso remoto."
        });

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeveAceitarListaVaziaDeRespostasFormulario()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
            RespostasFormulario = []
        });

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void DeveAceitarRespostaFormularioComValor()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
    public void DeveAceitarRespostaFormularioComValoresMultiplos()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar acessos",
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
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
    public void DeveRejeitarRespostaFormularioSemConteudo()
    {
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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
        var resultado = _validator.TestValidate(new AbrirRequisicaoServicoCatalogoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Titulo = "Solicitar VPN",
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

    [Fact]
    public void ContratoPublicoNaoDeveExporCamposSensiveisDeClassificacaoOuGovernanca()
    {
        var propriedades = typeof(AbrirRequisicaoServicoCatalogoRequest)
            .GetProperties()
            .Select(x => x.Name)
            .ToHashSet(StringComparer.Ordinal);

        Assert.DoesNotContain("NaturezaChamado", propriedades);
        Assert.DoesNotContain("CategoriaId", propriedades);
        Assert.DoesNotContain("SubcategoriaId", propriedades);
        Assert.DoesNotContain("PrioridadeId", propriedades);
        Assert.DoesNotContain("GrupoTecnicoId", propriedades);
        Assert.DoesNotContain("SlaId", propriedades);
        Assert.DoesNotContain("RequerAprovacao", propriedades);
        Assert.DoesNotContain("AprovacaoPendente", propriedades);
        Assert.DoesNotContain("AprovacaoChamadoId", propriedades);
    }
}
