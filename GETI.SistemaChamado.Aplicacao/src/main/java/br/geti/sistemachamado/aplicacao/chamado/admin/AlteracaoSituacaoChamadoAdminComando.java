package br.geti.sistemachamado.aplicacao.chamado.admin;

import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import java.util.UUID;

public record AlteracaoSituacaoChamadoAdminComando(
        UUID chamadoId,
        SituacaoChamado novaSituacao,
        UUID agenteId
) {
}
