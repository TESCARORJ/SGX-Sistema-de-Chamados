using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ChamadoRelacionamento : AuditableEntity
{
    private const int MaximoJustificativa = 2000;
    private const int MaximoMotivoRemocao = 1000;

    public Guid ChamadoOrigemId { get; private set; }
    public Guid ChamadoDestinoId { get; private set; }
    public TipoRelacionamentoChamadoEnum TipoRelacionamento { get; private set; }
    public string? Justificativa { get; private set; }
    public Guid CriadoPorUsuarioId { get; private set; }
    public DateTime? RemovidoEm { get; private set; }
    public Guid? RemovidoPorUsuarioId { get; private set; }
    public string? MotivoRemocao { get; private set; }

    public Chamado ChamadoOrigem { get; private set; } = default!;
    public Chamado ChamadoDestino { get; private set; } = default!;
    public Usuario CriadoPorUsuario { get; private set; } = default!;
    public Usuario? RemovidoPorUsuario { get; private set; }

    private ChamadoRelacionamento()
    {
    }

    public ChamadoRelacionamento(
        Guid chamadoOrigemId,
        Guid chamadoDestinoId,
        TipoRelacionamentoChamadoEnum tipoRelacionamento,
        Guid criadoPorUsuarioId,
        string criadoPor,
        string? justificativa = null)
    {
        if (chamadoOrigemId == Guid.Empty)
        {
            throw new ArgumentException("O chamado de origem e obrigatorio.", nameof(chamadoOrigemId));
        }

        if (chamadoDestinoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado de destino e obrigatorio.", nameof(chamadoDestinoId));
        }

        if (chamadoOrigemId == chamadoDestinoId)
        {
            throw new ArgumentException("O chamado de origem nao pode ser igual ao chamado de destino.", nameof(chamadoDestinoId));
        }

        if (!Enum.IsDefined(tipoRelacionamento))
        {
            throw new ArgumentException("O tipo de relacionamento informado e invalido.", nameof(tipoRelacionamento));
        }

        if (criadoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario criador do relacionamento e obrigatorio.", nameof(criadoPorUsuarioId));
        }

        ChamadoOrigemId = chamadoOrigemId;
        ChamadoDestinoId = chamadoDestinoId;
        TipoRelacionamento = tipoRelacionamento;
        CriadoPorUsuarioId = criadoPorUsuarioId;
        Justificativa = NormalizarTexto(justificativa, MaximoJustificativa, nameof(justificativa));
        DefinirCriacao(criadoPor);
    }

    public void Inativar(Guid removidoPorUsuarioId, string atualizadoPor, string? motivoRemocao = null)
    {
        if (!Ativo)
        {
            throw new InvalidOperationException("O relacionamento informado ja esta inativo.");
        }

        if (removidoPorUsuarioId == Guid.Empty)
        {
            throw new ArgumentException("O usuario de inativacao do relacionamento e obrigatorio.", nameof(removidoPorUsuarioId));
        }

        RemovidoEm = DateTime.UtcNow;
        RemovidoPorUsuarioId = removidoPorUsuarioId;
        MotivoRemocao = NormalizarTexto(motivoRemocao, MaximoMotivoRemocao, nameof(motivoRemocao));
        Desativar(atualizadoPor);
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
