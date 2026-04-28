package br.geti.sistemachamado.api.controlador.acesso;

import br.geti.sistemachamado.api.configuracao.seguranca.UsuarioAutenticadoPrincipal;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/me")
public class UsuarioAtualControlador {

    @GetMapping
    public UsuarioAtualResposta obterUsuarioAtual(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal
    ) {
        return new UsuarioAtualResposta(
                principal.usuarioId(),
                principal.login(),
                principal.nome(),
                principal.email(),
                principal.perfilAcesso(),
                principal.departamentoId()
        );
    }
}
