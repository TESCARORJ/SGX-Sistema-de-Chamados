namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class ListarGruposTecnicosRequest
{
    public string? Texto { get; init; }
    public bool? Ativo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "nome";
    public string DirecaoOrdenacao { get; init; } = "asc";
}

public sealed class CriarGrupoTecnicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}

public sealed class AtualizarGrupoTecnicoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}

public sealed class AlterarStatusGrupoTecnicoRequest
{
    public bool Ativo { get; init; }
}

public sealed class ListarMembrosGrupoTecnicoRequest
{
    public bool? Ativo { get; init; }
}

public sealed class ListarFilasAtendimentoGrupoTecnicoRequest
{
    public bool? Ativo { get; init; }
    public string? Busca { get; init; }
}

public sealed class AdicionarMembroGrupoTecnicoRequest
{
    public Guid UsuarioId { get; init; }
}

public sealed class AlterarStatusMembroGrupoTecnicoRequest
{
    public bool Ativo { get; init; }
}

public sealed record GrupoTecnicoResumoResponse(
    Guid Id,
    string Nome,
    bool Ativo);

public sealed record GrupoTecnicoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record MembroGrupoTecnicoResponse(
    Guid Id,
    Guid GrupoTecnicoId,
    Guid UsuarioId,
    string UsuarioNome,
    string UsuarioEmail,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm);

public sealed record GrupoTecnicoDoUsuarioResponse(
    Guid GrupoTecnicoId,
    string Nome,
    bool Ativo);

public sealed record FilaAtendimentoResumoResponse(
    Guid Id,
    Guid GrupoTecnicoId,
    string Nome,
    string? Descricao,
    bool Ativo);
