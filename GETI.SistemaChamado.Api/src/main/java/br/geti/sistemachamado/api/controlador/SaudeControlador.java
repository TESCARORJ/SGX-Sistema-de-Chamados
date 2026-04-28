package br.geti.sistemachamado.api.controlador;

import br.geti.sistemachamado.aplicacao.saude.ConsultaSaudeSistema;
import br.geti.sistemachamado.aplicacao.saude.EstadoSaudeSistema;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/saude")
public class SaudeControlador {

    private final ConsultaSaudeSistema consultaSaudeSistema;

    public SaudeControlador(final ConsultaSaudeSistema consultaSaudeSistema) {
        this.consultaSaudeSistema = consultaSaudeSistema;
    }

    @GetMapping
    public EstadoSaudeSistema consultar() {
        return consultaSaudeSistema.consultar();
    }
}

