package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.List;

public record RelatorioOperacionalChamadoAdminDto(
        List<IndicadorAdminChamadoDto> porDepartamento,
        List<IndicadorAdminChamadoDto> porSituacao,
        List<IndicadorAdminChamadoDto> porPrioridade,
        List<IndicadorAdminChamadoDto> porResponsavel,
        List<IndicadorAdminChamadoDto> porStatusSla,
        List<ChamadoAdminResumoDashboardDto> chamadosVencidosSla,
        List<ChamadoAdminResumoDashboardDto> chamadosProximosVencimentoSla
) {
}
