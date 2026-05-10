namespace SGX.SistemaChamado.Application.DTOs.Email;

public enum EmailProcessingStatus
{
    Processado = 1,
    Ignorado = 2,
    Erro = 3,
    Duplicado = 4,
    NaoCorrelacionado = 5
}

public sealed record EmailProcessingResult(
    EmailProcessingStatus Status,
    Guid? ChamadoId,
    string? Erro);
