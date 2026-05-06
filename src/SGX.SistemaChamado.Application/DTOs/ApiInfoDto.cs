namespace SGX.SistemaChamado.Application.DTOs;

public sealed record ApiInfoDto(
    string NomeSistema,
    string NomeFuncional,
    string Ambiente,
    string Versao,
    DateTime DataHoraUtc);
