package br.geti.sistemachamado.api.controlador.acesso;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotBlank;
import java.util.UUID;

public record SincronizarUsuarioAutenticadoRequisicao(
        @NotBlank(message = "deve ser informado")
        String nome,
        @NotBlank(message = "deve ser informado")
        String login,
        @NotBlank(message = "deve ser informado")
        @Email(message = "deve ser um e-mail valido")
        String email,
        UUID departamentoId
) {
}
