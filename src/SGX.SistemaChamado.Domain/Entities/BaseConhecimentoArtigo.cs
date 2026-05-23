using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class BaseConhecimentoArtigo : AuditableEntity
{
    public string Titulo { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string? Resumo { get; private set; }
    public string Conteudo { get; private set; } = string.Empty;
    public Guid? CategoriaId { get; private set; }
    public StatusArtigoConhecimento Status { get; private set; } = StatusArtigoConhecimento.Rascunho;
    public VisibilidadeArtigoConhecimento Visibilidade { get; private set; } = VisibilidadeArtigoConhecimento.Solicitante;
    public string? Tags { get; private set; }
    public DateTime? PublicadoEm { get; private set; }
    public Guid? PublicadoPorUsuarioId { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }
    public DateTime? ArquivadoEm { get; private set; }
    public Guid? ArquivadoPorUsuarioId { get; private set; }

    public CategoriaChamado? Categoria { get; private set; }
    public ICollection<ChamadoArtigoConhecimento> ChamadosVinculados { get; private set; } = [];

    private BaseConhecimentoArtigo()
    {
    }

    public BaseConhecimentoArtigo(
        string titulo,
        string slug,
        string? resumo,
        string conteudo,
        Guid? categoriaId,
        StatusArtigoConhecimento status,
        VisibilidadeArtigoConhecimento visibilidade,
        string? tags,
        Guid criadoPorUsuarioId,
        string criadoPor)
    {
        DefinirTitulo(titulo);
        DefinirSlug(slug);
        DefinirResumo(resumo);
        DefinirConteudo(conteudo);
        DefinirCategoria(categoriaId);
        DefinirStatus(status);
        DefinirVisibilidade(visibilidade);
        DefinirTags(tags);
        DefinirCriadoPorUsuario(criadoPorUsuarioId);
        DefinirCriacao(criadoPor);

        if (status == StatusArtigoConhecimento.Publicado)
        {
            PublicadoEm = DateTime.UtcNow;
            PublicadoPorUsuarioId = criadoPorUsuarioId;
        }
        else if (status == StatusArtigoConhecimento.Arquivado)
        {
            ArquivadoEm = DateTime.UtcNow;
            ArquivadoPorUsuarioId = criadoPorUsuarioId;
            Ativo = false;
        }
    }

    public void AtualizarDados(
        string titulo,
        string slug,
        string? resumo,
        string conteudo,
        Guid? categoriaId,
        VisibilidadeArtigoConhecimento visibilidade,
        string? tags,
        Guid atualizadoPorUsuarioId,
        string atualizadoPor)
    {
        DefinirTitulo(titulo);
        DefinirSlug(slug);
        DefinirResumo(resumo);
        DefinirConteudo(conteudo);
        DefinirCategoria(categoriaId);
        DefinirVisibilidade(visibilidade);
        DefinirTags(tags);
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void DefinirTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O titulo do artigo e obrigatorio.", nameof(titulo));
        }

        Titulo = titulo.Trim();
    }

    public void DefinirSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("O slug do artigo e obrigatorio.", nameof(slug));
        }

        Slug = slug.Trim().ToLowerInvariant();
    }

    public void DefinirResumo(string? resumo)
    {
        Resumo = string.IsNullOrWhiteSpace(resumo) ? null : resumo.Trim();
    }

    public void DefinirConteudo(string conteudo)
    {
        if (string.IsNullOrWhiteSpace(conteudo))
        {
            throw new ArgumentException("O conteudo do artigo e obrigatorio.", nameof(conteudo));
        }

        Conteudo = conteudo.Trim();
    }

    public void DefinirCategoria(Guid? categoriaId)
    {
        if (categoriaId == Guid.Empty)
        {
            throw new ArgumentException("A categoria informada e invalida.", nameof(categoriaId));
        }

        CategoriaId = categoriaId;
    }

    public void DefinirStatus(StatusArtigoConhecimento status)
    {
        if (!Enum.IsDefined(status))
        {
            throw new ArgumentException("Status do artigo invalido.", nameof(status));
        }

        Status = status;
    }

    public void DefinirVisibilidade(VisibilidadeArtigoConhecimento visibilidade)
    {
        if (!Enum.IsDefined(visibilidade))
        {
            throw new ArgumentException("Visibilidade do artigo invalida.", nameof(visibilidade));
        }

        Visibilidade = visibilidade;
    }

    public void DefinirTags(string? tags)
    {
        Tags = string.IsNullOrWhiteSpace(tags) ? null : tags.Trim();
    }

    public void DefinirCriadoPorUsuario(Guid criadoPorUsuarioId)
    {
        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        CriadoPorUsuarioId = criadoPorUsuarioId;
    }

    public void Publicar(Guid publicadoPorUsuarioId, string atualizadoPor)
    {
        if (publicadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario publicador e obrigatorio.", nameof(publicadoPorUsuarioId));
        }

        Status = StatusArtigoConhecimento.Publicado;
        PublicadoEm = DateTime.UtcNow;
        PublicadoPorUsuarioId = publicadoPorUsuarioId;
        Ativo = true;
        RegistrarAtualizacao(publicadoPorUsuarioId, atualizadoPor);
    }

    public void ColocarEmRevisao(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusArtigoConhecimento.EmRevisao;
        Ativo = true;
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void TornarRascunho(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        Status = StatusArtigoConhecimento.Rascunho;
        Ativo = true;
        RegistrarAtualizacao(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void Arquivar(Guid arquivadoPorUsuarioId, string atualizadoPor)
    {
        if (arquivadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de arquivamento e obrigatorio.", nameof(arquivadoPorUsuarioId));
        }

        Status = StatusArtigoConhecimento.Arquivado;
        ArquivadoEm = DateTime.UtcNow;
        ArquivadoPorUsuarioId = arquivadoPorUsuarioId;
        Ativo = false;
        RegistrarAtualizacao(arquivadoPorUsuarioId, atualizadoPor);
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
