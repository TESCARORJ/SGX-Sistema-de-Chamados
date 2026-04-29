package br.geti.sistemachamado.aplicacao.chamado.portal;

import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import java.util.UUID;

public record AberturaChamadoPortalComando(
        UUID solicitanteId,
        String titulo,
        String descricao,
        PrioridadeChamado prioridade,
        UUID departamentoId,
        UUID categoriaId,
        UUID servicoId
) {
}
