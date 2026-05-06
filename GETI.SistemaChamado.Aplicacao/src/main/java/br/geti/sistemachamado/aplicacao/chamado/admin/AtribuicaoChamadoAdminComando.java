package br.geti.sistemachamado.aplicacao.chamado.admin;

import java.util.UUID;

public record AtribuicaoChamadoAdminComando(
        UUID chamadoId,
        UUID responsavelId,
        UUID agenteId
) {
}
