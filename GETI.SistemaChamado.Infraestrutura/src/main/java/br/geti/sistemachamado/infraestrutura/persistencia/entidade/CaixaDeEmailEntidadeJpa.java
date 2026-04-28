package br.geti.sistemachamado.infraestrutura.persistencia.entidade;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "caixas_email")
public class CaixaDeEmailEntidadeJpa extends EntidadeBaseJpa {

    @Column(name = "endereco_email", nullable = false, length = 255)
    private String enderecoEmail;

    @Column(name = "nome_exibicao", nullable = false, length = 150)
    private String nomeExibicao;

    @Column(name = "ativa", nullable = false)
    private boolean ativa;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "departamento_id", nullable = false)
    private DepartamentoEntidadeJpa departamento;

    public String getEnderecoEmail() {
        return enderecoEmail;
    }

    public void setEnderecoEmail(final String enderecoEmail) {
        this.enderecoEmail = enderecoEmail;
    }

    public String getNomeExibicao() {
        return nomeExibicao;
    }

    public void setNomeExibicao(final String nomeExibicao) {
        this.nomeExibicao = nomeExibicao;
    }

    public boolean isAtiva() {
        return ativa;
    }

    public void setAtiva(final boolean ativa) {
        this.ativa = ativa;
    }

    public DepartamentoEntidadeJpa getDepartamento() {
        return departamento;
    }

    public void setDepartamento(final DepartamentoEntidadeJpa departamento) {
        this.departamento = departamento;
    }
}