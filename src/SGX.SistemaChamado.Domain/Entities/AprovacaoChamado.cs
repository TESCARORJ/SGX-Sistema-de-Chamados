using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class AprovacaoChamado : AuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public StatusAprovacaoChamado Status { get; private set; } = StatusAprovacaoChamado.Pendente;
    public TipoOrigemAprovacaoChamado TipoOrigem { get; private set; }
    public string? OrigemDescricao { get; private set; }
    public Guid? SolicitanteId { get; private set; }
    public Guid? AprovadorId { get; private set; }
    public string? JustificativaSolicitacao { get; private set; }
    public string? JustificativaDecisao { get; private set; }
    public DateTime SolicitadaEm { get; private set; }
    public DateTime? DecididaEm { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario? Solicitante { get; private set; }
    public Usuario? Aprovador { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? AtualizadoPorUsuario { get; private set; }

    private AprovacaoChamado()
    {
    }

    public AprovacaoChamado(
        Guid chamadoId,
        TipoOrigemAprovacaoChamado tipoOrigem,
        Guid criadoPorUsuarioId,
        string criadoPor,
        Guid? solicitanteId = null,
        string? origemDescricao = null,
        string? justificativaSolicitacao = null)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado da aprovacao e obrigatorio.", nameof(chamadoId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da aprovacao e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        if (solicitanteId == Guid.Empty)
        {
            throw new ArgumentException("O solicitante informado para aprovacao e invalido.", nameof(solicitanteId));
        }

        ChamadoId = chamadoId;
        TipoOrigem = tipoOrigem;
        SolicitanteId = solicitanteId;
        OrigemDescricao = NormalizarTexto(origemDescricao);
        JustificativaSolicitacao = NormalizarTexto(justificativaSolicitacao);
        SolicitadaEm = DateTime.UtcNow;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void DefinirAprovador(Guid? aprovadorId, Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (aprovadorId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador informado e invalido.", nameof(aprovadorId));
        }

        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
        AprovadorId = aprovadorId;
    }

    public void Aprovar(Guid aprovadorId, Guid atualizadoPorUsuarioId, string atualizadoPor, string? justificativaDecisao = null)
        => RegistrarDecisao(StatusAprovacaoChamado.Aprovado, aprovadorId, atualizadoPorUsuarioId, atualizadoPor, justificativaDecisao);

    public void Reprovar(Guid aprovadorId, Guid atualizadoPorUsuarioId, string atualizadoPor, string? justificativaDecisao = null)
        => RegistrarDecisao(StatusAprovacaoChamado.Reprovado, aprovadorId, atualizadoPorUsuarioId, atualizadoPor, justificativaDecisao);

    public void Cancelar(Guid atualizadoPorUsuarioId, string atualizadoPor, string? justificativaDecisao = null)
    {
        if (Status != StatusAprovacaoChamado.Pendente)
        {
            throw new InvalidOperationException("Somente aprovacoes pendentes podem ser canceladas.");
        }

        Status = StatusAprovacaoChamado.Cancelado;
        DecididaEm = DateTime.UtcNow;
        JustificativaDecisao = NormalizarTexto(justificativaDecisao);
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    private void RegistrarDecisao(
        StatusAprovacaoChamado status,
        Guid aprovadorId,
        Guid atualizadoPorUsuarioId,
        string atualizadoPor,
        string? justificativaDecisao)
    {
        if (Status != StatusAprovacaoChamado.Pendente)
        {
            throw new InvalidOperationException("Somente aprovacoes pendentes podem ser decididas.");
        }

        if (aprovadorId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador informado e obrigatorio.", nameof(aprovadorId));
        }

        AprovadorId = aprovadorId;
        Status = status;
        DecididaEm = DateTime.UtcNow;
        JustificativaDecisao = NormalizarTexto(justificativaDecisao);
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    private void AtualizarAuditoriaUsuario(Guid atualizadoPorUsuarioId, string atualizadoPor)
    {
        if (atualizadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da aprovacao e obrigatorio.", nameof(atualizadoPorUsuarioId));
        }

        AtualizadoPorUsuarioId = atualizadoPorUsuarioId;
        AtualizarAuditoria(atualizadoPor);
    }

    private static string? NormalizarTexto(string? valor)
        => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
