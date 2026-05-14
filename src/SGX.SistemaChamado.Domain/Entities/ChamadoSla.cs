using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class ChamadoSla : AuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public Guid? PoliticaSlaId { get; private set; }
    public Guid PrioridadeId { get; private set; }
    public DateTime DataInicio { get; private set; }
    public DateTime PrazoPrimeiraResposta { get; private set; }
    public DateTime PrazoResolucao { get; private set; }
    public DateTime? DataPrimeiraResposta { get; private set; }
    public DateTime? DataResolucao { get; private set; }
    public bool? PrimeiraRespostaCumprida { get; private set; }
    public bool? ResolucaoCumprida { get; private set; }
    public bool PrimeiraRespostaViolada { get; private set; }
    public bool ResolucaoViolada { get; private set; }
    public int? MinutosPrimeiraResposta { get; private set; }
    public int? MinutosResolucao { get; private set; }
    public bool Pausado { get; private set; }
    public DateTime? DataPausa { get; private set; }
    public int MinutosPausados { get; private set; }
    public bool PausarQuandoAguardandoSolicitante { get; private set; }
    public bool UsarHorarioComercial { get; private set; }
    public Guid? CalendarioCorporativoId { get; private set; }

    public Chamado Chamado { get; private set; } = default!;
    public PoliticaSla? PoliticaSla { get; private set; }
    public PrioridadeChamado Prioridade { get; private set; } = default!;
    public CalendarioCorporativo? CalendarioCorporativo { get; private set; }

    private ChamadoSla()
    {
    }

    public ChamadoSla(
        Guid chamadoId,
        Guid? politicaSlaId,
        Guid prioridadeId,
        DateTime dataInicio,
        DateTime prazoPrimeiraResposta,
        DateTime prazoResolucao,
        bool pausarQuandoAguardandoSolicitante,
        bool usarHorarioComercial,
        Guid? calendarioCorporativoId,
        string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado do SLA e obrigatorio.", nameof(chamadoId));
        }

        if (prioridadeId == Guid.Empty)
        {
            throw new ArgumentException("A prioridade do SLA e obrigatoria.", nameof(prioridadeId));
        }

        ChamadoId = chamadoId;
        PoliticaSlaId = politicaSlaId;
        PrioridadeId = prioridadeId;
        DataInicio = dataInicio;
        PrazoPrimeiraResposta = prazoPrimeiraResposta;
        PrazoResolucao = prazoResolucao;
        PausarQuandoAguardandoSolicitante = pausarQuandoAguardandoSolicitante;
        UsarHorarioComercial = usarHorarioComercial;
        CalendarioCorporativoId = calendarioCorporativoId;
        DefinirCriacao(criadoPor);
    }

    public void AtualizarPrazos(
        Guid? politicaSlaId,
        Guid prioridadeId,
        DateTime dataBase,
        DateTime prazoPrimeiraResposta,
        DateTime prazoResolucao,
        bool pausarQuandoAguardandoSolicitante,
        bool usarHorarioComercial,
        Guid? calendarioCorporativoId,
        string atualizadoPor)
    {
        PoliticaSlaId = politicaSlaId;
        PrioridadeId = prioridadeId;
        DataInicio = dataBase;
        PausarQuandoAguardandoSolicitante = pausarQuandoAguardandoSolicitante;
        UsarHorarioComercial = usarHorarioComercial;
        CalendarioCorporativoId = calendarioCorporativoId;

        if (!DataPrimeiraResposta.HasValue)
        {
            PrazoPrimeiraResposta = prazoPrimeiraResposta;
            PrimeiraRespostaCumprida = null;
            PrimeiraRespostaViolada = false;
            MinutosPrimeiraResposta = null;
        }

        if (!DataResolucao.HasValue)
        {
            PrazoResolucao = prazoResolucao;
            ResolucaoCumprida = null;
            ResolucaoViolada = false;
            MinutosResolucao = null;
        }

        AtualizarAuditoria(atualizadoPor);
    }

    public void RegistrarPrimeiraResposta(DateTime respostaEmUtc, string atualizadoPor)
        => RegistrarPrimeiraResposta(respostaEmUtc, CalcularMinutosDecorridos(respostaEmUtc), atualizadoPor);

    public void RegistrarPrimeiraResposta(DateTime respostaEmUtc, int minutosDecorridos, string atualizadoPor)
    {
        if (DataPrimeiraResposta.HasValue)
        {
            return;
        }

        DataPrimeiraResposta = respostaEmUtc;
        MinutosPrimeiraResposta = Math.Max(0, minutosDecorridos);

        var cumprida = respostaEmUtc <= PrazoPrimeiraResposta;
        PrimeiraRespostaCumprida = cumprida;
        PrimeiraRespostaViolada = !cumprida;
        AtualizarAuditoria(atualizadoPor);
    }

    public void RegistrarResolucao(DateTime resolucaoEmUtc, string atualizadoPor)
        => RegistrarResolucao(resolucaoEmUtc, CalcularMinutosDecorridos(resolucaoEmUtc), atualizadoPor);

    public void RegistrarResolucao(DateTime resolucaoEmUtc, int minutosDecorridos, string atualizadoPor)
    {
        if (DataResolucao.HasValue)
        {
            return;
        }

        DataResolucao = resolucaoEmUtc;
        MinutosResolucao = Math.Max(0, minutosDecorridos);

        var cumprida = resolucaoEmUtc <= PrazoResolucao;
        ResolucaoCumprida = cumprida;
        ResolucaoViolada = !cumprida;
        AtualizarAuditoria(atualizadoPor);
    }

    public void IniciarPausa(DateTime pausadoEmUtc, string atualizadoPor)
    {
        if (Pausado || !PausarQuandoAguardandoSolicitante)
        {
            return;
        }

        Pausado = true;
        DataPausa = pausadoEmUtc;
        AtualizarAuditoria(atualizadoPor);
    }

    public void FinalizarPausa(DateTime retomadoEmUtc, string atualizadoPor)
        => FinalizarPausa(retomadoEmUtc, CalcularMinutosPausaCorridos(retomadoEmUtc), atualizadoPor);

    public void FinalizarPausa(DateTime retomadoEmUtc, int minutosPausa, string atualizadoPor)
    {
        if (!Pausado || DataPausa is null)
        {
            return;
        }

        var pausaTotal = Math.Max(0, minutosPausa);
        if (pausaTotal > 0)
        {
            MinutosPausados += pausaTotal;

            if (!DataPrimeiraResposta.HasValue)
            {
                PrazoPrimeiraResposta = PrazoPrimeiraResposta.AddMinutes(pausaTotal);
            }

            if (!DataResolucao.HasValue)
            {
                PrazoResolucao = PrazoResolucao.AddMinutes(pausaTotal);
            }
        }

        Pausado = false;
        DataPausa = null;
        AtualizarAuditoria(atualizadoPor);
    }

    public void Reabrir(
        Guid? politicaSlaId,
        Guid prioridadeId,
        DateTime dataReabertura,
        DateTime novoPrazoResolucao,
        bool pausarQuandoAguardandoSolicitante,
        bool usarHorarioComercial,
        Guid? calendarioCorporativoId,
        string atualizadoPor)
    {
        PoliticaSlaId = politicaSlaId;
        PrioridadeId = prioridadeId;
        DataInicio = dataReabertura;
        DataResolucao = null;
        ResolucaoCumprida = null;
        ResolucaoViolada = false;
        MinutosResolucao = null;
        PrazoResolucao = novoPrazoResolucao;
        PausarQuandoAguardandoSolicitante = pausarQuandoAguardandoSolicitante;
        UsarHorarioComercial = usarHorarioComercial;
        CalendarioCorporativoId = calendarioCorporativoId;
        Pausado = false;
        DataPausa = null;
        AtualizarAuditoria(atualizadoPor);
    }

    private int CalcularMinutosDecorridos(DateTime referenciaUtc)
    {
        var minutos = (int)Math.Round((referenciaUtc - DataInicio).TotalMinutes);
        return Math.Max(0, minutos - MinutosPausados);
    }

    private int CalcularMinutosPausaCorridos(DateTime retomadoEmUtc)
    {
        if (DataPausa is null)
        {
            return 0;
        }

        return (int)Math.Max(0, Math.Round((retomadoEmUtc - DataPausa.Value).TotalMinutes));
    }
}
