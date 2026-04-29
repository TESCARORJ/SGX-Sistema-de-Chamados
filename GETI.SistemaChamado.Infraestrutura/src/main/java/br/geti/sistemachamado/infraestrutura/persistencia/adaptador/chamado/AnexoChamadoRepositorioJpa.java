package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado;

import br.geti.sistemachamado.dominio.chamado.AnexoChamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.AnexoChamadoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado.mapeador.ChamadoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.AnexoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.UsuarioJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.AnexoChamadoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.ChamadoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class AnexoChamadoRepositorioJpa implements AnexoChamadoRepositorio {

    private final AnexoChamadoJpaRepository anexoChamadoJpaRepository;
    private final ChamadoJpaRepository chamadoJpaRepository;
    private final UsuarioJpaRepository usuarioJpaRepository;

    public AnexoChamadoRepositorioJpa(
            final AnexoChamadoJpaRepository anexoChamadoJpaRepository,
            final ChamadoJpaRepository chamadoJpaRepository,
            final UsuarioJpaRepository usuarioJpaRepository
    ) {
        this.anexoChamadoJpaRepository = anexoChamadoJpaRepository;
        this.chamadoJpaRepository = chamadoJpaRepository;
        this.usuarioJpaRepository = usuarioJpaRepository;
    }

    @Override
    @Transactional
    public AnexoChamado salvar(final AnexoChamado agregado) {
        final var entidade = anexoChamadoJpaRepository.findById(agregado.id())
                .orElseGet(AnexoChamadoEntidadeJpa::new);
        entidade.setChamado(chamadoJpaRepository.getReferenceById(agregado.chamadoId()));
        entidade.setNomeArquivo(agregado.nomeArquivo());
        entidade.setNomeArmazenado(agregado.nomeArmazenado());
        entidade.setCaminhoArmazenamento(agregado.caminhoArmazenamento());
        entidade.setTipoConteudo(agregado.tipoConteudo());
        entidade.setTamanhoBytes(agregado.tamanhoBytes());
        entidade.setAutor(usuarioJpaRepository.getReferenceById(agregado.autor().id()));

        return ChamadoMapeadorJpa.paraDominio(anexoChamadoJpaRepository.save(entidade));
    }

    @Override
    public Optional<AnexoChamado> buscarPorId(final UUID id) {
        return anexoChamadoJpaRepository.findById(id).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public List<AnexoChamado> listarPorChamado(final UUID chamadoId) {
        return anexoChamadoJpaRepository.findByChamadoIdOrderByDataCriacaoAsc(chamadoId).stream()
                .map(ChamadoMapeadorJpa::paraDominio)
                .toList();
    }
}
