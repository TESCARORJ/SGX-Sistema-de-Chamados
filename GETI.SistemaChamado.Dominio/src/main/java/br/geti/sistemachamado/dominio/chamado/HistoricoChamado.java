package br.geti.sistemachamado.dominio.chamado;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record HistoricoChamado(
        UUID id,
        UUID chamadoId,
        String descricao,
        SituacaoChamado situacaoAnterior,
        SituacaoChamado situacaoNova,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public HistoricoChamado {
        ValidadorDominio.obrigatorio(id, "id do historico do chamado e obrigatorio");
        ValidadorDominio.obrigatorio(chamadoId, "chamado do historico e obrigatorio");
        descricao = ValidadorDominio.textoObrigatorio(descricao, "descricao do historico e obrigatoria");
        ValidadorDominio.obrigatorio(situacaoNova, "situacao nova do historico e obrigatoria");
        validarAuditoria();
    }
}
