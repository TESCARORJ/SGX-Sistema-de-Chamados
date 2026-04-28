package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;

public interface DepartamentoRepositorio extends RepositorioDominio<Departamento> {

    Optional<Departamento> buscarPorNome(String nome);

    List<Departamento> listarAtivos();
}
