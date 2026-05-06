package br.geti.sistemachamado.aplicacao.chamado.admin;

import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import java.util.UUID;

public record RelatorioOperacionalChamadoAdminFiltroComando(
        UUID departamentoId,
        SituacaoChamado situacao,
        PrioridadeChamado prioridade,
        UUID responsavelId,
        StatusSlaChamado statusSla
) {
}
