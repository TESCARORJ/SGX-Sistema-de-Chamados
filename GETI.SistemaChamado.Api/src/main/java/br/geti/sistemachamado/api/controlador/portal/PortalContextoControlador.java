package br.geti.sistemachamado.api.controlador.portal;

import br.geti.sistemachamado.api.configuracao.seguranca.UsuarioAutenticadoPrincipal;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/portal/contexto")
public class PortalContextoControlador {

    @GetMapping
    public String consultar(@AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal) {
        return "Acesso portal autorizado para " + principal.login();
    }
}
