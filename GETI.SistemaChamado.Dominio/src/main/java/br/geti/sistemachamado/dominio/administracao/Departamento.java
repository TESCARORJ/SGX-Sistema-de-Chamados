package br.geti.sistemachamado.dominio.administracao;

import java.time.LocalDateTime;
import java.util.UUID;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;

public record Departamento(
        UUID id,
        String nome,
        boolean ativo,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public Departamento {
        ValidadorDominio.obrigatorio(id, "id do departamento e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome do departamento e obrigatorio");
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

