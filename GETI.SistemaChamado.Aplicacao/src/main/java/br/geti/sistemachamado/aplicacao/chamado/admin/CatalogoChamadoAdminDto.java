package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.List;

public record CatalogoChamadoAdminDto(
        List<OpcaoCatalogoAdminDto> departamentos,
        List<OpcaoCatalogoAdminDto> categorias,
        List<OpcaoCatalogoAdminDto> servicos,
        List<ResponsavelChamadoAdminDto> responsaveis,
        List<String> situacoes,
        List<String> prioridades,
        List<String> origens
) {
}
