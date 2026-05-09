using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PermissaoSistema : AuditableEntity
{
    public string Codigo { get; private set; } = string.Empty;
    public string Modulo { get; private set; } = string.Empty;
    public string Acao { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public ICollection<PerfilAcessoPermissao> PerfilPermissoes { get; private set; } = [];

    private PermissaoSistema()
    {
    }

    public PermissaoSistema(string modulo, string acao, string? descricao, string criadoPor)
    {
        DefinirModuloAcao(modulo, acao);
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirModuloAcao(string modulo, string acao)
    {
        if (string.IsNullOrWhiteSpace(modulo))
        {
            throw new ArgumentException("O modulo da permissao e obrigatorio.", nameof(modulo));
        }

        if (string.IsNullOrWhiteSpace(acao))
        {
            throw new ArgumentException("A acao da permissao e obrigatoria.", nameof(acao));
        }

        Modulo = modulo.Trim();
        Acao = acao.Trim();
        Codigo = $"{Modulo}.{Acao}";
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
