package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.GrupoAtendimento;
import br.geti.sistemachamado.dominio.administracao.repositorio.GrupoAtendimentoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.GrupoAtendimentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.GrupoAtendimentoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class GrupoAtendimentoRepositorioJpa implements GrupoAtendimentoRepositorio {

    private final GrupoAtendimentoJpaRepository grupoAtendimentoJpaRepository;
    private final DepartamentoJpaRepository departamentoJpaRepository;

    public GrupoAtendimentoRepositorioJpa(
            final GrupoAtendimentoJpaRepository grupoAtendimentoJpaRepository,
            final DepartamentoJpaRepository departamentoJpaRepository
    ) {
        this.grupoAtendimentoJpaRepository = grupoAtendimentoJpaRepository;
        this.departamentoJpaRepository = departamentoJpaRepository;
    }

    @Override
    @Transactional
    public GrupoAtendimento salvar(final GrupoAtendimento grupoAtendimento) {
        final var entidade = grupoAtendimentoJpaRepository.findById(grupoAtendimento.id())
                .orElseGet(GrupoAtendimentoEntidadeJpa::new);
        entidade.setNome(grupoAtendimento.nome());
        entidade.setDescricao(grupoAtendimento.descricao());
        entidade.setAtivo(grupoAtendimento.ativo());
        entidade.setDepartamento(
                departamentoJpaRepository.getReferenceById(grupoAtendimento.departamento().id())
        );

        return AdministracaoMapeadorJpa.paraDominio(grupoAtendimentoJpaRepository.save(entidade));
    }

    @Override
    public Optional<GrupoAtendimento> buscarPorId(final UUID id) {
        return grupoAtendimentoJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<GrupoAtendimento> buscarPorNomeEDepartamento(final String nome, final UUID departamentoId) {
        return grupoAtendimentoJpaRepository.findByNomeIgnoreCaseAndDepartamentoId(nome, departamentoId)
                .map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<GrupoAtendimento> listarTodos() {
        return grupoAtendimentoJpaRepository.findAllByOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<GrupoAtendimento> listarPorDepartamento(final UUID departamentoId) {
        return grupoAtendimentoJpaRepository.findByDepartamentoIdOrderByNomeAsc(departamentoId).stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }

    @Override
    public List<GrupoAtendimento> listarAtivos() {
        return grupoAtendimentoJpaRepository.findByAtivoTrueOrderByNomeAsc().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}
