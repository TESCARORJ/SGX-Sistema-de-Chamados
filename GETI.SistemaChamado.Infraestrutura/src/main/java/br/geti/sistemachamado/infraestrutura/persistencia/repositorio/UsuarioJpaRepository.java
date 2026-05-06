package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;
import java.util.List;
import java.util.Optional;

public interface UsuarioJpaRepository extends RepositorioJpaBase<UsuarioEntidadeJpa> {

    Optional<UsuarioEntidadeJpa> findByLoginIgnoreCase(String login);

    Optional<UsuarioEntidadeJpa> findByEmailIgnoreCase(String email);

    List<UsuarioEntidadeJpa> findByAtivoTrue();
}
