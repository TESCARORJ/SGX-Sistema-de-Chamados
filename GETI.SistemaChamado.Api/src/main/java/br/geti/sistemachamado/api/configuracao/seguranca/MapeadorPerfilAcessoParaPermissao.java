package br.geti.sistemachamado.api.configuracao.seguranca;

import java.util.List;
import java.util.Locale;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.stereotype.Component;

@Component
public class MapeadorPerfilAcessoParaPermissao {

    public List<GrantedAuthority> mapearPermissoes(final String perfilAcesso) {
        final var perfilNormalizado = perfilAcesso == null ? "" : perfilAcesso.trim().toLowerCase(Locale.ROOT);

        return switch (perfilNormalizado) {
            case "solicitante" -> List.of(new SimpleGrantedAuthority("ROLE_SOLICITANTE"));
            case "atendente" -> List.of(new SimpleGrantedAuthority("ROLE_ATENDENTE"));
            case "supervisor" -> List.of(new SimpleGrantedAuthority("ROLE_SUPERVISOR"));
            case "administrador" -> List.of(new SimpleGrantedAuthority("ROLE_ADMINISTRADOR"));
            default -> List.of();
        };
    }
}
