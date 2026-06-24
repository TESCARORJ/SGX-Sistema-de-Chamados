using SGX.SistemaChamado.Application.DTOs.Notificacoes;

namespace SGX.SistemaChamado.Application.Interfaces.Notificacoes;

public interface ITransportadorEmailNotificacao
{
    Task<ResultadoTransporteEmailNotificacao> EnviarAsync(
        MensagemEmailNotificacao mensagem,
        CancellationToken cancellationToken = default);
}
