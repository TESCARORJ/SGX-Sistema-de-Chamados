using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class TokenRecuperacaoSenha : AuditableEntity
{
    public Guid UsuarioId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiraEm { get; private set; }
    public DateTime? UtilizadoEm { get; private set; }
    public string? IpSolicitacao { get; private set; }
    public string? UserAgentSolicitacao { get; private set; }

    public Usuario Usuario { get; private set; } = null!;

    private TokenRecuperacaoSenha()
    {
    }

    public TokenRecuperacaoSenha(
        Guid usuarioId,
        string tokenHash,
        DateTime expiraEm,
        string criadoPor,
        string? ipSolicitacao = null,
        string? userAgentSolicitacao = null)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException("Token hash obrigatorio.", nameof(tokenHash));
        }

        UsuarioId = usuarioId;
        TokenHash = tokenHash.Trim();
        ExpiraEm = expiraEm;
        IpSolicitacao = string.IsNullOrWhiteSpace(ipSolicitacao) ? null : ipSolicitacao.Trim();
        UserAgentSolicitacao = string.IsNullOrWhiteSpace(userAgentSolicitacao) ? null : userAgentSolicitacao.Trim();
        DefinirCriacao(criadoPor);
    }

    public bool EstaExpirado(DateTime agoraUtc) => ExpiraEm <= agoraUtc;

    public bool EstaUtilizado() => UtilizadoEm.HasValue;

    public void MarcarUtilizado(DateTime utilizadoEmUtc, string atualizadoPor)
    {
        UtilizadoEm = utilizadoEmUtc;
        Ativo = false;
        AtualizarAuditoria(atualizadoPor);
    }
}
