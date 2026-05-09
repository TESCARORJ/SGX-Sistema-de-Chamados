using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Admin;

public sealed class FiltroCadastroRequest
{
    public string? Texto { get; init; }
    public bool? Ativo { get; init; }
    public int Pagina { get; init; } = 1;
    public int TamanhoPagina { get; init; } = 20;
    public string OrdenarPor { get; init; } = "nome";
    public string DirecaoOrdenacao { get; init; } = "asc";
}

public sealed class PagedResultResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int Total { get; init; }
    public int Pagina { get; init; }
    public int TamanhoPagina { get; init; }
}

public sealed record CadastroResumoResponse(Guid Id, string Nome, bool Ativo);
public sealed record AlterarSituacaoCadastroResponse(Guid Id, bool Ativo, string Mensagem);

public sealed record PerfilAcessoResumoResponse(Guid Id, string Nome, int TipoPerfil, string TipoPerfilDescricao, bool Ativo);
public sealed record PerfilAcessoDetalheResponse(Guid Id, string Nome, int TipoPerfil, string TipoPerfilDescricao, string? Descricao, bool Ativo);
public sealed record PermissaoSistemaResponse(
    Guid Id,
    string Codigo,
    string Nome,
    string? Descricao,
    string Modulo,
    string Acao,
    bool Ativo);

public sealed record PerfilPermissoesResponse(
    Guid PerfilId,
    string Nome,
    int TipoPerfil,
    IReadOnlyCollection<PermissaoSistemaResponse> PermissoesDisponiveis,
    IReadOnlyCollection<PermissaoSistemaResponse> PermissoesVinculadas);

public sealed class AtualizarPermissoesPerfilRequest
{
    public IReadOnlyCollection<string> CodigosPermissoes { get; init; } = [];
}

public sealed class CriarPerfilAcessoRequest
{
    public string Nome { get; init; } = string.Empty;
    public TipoPerfil TipoPerfil { get; init; }
    public string? Descricao { get; init; }
}

public sealed class AtualizarPerfilAcessoRequest
{
    public string Nome { get; init; } = string.Empty;
    public TipoPerfil TipoPerfil { get; init; }
    public string? Descricao { get; init; }
}

public sealed record UsuarioAdminResumoResponse(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    string Situacao,
    Guid? DepartamentoId,
    string? Departamento,
    bool Ativo,
    IReadOnlyCollection<PerfilAcessoResumoResponse> Perfis);

public sealed record UsuarioAdminDetalheResponse(
    Guid Id,
    string Nome,
    string Email,
    string Login,
    string Situacao,
    DateTime? UltimoAcessoEm,
    Guid? DepartamentoId,
    string? Departamento,
    bool Ativo,
    IReadOnlyCollection<PerfilAcessoResumoResponse> Perfis);

public sealed class CriarUsuarioAdminRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Login { get; init; }
    public Guid? DepartamentoId { get; init; }
    public IReadOnlyCollection<Guid> PerfilIds { get; init; } = [];
}

public sealed class AtualizarUsuarioAdminRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Login { get; init; }
    public Guid? DepartamentoId { get; init; }
    public SituacaoUsuario Situacao { get; init; } = SituacaoUsuario.Ativo;
}

public sealed class AlterarPerfisUsuarioRequest
{
    public IReadOnlyCollection<Guid> PerfilIds { get; init; } = [];
}

public sealed record DepartamentoResumoResponse(Guid Id, string Nome, string Sigla, bool Ativo);
public sealed record DepartamentoDetalheResponse(Guid Id, string Nome, string Sigla, string? Descricao, bool Ativo);
public sealed class CriarDepartamentoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Sigla { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}

public sealed class AtualizarDepartamentoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string Sigla { get; init; } = string.Empty;
    public string? Descricao { get; init; }
}

public sealed record CategoriaChamadoResumoResponse(Guid Id, string Nome, Guid? DepartamentoId, string? Departamento, bool Ativo);
public sealed record CategoriaChamadoDetalheResponse(Guid Id, string Nome, string? Descricao, Guid? DepartamentoId, string? Departamento, bool Ativo);
public sealed class CriarCategoriaChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public Guid? DepartamentoId { get; init; }
}

public sealed class AtualizarCategoriaChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public Guid? DepartamentoId { get; init; }
}

public sealed record PrioridadeChamadoResumoResponse(Guid Id, string Nome, int Nivel, string? Descricao, int PrazoPrimeiraRespostaHoras, int PrazoResolucaoHoras, bool Ativo);
public sealed record PrioridadeChamadoDetalheResponse(Guid Id, string Nome, int Nivel, string? Descricao, int PrazoPrimeiraRespostaHoras, int PrazoResolucaoHoras, bool Ativo);
public sealed class CriarPrioridadeChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public int Nivel { get; init; }
    public string? Descricao { get; init; }
    public int PrazoPrimeiraRespostaHoras { get; init; }
    public int PrazoResolucaoHoras { get; init; }
}

public sealed class AtualizarPrioridadeChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public int Nivel { get; init; }
    public string? Descricao { get; init; }
    public int PrazoPrimeiraRespostaHoras { get; init; }
    public int PrazoResolucaoHoras { get; init; }
}

public sealed record StatusChamadoResumoResponse(Guid Id, string Nome, int Codigo, string? Descricao, bool EhStatusFinal, bool PausaSla, bool Ativo);
public sealed record StatusChamadoDetalheResponse(Guid Id, string Nome, int Codigo, string? Descricao, bool EhStatusFinal, bool PausaSla, bool Ativo);
public sealed class CriarStatusChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public int Codigo { get; init; }
    public string? Descricao { get; init; }
    public bool EhStatusFinal { get; init; }
    public bool PausaSla { get; init; }
}

public sealed class AtualizarStatusChamadoRequest
{
    public string Nome { get; init; } = string.Empty;
    public int Codigo { get; init; }
    public string? Descricao { get; init; }
    public bool EhStatusFinal { get; init; }
    public bool PausaSla { get; init; }
}

public sealed record ParametroSistemaResumoResponse(Guid Id, string Chave, string Valor, string? Descricao, bool Sensivel, bool Ativo);
public sealed record ParametroSistemaDetalheResponse(Guid Id, string Chave, string Valor, string? Descricao, bool Sensivel, bool Ativo);
public sealed class CriarParametroSistemaRequest
{
    public string Chave { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Sensivel { get; init; }
}

public sealed class AtualizarParametroSistemaRequest
{
    public string Chave { get; init; } = string.Empty;
    public string Valor { get; init; } = string.Empty;
    public string? Descricao { get; init; }
    public bool Sensivel { get; init; }
}
