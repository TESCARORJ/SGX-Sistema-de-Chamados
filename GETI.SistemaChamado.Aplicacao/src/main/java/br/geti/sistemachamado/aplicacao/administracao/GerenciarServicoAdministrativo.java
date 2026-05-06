package br.geti.sistemachamado.aplicacao.administracao;

import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.repositorio.CategoriaRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.ServicoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class GerenciarServicoAdministrativo {

    private final ServicoRepositorio servicoRepositorio;
    private final CategoriaRepositorio categoriaRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;

    public GerenciarServicoAdministrativo(
            final ServicoRepositorio servicoRepositorio,
            final CategoriaRepositorio categoriaRepositorio,
            final DepartamentoRepositorio departamentoRepositorio
    ) {
        this.servicoRepositorio = servicoRepositorio;
        this.categoriaRepositorio = categoriaRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
    }

    public List<ServicoAdministrativoDto> listar() {
        return servicoRepositorio.listarTodos().stream().map(this::paraDto).toList();
    }

    public ServicoAdministrativoDto buscarPorId(final UUID id) {
        return paraDto(obterPorId(id));
    }

    public ServicoAdministrativoDto criar(
            final String nome,
            final String descricao,
            final UUID categoriaId,
            final UUID departamentoId
    ) {
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(nome, "nome do servico e obrigatorio");
        final var categoria = obterCategoria(categoriaId);
        final var departamento = obterDepartamento(departamentoId);
        validarDuplicidade(nomeNormalizado, departamento.id(), null);

        final var salvo = servicoRepositorio.salvar(new Servico(
                UUID.randomUUID(),
                nomeNormalizado,
                descricao,
                true,
                categoria,
                departamento,
                LocalDateTime.now(),
                null
        ));
        return paraDto(salvo);
    }

    public ServicoAdministrativoDto atualizar(
            final UUID id,
            final String nome,
            final String descricao,
            final UUID categoriaId,
            final UUID departamentoId
    ) {
        final var existente = obterPorId(id);
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(nome, "nome do servico e obrigatorio");
        final var categoria = obterCategoria(categoriaId);
        final var departamento = obterDepartamento(departamentoId);
        validarDuplicidade(nomeNormalizado, departamento.id(), existente.id());

        final var atualizado = new Servico(
                existente.id(),
                nomeNormalizado,
                descricao,
                existente.ativo(),
                categoria,
                departamento,
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        return paraDto(servicoRepositorio.salvar(atualizado));
    }

    public void inativar(final UUID id) {
        final var existente = obterPorId(id);
        if (!existente.ativo()) {
            return;
        }

        servicoRepositorio.salvar(new Servico(
                existente.id(),
                existente.nome(),
                existente.descricao(),
                false,
                existente.categoria(),
                existente.departamento(),
                existente.dataCriacao(),
                LocalDateTime.now()
        ));
    }

    private Servico obterPorId(final UUID id) {
        ValidadorDominio.obrigatorio(id, "id do servico e obrigatorio");
        return servicoRepositorio.buscarPorId(id)
                .orElseThrow(() -> new ErroDeDominio("Servico nao encontrado."));
    }

    private br.geti.sistemachamado.dominio.administracao.Categoria obterCategoria(final UUID categoriaId) {
        ValidadorDominio.obrigatorio(categoriaId, "categoria do servico e obrigatoria");
        return categoriaRepositorio.buscarPorId(categoriaId)
                .orElseThrow(() -> new ErroDeDominio("Categoria do servico nao encontrada."));
    }

    private br.geti.sistemachamado.dominio.administracao.Departamento obterDepartamento(final UUID departamentoId) {
        ValidadorDominio.obrigatorio(departamentoId, "departamento do servico e obrigatorio");
        return departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio("Departamento do servico nao encontrado."));
    }

    private void validarDuplicidade(final String nome, final UUID departamentoId, final UUID idAtual) {
        final var existente = servicoRepositorio.buscarPorNomeEDepartamento(nome, departamentoId);
        if (existente.isPresent() && (idAtual == null || !existente.get().id().equals(idAtual))) {
            throw new ErroDeDominio("Ja existe servico com este nome no departamento.");
        }
    }

    private ServicoAdministrativoDto paraDto(final Servico servico) {
        return new ServicoAdministrativoDto(
                servico.id(),
                servico.nome(),
                servico.descricao(),
                servico.ativo(),
                servico.categoria().id(),
                servico.categoria().nome(),
                servico.departamento().id(),
                servico.departamento().nome(),
                servico.dataCriacao(),
                servico.dataAtualizacao()
        );
    }
}
