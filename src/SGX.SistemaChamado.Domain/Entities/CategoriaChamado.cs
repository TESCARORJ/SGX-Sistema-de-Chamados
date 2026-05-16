using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class CategoriaChamado : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public Guid? DepartamentoId { get; private set; }

    public Departamento? Departamento { get; private set; }
    public ICollection<SubcategoriaChamado> Subcategorias { get; private set; } = [];
    public ICollection<Chamado> Chamados { get; private set; } = [];
    public ICollection<SlaConfiguracao> SlaConfiguracoes { get; private set; } = [];

    private CategoriaChamado()
    {
    }

    public CategoriaChamado(string nome, string? descricao, Guid? departamentoId, string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DepartamentoId = departamentoId;
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da categoria e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirDepartamento(Guid? departamentoId, string atualizadoPor)
    {
        DepartamentoId = departamentoId;
        AtualizarAuditoria(atualizadoPor);
    }
}
