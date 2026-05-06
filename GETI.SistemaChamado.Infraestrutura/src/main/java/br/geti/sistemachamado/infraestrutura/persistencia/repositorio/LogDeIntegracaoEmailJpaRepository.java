package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.LogDeIntegracaoEmailEntidadeJpa;
import java.util.Optional;
import java.util.UUID;

public interface LogDeIntegracaoEmailJpaRepository extends RepositorioJpaBase<LogDeIntegracaoEmailEntidadeJpa> {

    Optional<LogDeIntegracaoEmailEntidadeJpa> findByCaixaEmailIdAndChaveDeduplicacao(
            UUID caixaEmailId,
            String chaveDeduplicacao
    );

    Optional<LogDeIntegracaoEmailEntidadeJpa> findFirstByCaixaEmailIdAndMessageIdIgnoreCaseAndChamadoIdIsNotNullOrderByDataProcessamentoDesc(
            UUID caixaEmailId,
            String messageId
    );
}
