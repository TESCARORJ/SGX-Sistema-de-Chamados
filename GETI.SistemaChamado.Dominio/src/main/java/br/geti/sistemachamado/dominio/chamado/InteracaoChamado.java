package br.geti.sistemachamado.dominio.chamado;

import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record InteracaoChamado(
        UUID id,
        UUID chamadoId,
        TipoInteracao tipoInteracao,
        String mensagem,
        boolean visivelSolicitante,
        Usuario autor,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public InteracaoChamado {
        ValidadorDominio.obrigatorio(id, "id da interacao do chamado e obrigatorio");
        ValidadorDominio.obrigatorio(chamadoId, "chamado da interacao e obrigatorio");
        ValidadorDominio.obrigatorio(tipoInteracao, "tipo da interacao e obrigatorio");
        mensagem = ValidadorDominio.textoObrigatorio(mensagem, "mensagem da interacao e obrigatoria");
        ValidadorDominio.obrigatorio(autor, "autor da interacao e obrigatorio");
        validarAuditoria(dataCriacao, dataAtualizacao);
    }
}

