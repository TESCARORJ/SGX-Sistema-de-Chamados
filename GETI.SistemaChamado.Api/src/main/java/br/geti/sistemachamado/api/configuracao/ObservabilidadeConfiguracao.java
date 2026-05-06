package br.geti.sistemachamado.api.configuracao;

import java.util.Map;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.boot.actuate.info.InfoContributor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class ObservabilidadeConfiguracao {

    @Bean
    public InfoContributor infoContributor(
            @Value("${spring.application.name}") final String nomeAplicacao,
            @Value("${app.ambiente}") final String ambiente
    ) {
        return builder -> builder.withDetails(Map.of(
                "servico", nomeAplicacao,
                "ambiente", ambiente,
                "dominio", "service-desk-corporativo"
        ));
    }
}

