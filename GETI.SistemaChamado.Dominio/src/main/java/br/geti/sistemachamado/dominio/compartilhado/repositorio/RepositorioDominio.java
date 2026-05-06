package br.geti.sistemachamado.dominio.compartilhado.repositorio;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import java.util.Optional;
import java.util.UUID;

public interface RepositorioDominio<T extends AgregadoRaiz> {

    T salvar(T agregado);

    Optional<T> buscarPorId(UUID id);
}
