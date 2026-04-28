package br.geti.sistemachamado.dominio.administracao.repositorio;

import br.geti.sistemachamado.dominio.administracao.GrupoAtendimento;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface GrupoAtendimentoRepositorio extends RepositorioDominio<GrupoAtendimento> {

    Optional<GrupoAtendimento> buscarPorNomeEDepartamento(String nome, UUID departamentoId);

    List<GrupoAtendimento> listarTodos();

    List<GrupoAtendimento> listarPorDepartamento(UUID departamentoId);

    List<GrupoAtendimento> listarAtivos();
}
