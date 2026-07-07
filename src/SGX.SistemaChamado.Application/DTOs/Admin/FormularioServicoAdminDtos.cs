using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed record FormularioServicoAdminDto(
    Guid Id,
    Guid CatalogoServicoId,
    string Nome,
    string? Descricao,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record FormularioServicoDetalheAdminDto(
    Guid Id,
    Guid CatalogoServicoId,
    string Nome,
    string? Descricao,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    IReadOnlyCollection<FormularioServicoVersaoAdminDto> Versoes);

public sealed record FormularioServicoVersaoAdminDto(
    Guid Id,
    Guid FormularioServicoId,
    int Numero,
    bool Publicada,
    DateTime? PublicadoEm,
    bool Ativo,
    IReadOnlyCollection<CampoFormularioServicoAdminDto> Campos);

public sealed record CampoFormularioServicoAdminDto(
    Guid Id,
    Guid FormularioServicoVersaoId,
    string Nome,
    string Rotulo,
    TipoCampoFormularioServico Tipo,
    bool Obrigatorio,
    int Ordem,
    string? TextoAjuda,
    bool Visivel,
    bool Ativo,
    IReadOnlyCollection<OpcaoCampoFormularioServicoAdminDto> Opcoes);

public sealed record OpcaoCampoFormularioServicoAdminDto(
    Guid Id,
    Guid CampoFormularioServicoId,
    string Valor,
    string Rotulo,
    int Ordem,
    bool Ativo);

public sealed class CriarFormularioServicoRequest
{
    public Guid CatalogoServicoId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class AtualizarFormularioServicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class CriarFormularioServicoVersaoRequest
{
    public Guid FormularioServicoId { get; init; }
    public int Numero { get; init; }
    public bool Publicada { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class AtualizarFormularioServicoVersaoRequest
{
    public int Numero { get; init; }
    public bool Publicada { get; init; }
    public DateTime? PublicadoEm { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class CriarCampoFormularioServicoRequest
{
    public Guid FormularioServicoVersaoId { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public TipoCampoFormularioServico Tipo { get; init; }
    public bool Obrigatorio { get; init; }
    public int Ordem { get; init; }
    public string? TextoAjuda { get; init; }
    public bool Visivel { get; init; } = true;
    public bool Ativo { get; init; } = true;
}

public sealed class AtualizarCampoFormularioServicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public TipoCampoFormularioServico Tipo { get; init; }
    public bool Obrigatorio { get; init; }
    public int Ordem { get; init; }
    public string? TextoAjuda { get; init; }
    public bool Visivel { get; init; } = true;
    public bool Ativo { get; init; } = true;
}

public sealed class CriarOpcaoCampoFormularioServicoRequest
{
    public Guid CampoFormularioServicoId { get; init; }
    public string Valor { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public int Ordem { get; init; }
    public bool Ativo { get; init; } = true;
}

public sealed class AtualizarOpcaoCampoFormularioServicoRequest
{
    public string Valor { get; init; } = string.Empty;
    public string Rotulo { get; init; } = string.Empty;
    public int Ordem { get; init; }
    public bool Ativo { get; init; } = true;
}
