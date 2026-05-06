using SGX.SistemaChamado.Application.DTOs;
using SGX.SistemaChamado.Application.Interfaces;

namespace SGX.SistemaChamado.Application.UseCases;

public sealed class ApiInfoUseCase : IApiInfoUseCase
{
    public ApiInfoDto Executar(string ambiente)
    {
        return new ApiInfoDto(
            NomeSistema: "SGX.SistemaChamado",
            NomeFuncional: "SGX Sistema de Chamados",
            Ambiente: ambiente,
            Versao: typeof(ApiInfoUseCase).Assembly.GetName().Version?.ToString() ?? "0.0.0",
            DataHoraUtc: DateTime.UtcNow);
    }
}
