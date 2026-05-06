using SGX.SistemaChamado.Application.UseCases;

namespace SGX.SistemaChamado.Tests;

public sealed class ApiInfoUseCaseTests
{
    [Fact]
    public void Executar_DeveRetornarNomeFuncionalCorreto()
    {
        var useCase = new ApiInfoUseCase();

        var response = useCase.Executar("Development");

        Assert.Equal("SGX Sistema de Chamados", response.NomeFuncional);
        Assert.Equal("Development", response.Ambiente);
    }
}
