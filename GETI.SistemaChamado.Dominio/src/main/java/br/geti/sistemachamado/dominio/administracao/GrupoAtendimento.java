package br.geti.sistemachamado.dominio.administracao;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record GrupoAtendimento(
        UUID id,
        String nome,
        String descricao,
        boolean ativo,
        Departamento departamento,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public GrupoAtendimento {
        ValidadorDominio.obrigatorio(id, "id do grupo de atendimento e obrigatorio");
        nome = ValidadorDominio.textoObrigatorio(nome, "nome do grupo de atendimento e obrigatorio");
        ValidadorDominio.obrigatorio(departamento, "departamento do grupo de atendimento e obrigatorio");
        if (descricao != null) {
            descricao = descricao.trim();
            if (descricao.isEmpty()) {
                descricao = null;
            }
        }
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

