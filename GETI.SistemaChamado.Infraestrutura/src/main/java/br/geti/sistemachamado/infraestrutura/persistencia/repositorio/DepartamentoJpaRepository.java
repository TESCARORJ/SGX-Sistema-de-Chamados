package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.DepartamentoEntidadeJpa;
import java.util.List;
import java.util.Optional;

public interface DepartamentoJpaRepository extends RepositorioJpaBase<DepartamentoEntidadeJpa> {

    Optional<DepartamentoEntidadeJpa> findByNomeIgnoreCase(String nome);

    List<DepartamentoEntidadeJpa> findAllByOrderByNomeAsc();

    List<DepartamentoEntidadeJpa> findByAtivoTrueOrderByNomeAsc();
}
