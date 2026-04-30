package br.geti.sistemachamado.aplicacao.chamado.portal;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.aplicacao.chamado.automacao.AutomacaoOperacionalChamado;
import br.geti.sistemachamado.aplicacao.chamado.automacao.ResultadoAtribuicaoAutomaticaChamado;
import br.geti.sistemachamado.aplicacao.chamado.sla.CalculadoraSlaChamado;
import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.CategoriaRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.ServicoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.InteracaoChamado;
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
class GerenciarChamadoPortalSolicitanteTest {

    @Mock
    private ChamadoRepositorio chamadoRepositorio;
    @Mock
    private InteracaoChamadoRepositorio interacaoChamadoRepositorio;
    @Mock
    private HistoricoChamadoRepositorio historicoChamadoRepositorio;
    @Mock
    private AnexoChamadoRepositorio anexoChamadoRepositorio;
    @Mock
    private UsuarioRepositorio usuarioRepositorio;
    @Mock
    private DepartamentoRepositorio departamentoRepositorio;
    @Mock
    private CategoriaRepositorio categoriaRepositorio;
    @Mock
    private ServicoRepositorio servicoRepositorio;
    @Mock
    private GeradorNumeroChamado geradorNumeroChamado;
    @Mock
    private ArmazenadorAnexoChamado armazenadorAnexoChamado;
    @Mock
    private AutomacaoOperacionalChamado automacaoOperacionalChamado;

    @Test
    void deveAbrirChamadoComSlaEAutoAtribuicao() {
        final var solicitante = criarSolicitante();
        final var departamento = solicitante.departamento();
        final var categoria = new Categoria(UUID.randomUUID(), "Sistemas", null, true, LocalDateTime.now().minusDays(2), null);
        final var servico = new Servico(UUID.randomUUID(), "Sistema X", null, true, categoria, departamento, LocalDateTime.now().minusDays(2), null);
        final var atendente = new Usuario(
                UUID.randomUUID(),
                "Atendente N1",
                "atendente.n1",
                "atendente.n1@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), "ATENDENTE", null, true, LocalDateTime.now().minusDays(2), null),
                departamento,
                LocalDateTime.now().minusDays(2),
                null
        );

        when(usuarioRepositorio.buscarPorId(solicitante.id())).thenReturn(Optional.of(solicitante));
        when(departamentoRepositorio.buscarPorId(departamento.id())).thenReturn(Optional.of(departamento));
        when(categoriaRepositorio.buscarPorId(categoria.id())).thenReturn(Optional.of(categoria));
        when(servicoRepositorio.buscarPorId(servico.id())).thenReturn(Optional.of(servico));
        when(geradorNumeroChamado.gerarNumero()).thenReturn("CH-2026-0001");
        when(automacaoOperacionalChamado.resolverAtribuicaoAutomatica(departamento, null))
                .thenReturn(Optional.of(new ResultadoAtribuicaoAutomaticaChamado(
                        atendente,
                        "Atribuicao automatica aplicada."
                )));
        when(chamadoRepositorio.salvar(any(Chamado.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));
        when(interacaoChamadoRepositorio.salvar(any(InteracaoChamado.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));
        when(chamadoRepositorio.buscarPorIdESolicitante(any(), any()))
                .thenAnswer(invocacao -> {
                    final UUID chamadoId = invocacao.getArgument(0);
                    return Optional.of(chamadoSalvo(chamadoId, solicitante, atendente, departamento, categoria, servico));
                });
        when(interacaoChamadoRepositorio.listarPorChamado(any())).thenReturn(List.of());
        when(historicoChamadoRepositorio.listarPorChamado(any())).thenReturn(List.of());
        when(anexoChamadoRepositorio.listarPorChamado(any())).thenReturn(List.of());

        final var servicoPortal = new GerenciarChamadoPortalSolicitante(
                chamadoRepositorio,
                interacaoChamadoRepositorio,
                historicoChamadoRepositorio,
                anexoChamadoRepositorio,
                usuarioRepositorio,
                departamentoRepositorio,
                categoriaRepositorio,
                servicoRepositorio,
                geradorNumeroChamado,
                armazenadorAnexoChamado,
                new CalculadoraSlaChamado(),
                automacaoOperacionalChamado
        );

        final var detalhe = servicoPortal.abrirChamado(new AberturaChamadoPortalComando(
                solicitante.id(),
                "Erro no sistema",
                "Nao consigo autenticar",
                PrioridadeChamado.ALTA,
                departamento.id(),
                categoria.id(),
                servico.id()
        ));

        final var captorChamado = ArgumentCaptor.forClass(Chamado.class);
        org.mockito.Mockito.verify(chamadoRepositorio).salvar(captorChamado.capture());
        final var chamadoCriado = captorChamado.getValue();

        assertThat(chamadoCriado.prazoSlaMinutos()).isEqualTo(480);
        assertThat(chamadoCriado.dataLimiteSla()).isAfter(chamadoCriado.dataCriacao());
        assertThat(chamadoCriado.responsavel()).isNotNull();
        assertThat(chamadoCriado.responsavel().id()).isEqualTo(atendente.id());
        assertThat(detalhe.numero()).isEqualTo("CH-2026-0001");
    }

    private Usuario criarSolicitante() {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, LocalDateTime.now().minusDays(2), null);
        return new Usuario(
                UUID.randomUUID(),
                "Solicitante",
                "solicitante",
                "solicitante@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), "SOLICITANTE", null, true, LocalDateTime.now().minusDays(2), null),
                departamento,
                LocalDateTime.now().minusDays(2),
                null
        );
    }

    private Chamado chamadoSalvo(
            final UUID chamadoId,
            final Usuario solicitante,
            final Usuario responsavel,
            final Departamento departamento,
            final Categoria categoria,
            final Servico servico
    ) {
        final var criacao = LocalDateTime.now();
        return new Chamado(
                chamadoId,
                "CH-2026-0001",
                "Erro no sistema",
                "Nao consigo autenticar",
                br.geti.sistemachamado.dominio.chamado.SituacaoChamado.ABERTO,
                PrioridadeChamado.ALTA,
                br.geti.sistemachamado.dominio.chamado.OrigemChamado.PORTAL,
                solicitante,
                responsavel,
                departamento,
                categoria,
                servico,
                480,
                criacao.plusMinutes(480),
                criacao,
                criacao
        );
    }
}
