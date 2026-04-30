package br.geti.sistemachamado.worker.email.integracao;

import java.util.List;

public interface LeitorMensagensEmail {

    List<MensagemEmailRecebida> listarMensagensElegiveis();
}

