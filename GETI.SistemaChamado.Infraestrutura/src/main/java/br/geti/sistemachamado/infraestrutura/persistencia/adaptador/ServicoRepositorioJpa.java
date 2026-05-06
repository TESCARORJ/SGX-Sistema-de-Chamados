package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.repositorio.ServicoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.ServicoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.CategoriaJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.ServicoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class ServicoRepositorioJpa implements ServicoRepositorio {

    private final ServicoJpaRepository servicoJpaRepository;
    private final CategoriaJpaRepository categoriaJpaRepository;
    private final DepartamentoJpaRepository departamentoJpaRepository;

    public ServicoRepositorioJpa(
            final ServicoJpaRepository servicoJpaRepository,
            final CategoriaJpaRepository categoriaJpaRepository,
            final DepartamentoJpaRepository departamentoJpaRepository
    ) {
        this.servicoJpaRepository = servicoJpaRepository;
        this.categoriaJpaRepository = categoriaJpaRepository;
        this.departamentoJpaRepository = departamentoJpaRepository;
    }

    @Override
    @Transactional
    public Servico salvar(final Servico servico) {
        final var entidade = servicoJpaRepository.findById(servico.id()).orElseGet(ServicoEntidadeJpa::new);
        entidade.setNome(servico.nome());
        entidade.setDescricao(servico.descricao());
        entidade.setAtivo(servico.ativo());
        entidade.setCategoria(categoriaJpaRepository.getReferenceById(servico.categoria().id()));
        entidade.setDepartamento(departamentoJpaRepository.getReferenceById(servico.departamento().id()));

        return AdministracaoMapeadorJpa.paraDominio(servicoJpaRepository.save(entidade));
    }

    @Override
    public Optional<Servico> buscarPorId(final UUID id) {
        return servicoJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Servico> buscarPorNomeEDepartamento(final String nome, final UUID departamentoId) {
        return servicoJpaRepository.findByNomeIgnoreCaseAndDepartamentoId(nome, departamentoId)
                .map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<Servico> listarTodos() {
        return servicoJpaRepository.findAllByOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<Servico> listarPorDepartamento(final UUID departamentoId) {
        return servicoJpaRepository.findByDepartamentoIdOrderByNomeAsc(departamentoId).stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<Servico> listarPorCategoria(final UUID categoriaId) {
        return servicoJpaRepository.findByCategoriaIdOrderByNomeAsc(categoriaId).stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<Servico> listarAtivos() {
        return servicoJpaRepository.findByAtivoTrueOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}
