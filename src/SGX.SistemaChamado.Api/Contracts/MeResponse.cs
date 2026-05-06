namespace SGX.SistemaChamado.Api.Contracts;

public sealed record MeResponse(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    string Situacao,
    IReadOnlyCollection<string> Perfis,
    IReadOnlyCollection<string> Permissoes,
    Guid? DepartamentoId,
    string AutenticadoPor);
