package br.geti.sistemachamado.api.controlador.admin;

import br.geti.sistemachamado.api.configuracao.seguranca.UsuarioAutenticadoPrincipal;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/admin/contexto")
public class AdminContextoControlador {

    @GetMapping
    public String consultar(@AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal) {
        return "Acesso administrativo autorizado para " + principal.login();
    }
}
