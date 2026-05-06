package br.geti.sistemachamado.dominio.integracaoemail.repositorio;

import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import java.util.Optional;
import java.util.UUID;

public interface LogDeIntegracaoEmailRepositorio extends RepositorioDominio<LogDeIntegracaoEmail> {

    Optional<LogDeIntegracaoEmail> buscarPorCaixaEChaveDeduplicacao(UUID caixaDeEmailId, String chaveDeduplicacao);

    Optional<LogDeIntegracaoEmail> buscarUltimoComChamadoPorCaixaEMessageId(UUID caixaDeEmailId, String messageId);
}
