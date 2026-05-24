using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class CatalogoServico : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? InstrucoesSolicitante { get; private set; }
    public Guid DepartamentoResponsavelId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public Guid? SubcategoriaId { get; private set; }
    public Guid? PrioridadePadraoId { get; private set; }
    public Guid? SlaPadraoId { get; private set; }
    public Guid? ArtigoBaseConhecimentoId { get; private set; }
    public StatusCatalogoServico Status { get; private set; } = StatusCatalogoServico.Rascunho;
    public VisibilidadeCatalogoServico Visibilidade { get; private set; } = VisibilidadeCatalogoServico.Interno;
    public bool PermiteAberturaChamado { get; private set; } = true;
    public bool RequerAprovacao { get; private set; }
    public int Ordem { get; private set; }
    public DateTime? PublicadoEm { get; private set; }
    public Guid? PublicadoPorUsuarioId { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }
    public DateTime? ArquivadoEm { get; private set; }
    public Guid? ArquivadoPorUsuarioId { get; private set; }

    public Departamento DepartamentoResponsavel { get; private set; } = default!;
    public CategoriaChamado? Categoria { get; private set; }
    public SubcategoriaChamado? Subcategoria { get; private set; }
    public PrioridadeChamado? PrioridadePadrao { get; private set; }
    public PoliticaSla? SlaPadrao { get; private set; }
    public BaseConhecimentoArtigo? ArtigoBaseConhecimento { get; private set; }
    public ICollection<Chamado> Chamados { get; private set; } = [];

    private CatalogoServico()
    {
    }

    public CatalogoServico(
        string nome,
        string slug,
        string? descricao,
        string? instrucoesSolicitante,
        Guid departamentoResponsavelId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        Guid? prioridadePadraoId,
        Guid? slaPadraoId,
        Guid? artigoBaseConhecimentoId,
        VisibilidadeCatalogoServico visibilidade,
        bool permiteAberturaChamado,
        bool requerAprovacao,
        int ordem,
        Guid criadoPorUsuarioId,
        string criadoPor)
    {
        DefinirNome(nome);
        DefinirSlug(slug);
        DefinirDescricao(descricao);
        DefinirInstrucoesSolicitante(instrucoesSolicitante);
        DefinirDepartamentoResponsavel(departamentoResponsavelId);
        DefinirCategoria(categoriaId);
        DefinirSubcategoria(subcategoriaId);
        DefinirPrioridadePadrao(prioridadePadraoId);
        DefinirSlaPadrao(slaPadraoId);
        DefinirArtigoBaseConhecimento(artigoBaseConhecimentoId);
        DefinirVisibilidade(visibilidade);
        DefinirPermiteAberturaChamado(permiteAberturaChamado);
        DefinirRequerAprovacao(requerAprovacao);
        DefinirOrdem(ordem);
        DefinirCriadoPorUsuario(criadoPorUsuarioId);
        DefinirCriacao(criadoPor);
    }

    public void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do servico e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    public void DefinirSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("O slug do servico e obrigatorio.", nameof(slug));
        }

        Slug = slug.Trim().ToLowerInvariant();
    }

    public void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    public void DefinirInstrucoesSolicitante(string? instrucoesSolicitante)
    {
        InstrucoesSolicitante = string.IsNullOrWhiteSpace(instrucoesSolicitante) ? null : instrucoesSolicitante.Trim();
    }

    public void DefinirDepartamentoResponsavel(Guid departamentoResponsavelId)
    {
        if (departamentoResponsavelId == Guid.Empty)
        {
            throw new ArgumentException("O departamento responsavel e obrigatorio.", nameof(departamentoResponsavelId));
        }

        DepartamentoResponsavelId = departamentoResponsavelId;
    }

    public void DefinirCategoria(Guid? categoriaId)
    {
        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria informada e invalida.", nameof(categoriaId));
        }

        CategoriaId = categoriaId;
    }

    public void DefinirSubcategoria(Guid? subcategoriaId)
    {
        if (subcategoriaId == Guid.Empty)
        {
            throw new ArgumentException("A subcategoria informada e invalida.", nameof(subcategoriaId));
        }

        SubcategoriaId = subcategoriaId;
    }

    public void DefinirPrioridadePadrao(Guid? prioridadePadraoId)
    {
        if (prioridadePadraoId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade padrao informada e invalida.", nameof(prioridadePadraoId));
        }

        PrioridadePadraoId = prioridadePadraoId;
    }

    public void DefinirSlaPadrao(Guid? slaPadraoId)
    {
        if (slaPadraoId == Guid.Empty)
        {
            throw new ArgumentException("O SLA padrao informado e invalido.", nameof(slaPadraoId));
        }

        SlaPadraoId = slaPadraoId;
    }

    public void DefinirArtigoBaseConhecimento(Guid? artigoBaseConhecimentoId)
    {
        if (artigoBaseConhecimentoId == Guid.Empty)
        {
            throw new ArgumentException("O artigo de base de conhecimento informado e invalido.", nameof(artigoBaseConhecimentoId));
        }

        ArtigoBaseConhecimentoId = artigoBaseConhecimentoId;
    }

    public void DefinirVisibilidade(VisibilidadeCatalogoServico visibilidade)
    {
        if (!Enum.IsDefined(visibilidade))
        {
            throw new ArgumentException("A visibilidade informada e invalida.", nameof(visibilidade));
        }

        Visibilidade = visibilidade;
    }

    public void DefinirPermiteAberturaChamado(bool permiteAberturaChamado)
    {
        PermiteAberturaChamado = permiteAberturaChamado;
    }

    public void DefinirRequerAprovacao(bool requerAprovacao)
    {
        RequerAprovacao = requerAprovacao;
    }

    public void DefinirOrdem(int ordem)
    {
        if (ordem < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ordem), "A ordem do servico nao pode ser negativa.");
        }

        Ordem = ordem;
    }

    public void DefinirCriadoPorUsuario(Guid criadoPorUsuarioId)
    {
        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        CriadoPorUsuarioId = criadoPorUsuarioId;
    }

    public void Atualizar(
        string nome,
        string slug,
        string? descricao,
        string? instrucoesSolicitante,
        Guid departamentoResponsavelId,
        Guid? categoriaId,
        Guid? subcategoriaId,
        Guid? prioridadePadraoId,
        Guid? slaPadraoId,
        Guid? artigoBaseConhecimentoId,
        VisibilidadeCatalogoServico visibilidade,
        bool permiteAberturaChamado,
        bool requerAprovacao,
        int ordem,
        Guid atualizadoPorUsuarioId,
        string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirSlug(slug);
        DefinirDescricao(descricao);
        DefinirInstrucoesSolicitante(instrucoesSolicitante);
        DefinirDepartamentoResponsavel(departamentoResponsavelId);
        DefinirCategoria(categoriaId);
        DefinirSubcategoria(subcategoriaId);
        DefinirPrioridadePadrao(prioridadePadraoId);
        DefinirSlaPadrao(slaPadraoId);
        DefinirArtigoBaseConhecimento(artigoBaseConhecimentoId);
        DefinirVisibilidade(visibilidade);
        DefinirPermiteAberturaChamado(permiteAberturaChamado);
        DefinirRequerAprovacao(requerAprovacao);
        DefinirOrdem(ordem);
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void Publicar(Guid publicadoPorUsuarioId, string atualizadoPor)
    {
        if (publicadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario publicador e obrigatorio.", nameof(publicadoPorUsuarioId));
        }

        Status = StatusCatalogoServico.Publicado;
        PublicadoEm = DateTime.UtcNow;
        PublicadoPorUsuarioId = publicadoPorUsuarioId;
        Ativo = true;
        RegistrarAtualizacao(publicadoPorUsuarioId, atualizadoPor);
    }

    public void Arquivar(Guid arquivadoPorUsuarioId, string atualizadoPor)
    {
        if (arquivadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de arquivamento e obrigatorio.", nameof(arquivadoPorUsuarioId));
        }

        Status = StatusCatalogoServico.Arquivado;
        ArquivadoEm = DateTime.UtcNow;
        ArquivadoPorUsuarioId = arquivadoPorUsuarioId;
        Ativo = false;
        RegistrarAtualizacao(arquivadoPorUsuarioId, atualizadoPor);
    }

    public void TornarRascunho(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        Status = StatusCatalogoServico.Rascunho;
        Ativo = true;
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
    }

    private void RegistrarAtualizacao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }
}
