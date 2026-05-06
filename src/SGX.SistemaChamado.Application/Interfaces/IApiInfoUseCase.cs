namespace SGX.SistemaChamado.Application.Interfaces;

public interface IApiInfoUseCase
{
    DTOs.ApiInfoDto Executar(string ambiente);
}
