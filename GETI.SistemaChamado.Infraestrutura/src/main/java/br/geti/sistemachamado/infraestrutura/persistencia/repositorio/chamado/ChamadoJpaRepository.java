package br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.ChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.RepositorioJpaBase;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ChamadoJpaRepository extends RepositorioJpaBase<ChamadoEntidadeJpa> {

    Optional<ChamadoEntidadeJpa> findByNumero(String numero);

    Optional<ChamadoEntidadeJpa> findByIdAndSolicitanteId(UUID id, UUID solicitanteId);

    List<ChamadoEntidadeJpa> findBySolicitanteIdOrderByDataCriacaoDesc(UUID solicitanteId);
}
