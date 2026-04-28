package br.geti.sistemachamado.aplicacao.administracao;

import java.time.LocalDateTime;
import java.util.UUID;

public record CaixaDeEmailAdministrativaDto(
        UUID id,
        String enderecoEmail,
        String nomeExibicao,
        boolean ativa,
        UUID departamentoId,
        String departamentoNome,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) {
}
