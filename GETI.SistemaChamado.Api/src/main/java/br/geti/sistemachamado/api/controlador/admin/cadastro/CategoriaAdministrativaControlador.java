package br.geti.sistemachamado.api.controlador.admin.cadastro;

import br.geti.sistemachamado.aplicacao.administracao.CategoriaAdministrativaDto;
import br.geti.sistemachamado.aplicacao.administracao.GerenciarCategoriaAdministrativa;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotBlank;
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
@RequestMapping("/api/admin/cadastros/categorias")
public class CategoriaAdministrativaControlador {

    private final GerenciarCategoriaAdministrativa gerenciarCategoriaAdministrativa;

    public CategoriaAdministrativaControlador(
            final GerenciarCategoriaAdministrativa gerenciarCategoriaAdministrativa
    ) {
        this.gerenciarCategoriaAdministrativa = gerenciarCategoriaAdministrativa;
    }

    @GetMapping
    public List<CategoriaAdministrativaDto> listar() {
        return gerenciarCategoriaAdministrativa.listar();
    }

    @GetMapping("/{id}")
    public CategoriaAdministrativaDto buscarPorId(@PathVariable final UUID id) {
        return gerenciarCategoriaAdministrativa.buscarPorId(id);
    }

    @PostMapping
    public CategoriaAdministrativaDto criar(@Valid @RequestBody final CategoriaRequisicao requisicao) {
        return gerenciarCategoriaAdministrativa.criar(requisicao.nome(), requisicao.descricao());
    }

    @PutMapping("/{id}")
    public CategoriaAdministrativaDto atualizar(
            @PathVariable final UUID id,
            @Valid @RequestBody final CategoriaRequisicao requisicao
    ) {
        return gerenciarCategoriaAdministrativa.atualizar(id, requisicao.nome(), requisicao.descricao());
    }

    @PatchMapping("/{id}/inativacao")
    public void inativar(@PathVariable final UUID id) {
        gerenciarCategoriaAdministrativa.inativar(id);
    }

    public record CategoriaRequisicao(
            @NotBlank(message = "deve ser informado")
            String nome,
            String descricao
    ) {
    }
}
