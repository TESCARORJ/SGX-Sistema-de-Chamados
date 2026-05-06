package br.geti.sistemachamado.aplicacao.chamado.admin;

import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import java.util.UUID;

public record ChamadoAdminFiltroFilaComando(
        SituacaoChamado situacao,
        PrioridadeChamado prioridade,
        UUID departamentoId,
        OrigemChamado origem,
        UUID responsavelId,
        StatusSlaChamado statusSla
) {
}
