using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class PoliticaSla : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public int Ordem { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? DepartamentoId { get; private set; }
    public Guid? CalendarioCorporativoId { get; private set; }
    public bool UsarHorarioComercial { get; private set; }
    public bool PausarQuandoAguardandoSolicitante { get; private set; }

    public CategoriaChamado? Categoria { get; private set; }
    public Departamento? Departamento { get; private set; }
    public CalendarioCorporativo? CalendarioCorporativo { get; private set; }
    public ICollection<MetaSla> Metas { get; private set; } = [];

    private PoliticaSla()
    {
    }

    public PoliticaSla(
        string nome,
        string? descricao,
        int ordem,
        Guid? categoriaId,
        Guid? departamentoId,
        Guid? calendarioCorporativoId,
        bool usarHorarioComercial,
        bool pausarQuandoAguardandoSolicitante,
        string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirOrdem(ordem);
        CategoriaId = categoriaId;
        DepartamentoId = departamentoId;
        CalendarioCorporativoId = calendarioCorporativoId;
        UsarHorarioComercial = usarHorarioComercial;
        PausarQuandoAguardandoSolicitante = pausarQuandoAguardandoSolicitante;
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(
        string nome,
        string? descricao,
        int ordem,
        Guid? categoriaId,
        Guid? departamentoId,
        Guid? calendarioCorporativoId,
        bool usarHorarioComercial,
        bool pausarQuandoAguardandoSolicitante,
        string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirOrdem(ordem);
        CategoriaId = categoriaId;
        DepartamentoId = departamentoId;
        CalendarioCorporativoId = calendarioCorporativoId;
        UsarHorarioComercial = usarHorarioComercial;
        PausarQuandoAguardandoSolicitante = pausarQuandoAguardandoSolicitante;
        AtualizarAuditoria(atualizadoPor);
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome da politica de SLA e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    private void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    private void DefinirOrdem(int ordem)
    {
        if (ordem <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem da politica de SLA deve ser maior que zero.");
        }

        Ordem = ordem;
    }
}
