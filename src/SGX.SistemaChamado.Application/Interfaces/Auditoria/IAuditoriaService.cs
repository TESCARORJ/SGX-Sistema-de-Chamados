using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Interfaces.Auditoria;

public sealed class RegistrarEventoAuditoriaRequest
{
    public string Modulo { get; init; } = string.Empty;
    public string Entidade { get; init; } = string.Empty;
    public string? EntidadeId { get; init; }
    public TipoAcaoAuditoria Acao { get; init; } = TipoAcaoAuditoria.Outro;
    public string Descricao { get; init; } = string.Empty;
    public string? DadosAntes { get; init; }
    public string? DadosDepois { get; init; }
    public string? Metadados { get; init; }
    public NivelAuditoria Nivel { get; init; } = NivelAuditoria.Informacao;
    public bool Sucesso { get; init; } = true;
    public string? MensagemErro { get; init; }
    public Guid? UsuarioId { get; init; }
    public string? UsuarioNome { get; init; }
    public string? UsuarioEmail { get; init; }
    public string? UsuarioLogin { get; init; }
    public string? IpOrigem { get; init; }
    public string? UserAgent { get; init; }
    public string? CorrelacaoId { get; init; }
    public bool FalhaCritica { get; init; }
}

public interface IAuditoriaService
{
    Task RegistrarAsync(RegistrarEventoAuditoriaRequest request, CancellationToken cancellationToken = default);

    Task RegistrarCriacaoAsync(
        string modulo,
        string entidade,
        string entidadeId,
        string descricao,
        string? dadosDepois = null,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarEdicaoAsync(
        string modulo,
        string entidade,
        string entidadeId,
        string descricao,
        string? dadosAntes = null,
        string? dadosDepois = null,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarExclusaoLogicaAsync(
        string modulo,
        string entidade,
        string entidadeId,
        string descricao,
        string? dadosAntes = null,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarAtivacaoAsync(
        string modulo,
        string entidade,
        string entidadeId,
        string descricao,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarInativacaoAsync(
        string modulo,
        string entidade,
        string entidadeId,
        string descricao,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarLoginAsync(
        bool sucesso,
        string descricao,
        string? mensagemErro = null,
        Guid? usuarioId = null,
        string? usuarioNome = null,
        string? usuarioEmail = null,
        string? usuarioLogin = null,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarLogoutAsync(
        string descricao,
        string? metadados = null,
        CancellationToken cancellationToken = default);

    Task RegistrarErroAsync(
        string modulo,
        string entidade,
        string descricao,
        string? entidadeId = null,
        Exception? exception = null,
        string? metadados = null,
        CancellationToken cancellationToken = default);
}
