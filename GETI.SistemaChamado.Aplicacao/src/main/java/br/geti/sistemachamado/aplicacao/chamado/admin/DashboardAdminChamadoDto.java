package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.List;

public record DashboardAdminChamadoDto(
        List<IndicadorAdminChamadoDto> porSituacao,
        List<IndicadorAdminChamadoDto> porPrioridade,
        List<IndicadorAdminChamadoDto> porDepartamento,
        List<ChamadoAdminResumoDashboardDto> pendentesRecentes
) {
}
