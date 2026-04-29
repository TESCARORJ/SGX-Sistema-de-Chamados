package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado;

import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.ChamadoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado.mapeador.ChamadoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.ChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.CategoriaJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.ServicoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.UsuarioJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.ChamadoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class ChamadoRepositorioJpa implements ChamadoRepositorio {

    private final ChamadoJpaRepository chamadoJpaRepository;
    private final UsuarioJpaRepository usuarioJpaRepository;
    private final DepartamentoJpaRepository departamentoJpaRepository;
    private final CategoriaJpaRepository categoriaJpaRepository;
    private final ServicoJpaRepository servicoJpaRepository;

    public ChamadoRepositorioJpa(
            final ChamadoJpaRepository chamadoJpaRepository,
            final UsuarioJpaRepository usuarioJpaRepository,
            final DepartamentoJpaRepository departamentoJpaRepository,
            final CategoriaJpaRepository categoriaJpaRepository,
            final ServicoJpaRepository servicoJpaRepository
    ) {
        this.chamadoJpaRepository = chamadoJpaRepository;
        this.usuarioJpaRepository = usuarioJpaRepository;
        this.departamentoJpaRepository = departamentoJpaRepository;
        this.categoriaJpaRepository = categoriaJpaRepository;
        this.servicoJpaRepository = servicoJpaRepository;
    }

    @Override
    @Transactional
    public Chamado salvar(final Chamado chamado) {
        final var entidade = chamadoJpaRepository.findById(chamado.id()).orElseGet(ChamadoEntidadeJpa::new);
        entidade.setNumero(chamado.numero());
        entidade.setTitulo(chamado.titulo());
        entidade.setDescricao(chamado.descricao());
        entidade.setSituacao(chamado.situacao());
        entidade.setPrioridade(chamado.prioridade());
        entidade.setOrigem(chamado.origem());
        entidade.setSolicitante(usuarioJpaRepository.getReferenceById(chamado.solicitante().id()));
        entidade.setDepartamento(departamentoJpaRepository.getReferenceById(chamado.departamento().id()));
        entidade.setCategoria(categoriaJpaRepository.getReferenceById(chamado.categoria().id()));
        entidade.setServico(servicoJpaRepository.getReferenceById(chamado.servico().id()));

        return ChamadoMapeadorJpa.paraDominio(chamadoJpaRepository.save(entidade));
    }

    @Override
    public Optional<Chamado> buscarPorId(final UUID id) {
        return chamadoJpaRepository.findById(id).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Chamado> buscarPorNumero(final String numero) {
        return chamadoJpaRepository.findByNumero(numero).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Chamado> buscarPorIdESolicitante(final UUID chamadoId, final UUID solicitanteId) {
        return chamadoJpaRepository.findByIdAndSolicitanteId(chamadoId, solicitanteId).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public List<Chamado> listarPorSolicitante(final UUID solicitanteId) {
        return chamadoJpaRepository.findBySolicitanteIdOrderByDataCriacaoDesc(solicitanteId).stream()
                .map(ChamadoMapeadorJpa::paraDominio)
                .toList();
    }
}
