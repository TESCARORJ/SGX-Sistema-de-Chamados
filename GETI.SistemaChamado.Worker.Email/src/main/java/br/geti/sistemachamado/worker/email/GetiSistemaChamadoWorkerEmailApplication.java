package br.geti.sistemachamado.worker.email;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;
import br.geti.sistemachamado.infraestrutura.configuracao.PacotesJpa;
import org.springframework.scheduling.annotation.EnableScheduling;

@SpringBootApplication(scanBasePackages = "br.geti.sistemachamado")
@EnableJpaRepositories(basePackages = PacotesJpa.REPOSITORIOS)
@EnableScheduling
public class GetiSistemaChamadoWorkerEmailApplication {

    public static void main(final String[] args) {
        SpringApplication.run(GetiSistemaChamadoWorkerEmailApplication.class, args);
    }
}

