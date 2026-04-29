package br.geti.sistemachamado.dominio.chamado;

import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.compartilhado.AgregadoRaiz;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.UUID;

public record AnexoChamado(
        UUID id,
        UUID chamadoId,
        String nomeArquivo,
        String nomeArmazenado,
        String caminhoArmazenamento,
        String tipoConteudo,
        long tamanhoBytes,
        Usuario autor,
        LocalDateTime dataCriacao,
        LocalDateTime dataAtualizacao
) implements AgregadoRaiz {

    public AnexoChamado {
        ValidadorDominio.obrigatorio(id, "id do anexo do chamado e obrigatorio");
        ValidadorDominio.obrigatorio(chamadoId, "chamado do anexo e obrigatorio");
        nomeArquivo = ValidadorDominio.textoObrigatorio(nomeArquivo, "nome do arquivo e obrigatorio");
        nomeArmazenado = ValidadorDominio.textoObrigatorio(nomeArmazenado, "nome armazenado do anexo e obrigatorio");
        caminhoArmazenamento = ValidadorDominio.textoObrigatorio(
                caminhoArmazenamento,
                "caminho de armazenamento do anexo e obrigatorio"
        );
        tipoConteudo = ValidadorDominio.textoObrigatorio(tipoConteudo, "tipo de conteudo do anexo e obrigatorio");
        if (tamanhoBytes <= 0) {
            throw new br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio(
                    "tamanho do anexo deve ser maior que zero"
            );
        }
        ValidadorDominio.obrigatorio(autor, "autor do anexo e obrigatorio");
        validarAuditoria();
    }
}
