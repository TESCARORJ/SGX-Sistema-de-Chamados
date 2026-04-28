package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.GrupoAtendimentoEntidadeJpa;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface GrupoAtendimentoJpaRepository extends RepositorioJpaBase<GrupoAtendimentoEntidadeJpa> {

    Optional<GrupoAtendimentoEntidadeJpa> findByNomeIgnoreCaseAndDepartamentoId(String nome, UUID departamentoId);

    List<GrupoAtendimentoEntidadeJpa> findAllByOrderByNomeAsc();

    List<GrupoAtendimentoEntidadeJpa> findByDepartamentoIdOrderByNomeAsc(UUID departamentoId);

    List<GrupoAtendimentoEntidadeJpa> findByAtivoTrueOrderByNomeAsc();
}
