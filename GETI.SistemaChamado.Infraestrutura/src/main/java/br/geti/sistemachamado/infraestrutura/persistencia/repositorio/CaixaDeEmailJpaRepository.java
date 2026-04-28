package br.geti.sistemachamado.infraestrutura.persistencia.repositorio;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CaixaDeEmailEntidadeJpa;
import java.util.List;
import java.util.Optional;

public interface CaixaDeEmailJpaRepository extends RepositorioJpaBase<CaixaDeEmailEntidadeJpa> {

    Optional<CaixaDeEmailEntidadeJpa> findByEnderecoEmailIgnoreCase(String enderecoEmail);

    List<CaixaDeEmailEntidadeJpa> findAllByOrderByNomeExibicaoAsc();

    List<CaixaDeEmailEntidadeJpa> findByAtivaTrueOrderByNomeExibicaoAsc();
}
