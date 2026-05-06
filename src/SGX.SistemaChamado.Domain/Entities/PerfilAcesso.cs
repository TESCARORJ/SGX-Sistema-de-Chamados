using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PerfilAcesso : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public TipoPerfil TipoPerfil { get; private set; }
    public string? Descricao { get; private set; }

    public ICollection<UsuarioPerfilAcesso> UsuarioPerfis { get; private set; } = [];

    private PerfilAcesso()
    {
    }

    public PerfilAcesso(string nome, TipoPerfil tipoPerfil, string? descricao, string criadoPor)
    {
        DefinirNome(nome);
        TipoPerfil = tipoPerfil;
        DefinirDescricao(descricao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do perfil de acesso e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirTipoPerfil(TipoPerfil tipoPerfil, string atualizadoPor)
    {
        TipoPerfil = tipoPerfil;
        AtualizarAuditoria(atualizadoPor);
    }
}
