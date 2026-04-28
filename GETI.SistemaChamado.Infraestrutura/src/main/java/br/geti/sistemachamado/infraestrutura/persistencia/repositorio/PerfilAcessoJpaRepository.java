package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.PerfilAcessoEntidadeJpa;
import java.util.List;
import java.util.Optional;

public interface PerfilAcessoJpaRepository extends RepositorioJpaBase<PerfilAcessoEntidadeJpa> {

    Optional<PerfilAcessoEntidadeJpa> findByNomeIgnoreCase(String nome);

    List<PerfilAcessoEntidadeJpa> findByAtivoTrue();
}