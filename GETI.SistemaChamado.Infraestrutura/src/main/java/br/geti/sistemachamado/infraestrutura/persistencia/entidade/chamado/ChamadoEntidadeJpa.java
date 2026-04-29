package br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado;

import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CategoriaEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.DepartamentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.EntidadeBaseJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.ServicoEntidadeJpa;
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
@Table(name = "chamados")
public class ChamadoEntidadeJpa extends EntidadeBaseJpa {

    @Column(name = "numero", nullable = false, length = 30)
    private String numero;

    @Column(name = "titulo", nullable = false, length = 150)
    private String titulo;

    @Column(name = "descricao", nullable = false, columnDefinition = "TEXT")
    private String descricao;

    @Enumerated(EnumType.STRING)
    @Column(name = "situacao", nullable = false, length = 40)
    private SituacaoChamado situacao;

    @Enumerated(EnumType.STRING)
    @Column(name = "prioridade", nullable = false, length = 20)
    private PrioridadeChamado prioridade;

    @Enumerated(EnumType.STRING)
    @Column(name = "origem", nullable = false, length = 20)
    private OrigemChamado origem;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "solicitante_id", nullable = false)
    private UsuarioEntidadeJpa solicitante;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "departamento_id", nullable = false)
    private DepartamentoEntidadeJpa departamento;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "categoria_id", nullable = false)
    private CategoriaEntidadeJpa categoria;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "servico_id", nullable = false)
    private ServicoEntidadeJpa servico;

    public String getNumero() {
        return numero;
    }

    public void setNumero(final String numero) {
        this.numero = numero;
    }

    public String getTitulo() {
        return titulo;
    }

    public void setTitulo(final String titulo) {
        this.titulo = titulo;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(final String descricao) {
        this.descricao = descricao;
    }

    public SituacaoChamado getSituacao() {
        return situacao;
    }

    public void setSituacao(final SituacaoChamado situacao) {
        this.situacao = situacao;
    }

    public PrioridadeChamado getPrioridade() {
        return prioridade;
    }

    public void setPrioridade(final PrioridadeChamado prioridade) {
        this.prioridade = prioridade;
    }

    public OrigemChamado getOrigem() {
        return origem;
    }

    public void setOrigem(final OrigemChamado origem) {
        this.origem = origem;
    }

    public UsuarioEntidadeJpa getSolicitante() {
        return solicitante;
    }

    public void setSolicitante(final UsuarioEntidadeJpa solicitante) {
        this.solicitante = solicitante;
    }

    public DepartamentoEntidadeJpa getDepartamento() {
        return departamento;
    }

    public void setDepartamento(final DepartamentoEntidadeJpa departamento) {
        this.departamento = departamento;
    }

    public CategoriaEntidadeJpa getCategoria() {
        return categoria;
    }

    public void setCategoria(final CategoriaEntidadeJpa categoria) {
        this.categoria = categoria;
    }

    public ServicoEntidadeJpa getServico() {
        return servico;
    }

    public void setServico(final ServicoEntidadeJpa servico) {
        this.servico = servico;
    }
}
