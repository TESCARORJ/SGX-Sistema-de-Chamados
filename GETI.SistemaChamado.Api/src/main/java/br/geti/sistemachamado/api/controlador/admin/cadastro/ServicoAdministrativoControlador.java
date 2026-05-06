package br.geti.sistemachamado.api.controlador.admin.cadastro;

import br.geti.sistemachamado.aplicacao.administracao.GerenciarServicoAdministrativo;
import br.geti.sistemachamado.aplicacao.administracao.ServicoAdministrativoDto;
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
@RequestMapping("/api/admin/cadastros/servicos")
public class ServicoAdministrativoControlador {

    private final GerenciarServicoAdministrativo gerenciarServicoAdministrativo;

    public ServicoAdministrativoControlador(final GerenciarServicoAdministrativo gerenciarServicoAdministrativo) {
        this.gerenciarServicoAdministrativo = gerenciarServicoAdministrativo;
    }

    @GetMapping
    public List<ServicoAdministrativoDto> listar() {
        return gerenciarServicoAdministrativo.listar();
    }

    @GetMapping("/{id}")
    public ServicoAdministrativoDto buscarPorId(@PathVariable final UUID id) {
        return gerenciarServicoAdministrativo.buscarPorId(id);
    }

    @PostMapping
    public ServicoAdministrativoDto criar(@Valid @RequestBody final ServicoRequisicao requisicao) {
        return gerenciarServicoAdministrativo.criar(
                requisicao.nome(),
                requisicao.descricao(),
                requisicao.categoriaId(),
                requisicao.departamentoId()
        );
    }

    @PutMapping("/{id}")
    public ServicoAdministrativoDto atualizar(
            @PathVariable final UUID id,
            @Valid @RequestBody final ServicoRequisicao requisicao
    ) {
        return gerenciarServicoAdministrativo.atualizar(
                id,
                requisicao.nome(),
                requisicao.descricao(),
                requisicao.categoriaId(),
                requisicao.departamentoId()
        );
    }

    @PatchMapping("/{id}/inativacao")
    public void inativar(@PathVariable final UUID id) {
        gerenciarServicoAdministrativo.inativar(id);
    }

    public record ServicoRequisicao(
            @NotBlank(message = "deve ser informado")
            String nome,
            String descricao,
            @NotNull(message = "deve ser informado")
            UUID categoriaId,
            @NotNull(message = "deve ser informado")
            UUID departamentoId
    ) {
    }
}
