namespace SGX.SistemaChamado.Domain.Abstractions;

public abstract class CreationAuditableEntity : EntityBase
{
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public string CriadoPor { get; protected set; } = "sistema";

    protected void DefinirCriacao(string criadoPor)
    {
        CriadoEm = DateTime.UtcNow;
        CriadoPor = string.IsNullOrWhiteSpace(criadoPor) ? "sistema" : criadoPor.Trim();
    }
}
