package br.geti.sistemachamado.aplicacao.saude;

import java.time.OffsetDateTime;

public class ConsultarSaudeSistemaPadrao implements ConsultaSaudeSistema {

    private final String nomeServico;
    private final String ambiente;

    public ConsultarSaudeSistemaPadrao(final String nomeServico, final String ambiente) {
        this.nomeServico = nomeServico;
        this.ambiente = ambiente;
    }

    @Override
    public EstadoSaudeSistema consultar() {
        return new EstadoSaudeSistema(nomeServico, ambiente, OffsetDateTime.now());
    }
}

