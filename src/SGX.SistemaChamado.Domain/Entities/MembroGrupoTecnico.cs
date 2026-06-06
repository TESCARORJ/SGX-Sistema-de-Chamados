using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class MembroGrupoTecnico : AuditableEntity
{
    public Guid GrupoTecnicoId { get; private set; }
    public Guid UsuarioId { get; private set; }

    public GrupoTecnico GrupoTecnico { get; private set; } = default!;
    public Usuario Usuario { get; private set; } = default!;

    private MembroGrupoTecnico()
    {
    }

    public MembroGrupoTecnico(Guid grupoTecnicoId, Guid usuarioId, string criadoPor)
    {
        if (grupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico do membro e obrigatorio.", nameof(grupoTecnicoId));
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario do membro do grupo tecnico e obrigatorio.", nameof(usuarioId));
        }

        GrupoTecnicoId = grupoTecnicoId;
        UsuarioId = usuarioId;
        DefinirCriacao(criadoPor);
    }

    public void Inativar(string atualizadoPor)
        => Desativar(atualizadoPor);

    public void Reativar(string atualizadoPor)
        => Ativar(atualizadoPor);
}
