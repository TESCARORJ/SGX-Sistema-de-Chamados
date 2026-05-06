package br.geti.sistemachamado.worker.email.integracao;

import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import java.util.ArrayList;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Locale;
import java.util.Optional;
import java.util.UUID;
import java.util.regex.Pattern;
import org.springframework.stereotype.Component;

@Component
public class CorrelacionadorRespostaEmail {

    private static final Pattern PADRAO_MESSAGE_ID = Pattern.compile("<([^>]+)>");

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
        adicionarCabecalhoMessageId(candidatos, mensagem.inReplyTo());

        if (mensagem.references() != null && !mensagem.references().isEmpty()) {
            final var referencias = new ArrayList<>(mensagem.references());
            for (int indice = referencias.size() - 1; indice >= 0; indice--) {
                adicionarCabecalhoMessageId(candidatos, referencias.get(indice));
            }
        }

        adicionarCabecalhoMessageId(candidatos, mensagem.messageId());
        return List.copyOf(candidatos);
    }

    private void adicionarCabecalhoMessageId(final LinkedHashSet<String> destino, final String valor) {
        if (valor == null || valor.isBlank()) {
            return;
        }

        final var candidatos = extrairMessageIds(valor);
        if (candidatos.isEmpty()) {
            destino.add(normalizarMessageId(valor));
            return;
        }

        destino.addAll(candidatos);
    }

    private List<String> extrairMessageIds(final String valorCabecalho) {
        final var encontrados = new LinkedHashSet<String>();
        final var matcher = PADRAO_MESSAGE_ID.matcher(valorCabecalho);
        while (matcher.find()) {
            encontrados.add(normalizarMessageId(matcher.group(1)));
        }

        if (!encontrados.isEmpty()) {
            return List.copyOf(encontrados);
        }

        for (final var token : valorCabecalho.split("[,\\s]+")) {
            if (token == null || token.isBlank()) {
                continue;
            }
            encontrados.add(normalizarMessageId(token));
        }
        return List.copyOf(encontrados);
    }

    private String normalizarMessageId(final String messageId) {
        return messageId.trim()
                .replace("<", "")
                .replace(">", "")
                .toLowerCase(Locale.ROOT);
    }
}
