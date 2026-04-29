package br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.HistoricoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.RepositorioJpaBase;
import java.util.List;
import java.util.UUID;

public interface HistoricoChamadoJpaRepository extends RepositorioJpaBase<HistoricoChamadoEntidadeJpa> {

    List<HistoricoChamadoEntidadeJpa> findByChamadoIdOrderByDataCriacaoAsc(UUID chamadoId);
}
