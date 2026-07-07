using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class FormularioServico : AuditableEntity
{
    public Guid CatalogoServicoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public CatalogoServico CatalogoServico { get; private set; } = default!;
    public ICollection<FormularioServicoVersao> Versoes { get; private set; } = [];

    private FormularioServico()
    {
    }

    public FormularioServico(Guid catalogoServicoId, string nome, string? descricao, string criadoPor)
    {
        DefinirCatalogoServico(catalogoServicoId);
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void AlterarDados(string nome, string? descricao, string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        AtualizarAuditoria(atualizadoPor);
    }

    public void Inativar(string atualizadoPor)
        => Desativar(atualizadoPor);

    public void Reativar(string atualizadoPor)
        => Ativar(atualizadoPor);

    private void DefinirCatalogoServico(Guid catalogoServicoId)
    {
        if (catalogoServicoId == Guid.Empty)
        {
            throw new ArgumentException("O servico de catalogo do formulario e obrigatorio.", nameof(catalogoServicoId));
        }

        CatalogoServicoId = catalogoServicoId;
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do formulario e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    private void DefinirDescricao(string? descricao)
    {
        if (descricao is not null && descricao.Trim().Length > 4000)
        {
            throw new ArgumentException("A descricao do formulario deve possuir no maximo 4000 caracteres.", nameof(descricao));
        }

        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
