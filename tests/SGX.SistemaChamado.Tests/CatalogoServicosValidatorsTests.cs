using FluentValidation.TestHelper;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Validators;

namespace SGX.SistemaChamado.Tests;

public sealed class CatalogoServicosValidatorsTests
{
    [Fact]
    public void CriarValidatorDeveRejeitarGrupoTecnicoIdVazio()
    {
        var validator = new CriarCatalogoServicoRequestValidator();
        var request = new CriarCatalogoServicoRequest
        {
            Nome = "Servico",
            Descricao = "Descricao valida",
            DepartamentoResponsavelId = Guid.NewGuid(),
            GrupoTecnicoId = Guid.Empty
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldHaveValidationErrorFor(x => x.GrupoTecnicoId);
    }

    [Fact]
    public void CriarValidatorDeveAceitarGrupoTecnicoIdValido()
    {
        var validator = new CriarCatalogoServicoRequestValidator();
        var request = new CriarCatalogoServicoRequest
        {
            Nome = "Servico",
            Descricao = "Descricao valida",
            DepartamentoResponsavelId = Guid.NewGuid(),
            GrupoTecnicoId = Guid.NewGuid()
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldNotHaveValidationErrorFor(x => x.GrupoTecnicoId);
    }

    [Fact]
    public void AtualizarValidatorDeveRejeitarGrupoTecnicoIdVazio()
    {
        var validator = new AtualizarCatalogoServicoRequestValidator();
        var request = new AtualizarCatalogoServicoRequest
        {
            Nome = "Servico",
            Descricao = "Descricao valida",
            DepartamentoResponsavelId = Guid.NewGuid(),
            GrupoTecnicoId = Guid.Empty
        };

        var resultado = validator.TestValidate(request);

        resultado.ShouldHaveValidationErrorFor(x => x.GrupoTecnicoId);
    }
}
