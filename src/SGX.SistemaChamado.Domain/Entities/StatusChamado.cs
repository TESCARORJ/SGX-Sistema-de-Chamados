using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class StatusChamado : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public StatusChamadoEnum Codigo { get; private set; }
    public string? Descricao { get; private set; }
    public bool EhStatusFinal { get; private set; }
    public bool PausaSla { get; private set; }

    public ICollection<Chamado> Chamados { get; private set; } = [];

    private StatusChamado()
    {
    }

    public StatusChamado(
        string nome,
        StatusChamadoEnum codigo,
        string? descricao,
        bool ehStatusFinal,
        bool pausaSla,
        string criadoPor)
    {
        DefinirNome(nome);
        Codigo = codigo;
        DefinirDescricao(descricao);
        EhStatusFinal = ehStatusFinal;
        PausaSla = pausaSla;
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do status e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirCodigo(StatusChamadoEnum codigo, string atualizadoPor)
    {
        Codigo = codigo;
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirRegras(bool ehStatusFinal, bool pausaSla, string atualizadoPor)
    {
        EhStatusFinal = ehStatusFinal;
        PausaSla = pausaSla;
        AtualizarAuditoria(atualizadoPor);
    }
}
