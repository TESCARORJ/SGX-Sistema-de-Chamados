using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Tests;

public sealed class StatusChamadoEnumTests
{
    [Fact]
    public void DevePreservarValoresNumericosLegados()
    {
        Assert.Equal(1, (int)StatusChamadoEnum.Aberto);
        Assert.Equal(2, (int)StatusChamadoEnum.EmAtendimento);
        Assert.Equal(3, (int)StatusChamadoEnum.AguardandoSolicitante);
        Assert.Equal(4, (int)StatusChamadoEnum.Resolvido);
        Assert.Equal(5, (int)StatusChamadoEnum.Encerrado);
        Assert.Equal(6, (int)StatusChamadoEnum.Cancelado);
    }
}
