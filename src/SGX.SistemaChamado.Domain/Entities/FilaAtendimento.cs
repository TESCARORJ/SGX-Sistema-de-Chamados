using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class FilaAtendimento : AuditableEntity
{
    public Guid GrupoTecnicoId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }

    public GrupoTecnico GrupoTecnico { get; private set; } = default!;
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private FilaAtendimento()
    {
    }

    public FilaAtendimento(Guid grupoTecnicoId, string nome, string? descricao, string criadoPor)
    {
        if (grupoTecnicoId == Guid.Empty)
        {
            throw new ArgumentException("O grupo tecnico da fila de atendimento e obrigatorio.", nameof(grupoTecnicoId));
        }

        GrupoTecnicoId = grupoTecnicoId;
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
            throw new ArgumentException("O nome da fila de atendimento e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    private void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }
}
