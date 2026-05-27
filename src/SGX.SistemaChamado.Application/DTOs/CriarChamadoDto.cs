using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs;

public sealed record CriarChamadoDto(
    string Titulo,
    string Descricao,
    Guid SolicitanteId,
    Guid CategoriaId,
    Guid? SubcategoriaId,
    Guid PrioridadeId,
    NaturezaChamadoEnum NaturezaChamado = NaturezaChamadoEnum.Requisicao,
    ImpactoChamadoEnum? ImpactoChamado = null,
    UrgenciaChamadoEnum? UrgenciaChamado = null,
    Guid? TipoSolicitacaoId = null,
    Guid? LocalUnidadeId = null,
    Guid? DepartamentoId = null,
    string Origem = "Portal");
