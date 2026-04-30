package br.geti.sistemachamado.worker.email.integracao;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.eq;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.StatusProcessamentoIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class CorrelacionadorRespostaEmailTest {

    @Mock
    private LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio;

    @Test
    void deveCorrelacionarPriorizandoInReplyToEReferences() {
        final var caixaId = UUID.randomUUID();
        final var chamadoId = UUID.randomUUID();
        final var correlacionador = new CorrelacionadorRespostaEmail(logDeIntegracaoEmailRepositorio);
        final var mensagem = new MensagemEmailRecebida(
                "origem",
                "nova-msg@corp",
                "<mensagem-antiga@corp> <thread@corp>",
                List.of("<primeira-referencia@corp>", "<segunda-referencia@corp>"),
                null,
                null,
                "Solicitante",
                "solicitante@corp.com",
                List.of("suporte@corp.com"),
                "Re: Chamado",
                "Resposta",
                LocalDateTime.now(),
                List.of()
        );

        when(logDeIntegracaoEmailRepositorio.buscarUltimoComChamadoPorCaixaEMessageId(eq(caixaId), eq("mensagem-antiga@corp")))
                .thenReturn(Optional.empty());
        when(logDeIntegracaoEmailRepositorio.buscarUltimoComChamadoPorCaixaEMessageId(eq(caixaId), eq("thread@corp")))
                .thenReturn(Optional.empty());
        when(logDeIntegracaoEmailRepositorio.buscarUltimoComChamadoPorCaixaEMessageId(eq(caixaId), eq("segunda-referencia@corp")))
                .thenReturn(Optional.of(criarLog(caixaId, chamadoId, "segunda-referencia@corp")));

        final var correlacao = correlacionador.correlacionar(caixaId, mensagem);

        assertThat(correlacao).isPresent();
        assertThat(correlacao.get().chamadoId()).isEqualTo(chamadoId);
        assertThat(correlacao.get().messageIdCorrelacionado()).isEqualTo("segunda-referencia@corp");
    }

    @Test
    void deveUsarMessageIdAtualQuandoNaoHouverCabecalhosDeResposta() {
        final var caixaId = UUID.randomUUID();
        final var chamadoId = UUID.randomUUID();
        final var correlacionador = new CorrelacionadorRespostaEmail(logDeIntegracaoEmailRepositorio);
        final var mensagem = new MensagemEmailRecebida(
                "origem",
                "<message-id-atual@corp>",
                null,
                List.of(),
                null,
                null,
                "Solicitante",
                "solicitante@corp.com",
                List.of("suporte@corp.com"),
                "Re: Chamado",
                "Resposta",
                LocalDateTime.now(),
                List.of()
        );

        when(logDeIntegracaoEmailRepositorio.buscarUltimoComChamadoPorCaixaEMessageId(eq(caixaId), eq("message-id-atual@corp")))
                .thenReturn(Optional.of(criarLog(caixaId, chamadoId, "message-id-atual@corp")));

        final var correlacao = correlacionador.correlacionar(caixaId, mensagem);

        assertThat(correlacao).isPresent();
        assertThat(correlacao.get().chamadoId()).isEqualTo(chamadoId);
        assertThat(correlacao.get().messageIdCorrelacionado()).isEqualTo("message-id-atual@corp");
    }

    private LogDeIntegracaoEmail criarLog(final UUID caixaId, final UUID chamadoId, final String messageId) {
        final var agora = LocalDateTime.now();
        return new LogDeIntegracaoEmail(
                UUID.randomUUID(),
                caixaId,
                messageId,
                "solicitante@corp.com",
                "suporte@corp.com",
                "Assunto",
                StatusProcessamentoIntegracaoEmail.SUCESSO,
                "ok",
                "MSGID:" + messageId,
                chamadoId,
                agora,
                agora,
                null
        );
    }
}
