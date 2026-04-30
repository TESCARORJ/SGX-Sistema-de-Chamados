package br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado;

import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.EntidadeBaseJpa;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "historicos_chamado")
public class HistoricoChamadoEntidadeJpa extends EntidadeBaseJpa {

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "chamado_id", nullable = false)
    private ChamadoEntidadeJpa chamado;

    @Column(name = "descricao", nullable = false, length = 255)
    private String descricao;

    @Enumerated(EnumType.STRING)
    @Column(name = "situacao_anterior", length = 40)
    private SituacaoChamado situacaoAnterior;

    @Enumerated(EnumType.STRING)
    @Column(name = "situacao_nova", nullable = false, length = 40)
    private SituacaoChamado situacaoNova;

    @Column(name = "visivel_solicitante", nullable = false)
    private boolean visivelSolicitante;

    public ChamadoEntidadeJpa getChamado() {
        return chamado;
    }

    public void setChamado(final ChamadoEntidadeJpa chamado) {
        this.chamado = chamado;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(final String descricao) {
        this.descricao = descricao;
    }

    public SituacaoChamado getSituacaoAnterior() {
        return situacaoAnterior;
    }

    public void setSituacaoAnterior(final SituacaoChamado situacaoAnterior) {
        this.situacaoAnterior = situacaoAnterior;
    }

    public SituacaoChamado getSituacaoNova() {
        return situacaoNova;
    }

    public void setSituacaoNova(final SituacaoChamado situacaoNova) {
        this.situacaoNova = situacaoNova;
    }

    public boolean isVisivelSolicitante() {
        return visivelSolicitante;
    }

    public void setVisivelSolicitante(final boolean visivelSolicitante) {
        this.visivelSolicitante = visivelSolicitante;
    }
}
