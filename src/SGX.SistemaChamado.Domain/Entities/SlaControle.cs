using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class SlaControle : AuditableEntity
{
    public Guid ChamadoId { get; private set; }
    public DateTime PrazoPrimeiraRespostaEm { get; private set; }
    public DateTime? PrimeiraRespostaEm { get; private set; }
    public DateTime PrazoResolucaoEm { get; private set; }
    public DateTime? ResolvidoEm { get; private set; }
    public bool EstaVencido { get; private set; }
    public bool EstaPausado { get; private set; }
    public DateTime? PausadoEm { get; private set; }
    public int TotalMinutosPausado { get; private set; }

    public Chamado Chamado { get; private set; } = default!;

    private SlaControle()
    {
    }

    public SlaControle(Guid chamadoId, DateTime prazoPrimeiraRespostaEm, DateTime prazoResolucaoEm, string criadoPor)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("O chamado do SLA e obrigatorio.", nameof(chamadoId));
        }

        ChamadoId = chamadoId;
        PrazoPrimeiraRespostaEm = prazoPrimeiraRespostaEm;
        PrazoResolucaoEm = prazoResolucaoEm;
        DefinirCriacao(criadoPor);
    }

    public void RegistrarPrimeiraResposta(DateTime respostaEmUtc, string atualizadoPor)
    {
        if (PrimeiraRespostaEm.HasValue)
        {
            return;
        }

        PrimeiraRespostaEm = respostaEmUtc;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void RegistrarResolucao(DateTime resolvidoEmUtc, string atualizadoPor)
    {
        ResolvidoEm = resolvidoEmUtc;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void IniciarPausa(DateTime pausadoEmUtc, string atualizadoPor)
    {
        if (EstaPausado)
        {
            return;
        }

        EstaPausado = true;
        PausadoEm = pausadoEmUtc;
        AtualizarAuditoria(atualizadoPor);
    }

    public void FinalizarPausa(DateTime retomadoEmUtc, string atualizadoPor)
    {
        if (!EstaPausado || PausadoEm is null)
        {
            return;
        }

        var pausa = (retomadoEmUtc - PausadoEm.Value).TotalMinutes;
        if (pausa > 0)
        {
            var minutosPausa = (int)Math.Round(pausa);
            TotalMinutosPausado += minutosPausa;

            if (!PrimeiraRespostaEm.HasValue)
            {
                PrazoPrimeiraRespostaEm = PrazoPrimeiraRespostaEm.AddMinutes(minutosPausa);
            }

            if (!ResolvidoEm.HasValue)
            {
                PrazoResolucaoEm = PrazoResolucaoEm.AddMinutes(minutosPausa);
            }
        }

        EstaPausado = false;
        PausadoEm = null;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void AtualizarPrazos(DateTime prazoPrimeiraRespostaEm, DateTime prazoResolucaoEm, string atualizadoPor)
    {
        PrazoPrimeiraRespostaEm = prazoPrimeiraRespostaEm;
        PrazoResolucaoEm = prazoResolucaoEm;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void RecalcularPrazoResolucao(DateTime novoPrazoResolucaoEm, string atualizadoPor)
    {
        PrazoResolucaoEm = novoPrazoResolucaoEm;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void Reabrir(DateTime novoPrazoResolucaoEm, string atualizadoPor)
    {
        ResolvidoEm = null;
        EstaPausado = false;
        PausadoEm = null;
        PrazoResolucaoEm = novoPrazoResolucaoEm;
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    public void RecalcularVencimento(string atualizadoPor)
    {
        AtualizarVencimento();
        AtualizarAuditoria(atualizadoPor);
    }

    private void AtualizarVencimento()
    {
        var referencia = ResolvidoEm ?? DateTime.UtcNow;
        EstaVencido = referencia > PrazoResolucaoEm;
    }
}
