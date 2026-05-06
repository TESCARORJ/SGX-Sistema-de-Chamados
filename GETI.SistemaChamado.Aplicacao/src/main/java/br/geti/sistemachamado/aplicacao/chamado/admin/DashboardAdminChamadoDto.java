package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.List;

public record DashboardAdminChamadoDto(
        List<IndicadorAdminChamadoDto> porSituacao,
        List<IndicadorAdminChamadoDto> porPrioridade,
        List<IndicadorAdminChamadoDto> porDepartamento,
        List<IndicadorAdminChamadoDto> porResponsavel,
        List<IndicadorAdminChamadoDto> porStatusSla,
        long totalVencidosSla,
        long totalProximosVencimentoSla,
        List<ChamadoAdminResumoDashboardDto> pendentesRecentes,
        List<ChamadoAdminResumoDashboardDto> chamadosVencidosSla,
        List<ChamadoAdminResumoDashboardDto> chamadosProximosVencimentoSla
) {
}
