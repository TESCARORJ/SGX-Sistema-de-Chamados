package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.EntidadeBaseJpa;
import java.util.UUID;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.repository.NoRepositoryBean;

@NoRepositoryBean
public interface RepositorioJpaBase<T extends EntidadeBaseJpa> extends JpaRepository<T, UUID> {
}