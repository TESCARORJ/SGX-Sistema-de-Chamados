package br.geti.sistemachamado.aplicacao.chamado.admin;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.ArgumentMatchers.any;
import static org.mockito.Mockito.when;

import br.geti.sistemachamado.aplicacao.chamado.automacao.AutomacaoOperacionalChamado;
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
import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.repositorio.AnexoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.ChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.HistoricoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.InteracaoChamadoRepositorio;
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
class GerenciarChamadoAdministrativoTest {

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
    private AutomacaoOperacionalChamado automacaoOperacionalChamado;

    @Test
    void deveAlterarSituacaoComRastreabilidade() {
        final var departamento = new Departamento(UUID.randomUUID(), "TI", true, LocalDateTime.now().minusDays(2), null);
        final var categoria = new Categoria(UUID.randomUUID(), "Sistema", null, true, LocalDateTime.now().minusDays(2), null);
        final var servico = new Servico(UUID.randomUUID(), "Sistema X", null, true, categoria, departamento, LocalDateTime.now().minusDays(2), null);
        final var solicitante = usuario("Solicitante", "SOLICITANTE", departamento);
        final var agente = usuario("Agente", "ATENDENTE", departamento);
        final var chamado = new Chamado(
                UUID.randomUUID(),
                "CH-2026-0002",
                "Falha de acesso",
                "Erro intermitente",
                SituacaoChamado.ABERTO,
                PrioridadeChamado.MEDIA,
                OrigemChamado.PORTAL,
                solicitante,
                null,
                departamento,
                categoria,
                servico,
                1440,
                LocalDateTime.now().plusHours(20),
                LocalDateTime.now().minusHours(4),
                null
        );

        when(chamadoRepositorio.buscarPorId(chamado.id())).thenReturn(Optional.of(chamado));
        when(usuarioRepositorio.buscarPorId(agente.id())).thenReturn(Optional.of(agente));
        when(chamadoRepositorio.salvar(any(Chamado.class)))
                .thenAnswer(invocacao -> invocacao.getArgument(0));
        when(interacaoChamadoRepositorio.listarPorChamado(chamado.id())).thenReturn(List.of());
        when(historicoChamadoRepositorio.listarPorChamado(chamado.id())).thenReturn(List.of());
        when(anexoChamadoRepositorio.listarPorChamado(chamado.id())).thenReturn(List.of());

        final var servicoAdmin = new GerenciarChamadoAdministrativo(
                chamadoRepositorio,
                interacaoChamadoRepositorio,
                historicoChamadoRepositorio,
                anexoChamadoRepositorio,
                usuarioRepositorio,
                departamentoRepositorio,
                categoriaRepositorio,
                servicoRepositorio,
                new CalculadoraSlaChamado(),
                automacaoOperacionalChamado
        );

        final var detalhe = servicoAdmin.alterarSituacao(new AlteracaoSituacaoChamadoAdminComando(
                chamado.id(),
                SituacaoChamado.EM_ATENDIMENTO,
                agente.id()
        ));

        final var captorChamado = ArgumentCaptor.forClass(Chamado.class);
        org.mockito.Mockito.verify(chamadoRepositorio).salvar(captorChamado.capture());
        assertThat(captorChamado.getValue().situacao()).isEqualTo(SituacaoChamado.EM_ATENDIMENTO);
        org.mockito.Mockito.verify(interacaoChamadoRepositorio).salvar(any());
        org.mockito.Mockito.verify(historicoChamadoRepositorio).salvar(any());
        assertThat(detalhe.situacao()).isEqualTo("EM_ATENDIMENTO");
    }

    private Usuario usuario(final String nome, final String perfilNome, final Departamento departamento) {
        return new Usuario(
                UUID.randomUUID(),
                nome,
                nome.toLowerCase(),
                nome.toLowerCase() + "@corp.com",
                true,
                new PerfilAcesso(UUID.randomUUID(), perfilNome, null, true, LocalDateTime.now().minusDays(2), null),
                departamento,
                LocalDateTime.now().minusDays(2),
                null
        );
    }
}
