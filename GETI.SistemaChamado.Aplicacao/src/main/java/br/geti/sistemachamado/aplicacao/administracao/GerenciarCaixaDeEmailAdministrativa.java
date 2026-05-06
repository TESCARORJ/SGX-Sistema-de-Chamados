package br.geti.sistemachamado.aplicacao.administracao;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Locale;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class GerenciarCaixaDeEmailAdministrativa {

    private final CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;

    public GerenciarCaixaDeEmailAdministrativa(
            final CaixaDeEmailRepositorio caixaDeEmailRepositorio,
            final DepartamentoRepositorio departamentoRepositorio
    ) {
        this.caixaDeEmailRepositorio = caixaDeEmailRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
    }

    public List<CaixaDeEmailAdministrativaDto> listar() {
        return caixaDeEmailRepositorio.listarTodos().stream().map(this::paraDto).toList();
    }

    public CaixaDeEmailAdministrativaDto buscarPorId(final UUID id) {
        return paraDto(obterPorId(id));
    }

    public CaixaDeEmailAdministrativaDto criar(
            final String enderecoEmail,
            final String nomeExibicao,
            final UUID departamentoId
    ) {
        final var enderecoNormalizado = normalizarEmail(enderecoEmail);
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(
                nomeExibicao,
                "nome de exibicao da caixa de email e obrigatorio"
        );
        final var departamento = obterDepartamento(departamentoId);
        validarEnderecoDuplicado(enderecoNormalizado, null);

        final var salvo = caixaDeEmailRepositorio.salvar(new CaixaDeEmail(
                UUID.randomUUID(),
                enderecoNormalizado,
                nomeNormalizado,
                true,
                departamento,
                LocalDateTime.now(),
                null
        ));
        return paraDto(salvo);
    }

    public CaixaDeEmailAdministrativaDto atualizar(
            final UUID id,
            final String enderecoEmail,
            final String nomeExibicao,
            final UUID departamentoId
    ) {
        final var existente = obterPorId(id);
        final var enderecoNormalizado = normalizarEmail(enderecoEmail);
        final var nomeNormalizado = ValidadorDominio.textoObrigatorio(
                nomeExibicao,
                "nome de exibicao da caixa de email e obrigatorio"
        );
        final var departamento = obterDepartamento(departamentoId);
        validarEnderecoDuplicado(enderecoNormalizado, existente.id());

        final var atualizado = new CaixaDeEmail(
                existente.id(),
                enderecoNormalizado,
                nomeNormalizado,
                existente.ativa(),
                departamento,
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        return paraDto(caixaDeEmailRepositorio.salvar(atualizado));
    }

    public void inativar(final UUID id) {
        final var existente = obterPorId(id);
        if (!existente.ativa()) {
            return;
        }

        final var inativa = new CaixaDeEmail(
                existente.id(),
                existente.enderecoEmail(),
                existente.nomeExibicao(),
                false,
                existente.departamento(),
                existente.dataCriacao(),
                LocalDateTime.now()
        );
        caixaDeEmailRepositorio.salvar(inativa);
    }

    private CaixaDeEmail obterPorId(final UUID id) {
        ValidadorDominio.obrigatorio(id, "id da caixa de email e obrigatorio");
        return caixaDeEmailRepositorio.buscarPorId(id)
                .orElseThrow(() -> new ErroDeDominio("Caixa de email nao encontrada."));
    }

    private br.geti.sistemachamado.dominio.administracao.Departamento obterDepartamento(final UUID departamentoId) {
        ValidadorDominio.obrigatorio(departamentoId, "departamento da caixa de email e obrigatorio");
        return departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio("Departamento da caixa de email nao encontrado."));
    }

    private String normalizarEmail(final String enderecoEmail) {
        return ValidadorDominio.textoObrigatorio(
                enderecoEmail,
                "endereco de email da caixa de email e obrigatorio"
        ).toLowerCase(Locale.ROOT);
    }

    private void validarEnderecoDuplicado(final String enderecoEmail, final UUID idAtual) {
        final var existente = caixaDeEmailRepositorio.buscarPorEnderecoEmail(enderecoEmail);
        if (existente.isPresent() && (idAtual == null || !existente.get().id().equals(idAtual))) {
            throw new ErroDeDominio("Ja existe caixa de email com este endereco.");
        }
    }

    private CaixaDeEmailAdministrativaDto paraDto(final CaixaDeEmail caixaDeEmail) {
        return new CaixaDeEmailAdministrativaDto(
                caixaDeEmail.id(),
                caixaDeEmail.enderecoEmail(),
                caixaDeEmail.nomeExibicao(),
                caixaDeEmail.ativa(),
                caixaDeEmail.departamento().id(),
                caixaDeEmail.departamento().nome(),
                caixaDeEmail.dataCriacao(),
                caixaDeEmail.dataAtualizacao()
        );
    }
}
