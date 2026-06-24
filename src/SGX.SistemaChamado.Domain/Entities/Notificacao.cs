using SGX.SistemaChamado.Domain.Abstractions;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Domain.ValueObjects;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class Notificacao : AuditableEntity
{
    public const int MaximoDestinatarioEndereco = 320;
    public const int MaximoAssunto = 300;
    public const int MaximoConteudo = 10000;
    public const int MaximoChaveCorrelacao = 200;
    public const int MaximoChaveIdempotencia = 200;
    public const int MaximoUltimoErro = 2000;
    public const int MaximoMotivoCancelamento = 1000;

    public Guid? ChamadoId { get; private set; }
    public TipoEventoNotificacao TipoEvento { get; private set; }
    public CanalNotificacao Canal { get; private set; }
    public StatusNotificacao Status { get; private set; } = StatusNotificacao.Pendente;
    public Guid? DestinatarioUsuarioId { get; private set; }
    public string? DestinatarioEndereco { get; private set; }
    public string? Assunto { get; private set; }
    public string Conteudo { get; private set; } = string.Empty;
    public string? ChaveCorrelacao { get; private set; }
    public string ChaveIdempotencia { get; private set; } = string.Empty;
    public DateTime? AgendadaEm { get; private set; }
    public DateTime? ProcessadaEm { get; private set; }
    public DateTime? EnviadaEm { get; private set; }
    public DateTime? LidaEm { get; private set; }
    public DateTime? FalhouEm { get; private set; }
    public DateTime? CanceladaEm { get; private set; }
    public int QuantidadeTentativas { get; private set; }
    public string? UltimoErro { get; private set; }
    public string? MotivoCancelamento { get; private set; }
    public Guid? CriadoPorUsuarioId { get; private set; }
    public Guid? AtualizadoPorUsuarioId { get; private set; }

    public Chamado? Chamado { get; private set; }
    public Usuario? DestinatarioUsuario { get; private set; }
    public Usuario? CriadoPorUsuario { get; private set; }
    public Usuario? AtualizadoPorUsuario { get; private set; }
    public bool Lida => LidaEm.HasValue;

    private Notificacao()
    {
    }

    public Notificacao(
        TipoEventoNotificacao tipoEvento,
        CanalNotificacao canal,
        string conteudo,
        string chaveIdempotencia,
        string criadoPor,
        Guid? destinatarioUsuarioId = null,
        string? destinatarioEndereco = null,
        Guid? chamadoId = null,
        string? assunto = null,
        string? chaveCorrelacao = null,
        Guid? criadoPorUsuarioId = null)
    {
        ValidarEnum(tipoEvento, nameof(tipoEvento), "O tipo de evento da notificacao e invalido.");
        ValidarEnum(canal, nameof(canal), "O canal da notificacao e invalido.");

        ChamadoId = ValidarGuidOpcional(chamadoId, nameof(chamadoId), "O chamado informado e invalido.");
        DestinatarioUsuarioId = ValidarGuidOpcional(destinatarioUsuarioId, nameof(destinatarioUsuarioId), "O destinatario usuario informado e invalido.");
        DestinatarioEndereco = NormalizarDestinatarioEndereco(destinatarioEndereco);
        ValidarDestinatario();

        TipoEvento = tipoEvento;
        Canal = canal;
        Assunto = NormalizarTextoOpcional(assunto, MaximoAssunto, nameof(assunto));
        Conteudo = NormalizarTextoObrigatorio(conteudo, MaximoConteudo, "O conteudo da notificacao e obrigatorio.", nameof(conteudo));
        ChaveCorrelacao = NormalizarTextoOpcional(chaveCorrelacao, MaximoChaveCorrelacao, nameof(chaveCorrelacao));
        ChaveIdempotencia = NormalizarTextoObrigatorio(
            chaveIdempotencia,
            MaximoChaveIdempotencia,
            "A chave de idempotencia da notificacao e obrigatoria.",
            nameof(chaveIdempotencia));
        Status = StatusNotificacao.Pendente;
        QuantidadeTentativas = 0;
        CriadoPorUsuarioId = ValidarGuidOpcional(criadoPorUsuarioId, nameof(criadoPorUsuarioId), "O usuario criador da notificacao e invalido.");
        DefinirCriacao(criadoPor);
    }

    public void Agendar(DateTime agendadaEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status != StatusNotificacao.Pendente)
        {
            throw new InvalidOperationException("Somente notificacoes pendentes podem ser agendadas.");
        }

        Status = StatusNotificacao.Agendada;
        AgendadaEm = NormalizarDataUtcObrigatoria(agendadaEm, nameof(agendadaEm));
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void ReagendarAposFalha(DateTime agendadaEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status != StatusNotificacao.Falhou)
        {
            throw new InvalidOperationException("Somente notificacoes com falha podem ser reagendadas.");
        }

        Status = StatusNotificacao.Agendada;
        AgendadaEm = NormalizarDataUtcObrigatoria(agendadaEm, nameof(agendadaEm));
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void IniciarProcessamento(DateTime processadaEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status is not StatusNotificacao.Pendente and not StatusNotificacao.Agendada and not StatusNotificacao.Falhou)
        {
            throw new InvalidOperationException("Somente notificacoes pendentes, agendadas ou com falha podem iniciar processamento.");
        }

        Status = StatusNotificacao.EmProcessamento;
        ProcessadaEm = NormalizarDataUtcObrigatoria(processadaEm, nameof(processadaEm));
        QuantidadeTentativas++;
        FalhouEm = null;
        UltimoErro = null;
        MotivoCancelamento = null;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void RegistrarEnvio(DateTime enviadaEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status != StatusNotificacao.EmProcessamento)
        {
            throw new InvalidOperationException("Somente notificacoes em processamento podem ser marcadas como enviadas.");
        }

        Status = StatusNotificacao.Enviada;
        EnviadaEm = NormalizarDataUtcObrigatoria(enviadaEm, nameof(enviadaEm));
        LidaEm = null;
        AgendadaEm = null;
        FalhouEm = null;
        UltimoErro = null;
        MotivoCancelamento = null;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void MarcarComoLida(DateTime lidaEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        ValidarLeituraPermitida();
        if (LidaEm.HasValue)
        {
            return;
        }

        var lidaEmUtc = NormalizarDataUtcObrigatoria(lidaEm, nameof(lidaEm));
        if (lidaEmUtc < CriadoEm)
        {
            throw new InvalidOperationException("A leitura da notificacao nao pode ocorrer antes da criacao.");
        }

        if (EnviadaEm.HasValue && lidaEmUtc < EnviadaEm.Value)
        {
            throw new InvalidOperationException("A leitura da notificacao nao pode ocorrer antes da disponibilizacao.");
        }

        LidaEm = lidaEmUtc;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void MarcarComoNaoLida(string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        ValidarLeituraPermitida();
        if (!LidaEm.HasValue)
        {
            return;
        }

        LidaEm = null;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void RegistrarFalha(string erro, DateTime falhouEm, string atualizadoPor, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status != StatusNotificacao.EmProcessamento)
        {
            throw new InvalidOperationException("Somente notificacoes em processamento podem registrar falha.");
        }

        Status = StatusNotificacao.Falhou;
        FalhouEm = NormalizarDataUtcObrigatoria(falhouEm, nameof(falhouEm));
        AgendadaEm = null;
        UltimoErro = NormalizarTextoObrigatorio(erro, MaximoUltimoErro, "O erro da notificacao e obrigatorio.", nameof(erro));
        MotivoCancelamento = null;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public void Cancelar(DateTime canceladaEm, string atualizadoPor, string? motivo = null, Guid? atualizadoPorUsuarioId = null)
    {
        if (Status is StatusNotificacao.Enviada or StatusNotificacao.Cancelada or StatusNotificacao.EmProcessamento)
        {
            throw new InvalidOperationException("A notificacao nao pode ser cancelada no estado atual.");
        }

        Status = StatusNotificacao.Cancelada;
        CanceladaEm = NormalizarDataUtcObrigatoria(canceladaEm, nameof(canceladaEm));
        MotivoCancelamento = NormalizarTextoOpcional(motivo, MaximoMotivoCancelamento, nameof(motivo));
        UltimoErro = null;
        AtualizarAuditoriaInterna(atualizadoPor, atualizadoPorUsuarioId);
    }

    public bool EstaAgendadaPara(DateTime dataReferencia)
    {
        var dataReferenciaUtc = NormalizarDataUtcObrigatoria(dataReferencia, nameof(dataReferencia));
        return Status == StatusNotificacao.Agendada
            && AgendadaEm.HasValue
            && AgendadaEm.Value <= dataReferenciaUtc;
    }

    public bool EstaProcessavel(DateTime dataReferencia, int limiteTentativas)
    {
        var dataReferenciaUtc = NormalizarDataUtcObrigatoria(dataReferencia, nameof(dataReferencia));
        if (limiteTentativas <= 0)
        {
            throw new ArgumentException("O limite de tentativas deve ser maior que zero.", nameof(limiteTentativas));
        }

        if (!Ativo
            || Status is StatusNotificacao.Enviada or StatusNotificacao.Cancelada or StatusNotificacao.EmProcessamento or StatusNotificacao.Falhou
            || QuantidadeTentativas >= limiteTentativas)
        {
            return false;
        }

        return Status == StatusNotificacao.Pendente || EstaAgendadaPara(dataReferenciaUtc);
    }

    private void AtualizarAuditoriaInterna(string atualizadoPor, Guid? atualizadoPorUsuarioId)
    {
        AtualizadoPorUsuarioId = ValidarGuidOpcional(atualizadoPorUsuarioId, nameof(atualizadoPorUsuarioId), "O usuario de atualizacao da notificacao e invalido.");
        AtualizarAuditoria(atualizadoPor);
    }

    private void ValidarLeituraPermitida()
    {
        if (Canal != CanalNotificacao.Sistema)
        {
            throw new InvalidOperationException("Somente notificacoes do canal Sistema permitem controle de leitura.");
        }

        if (Status != StatusNotificacao.Enviada || !EnviadaEm.HasValue)
        {
            throw new InvalidOperationException("Somente notificacoes entregues no canal Sistema permitem controle de leitura.");
        }
    }

    private void ValidarDestinatario()
    {
        if (!DestinatarioUsuarioId.HasValue && string.IsNullOrWhiteSpace(DestinatarioEndereco))
        {
            throw new ArgumentException("A notificacao deve possuir destinatario por usuario ou endereco.", nameof(DestinatarioEndereco));
        }
    }

    private static void ValidarEnum<TEnum>(TEnum valor, string paramName, string mensagem)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(valor))
        {
            throw new ArgumentException(mensagem, paramName);
        }
    }

    private static Guid? ValidarGuidOpcional(Guid? valor, string paramName, string mensagem)
    {
        if (valor.HasValue && valor.Value == Guid.Empty)
        {
            throw new ArgumentException(mensagem, paramName);
        }

        return valor;
    }

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

    private static string? NormalizarTextoOpcional(string? valor, int tamanhoMaximo, string paramName)
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

    private static string? NormalizarDestinatarioEndereco(string? destinatarioEndereco)
    {
        if (string.IsNullOrWhiteSpace(destinatarioEndereco))
        {
            return null;
        }

        var endereco = EmailCorporativo.Criar(destinatarioEndereco).Valor;
        if (endereco.Length > MaximoDestinatarioEndereco)
        {
            throw new ArgumentException($"O valor informado deve possuir no maximo {MaximoDestinatarioEndereco} caracteres.", nameof(destinatarioEndereco));
        }

        return endereco;
    }

    private static DateTime NormalizarDataUtcObrigatoria(DateTime valor, string paramName)
    {
        if (valor == default)
        {
            throw new ArgumentException("A data informada e obrigatoria.", paramName);
        }

        return valor.Kind switch
        {
            DateTimeKind.Utc => valor,
            DateTimeKind.Local => valor.ToUniversalTime(),
            _ => DateTime.SpecifyKind(valor, DateTimeKind.Utc)
        };
    }
}
