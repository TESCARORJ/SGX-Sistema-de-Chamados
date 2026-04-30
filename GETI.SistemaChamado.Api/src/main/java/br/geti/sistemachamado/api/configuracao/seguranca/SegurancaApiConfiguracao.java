package br.geti.sistemachamado.api.configuracao.seguranca;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.beans.factory.ObjectProvider;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpMethod;
import org.springframework.security.authentication.AuthenticationProvider;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.AnonymousAuthenticationFilter;

@Configuration
@EnableMethodSecurity
public class SegurancaApiConfiguracao {

    @Bean
    public SecurityFilterChain securityFilterChain(
            final HttpSecurity http,
            final ConversorJwtParaAutenticacaoInterna conversorJwtParaAutenticacaoInterna,
            final FiltroAutenticacaoLocalDesenvolvimento filtroAutenticacaoLocalDesenvolvimento,
            final ObjectProvider<AuthenticationProvider> autenticacaoAdministradorLocalProvider,
            @Value("${app.seguranca.modo-local-habilitado:false}") final boolean modoLocalHabilitado,
            @Value("${app.admin-local.autenticacao-habilitada:true}") final boolean autenticacaoAdminLocalHabilitada
    ) throws Exception {
        http
                .csrf(csrf -> csrf.disable())
                .cors(Customizer.withDefaults())
                .sessionManagement(session -> session.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
                .exceptionHandling(exceptions -> exceptions
                        .authenticationEntryPoint((request, response, authException) -> response.sendError(401))
                        .accessDeniedHandler((request, response, accessDeniedException) -> response.sendError(403))
                )
                .authorizeHttpRequests(auth -> auth
                        .requestMatchers(
                                "/",
                                "/actuator",
                                "/actuator/**",
                                "/swagger-ui.html",
                                "/swagger-ui/**",
                                "/v3/api-docs",
                                "/v3/api-docs/**",
                                "/favicon.ico",
                                "/error"
                        ).permitAll()
                        .requestMatchers(HttpMethod.POST, "/api/auth/admin/local-login").permitAll()
                        .requestMatchers(HttpMethod.GET, "/api/saude").permitAll()
                        .requestMatchers("/admin/**").hasAnyRole("ATENDENTE", "SUPERVISOR", "ADMINISTRADOR")
                        .requestMatchers("/api/admin/**").hasAnyRole("ATENDENTE", "SUPERVISOR", "ADMINISTRADOR")
                        .requestMatchers("/api/tecnico/acesso/**").hasAnyRole("SUPERVISOR", "ADMINISTRADOR")
                        .requestMatchers("/api/portal/**", "/api/me").authenticated()
                        .anyRequest().authenticated()
                );

        if (autenticacaoAdminLocalHabilitada) {
            final var provider = autenticacaoAdministradorLocalProvider.getIfAvailable();
            if (provider != null) {
                http.authenticationProvider(provider);
            }
            http.httpBasic(Customizer.withDefaults());
        }

        if (modoLocalHabilitado) {
            http.addFilterBefore(filtroAutenticacaoLocalDesenvolvimento, AnonymousAuthenticationFilter.class);
        } else {
            http.oauth2ResourceServer(oauth2 -> oauth2.jwt(
                    jwt -> jwt.jwtAuthenticationConverter(conversorJwtParaAutenticacaoInterna)
            ));
        }

        return http.build();
    }
}
