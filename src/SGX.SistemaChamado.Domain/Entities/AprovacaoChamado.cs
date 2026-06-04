using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class AprovacaoChamado : AuditableEntity
{
    private const int MaximoTitulo = 200;
    private const int MaximoDescricao = 4000;
    private const int MaximoJustificativa = 4000;
    private const int MaximoMotivoCancelamento = 1000;

    public Guid ChamadoId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public StatusAprovacaoChamado Status { get; private set; } = StatusAprovacaoChamado.Pendente;
    public TipoOrigemAprovacaoChamado TipoOrigem { get; private set; }
    public bool BloqueiaAvancoAtendimento { get; private set; } = true;
    public string? OrigemDescricao { get; private set; }
    public Guid? SolicitanteId { get; private set; }
    public Guid? AprovadorId { get; private set; }
    public string? JustificativaSolicitacao { get; private set; }
    public string? JustificativaDecisao { get; private set; }
    public DateTime SolicitadaEm { get; private set; }
    public DateTime? DecididaEm { get; private set; }
    public DateTime? CanceladoEm { get; private set; }
    public Guid? CanceladoPorUsuarioId { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario? Solicitante { get; private set; }
    public Usuario? Aprovador { get; private set; }
    public Usuario? CanceladoPorUsuario { get; private set; }
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
        string? justificativaSolicitacao = null,
        string? titulo = null,
        string? descricao = null,
        Guid? aprovadorId = null,
        bool bloqueiaAvancoAtendimento = true)
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

        if (aprovadorId == Guid.Empty)
        {
            throw new ArgumentException("O aprovador informado e invalido.", nameof(aprovadorId));
        }

        ChamadoId = chamadoId;
        Titulo = string.IsNullOrWhiteSpace(titulo)
            ? CriarTituloPadrao(tipoOrigem, origemDescricao)
            : NormalizarTextoObrigatorio(titulo, MaximoTitulo, "O titulo da aprovacao e obrigatorio.", nameof(titulo));
        Descricao = NormalizarTexto(descricao, MaximoDescricao, nameof(descricao));
        TipoOrigem = tipoOrigem;
        BloqueiaAvancoAtendimento = bloqueiaAvancoAtendimento;
        SolicitanteId = solicitanteId;
        AprovadorId = aprovadorId;
        OrigemDescricao = NormalizarTexto(origemDescricao, 300, nameof(origemDescricao));
        JustificativaSolicitacao = NormalizarTexto(justificativaSolicitacao, MaximoJustificativa, nameof(justificativaSolicitacao));
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
        CanceladoEm = DecididaEm;
        CanceladoPorUsuarioId = atualizadoPorUsuarioId;
        JustificativaDecisao = NormalizarTexto(justificativaDecisao, MaximoJustificativa, nameof(justificativaDecisao));
        MotivoCancelamento = JustificativaDecisao;
        AtualizarAuditoriaUsuario(atualizadoPorUsuarioId, atualizadoPor);
    }

    public void CancelarVinculada(Guid canceladoPorUsuarioId, string atualizadoPor, string? motivoCancelamento = null)
    {
        if (Status != StatusAprovacaoChamado.Pendente)
        {
            throw new InvalidOperationException("Somente aprovacoes pendentes podem ser canceladas.");
        }

        if (canceladoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de cancelamento da aprovacao e obrigatorio.", nameof(canceladoPorUsuarioId));
        }

        Status = StatusAprovacaoChamado.Cancelado;
        DecididaEm = DateTime.UtcNow;
        CanceladoEm = DecididaEm;
        CanceladoPorUsuarioId = canceladoPorUsuarioId;
        MotivoCancelamento = NormalizarTexto(motivoCancelamento, MaximoMotivoCancelamento, nameof(motivoCancelamento));
        JustificativaDecisao = MotivoCancelamento;
        Desativar(atualizadoPor);
        AtualizadoPorUsuarioId = canceladoPorUsuarioId;
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
        JustificativaDecisao = NormalizarTexto(justificativaDecisao, MaximoJustificativa, nameof(justificativaDecisao));
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

    private static string CriarTituloPadrao(TipoOrigemAprovacaoChamado tipoOrigem, string? origemDescricao)
        => string.IsNullOrWhiteSpace(origemDescricao)
            ? $"Aprovacao {tipoOrigem}"
            : origemDescricao.Trim();

    private static string NormalizarTextoObrigatorio(string? valor, int tamanhoMaximo, string mensagemObrigatorio, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ArgumentException(mensagemObrigatorio, paramName);
        }

        var textoNormalizado = valor.Trim();
        if (textoNormalizado.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {tamanhoMaximo} caracteres.", paramName);
        }

        return textoNormalizado;
    }

    private static string? NormalizarTexto(string? valor, int tamanhoMaximo, string paramName)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        var textoNormalizado = valor.Trim();
        if (textoNormalizado.Length > tamanhoMaximo)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {tamanhoMaximo} caracteres.", paramName);
        }

        return textoNormalizado;
    }
}
