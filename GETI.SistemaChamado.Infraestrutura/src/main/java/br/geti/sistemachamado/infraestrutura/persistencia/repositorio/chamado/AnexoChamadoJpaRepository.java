package br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.AnexoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.RepositorioJpaBase;
import java.util.List;
import java.util.UUID;

public interface AnexoChamadoJpaRepository extends RepositorioJpaBase<AnexoChamadoEntidadeJpa> {

    List<AnexoChamadoEntidadeJpa> findByChamadoIdOrderByDataCriacaoAsc(UUID chamadoId);
}
