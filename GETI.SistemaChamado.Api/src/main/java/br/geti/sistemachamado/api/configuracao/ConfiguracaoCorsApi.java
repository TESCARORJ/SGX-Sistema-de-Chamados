package br.geti.sistemachamado.api.configuracao;

import java.util.Arrays;
import java.util.List;
import java.util.stream.Collectors;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.util.StringUtils;
import org.springframework.web.cors.CorsConfiguration;
import org.springframework.web.cors.CorsConfigurationSource;
import org.springframework.web.cors.UrlBasedCorsConfigurationSource;

@Configuration
public class ConfiguracaoCorsApi {

    @Bean
    public CorsConfigurationSource corsConfigurationSource(
            @Value("${app.cors.origens-permitidas:}") final String origensPermitidasConfiguradas
    ) {
        final CorsConfiguration cors = new CorsConfiguration();
        cors.setAllowedOrigins(parseOrigensPermitidas(origensPermitidasConfiguradas));
        cors.setAllowedMethods(List.of("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS"));
        cors.setAllowedHeaders(List.of("Authorization", "Content-Type", "X-Auth-Login", "X-Auth-Nome", "X-Auth-Email"));
        cors.setExposedHeaders(List.of("Location"));
        cors.setAllowCredentials(true);
        cors.setMaxAge(3600L);

        final UrlBasedCorsConfigurationSource source = new UrlBasedCorsConfigurationSource();
        source.registerCorsConfiguration("/**", cors);
        return source;
    }

    private List<String> parseOrigensPermitidas(final String origensPermitidasConfiguradas) {
        if (!StringUtils.hasText(origensPermitidasConfiguradas)) {
            return List.of();
        }
        return Arrays.stream(origensPermitidasConfiguradas.split(","))
                .map(String::trim)
                .filter(StringUtils::hasText)
                .collect(Collectors.toList());
    }
}
