package br.geti.sistemachamado.worker.email.integracao;

import br.geti.sistemachamado.worker.email.configuracao.PropriedadesIntegracaoImap;
import jakarta.mail.Folder;
import jakarta.mail.Message;
import jakarta.mail.Session;
import jakarta.mail.Store;
import jakarta.mail.internet.MimeMessage;
import java.util.ArrayList;
import java.util.List;
import java.util.Properties;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.condition.ConditionalOnProperty;
import org.springframework.stereotype.Service;

@Service
@ConditionalOnProperty(
        prefix = "app.worker.email",
        name = "modo-local-habilitado",
        havingValue = "false",
        matchIfMissing = true
)
public class LeitorMensagensImap implements LeitorMensagensEmail {

    private static final Logger LOGGER = LoggerFactory.getLogger(LeitorMensagensImap.class);

    private final PropriedadesIntegracaoImap propriedadesIntegracaoImap;

    public LeitorMensagensImap(final PropriedadesIntegracaoImap propriedadesIntegracaoImap) {
        this.propriedadesIntegracaoImap = propriedadesIntegracaoImap;
    }

    @Override
    public List<MensagemEmailRecebida> listarMensagensElegiveis() {
        if (propriedadesIntegracaoImap.getHost() == null || propriedadesIntegracaoImap.getHost().isBlank()) {
            LOGGER.warn("Integracao IMAP sem host configurado. Nenhuma mensagem sera lida.");
            return List.of();
        }
        if (propriedadesIntegracaoImap.getUsuario() == null || propriedadesIntegracaoImap.getUsuario().isBlank()) {
            LOGGER.warn("Integracao IMAP sem usuario configurado. Nenhuma mensagem sera lida.");
            return List.of();
        }
        if (propriedadesIntegracaoImap.getSenha() == null || propriedadesIntegracaoImap.getSenha().isBlank()) {
            LOGGER.warn("Integracao IMAP sem senha configurada. Nenhuma mensagem sera lida.");
            return List.of();
        }

        final var mensagens = new ArrayList<MensagemEmailRecebida>();
        final var propriedades = construirPropriedadesImap();
        final var protocolo = propriedadesIntegracaoImap.isSslHabilitado() ? "imaps" : "imap";

        try {
            final Session session = Session.getInstance(propriedades);
            final Store store = session.getStore(protocolo);
            store.connect(
                    propriedadesIntegracaoImap.getHost(),
                    propriedadesIntegracaoImap.getPorta(),
                    propriedadesIntegracaoImap.getUsuario(),
                    propriedadesIntegracaoImap.getSenha()
            );

            final Folder pasta = store.getFolder(propriedadesIntegracaoImap.getPasta());
            pasta.open(Folder.READ_ONLY);
            final Message[] mensagensBrutas = pasta.getMessages();
            final int total = mensagensBrutas.length;
            final int limite = Math.max(1, propriedadesIntegracaoImap.getMaxMensagensPorCiclo());
            final int inicio = Math.max(0, total - limite);

            for (int indice = total - 1; indice >= inicio; indice--) {
                final var mensagem = mensagensBrutas[indice];
                if (mensagem instanceof MimeMessage mimeMessage) {
                    try {
                        mensagens.add(ExtratorMensagemMime.extrair(
                                mimeMessage,
                                "imap:" + propriedadesIntegracaoImap.getPasta() + ":" + (indice + 1)
                        ));
                    } catch (final Exception exception) {
                        LOGGER.warn("Falha ao extrair mensagem IMAP indice {}: {}", indice, exception.getMessage());
                    }
                }
            }

            pasta.close(false);
            store.close();
        } catch (final Exception exception) {
            LOGGER.error("Falha na leitura IMAP: {}", exception.getMessage(), exception);
            return List.of();
        }

        return List.copyOf(mensagens);
    }

    private Properties construirPropriedadesImap() {
        final var properties = new Properties();
        final String protocoloBase = propriedadesIntegracaoImap.isSslHabilitado() ? "imaps" : "imap";

        properties.setProperty("mail.store.protocol", protocoloBase);
        properties.setProperty("mail." + protocoloBase + ".host", propriedadesIntegracaoImap.getHost());
        properties.setProperty("mail." + protocoloBase + ".port", String.valueOf(propriedadesIntegracaoImap.getPorta()));
        properties.setProperty(
                "mail." + protocoloBase + ".connectiontimeout",
                String.valueOf(propriedadesIntegracaoImap.getConnectTimeoutMillis())
        );
        properties.setProperty(
                "mail." + protocoloBase + ".timeout",
                String.valueOf(propriedadesIntegracaoImap.getTimeoutMillis())
        );

        if (!propriedadesIntegracaoImap.isSslHabilitado()) {
            properties.setProperty("mail.imap.starttls.enable", String.valueOf(propriedadesIntegracaoImap.isTlsHabilitado()));
        }

        return properties;
    }
}

