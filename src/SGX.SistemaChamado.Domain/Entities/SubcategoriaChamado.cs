using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class SubcategoriaChamado : AuditableEntity
{
    public Guid CategoriaChamadoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public CategoriaChamado CategoriaChamado { get; private set; } = default!;
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private SubcategoriaChamado()
    {
    }

    public SubcategoriaChamado(Guid categoriaChamadoId, string nome, string? descricao, string criadoPor)
    {
        DefinirCategoriaChamado(categoriaChamadoId);
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirCategoriaChamado(Guid categoriaChamadoId)
    {
        if (categoriaChamadoId == Guid.Empty)
        {
            throw new ArgumentException("A categoria da subcategoria e obrigatoria.", nameof(categoriaChamadoId));
        }

        CategoriaChamadoId = categoriaChamadoId;
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da subcategoria e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
