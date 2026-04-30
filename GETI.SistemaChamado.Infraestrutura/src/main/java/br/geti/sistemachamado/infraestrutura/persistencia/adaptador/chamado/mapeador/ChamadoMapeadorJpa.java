package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.chamado.mapeador;

import br.geti.sistemachamado.dominio.chamado.AnexoChamado;
import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.HistoricoChamado;
import br.geti.sistemachamado.dominio.chamado.InteracaoChamado;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.AnexoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.ChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.HistoricoChamadoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado.InteracaoChamadoEntidadeJpa;
import java.time.LocalDateTime;

public final class ChamadoMapeadorJpa {

    private ChamadoMapeadorJpa() {
    }

    public static Chamado paraDominio(final ChamadoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new Chamado(
                entidade.getId(),
                entidade.getNumero(),
                entidade.getTitulo(),
                entidade.getDescricao(),
                entidade.getSituacao(),
                entidade.getPrioridade(),
                entidade.getOrigem(),
                AdministracaoMapeadorJpa.paraDominio(entidade.getSolicitante()),
                entidade.getResponsavel() != null ? AdministracaoMapeadorJpa.paraDominio(entidade.getResponsavel()) : null,
                AdministracaoMapeadorJpa.paraDominio(entidade.getDepartamento()),
                AdministracaoMapeadorJpa.paraDominio(entidade.getCategoria()),
                AdministracaoMapeadorJpa.paraDominio(entidade.getServico()),
                entidade.getPrazoSlaMinutos(),
                entidade.getDataLimiteSla(),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static InteracaoChamado paraDominio(final InteracaoChamadoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new InteracaoChamado(
                entidade.getId(),
                entidade.getChamado().getId(),
                entidade.getTipoInteracao(),
                entidade.getMensagem(),
                entidade.isVisivelSolicitante(),
                AdministracaoMapeadorJpa.paraDominio(entidade.getAutor()),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static HistoricoChamado paraDominio(final HistoricoChamadoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new HistoricoChamado(
                entidade.getId(),
                entidade.getChamado().getId(),
                entidade.getDescricao(),
                entidade.getSituacaoAnterior(),
                entidade.getSituacaoNova(),
                entidade.isVisivelSolicitante(),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static AnexoChamado paraDominio(final AnexoChamadoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new AnexoChamado(
                entidade.getId(),
                entidade.getChamado().getId(),
                entidade.getNomeArquivo(),
                entidade.getNomeArmazenado(),
                entidade.getCaminhoArmazenamento(),
                entidade.getTipoConteudo(),
                entidade.getTamanhoBytes(),
                AdministracaoMapeadorJpa.paraDominio(entidade.getAutor()),
                dataCriacao,
                dataAtualizacao
        );
    }

    private static LocalDateTime normalizarDataCriacao(final LocalDateTime dataCriacao) {
        return dataCriacao != null ? dataCriacao : LocalDateTime.now();
    }

    private static LocalDateTime normalizarDataAtualizacao(
            final LocalDateTime dataCriacao,
            final LocalDateTime dataAtualizacao
    ) {
        if (dataAtualizacao == null) {
            return null;
        }
        return dataAtualizacao.isBefore(dataCriacao) ? dataCriacao : dataAtualizacao;
    }
}
