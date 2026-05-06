namespace SGX.SistemaChamado.Domain.Abstractions;

public abstract class AuditableEntity : EntityBase
{
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
    public string CriadoPor { get; protected set; } = "sistema";
    public DateTime? AtualizadoEm { get; protected set; }
    public string? AtualizadoPor { get; protected set; }
    public bool Ativo { get; protected set; } = true;

    protected void DefinirCriacao(string criadoPor)
    {
        CriadoEm = DateTime.UtcNow;
        CriadoPor = string.IsNullOrWhiteSpace(criadoPor) ? "sistema" : criadoPor.Trim();
    }

    public void AtualizarAuditoria(string atualizadoPor)
    {
        AtualizadoEm = DateTime.UtcNow;
        AtualizadoPor = string.IsNullOrWhiteSpace(atualizadoPor) ? "sistema" : atualizadoPor.Trim();
    }

    public void Desativar(string atualizadoPor)
    {
        Ativo = false;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Ativar(string atualizadoPor)
    {
        Ativo = true;
        AtualizarAuditoria(atualizadoPor);
    }
}
