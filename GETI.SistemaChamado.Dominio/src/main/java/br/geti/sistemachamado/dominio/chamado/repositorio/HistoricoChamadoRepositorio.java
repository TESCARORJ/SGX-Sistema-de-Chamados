package br.geti.sistemachamado.dominio.chamado.repositorio;

import br.geti.sistemachamado.dominio.chamado.HistoricoChamado;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.UUID;

public interface HistoricoChamadoRepositorio extends RepositorioDominio<HistoricoChamado> {

    List<HistoricoChamado> listarPorChamado(UUID chamadoId);
}
