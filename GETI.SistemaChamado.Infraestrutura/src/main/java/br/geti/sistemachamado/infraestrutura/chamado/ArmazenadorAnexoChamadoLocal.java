package br.geti.sistemachamado.infraestrutura.chamado;

import br.geti.sistemachamado.aplicacao.chamado.portal.AnexoArmazenadoChamado;
import br.geti.sistemachamado.aplicacao.chamado.portal.ArmazenadorAnexoChamado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.text.Normalizer;
import java.util.Locale;
import java.util.UUID;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

@Component
public class ArmazenadorAnexoChamadoLocal implements ArmazenadorAnexoChamado {

    private final Path diretorioBase;

    public ArmazenadorAnexoChamadoLocal(
            @Value("${app.chamado.anexos-diretorio:uploads/chamados}") final String diretorioBase
    ) {
        this.diretorioBase = Path.of(diretorioBase).toAbsolutePath().normalize();
    }

    @Override
    public AnexoArmazenadoChamado armazenar(
            final UUID chamadoId,
            final UUID anexoId,
            final String nomeArquivo,
            final byte[] conteudo
    ) {
        try {
            final var pastaChamado = diretorioBase.resolve(chamadoId.toString());
            Files.createDirectories(pastaChamado);

            final var nomeSanitizado = normalizarNomeArquivo(nomeArquivo);
            final var nomeArmazenado = anexoId + "_" + nomeSanitizado;
            final var caminhoArquivo = pastaChamado.resolve(nomeArmazenado).normalize();

            if (!caminhoArquivo.startsWith(diretorioBase)) {
                throw new ErroDeDominio("caminho de anexo invalido");
            }

            Files.write(
                    caminhoArquivo,
                    conteudo,
                    StandardOpenOption.CREATE,
                    StandardOpenOption.TRUNCATE_EXISTING,
                    StandardOpenOption.WRITE
            );

            return new AnexoArmazenadoChamado(
                    anexoId,
                    nomeArmazenado,
                    caminhoArquivo.toString()
            );
        } catch (final IOException exception) {
            throw new ErroDeDominio("Falha ao armazenar anexo do chamado.");
        }
    }

    private String normalizarNomeArquivo(final String nomeArquivo) {
        final var semAcento = Normalizer.normalize(nomeArquivo, Normalizer.Form.NFD)
                .replaceAll("\\p{M}", "");
        final var apenasSeguro = semAcento
                .replaceAll("[^a-zA-Z0-9._-]", "_")
                .replaceAll("_+", "_")
                .toLowerCase(Locale.ROOT);

        if (apenasSeguro.isBlank()) {
            return "anexo.bin";
        }

        return apenasSeguro.length() > 180 ? apenasSeguro.substring(0, 180) : apenasSeguro;
    }
}
