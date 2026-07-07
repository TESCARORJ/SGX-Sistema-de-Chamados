using FluentValidation.TestHelper;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class FormularioServicoValidatorsTests
{
    [Fact]
    public void FormularioValidoDevePassar()
    {
        var validator = new CriarFormularioServicoRequestValidator();
        var request = new CriarFormularioServicoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Nome = "Formulario RH",
            Descricao = "Descricao valida"
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void FormularioSemCatalogoServicoDeveFalhar()
    {
        var validator = new CriarFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = Guid.Empty,
            Nome = "Formulario RH"
        });

        resultado.ShouldHaveValidationErrorFor(x => x.CatalogoServicoId);
    }

    [Fact]
    public void FormularioSemNomeDeveFalhar()
    {
        var validator = new CriarFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarFormularioServicoRequest
        {
            CatalogoServicoId = Guid.NewGuid(),
            Nome = string.Empty
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void FormularioComDescricaoAcimaDoLimiteDeveFalhar()
    {
        var validator = new AtualizarFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarFormularioServicoRequest
        {
            Nome = "Formulario",
            Descricao = new string('d', 4001)
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Descricao);
    }

    [Fact]
    public void VersaoValidaDevePassar()
    {
        var validator = new CriarFormularioServicoVersaoRequestValidator();
        var request = new CriarFormularioServicoVersaoRequest
        {
            FormularioServicoId = Guid.NewGuid(),
            Numero = 1,
            Publicada = true,
            PublicadoEm = DateTime.UtcNow,
            Ativo = true
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void VersaoComNumeroInvalidoDeveFalhar()
    {
        var validator = new CriarFormularioServicoVersaoRequestValidator();
        var resultado = validator.TestValidate(new CriarFormularioServicoVersaoRequest
        {
            FormularioServicoId = Guid.NewGuid(),
            Numero = 0
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Numero);
    }

    [Fact]
    public void VersaoComPublicadoEmSemPublicadaTrueDeveFalhar()
    {
        var validator = new AtualizarFormularioServicoVersaoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarFormularioServicoVersaoRequest
        {
            Numero = 1,
            Publicada = false,
            PublicadoEm = DateTime.UtcNow
        });

        resultado.ShouldHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void CampoValidoDevePassar()
    {
        var validator = new CriarCampoFormularioServicoRequestValidator();
        var request = new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = Guid.NewGuid(),
            Nome = "centroCusto",
            Rotulo = "Centro de custo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Obrigatorio = true,
            Ordem = 1,
            TextoAjuda = "Informe o codigo",
            Visivel = true
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CampoSemNomeDeveFalhar()
    {
        var validator = new CriarCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = Guid.NewGuid(),
            Nome = string.Empty,
            Rotulo = "Rotulo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void CampoComNomeInvalidoDeveFalhar()
    {
        var validator = new AtualizarCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarCampoFormularioServicoRequest
        {
            Nome = "1invalido",
            Rotulo = "Rotulo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void CampoComTipoInvalidoDeveFalhar()
    {
        var validator = new AtualizarCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarCampoFormularioServicoRequest
        {
            Nome = "nomeValido",
            Rotulo = "Rotulo",
            Tipo = (TipoCampoFormularioServico)999,
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Tipo);
    }

    [Fact]
    public void CampoComOrdemInvalidaDeveFalhar()
    {
        var validator = new CriarCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarCampoFormularioServicoRequest
        {
            FormularioServicoVersaoId = Guid.NewGuid(),
            Nome = "nomeValido",
            Rotulo = "Rotulo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Ordem = 0
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Ordem);
    }

    [Fact]
    public void CampoComTextoAjudaAcimaDoLimiteDeveFalhar()
    {
        var validator = new AtualizarCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarCampoFormularioServicoRequest
        {
            Nome = "nomeValido",
            Rotulo = "Rotulo",
            Tipo = TipoCampoFormularioServico.TextoCurto,
            Ordem = 1,
            TextoAjuda = new string('a', 501)
        });

        resultado.ShouldHaveValidationErrorFor(x => x.TextoAjuda);
    }

    [Fact]
    public void OpcaoValidaDevePassar()
    {
        var validator = new CriarOpcaoCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = Guid.NewGuid(),
            Valor = "financeiro",
            Rotulo = "Financeiro",
            Ordem = 1
        });

        resultado.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void OpcaoComValorAcimaDoLimiteDeveFalhar()
    {
        var validator = new CriarOpcaoCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = Guid.NewGuid(),
            Valor = new string('v', 201),
            Rotulo = "Financeiro",
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Valor);
    }

    [Fact]
    public void OpcaoSemValorDeveFalhar()
    {
        var validator = new CriarOpcaoCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new CriarOpcaoCampoFormularioServicoRequest
        {
            CampoFormularioServicoId = Guid.NewGuid(),
            Valor = string.Empty,
            Rotulo = "Financeiro",
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Valor);
    }

    [Fact]
    public void OpcaoSemRotuloDeveFalhar()
    {
        var validator = new AtualizarOpcaoCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarOpcaoCampoFormularioServicoRequest
        {
            Valor = "financeiro",
            Rotulo = string.Empty,
            Ordem = 1
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Rotulo);
    }

    [Fact]
    public void OpcaoComOrdemInvalidaDeveFalhar()
    {
        var validator = new AtualizarOpcaoCampoFormularioServicoRequestValidator();
        var resultado = validator.TestValidate(new AtualizarOpcaoCampoFormularioServicoRequest
        {
            Valor = "financeiro",
            Rotulo = "Financeiro",
            Ordem = 0
        });

        resultado.ShouldHaveValidationErrorFor(x => x.Ordem);
    }
}
