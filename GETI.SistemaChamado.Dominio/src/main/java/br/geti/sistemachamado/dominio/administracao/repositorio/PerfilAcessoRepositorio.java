package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;

public interface PerfilAcessoRepositorio extends RepositorioDominio<PerfilAcesso> {

    Optional<PerfilAcesso> buscarPorNome(String nome);

    List<PerfilAcesso> listarAtivos();
}
