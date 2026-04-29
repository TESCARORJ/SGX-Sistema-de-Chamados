package br.geti.sistemachamado.dominio.chamado.repositorio;

import br.geti.sistemachamado.dominio.chamado.InteracaoChamado;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.UUID;

public interface InteracaoChamadoRepositorio extends RepositorioDominio<InteracaoChamado> {

    List<InteracaoChamado> listarPorChamado(UUID chamadoId);
}
