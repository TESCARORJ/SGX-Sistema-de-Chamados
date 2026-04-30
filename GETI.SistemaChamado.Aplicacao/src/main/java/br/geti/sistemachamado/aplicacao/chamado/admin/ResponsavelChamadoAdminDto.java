package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.UUID;

public record ResponsavelChamadoAdminDto(
        UUID id,
        String nome,
        String login,
        String perfilAcesso
) {
}
