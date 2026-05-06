using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class HistoricoChamado : CreationAuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public TipoHistoricoChamado Tipo { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public Guid? UsuarioId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario? Usuario { get; private set; }

    private HistoricoChamado()
    {
    }

    public HistoricoChamado(Guid chamadoId, TipoHistoricoChamado tipo, string descricao, Guid? usuarioId, string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado e obrigatorio.", nameof(chamadoId));
        }

        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descricao do historico e obrigatoria.", nameof(descricao));
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario informado e invalido.", nameof(usuarioId));
        }

        ChamadoId = chamadoId;
        Tipo = tipo;
        Descricao = descricao.Trim();
        UsuarioId = usuarioId;
        DefinirCriacao(criadoPor);
    }
}
