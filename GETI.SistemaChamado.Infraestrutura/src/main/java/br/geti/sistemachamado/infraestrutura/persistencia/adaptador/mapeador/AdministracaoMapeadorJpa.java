package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.GrupoAtendimento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CaixaDeEmailEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CategoriaEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.DepartamentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.GrupoAtendimentoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.PerfilAcessoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.ServicoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;
import java.time.LocalDateTime;

public final class AdministracaoMapeadorJpa {

    private AdministracaoMapeadorJpa() {
    }

    public static Departamento paraDominio(final DepartamentoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new Departamento(
                entidade.getId(),
                entidade.getNome(),
                entidade.isAtivo(),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static PerfilAcesso paraDominio(final PerfilAcessoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new PerfilAcesso(
                entidade.getId(),
                entidade.getNome(),
                entidade.getDescricao(),
                entidade.isAtivo(),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static CaixaDeEmail paraDominio(final CaixaDeEmailEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new CaixaDeEmail(
                entidade.getId(),
                entidade.getEnderecoEmail(),
                entidade.getNomeExibicao(),
                entidade.isAtiva(),
                paraDominio(entidade.getDepartamento()),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static Categoria paraDominio(final CategoriaEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new Categoria(
                entidade.getId(),
                entidade.getNome(),
                entidade.getDescricao(),
                entidade.isAtivo(),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static GrupoAtendimento paraDominio(final GrupoAtendimentoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new GrupoAtendimento(
                entidade.getId(),
                entidade.getNome(),
                entidade.getDescricao(),
                entidade.isAtivo(),
                paraDominio(entidade.getDepartamento()),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static Servico paraDominio(final ServicoEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new Servico(
                entidade.getId(),
                entidade.getNome(),
                entidade.getDescricao(),
                entidade.isAtivo(),
                paraDominio(entidade.getCategoria()),
                paraDominio(entidade.getDepartamento()),
                dataCriacao,
                dataAtualizacao
        );
    }

    public static Usuario paraDominio(final UsuarioEntidadeJpa entidade) {
        final var dataCriacao = normalizarDataCriacao(entidade.getDataCriacao());
        final var dataAtualizacao = normalizarDataAtualizacao(dataCriacao, entidade.getDataAtualizacao());
        return new Usuario(
                entidade.getId(),
                entidade.getNome(),
                entidade.getLogin(),
                entidade.getEmail(),
                entidade.isAtivo(),
                paraDominio(entidade.getPerfilAcesso()),
                entidade.getDepartamento() != null ? paraDominio(entidade.getDepartamento()) : null,
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
