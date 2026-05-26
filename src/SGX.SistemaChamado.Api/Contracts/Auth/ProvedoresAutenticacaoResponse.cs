namespace SGX.SistemaChamado.Api.Contracts.Auth;

public sealed record ProvedoresAutenticacaoResponse(
    IReadOnlyCollection<ProvedorAutenticacaoDto> Provedores);

public sealed record ProvedorAutenticacaoDto(
    string Codigo,
    string Nome,
    string Descricao,
    bool Habilitado,
    bool Principal,
    int Ordem);
