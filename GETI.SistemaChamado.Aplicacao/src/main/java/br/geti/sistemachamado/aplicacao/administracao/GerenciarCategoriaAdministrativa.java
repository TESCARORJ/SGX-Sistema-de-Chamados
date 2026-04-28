package br.geti.sistemachamado.aplicacao.administracao;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.repositorio.CategoriaRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class GerenciarCategoriaAdministrativa {

    private final CategoriaRepositorio categoriaRepositorio;

    public GerenciarCategoriaAdministrativa(final CategoriaRepositorio categoriaRepositorio) {
        this.categoriaRepositorio = categoriaRepositorio;
    }

    public List<CategoriaAdministrativaDto> listar() {
        return categoriaRepositorio.listarTodos().stream().map(this::paraDto).toList();
    }

    public CategoriaAdministrativaDto buscarPorId(final UUID id) {
        return paraDto(obterPorId(id));
    }

    public CategoriaAdministrativaDto criar(final String nome, final String descricao) {
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(nome, "nome da categoria e obrigatorio");
        validarNomeDuplicado(nomeNormalizado, null);

        final var salvo = categoriaRepositorio.salvar(new Categoria(
                UUID.randomUUID(),
                nomeNormalizado,
                descricao,
                true,
                LocalDateTime.now(),
                null
        ));
        return paraDto(salvo);
    }

    public CategoriaAdministrativaDto atualizar(final UUID id, final String nome, final String descricao) {
        final var existente = obterPorId(id);
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(nome, "nome da categoria e obrigatorio");
        validarNomeDuplicado(nomeNormalizado, existente.id());

        final var atualizado = new Categoria(
                existente.id(),
                nomeNormalizado,
                descricao,
                existente.ativo(),
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        return paraDto(categoriaRepositorio.salvar(atualizado));
    }

    public void inativar(final UUID id) {
        final var existente = obterPorId(id);
        if (!existente.ativo()) {
            return;
        }

        categoriaRepositorio.salvar(new Categoria(
                existente.id(),
                existente.nome(),
                existente.descricao(),
                false,
                existente.dataCriacao(),
                LocalDateTime.now()
        ));
    }

    private Categoria obterPorId(final UUID id) {
        ValidadorDominio.obrigatorio(id, "id da categoria e obrigatorio");
        return categoriaRepositorio.buscarPorId(id)
                .orElseThrow(() -> new ErroDeDominio("Categoria nao encontrada."));
    }

    private void validarNomeDuplicado(final String nome, final UUID idAtual) {
        final var existente = categoriaRepositorio.buscarPorNome(nome);
        if (existente.isPresent() && (idAtual == null || !existente.get().id().equals(idAtual))) {
            throw new ErroDeDominio("Ja existe categoria com este nome.");
        }
    }

    private CategoriaAdministrativaDto paraDto(final Categoria categoria) {
        return new CategoriaAdministrativaDto(
                categoria.id(),
                categoria.nome(),
                categoria.descricao(),
                categoria.ativo(),
                categoria.dataCriacao(),
                categoria.dataAtualizacao()
        );
    }
}
