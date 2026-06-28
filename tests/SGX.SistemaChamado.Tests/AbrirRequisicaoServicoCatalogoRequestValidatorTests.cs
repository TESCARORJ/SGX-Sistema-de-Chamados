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
}
