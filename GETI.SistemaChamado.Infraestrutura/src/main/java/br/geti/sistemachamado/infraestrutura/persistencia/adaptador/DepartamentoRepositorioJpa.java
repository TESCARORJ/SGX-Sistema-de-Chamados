package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.DepartamentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class DepartamentoRepositorioJpa implements DepartamentoRepositorio {

    private final DepartamentoJpaRepository departamentoJpaRepository;

    public DepartamentoRepositorioJpa(final DepartamentoJpaRepository departamentoJpaRepository) {
        this.departamentoJpaRepository = departamentoJpaRepository;
    }

    @Override
    @Transactional
    public Departamento salvar(final Departamento departamento) {
        final var entidade = departamentoJpaRepository.findById(departamento.id()).orElseGet(DepartamentoEntidadeJpa::new);
        entidade.setNome(departamento.nome());
        entidade.setAtivo(departamento.ativo());

        final var salva = departamentoJpaRepository.save(entidade);
        return AdministracaoMapeadorJpa.paraDominio(salva);
    }

    @Override
    public Optional<Departamento> buscarPorId(final UUID id) {
        return departamentoJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Departamento> buscarPorNome(final String nome) {
        return departamentoJpaRepository.findByNomeIgnoreCase(nome).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<Departamento> listarTodos() {
        return departamentoJpaRepository.findAllByOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<Departamento> listarAtivos() {
        return departamentoJpaRepository.findByAtivoTrueOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}
