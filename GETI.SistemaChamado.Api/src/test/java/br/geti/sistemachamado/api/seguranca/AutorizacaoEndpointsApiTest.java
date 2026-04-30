package br.geti.sistemachamado.api.seguranca;

import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.doAnswer;
import static org.springframework.security.test.web.servlet.request.SecurityMockMvcRequestPostProcessors.user;
import static org.springframework.test.web.servlet.request.MockMvcRequestBuilders.get;
import static org.springframework.test.web.servlet.result.MockMvcResultMatchers.status;

import br.geti.sistemachamado.api.configuracao.seguranca.ConversorJwtParaAutenticacaoInterna;
import br.geti.sistemachamado.api.configuracao.seguranca.FiltroAutenticacaoLocalDesenvolvimento;
import br.geti.sistemachamado.api.configuracao.seguranca.SegurancaApiConfiguracao;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletRequest;
import jakarta.servlet.ServletResponse;
import java.util.Map;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.SpringBootConfiguration;
import org.springframework.boot.autoconfigure.EnableAutoConfiguration;
import org.springframework.boot.data.jpa.autoconfigure.DataJpaRepositoriesAutoConfiguration;
import org.springframework.boot.hibernate.autoconfigure.HibernateJpaAutoConfiguration;
import org.springframework.boot.jdbc.autoconfigure.DataSourceAutoConfiguration;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.webmvc.test.autoconfigure.AutoConfigureMockMvc;
import org.springframework.context.annotation.Import;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.context.TestPropertySource;
import org.springframework.test.web.servlet.MockMvc;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@SpringBootTest(classes = AutorizacaoEndpointsApiTest.ConfiguracaoTeste.class)
@AutoConfigureMockMvc
@TestPropertySource(properties = {
        "app.seguranca.modo-local-habilitado=true"
})
class AutorizacaoEndpointsApiTest {

    @Autowired
    private MockMvc mockMvc;

    @MockitoBean
    private FiltroAutenticacaoLocalDesenvolvimento filtroAutenticacaoLocalDesenvolvimento;
    @MockitoBean
    private ConversorJwtParaAutenticacaoInterna conversorJwtParaAutenticacaoInterna;

    @BeforeEach
    void prepararPassagemDoFiltroLocal() throws Exception {
        doAnswer(invocacao -> {
            final ServletRequest request = invocacao.getArgument(0);
            final ServletResponse response = invocacao.getArgument(1);
            final FilterChain chain = invocacao.getArgument(2);
            chain.doFilter(request, response);
            return null;
        }).when(filtroAutenticacaoLocalDesenvolvimento).doFilter(any(), any(), any());
    }

    @Test
    void devePermitirAcessoAoEndpointPublicoSaudeSemAutenticacao() throws Exception {
        mockMvc.perform(get("/api/saude"))
                .andExpect(status().isOk());
    }

    @Test
    void deveBloquearEndpointAdminSemAutenticacao() throws Exception {
        mockMvc.perform(get("/api/admin/chamados/dashboard"))
                .andExpect(status().isUnauthorized());
    }

    @Test
    void deveNegarAcessoAdminParaPerfilSolicitante() throws Exception {
        mockMvc.perform(get("/api/admin/chamados/dashboard")
                        .with(user("solicitante").roles("SOLICITANTE")))
                .andExpect(status().isForbidden());
    }

    @Test
    void devePermitirAcessoAdminParaPerfilAtendente() throws Exception {
        mockMvc.perform(get("/api/admin/chamados/dashboard")
                        .with(user("atendente").roles("ATENDENTE")))
                .andExpect(status().isOk());
    }

    @SpringBootConfiguration
    @EnableAutoConfiguration(exclude = {
            DataSourceAutoConfiguration.class,
            HibernateJpaAutoConfiguration.class,
            DataJpaRepositoriesAutoConfiguration.class
    })
    @Import({SegurancaApiConfiguracao.class, EndpointsTesteControlador.class})
    static class ConfiguracaoTeste {
    }

    @RestController
    static class EndpointsTesteControlador {

        @GetMapping("/api/saude")
        Map<String, String> saude() {
            return Map.of("status", "ok");
        }

        @GetMapping("/api/admin/chamados/dashboard")
        Map<String, String> dashboard() {
            return Map.of("status", "ok");
        }
    }
}
