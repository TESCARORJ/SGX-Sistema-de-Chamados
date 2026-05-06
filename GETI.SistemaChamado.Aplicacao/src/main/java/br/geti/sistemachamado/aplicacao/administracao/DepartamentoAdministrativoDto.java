package br.geti.sistemachamado.aplicacao.administracao;

import java.time.LocalDateTime;
import java.util.UUID;

public record DepartamentoAdministrativoDto(
        UUID id,
        String nome,
        boolean ativo,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
