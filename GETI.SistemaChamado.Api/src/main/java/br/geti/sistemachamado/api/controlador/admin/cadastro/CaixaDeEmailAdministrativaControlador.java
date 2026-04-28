package br.geti.sistemachamado.api.controlador.admin.cadastro;

import br.geti.sistemachamado.aplicacao.administracao.CaixaDeEmailAdministrativaDto;
import br.geti.sistemachamado.aplicacao.administracao.GerenciarCaixaDeEmailAdministrativa;
import jakarta.validation.Valid;
import jakarta.validation.constraints.Email;
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
@RequestMapping("/api/admin/cadastros/caixas-email")
public class CaixaDeEmailAdministrativaControlador {

    private final GerenciarCaixaDeEmailAdministrativa gerenciarCaixaDeEmailAdministrativa;

    public CaixaDeEmailAdministrativaControlador(
            final GerenciarCaixaDeEmailAdministrativa gerenciarCaixaDeEmailAdministrativa
    ) {
        this.gerenciarCaixaDeEmailAdministrativa = gerenciarCaixaDeEmailAdministrativa;
    }

    @GetMapping
    public List<CaixaDeEmailAdministrativaDto> listar() {
        return gerenciarCaixaDeEmailAdministrativa.listar();
    }

    @GetMapping("/{id}")
    public CaixaDeEmailAdministrativaDto buscarPorId(@PathVariable final UUID id) {
        return gerenciarCaixaDeEmailAdministrativa.buscarPorId(id);
    }

    @PostMapping
    public CaixaDeEmailAdministrativaDto criar(@Valid @RequestBody final CaixaDeEmailRequisicao requisicao) {
        return gerenciarCaixaDeEmailAdministrativa.criar(
                requisicao.enderecoEmail(),
                requisicao.nomeExibicao(),
                requisicao.departamentoId()
        );
    }

    @PutMapping("/{id}")
    public CaixaDeEmailAdministrativaDto atualizar(
            @PathVariable final UUID id,
            @Valid @RequestBody final CaixaDeEmailRequisicao requisicao
    ) {
        return gerenciarCaixaDeEmailAdministrativa.atualizar(
                id,
                requisicao.enderecoEmail(),
                requisicao.nomeExibicao(),
                requisicao.departamentoId()
        );
    }

    @PatchMapping("/{id}/inativacao")
    public void inativar(@PathVariable final UUID id) {
        gerenciarCaixaDeEmailAdministrativa.inativar(id);
    }

    public record CaixaDeEmailRequisicao(
            @NotBlank(message = "deve ser informado")
            @Email(message = "deve ser um e-mail valido")
            String enderecoEmail,
            @NotBlank(message = "deve ser informado")
            String nomeExibicao,
            @NotNull(message = "deve ser informado")
            UUID departamentoId
    ) {
    }
}
