using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class UsuarioPerfilAcesso : CreationAuditableEntity
{
    public Guid UsuarioId { get; private set; }
    public Guid PerfilAcessoId { get; private set; }

    public Usuario Usuario { get; private set; } = default!;
    public PerfilAcesso PerfilAcesso { get; private set; } = default!;

    private UsuarioPerfilAcesso()
    {
    }

    public UsuarioPerfilAcesso(Guid usuarioId, Guid perfilAcessoId, string criadoPor)
    {
        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario e obrigatorio.", nameof(usuarioId));
        }

        if (perfilAcessoId == Guid.Empty)
        {
            throw new ArgumentException("O perfil de acesso e obrigatorio.", nameof(perfilAcessoId));
        }

        UsuarioId = usuarioId;
        PerfilAcessoId = perfilAcessoId;
        DefinirCriacao(criadoPor);
    }
}
