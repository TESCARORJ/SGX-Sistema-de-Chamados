package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import br.geti.sistemachamado.dominio.integracaoemail.repositorio.LogDeIntegracaoEmailRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.IntegracaoEmailMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.LogDeIntegracaoEmailEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.CaixaDeEmailJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.LogDeIntegracaoEmailJpaRepository;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class LogDeIntegracaoEmailRepositorioJpa implements LogDeIntegracaoEmailRepositorio {

    private final LogDeIntegracaoEmailJpaRepository logDeIntegracaoEmailJpaRepository;
    private final CaixaDeEmailJpaRepository caixaDeEmailJpaRepository;

    public LogDeIntegracaoEmailRepositorioJpa(
            final LogDeIntegracaoEmailJpaRepository logDeIntegracaoEmailJpaRepository,
            final CaixaDeEmailJpaRepository caixaDeEmailJpaRepository
    ) {
        this.logDeIntegracaoEmailJpaRepository = logDeIntegracaoEmailJpaRepository;
        this.caixaDeEmailJpaRepository = caixaDeEmailJpaRepository;
    }

    @Override
    @Transactional
    public LogDeIntegracaoEmail salvar(final LogDeIntegracaoEmail agregado) {
        final var entidade = logDeIntegracaoEmailJpaRepository.findById(agregado.id())
                .orElseGet(LogDeIntegracaoEmailEntidadeJpa::new);
        entidade.setCaixaEmail(caixaDeEmailJpaRepository.getReferenceById(agregado.caixaDeEmailId()));
        entidade.setMessageId(agregado.messageId());
        entidade.setRemetente(agregado.remetente());
        entidade.setDestinatario(agregado.destinatario());
        entidade.setAssunto(agregado.assunto());
        entidade.setStatusProcessamento(agregado.statusProcessamento());
        entidade.setDetalheProcessamento(agregado.detalheProcessamento());
        entidade.setChaveDeduplicacao(agregado.chaveDeduplicacao());
        entidade.setChamadoId(agregado.chamadoId());
        entidade.setDataProcessamento(agregado.dataProcessamento());

        final var salvo = logDeIntegracaoEmailJpaRepository.saveAndFlush(entidade);
        return IntegracaoEmailMapeadorJpa.paraDominio(salvo);
    }

    @Override
    public Optional<LogDeIntegracaoEmail> buscarPorId(final UUID id) {
        return logDeIntegracaoEmailJpaRepository.findById(id).map(IntegracaoEmailMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<LogDeIntegracaoEmail> buscarPorCaixaEChaveDeduplicacao(
            final UUID caixaDeEmailId,
            final String chaveDeduplicacao
    ) {
        return logDeIntegracaoEmailJpaRepository.findByCaixaEmailIdAndChaveDeduplicacao(
                caixaDeEmailId,
                chaveDeduplicacao
        ).map(IntegracaoEmailMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<LogDeIntegracaoEmail> buscarUltimoComChamadoPorCaixaEMessageId(
            final UUID caixaDeEmailId,
            final String messageId
    ) {
        if (messageId == null || messageId.isBlank()) {
            return Optional.empty();
        }
        return logDeIntegracaoEmailJpaRepository
                .findFirstByCaixaEmailIdAndMessageIdIgnoreCaseAndChamadoIdIsNotNullOrderByDataProcessamentoDesc(
                        caixaDeEmailId,
                        normalizarMessageId(messageId)
                )
                .map(IntegracaoEmailMapeadorJpa::paraDominio);
    }

    private String normalizarMessageId(final String messageId) {
        return messageId.trim()
                .replace("<", "")
                .replace(">", "");
    }
}
