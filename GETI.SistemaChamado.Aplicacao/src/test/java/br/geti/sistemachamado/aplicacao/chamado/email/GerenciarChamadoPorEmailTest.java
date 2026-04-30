package br.geti.sistemachamado.aplicacao.chamado.email;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.aplicacao.acesso.ResultadoSincronizacaoUsuario;
import br.geti.sistemachamado.aplicacao.acesso.SincronizarUsuarioAutenticado;
import br.geti.sistemachamado.aplicacao.chamado.automacao.AutomacaoOperacionalChamado;
import br.geti.sistemachamado.aplicacao.chamado.automacao.ResultadoAtribuicaoAutomaticaChamado;
import br.geti.sistemachamado.aplicacao.chamado.portal.ArmazenadorAnexoChamado;
import br.geti.sistemachamado.aplicacao.chamado.sla.CalculadoraSlaChamado;
import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.ServicoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.AnexoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.ChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.HistoricoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.InteracaoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.servico.GeradorNumeroChamado;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.ArgumentCaptor;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class GerenciarChamadoPorEmailTest {

    @Mock
    private ChamadoRepositorio chamadoRepositorio;
    @Mock
    private InteracaoChamadoRepositorio interacaoChamadoRepositorio;
    @Mock
    private HistoricoChamadoRepositorio historicoChamadoRepositorio;
    @Mock
    private AnexoChamadoRepositorio anexoChamadoRepositorio;
    @Mock
    private CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    @Mock
    private ServicoRepositorio servicoRepositorio;
    @Mock
    private UsuarioRepositorio usuarioRepositorio;
    @Mock
    private SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado;
    @Mock
    private GeradorNumeroChamado geradorNumeroChamado;
    @Mock
    private ArmazenadorAnexoChamado armazenadorAnexoChamado;
    @Mock
    private AutomacaoOperacionalChamado automacaoOperacionalChamado;

    @Test
    void deveAbrirChamadoPorEmailComSlaEAutoAtribuicao() {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, LocalDateTime.now().minusDays(3), null);
        final var caixa = new CaixaDeEmail(UUID.randomUUID(), "suporte@corp.com", "Suporte", true, departamento, LocalDateTime.now().minusDays(3), null);
        final var categoria = new Categoria(UUID.randomUUID(), "Sistemas", null, true, LocalDateTime.now().minusDays(3), null);
        final var servico = new Servico(UUID.randomUUID(), "Acesso", null, true, categoria, departamento, LocalDateTime.now().minusDays(3), null);
        final var solicitante = new Usuario(
                UUID.randomUUID(),
                "Solicitante Email",
                "solicitante.email",
                "solicitante.email@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), "SOLICITANTE", null, true, LocalDateTime.now().minusDays(3), null),
                departamento,
                LocalDateTime.now().minusDays(3),
                null
        );
        final var atendente = new Usuario(
                UUID.randomUUID(),
                "Atendente Email",
                "atendente.email",
                "atendente.email@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), "ATENDENTE", null, true, LocalDateTime.now().minusDays(3), null),
                departamento,
                LocalDateTime.now().minusDays(3),
                null
        );

        when(caixaDeEmailRepositorio.buscarPorId(caixa.id())).thenReturn(Optional.of(caixa));
        when(servicoRepositorio.listarPorDepartamento(departamento.id())).thenReturn(List.of(servico));
        when(sincronizarUsuarioAutenticado.sincronizar(any()))
                .thenReturn(new ResultadoSincronizacaoUsuario(
                        solicitante.id(),
                        solicitante.nome(),
                        solicitante.login(),
                        solicitante.email(),
                        solicitante.perfilAcesso().nome(),
                        departamento.id(),
                        false
                ));
        when(usuarioRepositorio.buscarPorEmail(solicitante.email())).thenReturn(Optional.of(solicitante));
        when(geradorNumeroChamado.gerarNumero()).thenReturn("CH-2026-0201");
        when(automacaoOperacionalChamado.resolverAtribuicaoAutomatica(departamento, null))
                .thenReturn(Optional.of(new ResultadoAtribuicaoAutomaticaChamado(
                        atendente,
                        "Atribuicao automatica aplicada."
                )));
        when(chamadoRepositorio.salvar(any(Chamado.class))).thenAnswer(invocacao -> invocacao.getArgument(0));

        final var servicoEmail = new GerenciarChamadoPorEmail(
                chamadoRepositorio,
                interacaoChamadoRepositorio,
                historicoChamadoRepositorio,
                anexoChamadoRepositorio,
                caixaDeEmailRepositorio,
                servicoRepositorio,
                usuarioRepositorio,
                sincronizarUsuarioAutenticado,
                geradorNumeroChamado,
                armazenadorAnexoChamado,
                new CalculadoraSlaChamado(),
                automacaoOperacionalChamado
        );

        final var retorno = servicoEmail.abrirChamadoPorEmail(new AberturaChamadoEmailComando(
                caixa.id(),
                "Solicitante Email",
                solicitante.email(),
                caixa.enderecoEmail(),
                "Sem acesso",
                "Nao consigo acessar VPN",
                "msg-1@corp",
                PrioridadeChamado.ALTA,
                List.of()
        ));

        final var captorChamado = ArgumentCaptor.forClass(Chamado.class);
        org.mockito.Mockito.verify(chamadoRepositorio).salvar(captorChamado.capture());
        final var chamado = captorChamado.getValue();
        assertThat(chamado.prazoSlaMinutos()).isEqualTo(480);
        assertThat(chamado.responsavel()).isNotNull();
        assertThat(chamado.responsavel().id()).isEqualTo(atendente.id());
        assertThat(retorno.numeroChamado()).isEqualTo("CH-2026-0201");
    }
}
