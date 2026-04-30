package br.geti.sistemachamado.api.controlador.admin.chamado;

import br.geti.sistemachamado.api.configuracao.seguranca.UsuarioAutenticadoPrincipal;
import br.geti.sistemachamado.aplicacao.chamado.admin.AlteracaoSituacaoChamadoAdminComando;
import br.geti.sistemachamado.aplicacao.chamado.admin.AtribuicaoChamadoAdminComando;
import br.geti.sistemachamado.aplicacao.chamado.admin.CatalogoChamadoAdminDto;
import br.geti.sistemachamado.aplicacao.chamado.admin.ChamadoAdminDetalheDto;
import br.geti.sistemachamado.aplicacao.chamado.admin.ChamadoAdminFilaDto;
import br.geti.sistemachamado.aplicacao.chamado.admin.ChamadoAdminFiltroFilaComando;
import br.geti.sistemachamado.aplicacao.chamado.admin.ComentarioChamadoAdminComando;
import br.geti.sistemachamado.aplicacao.chamado.admin.DashboardAdminChamadoDto;
import br.geti.sistemachamado.aplicacao.chamado.admin.EncaminhamentoChamadoAdminComando;
import br.geti.sistemachamado.aplicacao.chamado.admin.GerenciarChamadoAdministrativo;
import br.geti.sistemachamado.aplicacao.chamado.admin.RelatorioOperacionalChamadoAdminDto;
import br.geti.sistemachamado.aplicacao.chamado.admin.RelatorioOperacionalChamadoAdminFiltroComando;
import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.StatusSlaChamado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import java.util.List;
import java.util.UUID;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/admin/chamados")
public class ChamadoAdministrativoControlador {

    private final GerenciarChamadoAdministrativo gerenciarChamadoAdministrativo;

    public ChamadoAdministrativoControlador(final GerenciarChamadoAdministrativo gerenciarChamadoAdministrativo) {
        this.gerenciarChamadoAdministrativo = gerenciarChamadoAdministrativo;
    }

    @GetMapping("/catalogo")
    public CatalogoChamadoAdminDto consultarCatalogo() {
        return gerenciarChamadoAdministrativo.consultarCatalogo();
    }

    @GetMapping("/dashboard")
    public DashboardAdminChamadoDto consultarDashboard() {
        return gerenciarChamadoAdministrativo.consultarDashboard();
    }

    @GetMapping("/relatorios-operacionais")
    public RelatorioOperacionalChamadoAdminDto consultarRelatoriosOperacionais(
            @RequestParam(name = "departamentoId", required = false) final UUID departamentoId,
            @RequestParam(name = "situacao", required = false) final SituacaoChamado situacao,
            @RequestParam(name = "prioridade", required = false) final PrioridadeChamado prioridade,
            @RequestParam(name = "responsavelId", required = false) final UUID responsavelId,
            @RequestParam(name = "statusSla", required = false) final StatusSlaChamado statusSla
    ) {
        return gerenciarChamadoAdministrativo.consultarRelatorioOperacional(new RelatorioOperacionalChamadoAdminFiltroComando(
                departamentoId,
                situacao,
                prioridade,
                responsavelId,
                statusSla
        ));
    }

    @GetMapping
    public List<ChamadoAdminFilaDto> listarFila(
            @RequestParam(name = "situacao", required = false) final SituacaoChamado situacao,
            @RequestParam(name = "prioridade", required = false) final PrioridadeChamado prioridade,
            @RequestParam(name = "departamentoId", required = false) final UUID departamentoId,
            @RequestParam(name = "origem", required = false) final OrigemChamado origem,
            @RequestParam(name = "responsavelId", required = false) final UUID responsavelId,
            @RequestParam(name = "statusSla", required = false) final StatusSlaChamado statusSla
    ) {
        return gerenciarChamadoAdministrativo.listarFila(new ChamadoAdminFiltroFilaComando(
                situacao,
                prioridade,
                departamentoId,
                origem,
                responsavelId,
                statusSla
        ));
    }

    @GetMapping("/{id}")
    public ChamadoAdminDetalheDto detalhar(@PathVariable("id") final UUID id) {
        return gerenciarChamadoAdministrativo.detalhar(id);
    }

    @PatchMapping("/{id}/atribuicao")
    public ChamadoAdminDetalheDto atribuir(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @Valid @RequestBody final AtribuicaoRequisicao requisicao
    ) {
        return gerenciarChamadoAdministrativo.atribuir(new AtribuicaoChamadoAdminComando(
                id,
                requisicao.responsavelId(),
                obterUsuarioId(principal)
        ));
    }

    @PatchMapping("/{id}/situacao")
    public ChamadoAdminDetalheDto alterarSituacao(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @Valid @RequestBody final AlteracaoSituacaoRequisicao requisicao
    ) {
        return gerenciarChamadoAdministrativo.alterarSituacao(new AlteracaoSituacaoChamadoAdminComando(
                id,
                requisicao.novaSituacao(),
                obterUsuarioId(principal)
        ));
    }

    @PatchMapping("/{id}/encaminhamento")
    public ChamadoAdminDetalheDto encaminhar(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @Valid @RequestBody final EncaminhamentoRequisicao requisicao
    ) {
        return gerenciarChamadoAdministrativo.encaminhar(new EncaminhamentoChamadoAdminComando(
                id,
                requisicao.departamentoId(),
                requisicao.categoriaId(),
                requisicao.servicoId(),
                obterUsuarioId(principal)
        ));
    }

    @PostMapping("/{id}/comentarios/publico")
    public ChamadoAdminDetalheDto comentarPublicamente(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @Valid @RequestBody final ComentarioRequisicao requisicao
    ) {
        return gerenciarChamadoAdministrativo.comentarPublicamente(new ComentarioChamadoAdminComando(
                id,
                obterUsuarioId(principal),
                requisicao.mensagem()
        ));
    }

    @PostMapping("/{id}/comentarios/interno")
    public ChamadoAdminDetalheDto comentarInternamente(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @Valid @RequestBody final ComentarioRequisicao requisicao
    ) {
        return gerenciarChamadoAdministrativo.comentarInternamente(new ComentarioChamadoAdminComando(
                id,
                obterUsuarioId(principal),
                requisicao.mensagem()
        ));
    }

    private UUID obterUsuarioId(final UsuarioAutenticadoPrincipal principal) {
        if (principal == null || principal.usuarioId() == null) {
            throw new ErroDeDominio("Usuario autenticado nao identificado.");
        }
        return principal.usuarioId();
    }

    public record AtribuicaoRequisicao(
            @NotNull(message = "deve ser informado")
            UUID responsavelId
    ) {
    }

    public record AlteracaoSituacaoRequisicao(
            @NotNull(message = "deve ser informado")
            SituacaoChamado novaSituacao
    ) {
    }

    public record EncaminhamentoRequisicao(
            @NotNull(message = "deve ser informado")
            UUID departamentoId,
            @NotNull(message = "deve ser informado")
            UUID categoriaId,
            @NotNull(message = "deve ser informado")
            UUID servicoId
    ) {
    }

    public record ComentarioRequisicao(
            @NotBlank(message = "deve ser informado")
            String mensagem
    ) {
    }
}
