namespace SGX.SistemaChamado.Application.DTOs;

public sealed record CriarChamadoDto(
    string Titulo,
    string Descricao,
    Guid SolicitanteId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    Guid PrioridadeId,
    Guid? TipoSolicitacaoId,
    Guid? LocalUnidadeId,
    Guid? DepartamentoId,
    string Origem = "Portal");
