package br.geti.sistemachamado.aplicacao.chamado.automacao;

import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.util.List;
import java.util.Locale;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Service;

@Service
public class AutomacaoOperacionalChamado {

    private final UsuarioRepositorio usuarioRepositorio;

    public AutomacaoOperacionalChamado(final UsuarioRepositorio usuarioRepositorio) {
        this.usuarioRepositorio = usuarioRepositorio;
    }

    public Optional<ResultadoAtribuicaoAutomaticaChamado> resolverAtribuicaoAutomatica(
            final Departamento departamento,
            final UUID usuarioExcluidoId
    ) {
        final var departamentoAplicado = ValidadorDominio.obrigatorio(
                departamento,
                "departamento para automacao de atribuicao e obrigatorio"
        );
        final List<Usuario> candidatos = usuarioRepositorio.listarAtivos().stream()
                .filter(this::usuarioInterno)
                .filter(usuario -> usuario.departamento() != null)
                .filter(usuario -> usuario.departamento().id().equals(departamentoAplicado.id()))
                .filter(usuario -> usuarioExcluidoId == null || !usuario.id().equals(usuarioExcluidoId))
                .toList();

        if (candidatos.size() != 1) {
            return Optional.empty();
        }

        final var responsavel = candidatos.getFirst();
        final var motivo = "Atribuicao automatica aplicada por existir unico atendente interno ativo no departamento "
                + departamentoAplicado.nome() + ".";
        return Optional.of(new ResultadoAtribuicaoAutomaticaChamado(responsavel, motivo));
    }

    private boolean usuarioInterno(final Usuario usuario) {
        final var perfil = usuario.perfilAcesso().nome().trim().toLowerCase(Locale.ROOT);
        return !perfil.equals("solicitante");
    }
}
