using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroInventarioAtivoRequest
{
    public string? Termo { get; init; }
    public Guid? TipoAtivoInventarioId { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public StatusOperacionalAtivo? StatusOperacional { get; init; }
    public StatusPatrimonialAtivo? StatusPatrimonial { get; init; }
    public CriticidadeAtivo? Criticidade { get; init; }
    public bool? Ativo { get; init; }
    public DateTime? DataAquisicaoInicial { get; init; }
    public DateTime? DataAquisicaoFinal { get; init; }
    public DateTime? DataFimGarantiaInicial { get; init; }
    public DateTime? DataFimGarantiaFinal { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "atualizadoEm";
    public string DirecaoOrdenacao { get; init; } = "desc";
}

public sealed record InventarioAtivoListagemDto(
    Guid Id,
    string Codigo,
    string Nome,
    string? NumeroPatrimonio,
    string? NumeroSerie,
    Guid TipoAtivoInventarioId,
    string TipoAtivoInventarioNome,
    Guid? DepartamentoId,
    string? DepartamentoNome,
    Guid? LocalUnidadeId,
    string? LocalUnidadeNome,
    Guid? UsuarioResponsavelId,
    string? UsuarioResponsavelNome,
    StatusOperacionalAtivo StatusOperacional,
    string StatusOperacionalDescricao,
    StatusPatrimonialAtivo StatusPatrimonial,
    string StatusPatrimonialDescricao,
    CriticidadeAtivo Criticidade,
    string CriticidadeDescricao,
    DateTime? DataAquisicao,
    DateTime? DataFimGarantia,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record InventarioAtivoDetalheDto(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao,
    string? NumeroPatrimonio,
    string? NumeroSerie,
    Guid TipoAtivoInventarioId,
    string TipoAtivoInventarioNome,
    string? Fabricante,
    string? Modelo,
    Guid? DepartamentoId,
    string? DepartamentoNome,
    Guid? LocalUnidadeId,
    string? LocalUnidadeNome,
    Guid? UsuarioResponsavelId,
    string? UsuarioResponsavelNome,
    StatusOperacionalAtivo StatusOperacional,
    string StatusOperacionalDescricao,
    StatusPatrimonialAtivo StatusPatrimonial,
    string StatusPatrimonialDescricao,
    CriticidadeAtivo Criticidade,
    string CriticidadeDescricao,
    DateTime? DataAquisicao,
    DateTime? DataFimGarantia,
    decimal? ValorAquisicao,
    string? Fornecedor,
    string? Observacoes,
    bool Ativo,
    DateTime CriadoEm,
    Guid CriadoPorUsuarioId,
    DateTime? AtualizadoEm,
    Guid? AtualizadoPorUsuarioId,
    DateTime? InativadoEm,
    Guid? InativadoPorUsuarioId);

public sealed class CriarInventarioAtivoRequest
{
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? NumeroPatrimonio { get; init; }
    public string? NumeroSerie { get; init; }
    public Guid TipoAtivoInventarioId { get; init; }
    public string? Fabricante { get; init; }
    public string? Modelo { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public StatusOperacionalAtivo? StatusOperacional { get; init; }
    public StatusPatrimonialAtivo? StatusPatrimonial { get; init; }
    public CriticidadeAtivo? Criticidade { get; init; }
    public DateTime? DataAquisicao { get; init; }
    public DateTime? DataFimGarantia { get; init; }
    public decimal? ValorAquisicao { get; init; }
    public string? Fornecedor { get; init; }
    public string? Observacoes { get; init; }
}

public sealed class AtualizarInventarioAtivoRequest
{
    public string Codigo { get; init; } = string.Empty;
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? NumeroPatrimonio { get; init; }
    public string? NumeroSerie { get; init; }
    public Guid TipoAtivoInventarioId { get; init; }
    public string? Fabricante { get; init; }
    public string? Modelo { get; init; }
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public StatusOperacionalAtivo StatusOperacional { get; init; } = StatusOperacionalAtivo.Operacional;
    public StatusPatrimonialAtivo StatusPatrimonial { get; init; } = StatusPatrimonialAtivo.EmUso;
    public CriticidadeAtivo Criticidade { get; init; } = CriticidadeAtivo.Media;
    public DateTime? DataAquisicao { get; init; }
    public DateTime? DataFimGarantia { get; init; }
    public decimal? ValorAquisicao { get; init; }
    public string? Fornecedor { get; init; }
    public string? Observacoes { get; init; }
}

public sealed class FiltroHistoricoInventarioAtivoRequest
{
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
}

public sealed class FiltroChamadosRelacionadosInventarioAtivoRequest
{
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
}

public sealed class MovimentarInventarioAtivoRequest
{
    public Guid? DepartamentoId { get; init; }
    public Guid? LocalUnidadeId { get; init; }
    public Guid? UsuarioResponsavelId { get; init; }
    public StatusOperacionalAtivo? StatusOperacional { get; init; }
    public StatusPatrimonialAtivo? StatusPatrimonial { get; init; }
    public string? Observacao { get; init; }
}

public sealed record HistoricoInventarioAtivoDto(
    Guid Id,
    Guid InventarioAtivoId,
    TipoMovimentacaoAtivo TipoMovimentacao,
    string TipoMovimentacaoDescricao,
    string? DepartamentoOrigemNome,
    string? DepartamentoDestinoNome,
    string? LocalUnidadeOrigemNome,
    string? LocalUnidadeDestinoNome,
    string? UsuarioResponsavelOrigemNome,
    string? UsuarioResponsavelDestinoNome,
    StatusOperacionalAtivo? StatusOperacionalAnterior,
    StatusOperacionalAtivo? StatusOperacionalNovo,
    StatusPatrimonialAtivo? StatusPatrimonialAnterior,
    StatusPatrimonialAtivo? StatusPatrimonialNovo,
    string? Observacao,
    DateTime CriadoEm,
    string CriadoPorUsuarioNome);

public sealed record TipoAtivoInventarioDto(
    Guid Id,
    string Nome,
    string? Descricao,
    bool Ativo);

public sealed record ChamadoRelacionadoInventarioAtivoDto(
    Guid ChamadoId,
    string Protocolo,
    string Titulo,
    string Status,
    string Prioridade,
    string SolicitanteNome,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    DateTime? EncerradoEm);
