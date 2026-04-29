package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado;

import br.geti.sistemachamado.dominio.chamado.HistoricoChamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.HistoricoChamadoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado.mapeador.ChamadoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.HistoricoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.ChamadoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.chamado.HistoricoChamadoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class HistoricoChamadoRepositorioJpa implements HistoricoChamadoRepositorio {

    private final HistoricoChamadoJpaRepository historicoChamadoJpaRepository;
    private final ChamadoJpaRepository chamadoJpaRepository;

    public HistoricoChamadoRepositorioJpa(
            final HistoricoChamadoJpaRepository historicoChamadoJpaRepository,
            final ChamadoJpaRepository chamadoJpaRepository
    ) {
        this.historicoChamadoJpaRepository = historicoChamadoJpaRepository;
        this.chamadoJpaRepository = chamadoJpaRepository;
    }

    @Override
    @Transactional
    public HistoricoChamado salvar(final HistoricoChamado agregado) {
        final var entidade = historicoChamadoJpaRepository.findById(agregado.id())
                .orElseGet(HistoricoChamadoEntidadeJpa::new);
        entidade.setChamado(chamadoJpaRepository.getReferenceById(agregado.chamadoId()));
        entidade.setDescricao(agregado.descricao());
        entidade.setSituacaoAnterior(agregado.situacaoAnterior());
        entidade.setSituacaoNova(agregado.situacaoNova());

        return ChamadoMapeadorJpa.paraDominio(historicoChamadoJpaRepository.save(entidade));
    }

    @Override
    public Optional<HistoricoChamado> buscarPorId(final UUID id) {
        return historicoChamadoJpaRepository.findById(id).map(ChamadoMapeadorJpa::paraDominio);
    }

    @Override
    public List<HistoricoChamado> listarPorChamado(final UUID chamadoId) {
        return historicoChamadoJpaRepository.findByChamadoIdOrderByDataCriacaoAsc(chamadoId).stream()
                .map(ChamadoMapeadorJpa::paraDominio)
                .toList();
    }
}
