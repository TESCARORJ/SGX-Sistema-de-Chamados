package br.geti.sistemachamado.worker.email.integracao;

import java.util.Locale;
import org.springframework.stereotype.Component;

@Component
public class DetectorRespostaAutomaticaEmail {

    public boolean ehRespostaAutomatica(final MensagemEmailRecebida mensagem) {
        if (mensagem == null) {
            return false;
        }

        final var autoSubmitted = normalizar(mensagem.autoSubmitted());
        if (autoSubmitted != null && !autoSubmitted.equals("no")) {
            return true;
        }

        final var precedence = normalizar(mensagem.precedence());
        if (precedence != null && (precedence.contains("bulk")
                || precedence.contains("auto_reply")
                || precedence.contains("junk")
                || precedence.contains("list"))) {
            return true;
        }

        final var remetente = normalizar(mensagem.remetenteEmail());
        if (remetente != null && (remetente.contains("mailer-daemon")
                || remetente.contains("postmaster")
                || remetente.contains("no-reply")
                || remetente.contains("noreply"))) {
            return true;
        }

        final var assunto = normalizar(mensagem.assunto());
        return assunto != null && (assunto.contains("out of office")
                || assunto.contains("automatic reply")
                || assunto.contains("auto reply")
                || assunto.contains("resposta automatica")
                || assunto.contains("fora do escritorio")
                || assunto.contains("ausente do escritorio"));
    }

    private String normalizar(final String valor) {
        if (valor == null || valor.isBlank()) {
            return null;
        }
        return valor.trim().toLowerCase(Locale.ROOT);
    }
}
