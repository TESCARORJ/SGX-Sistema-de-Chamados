using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class LocalUnidade : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? Endereco { get; private set; }
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private LocalUnidade()
    {
    }

    public LocalUnidade(string nome, string? descricao, string? endereco, string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirEndereco(endereco);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do local/unidade e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirEndereco(string? endereco)
    {
        Endereco = string.IsNullOrWhiteSpace(endereco) ? null : endereco.Trim();
    }
}
