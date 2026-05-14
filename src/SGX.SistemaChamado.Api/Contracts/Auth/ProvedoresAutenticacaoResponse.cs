namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed record ProvedoresAutenticacaoResponse(
    string ProvedorPrincipal,
    bool LoginMicrosoftHabilitado,
    bool LoginLocalSgxHabilitado,
    bool LoginLocalDevelopmentHabilitado);
