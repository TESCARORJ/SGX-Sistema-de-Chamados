using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.DTOs.Auditoria;

public sealed record EventoAuditoriaResponse(
    Guid Id,
    DateTime DataEvento,
    string? UsuarioNome,
    string? UsuarioEmail,
    string? IpOrigem,
    string Modulo,
    string Entidade,
    string? EntidadeId,
    TipoAcaoAuditoria Acao,
    string Descricao,
    NivelAuditoria Nivel,
    bool Sucesso,
    string? CorrelacaoId);

public sealed record EventoAuditoriaDetalheResponse(
    Guid Id,
    DateTime DataEvento,
    Guid? UsuarioId,
    string? UsuarioNome,
    string? UsuarioEmail,
    string? UsuarioLogin,
    string? IpOrigem,
    string? UserAgent,
    string Modulo,
    string Entidade,
    string? EntidadeId,
    TipoAcaoAuditoria Acao,
    string Descricao,
    string? DadosAntes,
    string? DadosDepois,
    string? Metadados,
    NivelAuditoria Nivel,
    bool Sucesso,
    string? MensagemErro,
    string? CorrelacaoId,
    DateTime CriadoEm);
