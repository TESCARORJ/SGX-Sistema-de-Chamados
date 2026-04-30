package br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador;

import br.geti.sistemachamado.dominio.integracaoemail.LogDeIntegracaoEmail;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.LogDeIntegracaoEmailEntidadeJpa;

public final class IntegracaoEmailMapeadorJpa {

    private IntegracaoEmailMapeadorJpa() {
    }

    public static LogDeIntegracaoEmail paraDominio(final LogDeIntegracaoEmailEntidadeJpa entidade) {
        return new LogDeIntegracaoEmail(
                entidade.getId(),
                entidade.getCaixaEmail().getId(),
                entidade.getMessageId(),
                entidade.getRemetente(),
                entidade.getDestinatario(),
                entidade.getAssunto(),
                entidade.getStatusProcessamento(),
                entidade.getDetalheProcessamento(),
                entidade.getChaveDeduplicacao(),
                entidade.getChamadoId(),
                entidade.getDataProcessamento(),
                entidade.getDataCriacao(),
                entidade.getDataAtualizacao()
        );
    }
}

