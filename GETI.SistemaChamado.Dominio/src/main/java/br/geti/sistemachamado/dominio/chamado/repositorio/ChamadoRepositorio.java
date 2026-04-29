package br.geti.sistemachamado.dominio.chamado.repositorio;

import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.Optional;
import java.util.UUID;

public interface ChamadoRepositorio extends RepositorioDominio<Chamado> {

    Optional<Chamado> buscarPorNumero(String numero);

    Optional<Chamado> buscarPorIdESolicitante(UUID chamadoId, UUID solicitanteId);

    List<Chamado> listarPorSolicitante(UUID solicitanteId);
}
