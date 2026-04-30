package br.geti.sistemachamado.worker.email.integracao;

import java.time.LocalDateTime;
import java.util.List;

public record MensagemEmailRecebida(
        String identificadorOrigem,
        String messageId,
        String inReplyTo,
        List<String> references,
        String autoSubmitted,
        String precedence,
        String remetenteNome,
        String remetenteEmail,
        List<String> destinatarios,
        String assunto,
        String corpoMensagem,
        LocalDateTime dataRecebimento,
        List<AnexoMensagemEmailRecebida> anexos
) {
}
