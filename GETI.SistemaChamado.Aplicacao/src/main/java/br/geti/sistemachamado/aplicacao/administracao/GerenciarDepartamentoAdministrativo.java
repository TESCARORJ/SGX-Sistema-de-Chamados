package br.geti.sistemachamado.aplicacao.administracao;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class GerenciarDepartamentoAdministrativo {

    private final DepartamentoRepositorio departamentoRepositorio;

    public GerenciarDepartamentoAdministrativo(final DepartamentoRepositorio departamentoRepositorio) {
        this.departamentoRepositorio = departamentoRepositorio;
    }

    public List<DepartamentoAdministrativoDto> listar() {
        return departamentoRepositorio.listarTodos().stream().map(this::paraDto).toList();
    }

    public DepartamentoAdministrativoDto buscarPorId(final UUID id) {
        return paraDto(obterPorId(id));
    }

    public DepartamentoAdministrativoDto criar(final String nome) {
        final var nomeNormalizado = normalizarNome(nome);
        validarNomeDuplicado(nomeNormalizado, null);

        final var salvo = departamentoRepositorio.salvar(new Departamento(
                UUID.randomUUID(),
                nomeNormalizado,
                true,
                LocalDateTime.now(),
                null
        ));
        return paraDto(salvo);
    }

    public DepartamentoAdministrativoDto atualizar(final UUID id, final String nome) {
        final var existente = obterPorId(id);
        final var nomeNormalizado = normalizarNome(nome);
        validarNomeDuplicado(nomeNormalizado, existente.id());

        final var atualizado = new Departamento(
                existente.id(),
                nomeNormalizado,
                existente.ativo(),
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        return paraDto(departamentoRepositorio.salvar(atualizado));
    }

    public void inativar(final UUID id) {
        final var existente = obterPorId(id);
        if (!existente.ativo()) {
            return;
        }

        final var inativo = new Departamento(
                existente.id(),
                existente.nome(),
                false,
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        departamentoRepositorio.salvar(inativo);
    }

    private Departamento obterPorId(final UUID id) {
        ValidadorDominio.obrigatorio(id, "id do departamento e obrigatorio");
        return departamentoRepositorio.buscarPorId(id)
                .orElseThrow(() -> new ErroDeDominio("Departamento nao encontrado."));
    }

    private String normalizarNome(final String nome) {
        return ValidadorDominio.textoObrigatorio(nome, "nome do departamento e obrigatorio");
    }

    private void validarNomeDuplicado(final String nome, final UUID idAtual) {
        final var existente = departamentoRepositorio.buscarPorNome(nome);
        if (existente.isPresent() && (idAtual == null || !existente.get().id().equals(idAtual))) {
            throw new ErroDeDominio("Ja existe departamento com este nome.");
        }
    }

    private DepartamentoAdministrativoDto paraDto(final Departamento departamento) {
        return new DepartamentoAdministrativoDto(
                departamento.id(),
                departamento.nome(),
                departamento.ativo(),
                departamento.dataCriacao(),
                departamento.dataAtualizacao()
        );
    }
}
