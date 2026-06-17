namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public static class ConfiguracaoReaberturaChamadoConstantes
{
    public const string ChaveParametroPrazoMaximoHoras = "chamados.reabertura.prazo_maximo_horas";
    public const int PrazoMinimoHoras = 1;
    public const int PrazoMaximoHoras = 2160;
    public const int PrazoPadraoHoras = 168;
    public const string DescricaoParametroPrazoMaximoHoras =
        "Prazo maximo em horas para reabertura controlada de chamados encerrados.";
}
