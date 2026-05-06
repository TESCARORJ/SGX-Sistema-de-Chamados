package br.geti.sistemachamado.dominio.compartilhado;

import java.time.LocalDateTime;

public interface Auditavel {

    LocalDateTime dataCriacao();

    LocalDateTime dataAtualizacao();

    default void validarAuditoria() {
        validarAuditoria(dataCriacao(), dataAtualizacao());
    }

    default void validarAuditoria(
            final LocalDateTime dataCriacao,
            final LocalDateTime dataAtualizacao
    ) {
        ValidadorDominio.obrigatorio(dataCriacao, "dataCriacao e obrigatoria");
        if (dataAtualizacao != null && dataAtualizacao.isBefore(dataCriacao)) {
            throw new ErroDeDominio("dataAtualizacao nao pode ser anterior a dataCriacao");
        }
    }
}
