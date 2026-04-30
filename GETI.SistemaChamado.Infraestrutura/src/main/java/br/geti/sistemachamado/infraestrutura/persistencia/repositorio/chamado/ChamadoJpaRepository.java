package br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.ChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.RepositorioJpaBase;
import org.springframework.data.jpa.repository.EntityGraph;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ChamadoJpaRepository extends RepositorioJpaBase<ChamadoEntidadeJpa> {

    Optional<ChamadoEntidadeJpa> findByNumero(String numero);

    @EntityGraph(attributePaths = {
            "solicitante",
            "responsavel",
            "departamento",
            "categoria",
            "servico",
            "servico.departamento",
            "servico.categoria"
    })
    Optional<ChamadoEntidadeJpa> findByIdAndSolicitanteId(UUID id, UUID solicitanteId);

    @EntityGraph(attributePaths = {
            "solicitante",
            "responsavel",
            "departamento",
            "categoria",
            "servico",
            "servico.departamento",
            "servico.categoria"
    })
    List<ChamadoEntidadeJpa> findBySolicitanteIdOrderByDataCriacaoDesc(UUID solicitanteId);

    @EntityGraph(attributePaths = {
            "solicitante",
            "responsavel",
            "departamento",
            "categoria",
            "servico",
            "servico.departamento",
            "servico.categoria"
    })
    List<ChamadoEntidadeJpa> findAllByOrderByDataCriacaoDesc();
}
