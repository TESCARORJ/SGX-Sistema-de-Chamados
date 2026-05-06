using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class Departamento : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Sigla { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public ICollection<Usuario> Usuarios { get; private set; } = [];
    public ICollection<CategoriaChamado> Categorias { get; private set; } = [];
    public ICollection<Chamado> Chamados { get; private set; } = [];
    public ICollection<SlaConfiguracao> SlaConfiguracoes { get; private set; } = [];

    private Departamento()
    {
    }

    public Departamento(string nome, string sigla, string? descricao, string criadoPor)
    {
        DefinirNome(nome);
        DefinirSigla(sigla);
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do departamento e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirSigla(string sigla)
    {
        if (string.IsNullOrWhiteSpace(sigla))
        {
            throw new ArgumentException("A sigla do departamento e obrigatoria.", nameof(sigla));
        }

        Sigla = sigla.Trim().ToUpperInvariant();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
