package br.geti.sistemachamado.infraestrutura.persistencia.entidade;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "usuarios")
public class UsuarioEntidadeJpa extends EntidadeBaseJpa {

    @Column(name = "nome", nullable = false, length = 150)
    private String nome;

    @Column(name = "login", nullable = false, length = 120)
    private String login;

    @Column(name = "email", nullable = false, length = 255)
    private String email;

    @Column(name = "ativo", nullable = false)
    private boolean ativo;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "perfil_acesso_id", nullable = false)
    private PerfilAcessoEntidadeJpa perfilAcesso;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "departamento_id")
    private DepartamentoEntidadeJpa departamento;

    public String getNome() {
        return nome;
    }

    public void setNome(final String nome) {
        this.nome = nome;
    }

    public String getLogin() {
        return login;
    }

    public void setLogin(final String login) {
        this.login = login;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(final String email) {
        this.email = email;
    }

    public boolean isAtivo() {
        return ativo;
    }

    public void setAtivo(final boolean ativo) {
        this.ativo = ativo;
    }

    public PerfilAcessoEntidadeJpa getPerfilAcesso() {
        return perfilAcesso;
    }

    public void setPerfilAcesso(final PerfilAcessoEntidadeJpa perfilAcesso) {
        this.perfilAcesso = perfilAcesso;
    }

    public DepartamentoEntidadeJpa getDepartamento() {
        return departamento;
    }

    public void setDepartamento(final DepartamentoEntidadeJpa departamento) {
        this.departamento = departamento;
    }
}
