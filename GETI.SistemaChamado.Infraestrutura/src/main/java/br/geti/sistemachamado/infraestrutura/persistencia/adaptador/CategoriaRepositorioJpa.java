package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.repositorio.CategoriaRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CategoriaEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.CategoriaJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class CategoriaRepositorioJpa implements CategoriaRepositorio {

    private final CategoriaJpaRepository categoriaJpaRepository;

    public CategoriaRepositorioJpa(final CategoriaJpaRepository categoriaJpaRepository) {
        this.categoriaJpaRepository = categoriaJpaRepository;
    }

    @Override
    @Transactional
    public Categoria salvar(final Categoria categoria) {
        final var entidade = categoriaJpaRepository.findById(categoria.id()).orElseGet(CategoriaEntidadeJpa::new);
        entidade.setNome(categoria.nome());
        entidade.setDescricao(categoria.descricao());
        entidade.setAtivo(categoria.ativo());

        return AdministracaoMapeadorJpa.paraDominio(categoriaJpaRepository.save(entidade));
    }

    @Override
    public Optional<Categoria> buscarPorId(final UUID id) {
        return categoriaJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Categoria> buscarPorNome(final String nome) {
        return categoriaJpaRepository.findByNomeIgnoreCase(nome).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<Categoria> listarTodos() {
        return categoriaJpaRepository.findAllByOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<Categoria> listarAtivas() {
        return categoriaJpaRepository.findByAtivoTrueOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}
