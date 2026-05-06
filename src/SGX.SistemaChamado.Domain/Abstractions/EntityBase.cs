using SGX.SistemaChamado.Domain.Interfaces;

namespace SGX.SistemaChamado.Domain.Abstractions;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}
