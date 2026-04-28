package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;

public interface CategoriaRepositorio extends RepositorioDominio<Categoria> {

    Optional<Categoria> buscarPorNome(String nome);

    List<Categoria> listarTodos();

    List<Categoria> listarAtivas();
}
