using SGX.SistemaChamado.Application.DTOs.Portal;
using SGX.SistemaChamado.Application.Validators;

namespace SGX.SistemaChamado.Tests;

public sealed class CriarChamadoRequestValidatorTests
{
    private readonly CriarChamadoRequestValidator _validator = new();

    [Fact]
    public void DeveRejeitarTituloObrigatorio()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = string.Empty,
            Descricao = "Descricao valida",
            CategoriaId = Guid.NewGuid(),
            PrioridadeId = Guid.NewGuid()
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
            PrioridadeId = Guid.NewGuid()
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
            PrioridadeId = Guid.NewGuid()
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
            PrioridadeId = Guid.Empty
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
            CatalogoServicoId = Guid.NewGuid()
        });

        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.CategoriaId));
        Assert.DoesNotContain(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.PrioridadeId));
    }

    [Fact]
    public void DeveRejeitarCatalogoServicoIdVazio()
    {
        var resultado = _validator.Validate(new CriarChamadoRequest
        {
            Titulo = "Titulo valido",
            Descricao = "Descricao valida",
            CatalogoServicoId = Guid.Empty
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarChamadoRequest.CatalogoServicoId));
    }
}
