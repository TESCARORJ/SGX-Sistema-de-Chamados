package br.geti.sistemachamado.dominio.administracao;

import java.time.LocalDateTime;
import java.util.UUID;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;

public record CaixaDeEmail(
        UUID id,
        String enderecoEmail,
        String nomeExibicao,
        boolean ativa,
        Departamento departamento,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public CaixaDeEmail {
        ValidadorDominio.obrigatorio(id, "id da caixa de email e obrigatorio");
        enderecoEmail = ValidadorDominio.textoObrigatorio(enderecoEmail, "enderecoEmail da caixa de email e obrigatorio");
        nomeExibicao = ValidadorDominio.textoObrigatorio(nomeExibicao, "nomeExibicao da caixa de email e obrigatorio");
        ValidadorDominio.obrigatorio(departamento, "departamento da caixa de email e obrigatorio");
        validarAuditoria();
    }
}
