package br.geti.sistemachamado.dominio.chamado.repositorio;

import br.geti.sistemachamado.dominio.chamado.AnexoChamado;
import br.geti.sistemachamado.dominio.compartilhado.repositorio.RepositorioDominio;
import java.util.List;
import java.util.UUID;

public interface AnexoChamadoRepositorio extends RepositorioDominio<AnexoChamado> {

    List<AnexoChamado> listarPorChamado(UUID chamadoId);
}
