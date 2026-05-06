namespace SGX.SistemaChamado.Application.DTOs;

public sealed record CriarChamadoDto(
    string Titulo,
    string Descricao,
    Guid SolicitanteId,
    Guid CategoriaId,
    Guid PrioridadeId,
    Guid? DepartamentoId,
    string Origem = "Portal");
