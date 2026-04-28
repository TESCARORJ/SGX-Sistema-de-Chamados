package br.geti.sistemachamado.api.controlador.acesso;

import br.geti.sistemachamado.aplicacao.acesso.ComandoSincronizacaoUsuarioAutenticado;
import br.geti.sistemachamado.aplicacao.acesso.SincronizarUsuarioAutenticado;
import jakarta.validation.Valid;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/tecnico/acesso/usuarios")
public class SincronizacaoUsuarioAutenticadoControlador {

    private final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado;

    public SincronizacaoUsuarioAutenticadoControlador(
            final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado
    ) {
        this.sincronizarUsuarioAutenticado = sincronizarUsuarioAutenticado;
    }

    @PostMapping("/sincronizacao")
    public SincronizarUsuarioAutenticadoResposta sincronizar(
            @Valid @RequestBody final SincronizarUsuarioAutenticadoRequisicao requisicao
    ) {
        final var resultado = sincronizarUsuarioAutenticado.sincronizar(
                new ComandoSincronizacaoUsuarioAutenticado(
                        requisicao.nome(),
                        requisicao.login(),
                        requisicao.email(),
                        requisicao.departamentoId()
                )
        );

        return new SincronizarUsuarioAutenticadoResposta(
                resultado.usuarioId(),
                resultado.nome(),
                resultado.login(),
                resultado.email(),
                resultado.perfilAcesso(),
                resultado.departamentoId(),
                resultado.criado()
        );
    }
}
