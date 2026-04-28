package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CategoriaEntidadeJpa;
import java.util.List;
import java.util.Optional;

public interface CategoriaJpaRepository extends RepositorioJpaBase<CategoriaEntidadeJpa> {

    Optional<CategoriaEntidadeJpa> findByNomeIgnoreCase(String nome);

    List<CategoriaEntidadeJpa> findAllByOrderByNomeAsc();

    List<CategoriaEntidadeJpa> findByAtivoTrueOrderByNomeAsc();
}
