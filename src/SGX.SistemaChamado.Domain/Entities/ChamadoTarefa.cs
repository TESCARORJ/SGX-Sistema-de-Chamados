using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ChamadoTarefa : AuditableEntity
{
    private const int MaximoTitulo = 200;
    private const int MaximoDescricao = 4000;
    private const int MaximoMotivoCancelamento = 1000;

    public Guid ChamadoId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public StatusTarefaChamadoEnum Status { get; private set; }
    public Guid? ResponsavelUsuarioId { get; private set; }
    public DateTime? Prazo { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public DateTime? ConcluidoEm { get; private set; }
    public Guid? ConcluidoPorUsuarioId { get; private set; }
    public DateTime? CanceladoEm { get; private set; }
    public Guid? CanceladoPorUsuarioId { get; private set; }
    public string? MotivoCancelamento { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public Usuario? ResponsavelUsuario { get; private set; }
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? ConcluidoPorUsuario { get; private set; }
    public Usuario? CanceladoPorUsuario { get; private set; }

    private ChamadoTarefa()
    {
    }

    public ChamadoTarefa(
        Guid chamadoId,
        string titulo,
        string? descricao,
        Guid? responsavelUsuarioId,
        DateTime? prazo,
        Guid criadoPorUsuarioId,
        string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado da tarefa e obrigatorio.", nameof(chamadoId));
        }

        if (responsavelUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O responsavel informado e invalido.", nameof(responsavelUsuarioId));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador da tarefa e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        ChamadoId = chamadoId;
        Titulo = NormalizarObrigatorio(titulo, MaximoTitulo, "O titulo da tarefa e obrigatorio.", nameof(titulo));
        Descricao = NormalizarOpcional(descricao, MaximoDescricao, nameof(descricao));
        Status = StatusTarefaChamadoEnum.Pendente;
        ResponsavelUsuarioId = responsavelUsuarioId;
        Prazo = prazo;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        DefinirCriacao(criadoPor);
    }

    public void AlterarStatus(StatusTarefaChamadoEnum novoStatus, Guid usuarioId, string atualizadoPor)
    {
        if (!Enum.IsDefined(novoStatus))
        {
            throw new ArgumentException("O status da tarefa informado e invalido.", nameof(novoStatus));
        }

        if (novoStatus == StatusTarefaChamadoEnum.Cancelada)
        {
            throw new InvalidOperationException("Use o fluxo de cancelamento para cancelar a tarefa.");
        }

        if (!Ativo)
        {
            throw new InvalidOperationException("Tarefa inativa nao pode ter status alterado.");
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de atualizacao da tarefa e obrigatorio.", nameof(usuarioId));
        }

        Status = novoStatus;
        if (novoStatus == StatusTarefaChamadoEnum.Concluida)
        {
            ConcluidoEm = DateTime.UtcNow;
            ConcluidoPorUsuarioId = usuarioId;
        }
        else
        {
            ConcluidoEm = null;
            ConcluidoPorUsuarioId = null;
        }

        AtualizarAuditoria(atualizadoPor);
    }

    public void Cancelar(Guid usuarioId, string atualizadoPor, string? motivoCancelamento = null)
    {
        if (!Ativo)
        {
            throw new InvalidOperationException("Tarefa ja esta inativa.");
        }

        if (usuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de cancelamento da tarefa e obrigatorio.", nameof(usuarioId));
        }

        Status = StatusTarefaChamadoEnum.Cancelada;
        CanceladoEm = DateTime.UtcNow;
        CanceladoPorUsuarioId = usuarioId;
        MotivoCancelamento = NormalizarOpcional(motivoCancelamento, MaximoMotivoCancelamento, nameof(motivoCancelamento));
        Desativar(atualizadoPor);
    }

    private static string NormalizarObrigatorio(string valor, int tamanhoMaximo, string mensagemObrigatorio, string paramName)
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

    private static string? NormalizarOpcional(string? valor, int tamanhoMaximo, string paramName)
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
