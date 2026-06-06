using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class GrupoTecnico : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public ICollection<MembroGrupoTecnico> Membros { get; private set; } = [];
    public ICollection<FilaAtendimento> FilasAtendimento { get; private set; } = [];
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private GrupoTecnico()
    {
    }

    public GrupoTecnico(string nome, string? descricao, string criadoPor)
    {
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

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do grupo tecnico e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    private void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
