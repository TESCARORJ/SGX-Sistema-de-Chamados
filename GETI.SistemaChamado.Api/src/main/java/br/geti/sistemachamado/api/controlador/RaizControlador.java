package br.geti.sistemachamado.api.controlador;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class RaizControlador {

    @GetMapping("/")
    public String consultar() {
        return "Sistema de Chamados CREA-RJ API em execucao";
    }
}
