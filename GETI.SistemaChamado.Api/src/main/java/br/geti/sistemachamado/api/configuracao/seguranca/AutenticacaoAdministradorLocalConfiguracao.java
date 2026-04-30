package br.geti.sistemachamado.api.configuracao.seguranca;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationProvider;
import org.springframework.security.authentication.dao.DaoAuthenticationProvider;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;

@Configuration
public class AutenticacaoAdministradorLocalConfiguracao {

    @Bean
    public PasswordEncoder passwordEncoder() {
        return new BCryptPasswordEncoder();
    }

    @Bean
    public AuthenticationProvider autenticacaoAdministradorLocalProvider(
            final UserDetailsService servicoDetalhesAdministradorLocal,
            final PasswordEncoder passwordEncoder
    ) {
        final DaoAuthenticationProvider provider = DaoAuthenticationProvider.withDefaultsForSpringSecurity(servicoDetalhesAdministradorLocal);
        provider.setPasswordEncoder(passwordEncoder);
        return provider;
    }
}
