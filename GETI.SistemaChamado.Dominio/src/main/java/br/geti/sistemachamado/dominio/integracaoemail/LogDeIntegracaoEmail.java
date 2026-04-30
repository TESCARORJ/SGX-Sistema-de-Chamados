package br.geti.sistemachamado.dominio.integracaoemail;

import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record LogDeIntegracaoEmail(
        UUID id,
        UUID caixaDeEmailId,
        String messageId,
        String remetente,
        String destinatario,
        String assunto,
        StatusProcessamentoIntegracaoEmail statusProcessamento,
        String detalheProcessamento,
        String chaveDeduplicacao,
        UUID chamadoId,
        LocalDateTime dataProcessamento,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public LogDeIntegracaoEmail {
        ValidadorDominio.obrigatorio(id, "id do log de integracao de email e obrigatorio");
        ValidadorDominio.obrigatorio(caixaDeEmailId, "caixa de email do log de integracao e obrigatoria");
        ValidadorDominio.obrigatorio(statusProcessamento, "status do log de integracao e obrigatorio");
        chaveDeduplicacao = ValidadorDominio.textoObrigatorio(
                chaveDeduplicacao,
                "chave de deduplicacao do log de integracao e obrigatoria"
        );
        ValidadorDominio.obrigatorio(dataProcessamento, "data de processamento do log de integracao e obrigatoria");
        validarAuditoria(dataCriacao, dataAtualizacao);

        messageId = normalizarTexto(messageId);
        remetente = normalizarTexto(remetente);
        destinatario = normalizarTexto(destinatario);
        assunto = normalizarTexto(assunto);
        detalheProcessamento = normalizarTexto(detalheProcessamento);
    }

    private static String normalizarTexto(final String valor) {
        if (valor == null) {
            return null;
        }
        final var normalizado = valor.trim();
        return normalizado.isEmpty() ? null : normalizado;
    }
}

