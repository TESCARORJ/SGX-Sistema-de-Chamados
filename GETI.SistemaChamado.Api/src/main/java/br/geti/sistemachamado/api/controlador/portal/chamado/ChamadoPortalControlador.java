package br.geti.sistemachamado.api.controlador.portal.chamado;

import br.geti.sistemachamado.api.configuracao.seguranca.UsuarioAutenticadoPrincipal;
import br.geti.sistemachamado.aplicacao.chamado.portal.AberturaChamadoPortalComando;
import br.geti.sistemachamado.aplicacao.chamado.portal.AnexoChamadoPortalComando;
import br.geti.sistemachamado.aplicacao.chamado.portal.AnexoChamadoPortalDto;
import br.geti.sistemachamado.aplicacao.chamado.portal.CatalogoAberturaChamadoPortalDto;
import br.geti.sistemachamado.aplicacao.chamado.portal.ChamadoPortalDetalheDto;
import br.geti.sistemachamado.aplicacao.chamado.portal.ChamadoPortalResumoDto;
import br.geti.sistemachamado.aplicacao.chamado.portal.GerenciarChamadoPortalSolicitante;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import java.io.IOException;
import java.util.List;
import java.util.UUID;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestPart;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.multipart.MultipartFile;

@RestController
@RequestMapping("/api/portal/chamados")
public class ChamadoPortalControlador {

    private final GerenciarChamadoPortalSolicitante gerenciarChamadoPortalSolicitante;

    public ChamadoPortalControlador(final GerenciarChamadoPortalSolicitante gerenciarChamadoPortalSolicitante) {
        this.gerenciarChamadoPortalSolicitante = gerenciarChamadoPortalSolicitante;
    }

    @GetMapping("/catalogo-abertura")
    public CatalogoAberturaChamadoPortalDto consultarCatalogoAbertura() {
        return gerenciarChamadoPortalSolicitante.consultarCatalogoAbertura();
    }

    @PostMapping
    public ChamadoPortalDetalheDto abrirChamado(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @Valid @org.springframework.web.bind.annotation.RequestBody final AberturaChamadoPortalRequisicao requisicao
    ) {
        return gerenciarChamadoPortalSolicitante.abrirChamado(new AberturaChamadoPortalComando(
                obterSolicitanteId(principal),
                requisicao.titulo(),
                requisicao.descricao(),
                requisicao.prioridade(),
                requisicao.departamentoId(),
                requisicao.categoriaId(),
                requisicao.servicoId()
        ));
    }

    @GetMapping
    public List<ChamadoPortalResumoDto> listarChamadosDoSolicitante(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal
    ) {
        return gerenciarChamadoPortalSolicitante.listarChamadosDoSolicitante(obterSolicitanteId(principal));
    }

    @GetMapping("/{id}")
    public ChamadoPortalDetalheDto detalharChamado(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id
    ) {
        return gerenciarChamadoPortalSolicitante.buscarDetalheDoSolicitante(obterSolicitanteId(principal), id);
    }

    @PostMapping(path = "/{id}/anexos", consumes = "multipart/form-data")
    public AnexoChamadoPortalDto anexarArquivo(
            @AuthenticationPrincipal final UsuarioAutenticadoPrincipal principal,
            @PathVariable("id") final UUID id,
            @RequestPart("arquivo") final MultipartFile arquivo
    ) {
        if (arquivo == null || arquivo.isEmpty()) {
            throw new ErroDeDominio("arquivo do anexo e obrigatorio");
        }

        try {
            return gerenciarChamadoPortalSolicitante.anexarArquivo(
                    obterSolicitanteId(principal),
                    id,
                    new AnexoChamadoPortalComando(
                            arquivo.getOriginalFilename(),
                            arquivo.getContentType(),
                            arquivo.getBytes()
                    )
            );
        } catch (final IOException exception) {
            throw new ErroDeDominio("Falha ao ler arquivo enviado para anexo.");
        }
    }

    private UUID obterSolicitanteId(final UsuarioAutenticadoPrincipal principal) {
        if (principal == null || principal.usuarioId() == null) {
            throw new ErroDeDominio("Usuario autenticado nao identificado.");
        }
        return principal.usuarioId();
    }

    public record AberturaChamadoPortalRequisicao(
            @NotBlank(message = "deve ser informado")
            String titulo,
            @NotBlank(message = "deve ser informado")
            String descricao,
            @NotNull(message = "deve ser informado")
            PrioridadeChamado prioridade,
            @NotNull(message = "deve ser informado")
            UUID departamentoId,
            @NotNull(message = "deve ser informado")
            UUID categoriaId,
            @NotNull(message = "deve ser informado")
            UUID servicoId
    ) {
    }
}
