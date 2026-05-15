using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class EventoAuditoria : EntityBase
{
    public DateTime DataEvento { get; private set; }
    public Guid? UsuarioId { get; private set; }
    public string? UsuarioNome { get; private set; }
    public string? UsuarioEmail { get; private set; }
    public string? UsuarioLogin { get; private set; }
    public string? IpOrigem { get; private set; }
    public string? UserAgent { get; private set; }
    public string Modulo { get; private set; } = string.Empty;
    public string Entidade { get; private set; } = string.Empty;
    public string? EntidadeId { get; private set; }
    public TipoAcaoAuditoria Acao { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string? DadosAntes { get; private set; }
    public string? DadosDepois { get; private set; }
    public string? Metadados { get; private set; }
    public NivelAuditoria Nivel { get; private set; }
    public bool Sucesso { get; private set; }
    public string? MensagemErro { get; private set; }
    public string? CorrelacaoId { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private EventoAuditoria()
    {
    }

    public EventoAuditoria(
        DateTime dataEvento,
        Guid? usuarioId,
        string? usuarioNome,
        string? usuarioEmail,
        string? usuarioLogin,
        string? ipOrigem,
        string? userAgent,
        string modulo,
        string entidade,
        string? entidadeId,
        TipoAcaoAuditoria acao,
        string descricao,
        string? dadosAntes,
        string? dadosDepois,
        string? metadados,
        NivelAuditoria nivel,
        bool sucesso,
        string? mensagemErro,
        string? correlacaoId)
    {
        if (string.IsNullOrWhiteSpace(modulo))
        {
            throw new ArgumentException("Modulo e obrigatorio.", nameof(modulo));
        }

        if (string.IsNullOrWhiteSpace(entidade))
        {
            throw new ArgumentException("Entidade e obrigatoria.", nameof(entidade));
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("Descricao e obrigatoria.", nameof(descricao));
        }

        DataEvento = dataEvento.Kind == DateTimeKind.Utc ? dataEvento : dataEvento.ToUniversalTime();
        UsuarioId = usuarioId;
        UsuarioNome = NormalizarNulo(usuarioNome);
        UsuarioEmail = NormalizarNulo(usuarioEmail);
        UsuarioLogin = NormalizarNulo(usuarioLogin);
        IpOrigem = NormalizarNulo(ipOrigem);
        UserAgent = NormalizarNulo(userAgent);
        Modulo = modulo.Trim();
        Entidade = entidade.Trim();
        EntidadeId = NormalizarNulo(entidadeId);
        Acao = acao;
        Descricao = descricao.Trim();
        DadosAntes = NormalizarNulo(dadosAntes);
        DadosDepois = NormalizarNulo(dadosDepois);
        Metadados = NormalizarNulo(metadados);
        Nivel = nivel;
        Sucesso = sucesso;
        MensagemErro = NormalizarNulo(mensagemErro);
        CorrelacaoId = NormalizarNulo(correlacaoId);
        CriadoEm = DateTime.UtcNow;
    }

    private static string? NormalizarNulo(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
