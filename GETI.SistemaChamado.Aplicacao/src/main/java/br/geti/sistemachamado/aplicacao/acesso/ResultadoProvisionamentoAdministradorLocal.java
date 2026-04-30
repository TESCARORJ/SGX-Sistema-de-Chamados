package br.geti.sistemachamado.aplicacao.acesso;

import java.util.UUID;

public record ResultadoProvisionamentoAdministradorLocal(
        boolean criado,
        UUID usuarioId,
        String email,
        String motivo
) {
}
