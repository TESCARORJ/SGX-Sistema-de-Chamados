package br.geti.sistemachamado.infraestrutura.persistencia.entidade;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "grupos_atendimento")
public class GrupoAtendimentoEntidadeJpa extends EntidadeBaseJpa {

    @Column(name = "nome", nullable = false, length = 120)
    private String nome;

    @Column(name = "descricao", length = 255)
    private String descricao;

    @Column(name = "ativo", nullable = false)
    private boolean ativo;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "departamento_id", nullable = false)
    private DepartamentoEntidadeJpa departamento;

    public String getNome() {
        return nome;
    }

    public void setNome(final String nome) {
        this.nome = nome;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(final String descricao) {
        this.descricao = descricao;
    }

    public boolean isAtivo() {
        return ativo;
    }

    public void setAtivo(final boolean ativo) {
        this.ativo = ativo;
    }

    public DepartamentoEntidadeJpa getDepartamento() {
        return departamento;
    }

    public void setDepartamento(final DepartamentoEntidadeJpa departamento) {
        this.departamento = departamento;
    }
}
