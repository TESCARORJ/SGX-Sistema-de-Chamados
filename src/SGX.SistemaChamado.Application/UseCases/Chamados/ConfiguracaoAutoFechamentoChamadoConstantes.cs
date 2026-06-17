namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public static class ConfiguracaoAutoFechamentoChamadoConstantes
{
    public const string ChaveParametroPrazoAceiteHoras = "chamados.fechamento_automatico.prazo_aceite_horas";
    public const int PrazoMinimoHoras = 1;
    public const int PrazoMaximoHoras = 720;
    public const int PrazoPadraoHoras = 72;
    public const string DescricaoParametroPrazoAceiteHoras =
        "Prazo em horas para fechamento automatico de chamados resolvidos sem manifestacao do solicitante.";
}
