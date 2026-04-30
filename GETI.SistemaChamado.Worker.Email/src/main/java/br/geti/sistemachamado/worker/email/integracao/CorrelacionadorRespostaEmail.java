package br.geti.sistemachamado.worker.email.integracao;

import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import java.util.ArrayList;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Component;

@Component
public class CorrelacionadorRespostaEmail {

    private final LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio;

    public CorrelacionadorRespostaEmail(final LogDeIntegracaoEmailRepositorio logDeIntegracaoEmailRepositorio) {
        this.logDeIntegracaoEmailRepositorio = logDeIntegracaoEmailRepositorio;
    }

    public Optional<CorrelacaoRespostaEmail> correlacionar(final UUID caixaDeEmailId, final MensagemEmailRecebida mensagem) {
        if (caixaDeEmailId == null || mensagem == null) {
            return Optional.empty();
        }

        final var candidatos = extrairMessageIdsCandidatos(mensagem);
        for (final var messageIdOrigem : candidatos) {
            final var log = logDeIntegracaoEmailRepositorio
                    .buscarUltimoComChamadoPorCaixaEMessageId(caixaDeEmailId, messageIdOrigem);
            if (log.isPresent() && log.get().chamadoId() != null) {
                return Optional.of(new CorrelacaoRespostaEmail(log.get().chamadoId(), messageIdOrigem));
            }
        }
        return Optional.empty();
    }

    private List<String> extrairMessageIdsCandidatos(final MensagemEmailRecebida mensagem) {
        final var candidatos = new LinkedHashSet<String>();
        adicionarSeValido(candidatos, mensagem.inReplyTo());

        if (mensagem.references() != null && !mensagem.references().isEmpty()) {
            final var referencias = new ArrayList<>(mensagem.references());
            for (int indice = referencias.size() - 1; indice >= 0; indice--) {
                adicionarSeValido(candidatos, referencias.get(indice));
            }
        }
        return List.copyOf(candidatos);
    }

    private void adicionarSeValido(final LinkedHashSet<String> destino, final String valor) {
        if (valor == null || valor.isBlank()) {
            return;
        }
        destino.add(normalizarMessageId(valor));
    }

    private String normalizarMessageId(final String messageId) {
        return messageId.trim()
                .replace("<", "")
                .replace(">", "")
                .toLowerCase(Locale.ROOT);
    }
}
