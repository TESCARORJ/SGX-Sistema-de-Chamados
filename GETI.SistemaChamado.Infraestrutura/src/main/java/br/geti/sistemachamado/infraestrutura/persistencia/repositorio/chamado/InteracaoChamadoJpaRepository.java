package br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.InteracaoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.RepositorioJpaBase;
import java.util.List;
import java.util.UUID;

public interface InteracaoChamadoJpaRepository extends RepositorioJpaBase<InteracaoChamadoEntidadeJpa> {

    List<InteracaoChamadoEntidadeJpa> findByChamadoIdOrderByDataCriacaoAsc(UUID chamadoId);
}
