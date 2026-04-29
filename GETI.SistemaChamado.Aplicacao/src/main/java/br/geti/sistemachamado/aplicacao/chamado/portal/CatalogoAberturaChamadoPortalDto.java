package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.util.List;

public record CatalogoAberturaChamadoPortalDto(
        List<OpcaoCatalogoPortalDto> departamentos,
        List<OpcaoCatalogoPortalDto> categorias,
        List<OpcaoCatalogoPortalDto> servicos
) {
}
