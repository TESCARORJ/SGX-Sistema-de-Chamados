package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.UUID;

public record OpcaoCatalogoAdminDto(
        UUID id,
        String nome
) {
}
