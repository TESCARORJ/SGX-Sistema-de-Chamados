package br.geti.sistemachamado.api.controlador.admin.cadastro;

import br.geti.sistemachamado.aplicacao.administracao.GerenciarGrupoAtendimentoAdministrativo;
import br.geti.sistemachamado.aplicacao.administracao.GrupoAtendimentoAdministrativoDto;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import java.util.List;
import java.util.UUID;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/admin/cadastros/grupos-atendimento")
public class GrupoAtendimentoAdministrativoControlador {

    private final GerenciarGrupoAtendimentoAdministrativo gerenciarGrupoAtendimentoAdministrativo;

    public GrupoAtendimentoAdministrativoControlador(
            final GerenciarGrupoAtendimentoAdministrativo gerenciarGrupoAtendimentoAdministrativo
    ) {
        this.gerenciarGrupoAtendimentoAdministrativo = gerenciarGrupoAtendimentoAdministrativo;
    }

    @GetMapping
    public List<GrupoAtendimentoAdministrativoDto> listar() {
        return gerenciarGrupoAtendimentoAdministrativo.listar();
    }

    @GetMapping("/{id}")
    public GrupoAtendimentoAdministrativoDto buscarPorId(@PathVariable final UUID id) {
        return gerenciarGrupoAtendimentoAdministrativo.buscarPorId(id);
    }

    @PostMapping
    public GrupoAtendimentoAdministrativoDto criar(
            @Valid @RequestBody final GrupoAtendimentoRequisicao requisicao
    ) {
        return gerenciarGrupoAtendimentoAdministrativo.criar(
                requisicao.nome(),
                requisicao.descricao(),
                requisicao.departamentoId()
        );
    }

    @PutMapping("/{id}")
    public GrupoAtendimentoAdministrativoDto atualizar(
            @PathVariable final UUID id,
            @Valid @RequestBody final GrupoAtendimentoRequisicao requisicao
    ) {
        return gerenciarGrupoAtendimentoAdministrativo.atualizar(
                id,
                requisicao.nome(),
                requisicao.descricao(),
                requisicao.departamentoId()
        );
    }

    @PatchMapping("/{id}/inativacao")
    public void inativar(@PathVariable final UUID id) {
        gerenciarGrupoAtendimentoAdministrativo.inativar(id);
    }

    public record GrupoAtendimentoRequisicao(
            @NotBlank(message = "deve ser informado")
            String nome,
            String descricao,
            @NotNull(message = "deve ser informado")
            UUID departamentoId
    ) {
    }
}
