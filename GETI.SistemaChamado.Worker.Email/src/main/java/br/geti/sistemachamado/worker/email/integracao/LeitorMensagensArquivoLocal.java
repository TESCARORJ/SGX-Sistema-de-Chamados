package br.geti.sistemachamado.worker.email.integracao;

import br.geti.sistemachamado.worker.email.configuracao.PropriedadesIntegracaoImap;
import br.geti.sistemachamado.worker.email.configuracao.PropriedadesWorkerEmail;
import jakarta.mail.Session;
import jakarta.mail.internet.MimeMessage;
import java.io.InputStream;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Comparator;
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
        havingValue = "true"
)
public class LeitorMensagensArquivoLocal implements LeitorMensagensEmail {

    private static final Logger LOGGER = LoggerFactory.getLogger(LeitorMensagensArquivoLocal.class);

    private final PropriedadesWorkerEmail propriedadesWorkerEmail;
    private final PropriedadesIntegracaoImap propriedadesIntegracaoImap;

    public LeitorMensagensArquivoLocal(
            final PropriedadesWorkerEmail propriedadesWorkerEmail,
            final PropriedadesIntegracaoImap propriedadesIntegracaoImap
    ) {
        this.propriedadesWorkerEmail = propriedadesWorkerEmail;
        this.propriedadesIntegracaoImap = propriedadesIntegracaoImap;
    }

    @Override
    public List<MensagemEmailRecebida> listarMensagensElegiveis() {
        final var diretorio = Path.of(propriedadesWorkerEmail.getDiretorioArquivosEml()).toAbsolutePath().normalize();
        if (!Files.exists(diretorio) || !Files.isDirectory(diretorio)) {
            LOGGER.info("Modo local habilitado sem diretorio EML valido: {}", diretorio);
            return List.of();
        }

        final var resultado = new ArrayList<MensagemEmailRecebida>();
        final var session = Session.getInstance(new Properties());
        final var limite = Math.max(1, propriedadesIntegracaoImap.getMaxMensagensPorCiclo());

        try (var stream = Files.list(diretorio)) {
            final var arquivos = stream
                    .filter(path -> Files.isRegularFile(path))
                    .filter(path -> path.getFileName().toString().toLowerCase().endsWith(".eml"))
                    .sorted(Comparator.comparing(Path::getFileName).reversed())
                    .limit(limite)
                    .toList();

            for (final var arquivo : arquivos) {
                try (InputStream inputStream = Files.newInputStream(arquivo)) {
                    final var mimeMessage = new MimeMessage(session, inputStream);
                    resultado.add(ExtratorMensagemMime.extrair(
                            mimeMessage,
                            "arquivo-local:" + arquivo.getFileName()
                    ));
                } catch (final Exception exception) {
                    LOGGER.warn("Falha ao processar arquivo EML {}: {}", arquivo, exception.getMessage());
                }
            }
        } catch (final Exception exception) {
            LOGGER.error("Falha ao listar diretorio de mensagens locais {}: {}", diretorio, exception.getMessage(), exception);
            return List.of();
        }

        return List.copyOf(resultado);
    }
}

