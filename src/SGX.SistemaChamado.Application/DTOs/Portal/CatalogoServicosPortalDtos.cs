using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Portal;

public sealed class PortalFiltroCatalogoServicoRequest
{
    public string? Termo { get; set; }
    public Guid? DepartamentoResponsavelId { get; set; }
    public Guid? CategoriaId { get; set; }
    public Guid? SubcategoriaId { get; set; }
    public bool? PermiteAberturaChamado { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 20;
}

public sealed class PortalCatalogoServicoListagemDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public Guid DepartamentoResponsavelId { get; init; }
    public string? DepartamentoResponsavelNome { get; init; }
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public string? SubcategoriaNome { get; init; }
    public bool PermiteAberturaChamado { get; init; }
    public bool RequerAprovacao { get; init; }
    public VisibilidadeCatalogoServico Visibilidade { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public int Ordem { get; init; }
}

public sealed class PortalCatalogoServicoDetalheDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? InstrucoesSolicitante { get; init; }
    public Guid DepartamentoResponsavelId { get; init; }
    public string? DepartamentoResponsavelNome { get; init; }
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public string? SubcategoriaNome { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public string? PrioridadePadraoNome { get; init; }
    public Guid? SlaPadraoId { get; init; }
    public string? SlaPadraoNome { get; init; }
    public Guid? ArtigoBaseConhecimentoId { get; init; }
    public string? ArtigoBaseConhecimentoTitulo { get; init; }
    public string? ArtigoBaseConhecimentoSlug { get; init; }
    public bool PermiteAberturaChamado { get; init; }
    public bool RequerAprovacao { get; init; }
    public VisibilidadeCatalogoServico Visibilidade { get; init; }
    public DateTime? PublicadoEm { get; init; }
}

public sealed class PortalPrepararChamadoCatalogoServicoDto
{
    public Guid CatalogoServicoId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public string? InstrucoesSolicitante { get; init; }
    public Guid DepartamentoResponsavelId { get; init; }
    public string? DepartamentoResponsavelNome { get; init; }
    public Guid? CategoriaId { get; init; }
    public string? CategoriaNome { get; init; }
    public Guid? SubcategoriaId { get; init; }
    public string? SubcategoriaNome { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public string? PrioridadePadraoNome { get; init; }
    public Guid? SlaPadraoId { get; init; }
    public string? SlaPadraoNome { get; init; }
    public bool RequerAprovacao { get; init; }
    public bool PermiteAberturaChamado { get; init; }
    public PortalFormularioPreparacaoDto? Formulario { get; init; }
}

public sealed class PortalFormularioPreparacaoDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public PortalFormularioPreparacaoVersaoDto Versao { get; init; } = new();
}

public sealed class PortalFormularioPreparacaoVersaoDto
{
    public Guid Id { get; init; }
    public int Numero { get; init; }
    public bool Publicada { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public IReadOnlyCollection<PortalFormularioPreparacaoCampoDto> Campos { get; init; } = [];
}

public sealed class PortalFormularioPreparacaoCampoDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public TipoCampoFormularioServico Tipo { get; init; }
    public bool Obrigatorio { get; init; }
    public int Ordem { get; init; }
    public string? TextoAjuda { get; init; }
    public IReadOnlyCollection<PortalFormularioPreparacaoOpcaoDto> Opcoes { get; init; } = [];
}

public sealed class PortalFormularioPreparacaoOpcaoDto
{
    public Guid Id { get; init; }
    public string Valor { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public int Ordem { get; init; }
}

public sealed class PortalListaCatalogoServicosResponse
{
    public IReadOnlyCollection<PortalCatalogoServicoListagemDto> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}
