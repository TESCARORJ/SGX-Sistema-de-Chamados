package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.ServicoEntidadeJpa;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ServicoJpaRepository extends RepositorioJpaBase<ServicoEntidadeJpa> {

    Optional<ServicoEntidadeJpa> findByNomeIgnoreCaseAndDepartamentoId(String nome, UUID departamentoId);

    List<ServicoEntidadeJpa> findAllByOrderByNomeAsc();

    List<ServicoEntidadeJpa> findByDepartamentoIdOrderByNomeAsc(UUID departamentoId);

    List<ServicoEntidadeJpa> findByCategoriaIdOrderByNomeAsc(UUID categoriaId);

    List<ServicoEntidadeJpa> findByAtivoTrueOrderByNomeAsc();
}
