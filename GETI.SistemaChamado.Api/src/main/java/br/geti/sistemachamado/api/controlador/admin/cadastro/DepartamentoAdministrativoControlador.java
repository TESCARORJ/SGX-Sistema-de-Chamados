package br.geti.sistemachamado.api.controlador.admin.cadastro;

import br.geti.sistemachamado.aplicacao.administracao.DepartamentoAdministrativoDto;
import br.geti.sistemachamado.aplicacao.administracao.GerenciarDepartamentoAdministrativo;
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
@RequestMapping("/api/admin/cadastros/departamentos")
public class DepartamentoAdministrativoControlador {

    private final GerenciarDepartamentoAdministrativo gerenciarDepartamentoAdministrativo;

    public DepartamentoAdministrativoControlador(
            final GerenciarDepartamentoAdministrativo gerenciarDepartamentoAdministrativo
    ) {
        this.gerenciarDepartamentoAdministrativo = gerenciarDepartamentoAdministrativo;
    }

    @GetMapping
    public List<DepartamentoAdministrativoDto> listar() {
        return gerenciarDepartamentoAdministrativo.listar();
    }

    @GetMapping("/{id}")
    public DepartamentoAdministrativoDto buscarPorId(@PathVariable final UUID id) {
        return gerenciarDepartamentoAdministrativo.buscarPorId(id);
    }

    @PostMapping
    public DepartamentoAdministrativoDto criar(@Valid @RequestBody final DepartamentoRequisicao requisicao) {
        return gerenciarDepartamentoAdministrativo.criar(requisicao.nome());
    }

    @PutMapping("/{id}")
    public DepartamentoAdministrativoDto atualizar(
            @PathVariable final UUID id,
            @Valid @RequestBody final DepartamentoRequisicao requisicao
    ) {
        return gerenciarDepartamentoAdministrativo.atualizar(id, requisicao.nome());
    }

    @PatchMapping("/{id}/inativacao")
    public void inativar(@PathVariable final UUID id) {
        gerenciarDepartamentoAdministrativo.inativar(id);
    }

    public record DepartamentoRequisicao(
            @NotBlank(message = "deve ser informado")
            String nome
    ) {
    }
}
