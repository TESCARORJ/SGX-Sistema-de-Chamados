using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PerfilAcessoPermissao : CreationAuditableEntity
{
    public Guid PerfilAcessoId { get; private set; }
    public Guid PermissaoSistemaId { get; private set; }

    public PerfilAcesso PerfilAcesso { get; private set; } = default!;
    public PermissaoSistema PermissaoSistema { get; private set; } = default!;

    private PerfilAcessoPermissao()
    {
    }

    public PerfilAcessoPermissao(Guid perfilAcessoId, Guid permissaoSistemaId, string criadoPor)
    {
        if (perfilAcessoId == Guid.Empty)
        {
            throw new ArgumentException("O perfil de acesso e obrigatorio.", nameof(perfilAcessoId));
        }

        if (permissaoSistemaId == Guid.Empty)
        {
            throw new ArgumentException("A permissao do sistema e obrigatoria.", nameof(permissaoSistemaId));
        }

        PerfilAcessoId = perfilAcessoId;
        PermissaoSistemaId = permissaoSistemaId;
        DefinirCriacao(criadoPor);
    }
}
