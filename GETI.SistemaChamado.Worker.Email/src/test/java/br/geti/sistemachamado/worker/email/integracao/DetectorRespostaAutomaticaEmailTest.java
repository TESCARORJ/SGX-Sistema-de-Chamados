package br.geti.sistemachamado.worker.email.integracao;

import static org.assertj.core.api.Assertions.assertThat;

import java.time.LocalDateTime;
import java.util.List;
import org.junit.jupiter.api.Test;

class DetectorRespostaAutomaticaEmailTest {

    private final DetectorRespostaAutomaticaEmail detector = new DetectorRespostaAutomaticaEmail();

    @Test
    void deveIdentificarAutoSubmitted() {
        final var mensagem = criarMensagem("auto-generated", null, "usuario@corp.com", "Atualizacao de chamado");

        assertThat(detector.ehRespostaAutomatica(mensagem)).isTrue();
    }

    @Test
    void deveIdentificarRemetenteMailerDaemon() {
        final var mensagem = criarMensagem(null, null, "mailer-daemon@corp.com", "Delivery Status Notification");

        assertThat(detector.ehRespostaAutomatica(mensagem)).isTrue();
    }

    @Test
    void naoDeveMarcarMensagemComumComoAutomatica() {
        final var mensagem = criarMensagem(null, null, "solicitante@corp.com", "Re: Erro no sistema");

        assertThat(detector.ehRespostaAutomatica(mensagem)).isFalse();
    }

    private MensagemEmailRecebida criarMensagem(
            final String autoSubmitted,
            final String precedence,
            final String remetente,
            final String assunto
    ) {
        return new MensagemEmailRecebida(
                "origem",
                "msg@corp",
                null,
                List.of(),
                autoSubmitted,
                precedence,
                "Nome",
                remetente,
                List.of("suporte@corp.com"),
                assunto,
                "corpo",
                LocalDateTime.now(),
                List.of()
        );
    }
}
