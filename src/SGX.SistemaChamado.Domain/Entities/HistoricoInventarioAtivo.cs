using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class HistoricoInventarioAtivo : CreationAuditableEntity
{
    public Guid InventarioAtivoId { get; private set; }
    public TipoMovimentacaoAtivo TipoMovimentacao { get; private set; }
    public Guid? DepartamentoOrigemId { get; private set; }
    public Guid? DepartamentoDestinoId { get; private set; }
    public Guid? LocalUnidadeOrigemId { get; private set; }
    public Guid? LocalUnidadeDestinoId { get; private set; }
    public Guid? UsuarioResponsavelOrigemId { get; private set; }
    public Guid? UsuarioResponsavelDestinoId { get; private set; }
    public StatusOperacionalAtivo? StatusOperacionalAnterior { get; private set; }
    public StatusOperacionalAtivo? StatusOperacionalNovo { get; private set; }
    public StatusPatrimonialAtivo? StatusPatrimonialAnterior { get; private set; }
    public StatusPatrimonialAtivo? StatusPatrimonialNovo { get; private set; }
    public string? Observacao { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }

    public InventarioAtivo InventarioAtivo { get; private set; } = default!;
    public Departamento? DepartamentoOrigem { get; private set; }
    public Departamento? DepartamentoDestino { get; private set; }
    public LocalUnidade? LocalUnidadeOrigem { get; private set; }
    public LocalUnidade? LocalUnidadeDestino { get; private set; }
    public Usuario? UsuarioResponsavelOrigem { get; private set; }
    public Usuario? UsuarioResponsavelDestino { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;

    private HistoricoInventarioAtivo()
    {
    }

    public HistoricoInventarioAtivo(
        Guid inventarioAtivoId,
        TipoMovimentacaoAtivo tipoMovimentacao,
        Guid criadoPorUsuarioId,
        string criadoPor,
        string? observacao = null)
    {
        if (inventarioAtivoId == Guid.Empty)
        {
            throw new ArgumentException("O ativo de inventario e obrigatorio.", nameof(inventarioAtivoId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador do historico e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        InventarioAtivoId = inventarioAtivoId;
        TipoMovimentacao = tipoMovimentacao;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirObservacao(observacao);
        DefinirCriacao(criadoPor);
    }

    public void DefinirDepartamentos(Guid? departamentoOrigemId, Guid? departamentoDestinoId)
    {
        if (departamentoOrigemId == Guid.Empty || departamentoDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Departamento informado e invalido.");
        }

        DepartamentoOrigemId = departamentoOrigemId;
        DepartamentoDestinoId = departamentoDestinoId;
    }

    public void DefinirLocaisUnidade(Guid? localUnidadeOrigemId, Guid? localUnidadeDestinoId)
    {
        if (localUnidadeOrigemId == Guid.Empty || localUnidadeDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Local/unidade informado e invalido.");
        }

        LocalUnidadeOrigemId = localUnidadeOrigemId;
        LocalUnidadeDestinoId = localUnidadeDestinoId;
    }

    public void DefinirUsuariosResponsaveis(Guid? usuarioResponsavelOrigemId, Guid? usuarioResponsavelDestinoId)
    {
        if (usuarioResponsavelOrigemId == Guid.Empty || usuarioResponsavelDestinoId == Guid.Empty)
        {
            throw new ArgumentException("Usuario responsavel informado e invalido.");
        }

        UsuarioResponsavelOrigemId = usuarioResponsavelOrigemId;
        UsuarioResponsavelDestinoId = usuarioResponsavelDestinoId;
    }

    public void DefinirStatusOperacional(StatusOperacionalAtivo? statusOperacionalAnterior, StatusOperacionalAtivo? statusOperacionalNovo)
    {
        StatusOperacionalAnterior = statusOperacionalAnterior;
        StatusOperacionalNovo = statusOperacionalNovo;
    }

    public void DefinirStatusPatrimonial(StatusPatrimonialAtivo? statusPatrimonialAnterior, StatusPatrimonialAtivo? statusPatrimonialNovo)
    {
        StatusPatrimonialAnterior = statusPatrimonialAnterior;
        StatusPatrimonialNovo = statusPatrimonialNovo;
    }

    public void DefinirObservacao(string? observacao)
    {
        Observacao = string.IsNullOrWhiteSpace(observacao) ? null : observacao.Trim();
    }
}
