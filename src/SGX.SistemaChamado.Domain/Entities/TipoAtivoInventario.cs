using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class TipoAtivoInventario : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public ICollection<InventarioAtivo> Ativos { get; private set; } = [];

    private TipoAtivoInventario()
    {
    }

    public TipoAtivoInventario(string nome, string? descricao, string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do tipo de ativo e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
