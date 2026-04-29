package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado;

import br.geti.sistemachamado.dominio.chamado.InteracaoChamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.InteracaoChamadoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado.mapeador.ChamadoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.InteracaoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.UsuarioJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.ChamadoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.InteracaoChamadoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class InteracaoChamadoRepositorioJpa implements InteracaoChamadoRepositorio {

    private final InteracaoChamadoJpaRepository interacaoChamadoJpaRepository;
    private final ChamadoJpaRepository chamadoJpaRepository;
    private final UsuarioJpaRepository usuarioJpaRepository;

    public InteracaoChamadoRepositorioJpa(
            final InteracaoChamadoJpaRepository interacaoChamadoJpaRepository,
            final ChamadoJpaRepository chamadoJpaRepository,
            final UsuarioJpaRepository usuarioJpaRepository
    ) {
        this.interacaoChamadoJpaRepository = interacaoChamadoJpaRepository;
        this.chamadoJpaRepository = chamadoJpaRepository;
        this.usuarioJpaRepository = usuarioJpaRepository;
    }

    @Override
    @Transactional
    public InteracaoChamado salvar(final InteracaoChamado agregado) {
        final var entidade = interacaoChamadoJpaRepository.findById(agregado.id())
                .orElseGet(InteracaoChamadoEntidadeJpa::new);
        entidade.setChamado(chamadoJpaRepository.getReferenceById(agregado.chamadoId()));
        entidade.setTipoInteracao(agregado.tipoInteracao());
        entidade.setMensagem(agregado.mensagem());
        entidade.setVisivelSolicitante(agregado.visivelSolicitante());
        entidade.setAutor(usuarioJpaRepository.getReferenceById(agregado.autor().id()));

        return ChamadoMapeadorJpa.paraDominio(interacaoChamadoJpaRepository.save(entidade));
    }

    @Override
    public Optional<InteracaoChamado> buscarPorId(final UUID id) {
        return interacaoChamadoJpaRepository.findById(id).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public List<InteracaoChamado> listarPorChamado(final UUID chamadoId) {
        return interacaoChamadoJpaRepository.findByChamadoIdOrderByDataCriacaoAsc(chamadoId).stream()
                .map(ChamadoMapeadorJpa::paraDominio)
                .toList();
    }
}
