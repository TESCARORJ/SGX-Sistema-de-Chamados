package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CaixaDeEmailEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.DepartamentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.PerfilAcessoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;

public final class AdministracaoMapeadorJpa {

    private AdministracaoMapeadorJpa() {
    }

    public static Departamento paraDominio(final DepartamentoEntidadeJpa entidade) {
        return new Departamento(
                entidade.getId(),
                entidade.getNome(),
                entidade.isAtivo(),
                entidade.getDataCriacao(),
                entidade.getDataAtualizacao()
        );
    }

    public static PerfilAcesso paraDominio(final PerfilAcessoEntidadeJpa entidade) {
        return new PerfilAcesso(
                entidade.getId(),
                entidade.getNome(),
                entidade.getDescricao(),
                entidade.isAtivo(),
                entidade.getDataCriacao(),
                entidade.getDataAtualizacao()
        );
    }

    public static CaixaDeEmail paraDominio(final CaixaDeEmailEntidadeJpa entidade) {
        return new CaixaDeEmail(
                entidade.getId(),
                entidade.getEnderecoEmail(),
                entidade.getNomeExibicao(),
                entidade.isAtiva(),
                paraDominio(entidade.getDepartamento()),
                entidade.getDataCriacao(),
                entidade.getDataAtualizacao()
        );
    }

    public static Usuario paraDominio(final UsuarioEntidadeJpa entidade) {
        return new Usuario(
                entidade.getId(),
                entidade.getNome(),
                entidade.getLogin(),
                entidade.getEmail(),
                entidade.isAtivo(),
                paraDominio(entidade.getPerfilAcesso()),
                entidade.getDepartamento() != null ? paraDominio(entidade.getDepartamento()) : null,
                entidade.getDataCriacao(),
                entidade.getDataAtualizacao()
        );
    }
}
