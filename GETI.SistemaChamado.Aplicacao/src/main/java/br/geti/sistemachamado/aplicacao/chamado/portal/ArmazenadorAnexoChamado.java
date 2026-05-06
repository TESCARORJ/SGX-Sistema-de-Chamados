package br.geti.sistemachamado.aplicacao.chamado.portal;

import java.util.UUID;

public interface ArmazenadorAnexoChamado {

    AnexoArmazenadoChamado armazenar(
            UUID chamadoId,
            UUID anexoId,
            String nomeArquivo,
            byte[] conteudo
    );
}
