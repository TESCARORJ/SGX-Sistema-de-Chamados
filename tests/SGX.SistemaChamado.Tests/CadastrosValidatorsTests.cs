using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Validators;

namespace SGX.SistemaChamado.Tests;

public sealed class CadastrosValidatorsTests
{
    [Fact]
    public void CriarPrioridadeRejeitaCorForaDoFormatoHexadecimal()
    {
        var validator = new CriarPrioridadeChamadoRequestValidator();

        var resultado = validator.Validate(new CriarPrioridadeChamadoRequest
        {
            Nome = "Prioridade Teste",
            Peso = 2,
            Cor = "FF0000"
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarPrioridadeChamadoRequest.Cor));
    }

    [Fact]
    public void AtualizarPrioridadeAceitaCorHexadecimalValida()
    {
        var validator = new AtualizarPrioridadeChamadoRequestValidator();

        var resultado = validator.Validate(new AtualizarPrioridadeChamadoRequest
        {
            Nome = "Prioridade Atualizada",
            Peso = 3,
            Cor = "#00AAFF"
        });

        Assert.Empty(resultado.Errors);
    }

    [Fact]
    public void CriarSubcategoriaExigeCategoria()
    {
        var validator = new CriarSubcategoriaChamadoRequestValidator();

        var resultado = validator.Validate(new CriarSubcategoriaChamadoRequest
        {
            CategoriaChamadoId = Guid.Empty,
            Nome = "Subcategoria"
        });

        Assert.Contains(resultado.Errors, x => x.PropertyName == nameof(CriarSubcategoriaChamadoRequest.CategoriaChamadoId));
    }
}

