package br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado;

import br.geti.sistemachamado.dominio.chamado.TipoInteracao;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.EntidadeBaseJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "interacoes_chamado")
public class InteracaoChamadoEntidadeJpa extends EntidadeBaseJpa {

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "chamado_id", nullable = false)
    private ChamadoEntidadeJpa chamado;

    @Enumerated(EnumType.STRING)
    @Column(name = "tipo_interacao", nullable = false, length = 40)
    private TipoInteracao tipoInteracao;

    @Column(name = "mensagem", nullable = false, columnDefinition = "TEXT")
    private String mensagem;

    @Column(name = "visivel_solicitante", nullable = false)
    private boolean visivelSolicitante;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "autor_usuario_id", nullable = false)
    private UsuarioEntidadeJpa autor;

    public ChamadoEntidadeJpa getChamado() {
        return chamado;
    }

    public void setChamado(final ChamadoEntidadeJpa chamado) {
        this.chamado = chamado;
    }

    public TipoInteracao getTipoInteracao() {
        return tipoInteracao;
    }

    public void setTipoInteracao(final TipoInteracao tipoInteracao) {
        this.tipoInteracao = tipoInteracao;
    }

    public String getMensagem() {
        return mensagem;
    }

    public void setMensagem(final String mensagem) {
        this.mensagem = mensagem;
    }

    public boolean isVisivelSolicitante() {
        return visivelSolicitante;
    }

    public void setVisivelSolicitante(final boolean visivelSolicitante) {
        this.visivelSolicitante = visivelSolicitante;
    }

    public UsuarioEntidadeJpa getAutor() {
        return autor;
    }

    public void setAutor(final UsuarioEntidadeJpa autor) {
        this.autor = autor;
    }
}
