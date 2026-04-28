package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ServicoRepositorio extends RepositorioDominio<Servico> {

    Optional<Servico> buscarPorNomeEDepartamento(String nome, UUID departamentoId);

    List<Servico> listarTodos();

    List<Servico> listarPorDepartamento(UUID departamentoId);

    List<Servico> listarPorCategoria(UUID categoriaId);

    List<Servico> listarAtivos();
}
