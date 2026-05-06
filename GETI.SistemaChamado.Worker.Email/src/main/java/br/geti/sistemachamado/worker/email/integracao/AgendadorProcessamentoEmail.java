package br.geti.sistemachamado.worker.email.integracao;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

@Component
public class AgendadorProcessamentoEmail {

    private static final Logger LOGGER = LoggerFactory.getLogger(AgendadorProcessamentoEmail.class);

    private final ProcessadorIntegracaoEmail processadorIntegracaoEmail;

    public AgendadorProcessamentoEmail(final ProcessadorIntegracaoEmail processadorIntegracaoEmail) {
        this.processadorIntegracaoEmail = processadorIntegracaoEmail;
    }

    @Scheduled(
            fixedDelayString = "${app.worker.email.intervalo-processamento-ms:60000}",
            initialDelayString = "${app.worker.email.intervalo-processamento-ms:60000}"
    )
    public void executarCiclo() {
        LOGGER.debug("Iniciando ciclo de processamento de e-mails via IMAP.");
        processadorIntegracaoEmail.processarCiclo();
    }
}

