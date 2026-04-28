package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;

public interface CaixaDeEmailRepositorio extends RepositorioDominio<CaixaDeEmail> {

    Optional<CaixaDeEmail> buscarPorEnderecoEmail(String enderecoEmail);

    List<CaixaDeEmail> listarAtivas();
}
