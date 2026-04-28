package br.geti.sistemachamado.aplicacao.administracao;

import br.geti.sistemachamado.dominio.administracao.GrupoAtendimento;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.GrupoAtendimentoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class GerenciarGrupoAtendimentoAdministrativo {

    private final GrupoAtendimentoRepositorio grupoAtendimentoRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;

    public GerenciarGrupoAtendimentoAdministrativo(
            final GrupoAtendimentoRepositorio grupoAtendimentoRepositorio,
            final DepartamentoRepositorio departamentoRepositorio
    ) {
        this.grupoAtendimentoRepositorio = grupoAtendimentoRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
    }

    public List<GrupoAtendimentoAdministrativoDto> listar() {
        return grupoAtendimentoRepositorio.listarTodos().stream().map(this::paraDto).toList();
    }

    public GrupoAtendimentoAdministrativoDto buscarPorId(final UUID id) {
        return paraDto(obterPorId(id));
    }

    public GrupoAtendimentoAdministrativoDto criar(
            final String nome,
            final String descricao,
            final UUID departamentoId
    ) {
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(
                nome,
                "nome do grupo de atendimento e obrigatorio"
        );
        final var departamento = obterDepartamento(departamentoId);
        validarDuplicidade(nomeNormalizado, departamento.id(), null);

        final var salvo = grupoAtendimentoRepositorio.salvar(new GrupoAtendimento(
                UUID.randomUUID(),
                nomeNormalizado,
                descricao,
                true,
                departamento,
                LocalDateTime.now(),
                null
        ));
        return paraDto(salvo);
    }

    public GrupoAtendimentoAdministrativoDto atualizar(
            final UUID id,
            final String nome,
            final String descricao,
            final UUID departamentoId
    ) {
        final var existente = obterPorId(id);
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(
                nome,
                "nome do grupo de atendimento e obrigatorio"
        );
        final var departamento = obterDepartamento(departamentoId);
        validarDuplicidade(nomeNormalizado, departamento.id(), existente.id());

        final var atualizado = new GrupoAtendimento(
                existente.id(),
                nomeNormalizado,
                descricao,
                existente.ativo(),
                departamento,
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        return paraDto(grupoAtendimentoRepositorio.salvar(atualizado));
    }

    public void inativar(final UUID id) {
        final var existente = obterPorId(id);
        if (!existente.ativo()) {
            return;
        }

        grupoAtendimentoRepositorio.salvar(new GrupoAtendimento(
                existente.id(),
                existente.nome(),
                existente.descricao(),
                false,
                existente.departamento(),
                existente.dataCriacao(),
                LocalDateTime.now()
        ));
    }

    private GrupoAtendimento obterPorId(final UUID id) {
        ValidadorDominio.obrigatorio(id, "id do grupo de atendimento e obrigatorio");
        return grupoAtendimentoRepositorio.buscarPorId(id)
                .orElseThrow(() -> new ErroDeDominio("Grupo de atendimento nao encontrado."));
    }

    private br.geti.sistemachamado.dominio.administracao.Departamento obterDepartamento(final UUID departamentoId) {
        ValidadorDominio.obrigatorio(departamentoId, "departamento do grupo de atendimento e obrigatorio");
        return departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio("Departamento do grupo de atendimento nao encontrado."));
    }

    private void validarDuplicidade(final String nome, final UUID departamentoId, final UUID idAtual) {
        final var existente = grupoAtendimentoRepositorio.buscarPorNomeEDepartamento(nome, departamentoId);
        if (existente.isPresent() && (idAtual == null || !existente.get().id().equals(idAtual))) {
            throw new ErroDeDominio("Ja existe grupo de atendimento com este nome no departamento.");
        }
    }

    private GrupoAtendimentoAdministrativoDto paraDto(final GrupoAtendimento grupoAtendimento) {
        return new GrupoAtendimentoAdministrativoDto(
                grupoAtendimento.id(),
                grupoAtendimento.nome(),
                grupoAtendimento.descricao(),
                grupoAtendimento.ativo(),
                grupoAtendimento.departamento().id(),
                grupoAtendimento.departamento().nome(),
                grupoAtendimento.dataCriacao(),
                grupoAtendimento.dataAtualizacao()
        );
    }
}
