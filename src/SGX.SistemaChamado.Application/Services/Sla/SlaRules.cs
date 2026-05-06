using SGX.SistemaChamado.Domain.Entities;

namespace SGX.SistemaChamado.Application.Services.Sla;

internal static class SlaRules
{
    public const int LimiteHorasProximoVencimento = 4;

    public static bool EstaProximoDoVencimento(SlaControle? slaControle, DateTime agoraUtc)
    {
        if (slaControle is null ||
            slaControle.EstaVencido ||
            slaControle.EstaPausado ||
            slaControle.ResolvidoEm.HasValue)
        {
            return false;
        }

        return slaControle.PrazoResolucaoEm <= agoraUtc.AddHours(LimiteHorasProximoVencimento);
    }
}
