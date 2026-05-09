using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class RoadmapCategoria : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? Cor { get; private set; }
    public string? Icone { get; private set; }
    public int? Ordem { get; private set; }

    public ICollection<RoadmapItsmItem> ItensRoadmap { get; private set; } = [];

    private RoadmapCategoria()
    {
    }

    public RoadmapCategoria(
        string nome,
        string? descricao,
        string? cor,
        string? icone,
        int? ordem,
        string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirCor(cor);
        DefinirIcone(icone);
        DefinirOrdem(ordem);
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        string nome,
        string? descricao,
        string? cor,
        string? icone,
        int? ordem,
        string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirCor(cor);
        DefinirIcone(icone);
        DefinirOrdem(ordem);
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("Nome da categoria e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirCor(string? cor)
    {
        Cor = string.IsNullOrWhiteSpace(cor) ? null : cor.Trim();
    }

    public void DefinirIcone(string? icone)
    {
        Icone = string.IsNullOrWhiteSpace(icone) ? null : icone.Trim();
    }

    public void DefinirOrdem(int? ordem)
    {
        if (ordem.HasValue && ordem.Value < 0)
        {
            throw new ArgumentException("Ordem da categoria nao pode ser negativa.", nameof(ordem));
        }

        Ordem = ordem;
    }
}
