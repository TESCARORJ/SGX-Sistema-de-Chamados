package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;

public interface UsuarioRepositorio extends RepositorioDominio<Usuario> {

    Optional<Usuario> buscarPorLogin(String login);

    Optional<Usuario> buscarPorEmail(String email);

    List<Usuario> listarAtivos();
}
