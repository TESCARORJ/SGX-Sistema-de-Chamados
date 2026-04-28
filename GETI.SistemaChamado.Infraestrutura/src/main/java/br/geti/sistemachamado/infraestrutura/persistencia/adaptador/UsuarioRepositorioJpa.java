package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.PerfilAcessoJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.UsuarioJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class UsuarioRepositorioJpa implements UsuarioRepositorio {

    private final UsuarioJpaRepository usuarioJpaRepository;
    private final PerfilAcessoJpaRepository perfilAcessoJpaRepository;
    private final DepartamentoJpaRepository departamentoJpaRepository;

    public UsuarioRepositorioJpa(
            final UsuarioJpaRepository usuarioJpaRepository,
            final PerfilAcessoJpaRepository perfilAcessoJpaRepository,
            final DepartamentoJpaRepository departamentoJpaRepository
    ) {
        this.usuarioJpaRepository = usuarioJpaRepository;
        this.perfilAcessoJpaRepository = perfilAcessoJpaRepository;
        this.departamentoJpaRepository = departamentoJpaRepository;
    }

    @Override
    @Transactional
    public Usuario salvar(final Usuario usuario) {
        final var entidade = usuarioJpaRepository.findById(usuario.id()).orElseGet(UsuarioEntidadeJpa::new);
        entidade.setNome(usuario.nome());
        entidade.setLogin(usuario.login());
        entidade.setEmail(usuario.email());
        entidade.setAtivo(usuario.ativo());
        entidade.setPerfilAcesso(perfilAcessoJpaRepository.getReferenceById(usuario.perfilAcesso().id()));

        if (usuario.departamento() != null) {
            entidade.setDepartamento(departamentoJpaRepository.getReferenceById(usuario.departamento().id()));
        } else {
            entidade.setDepartamento(null);
        }

        final var salva = usuarioJpaRepository.save(entidade);
        return AdministracaoMapeadorJpa.paraDominio(salva);
    }

    @Override
    public Optional<Usuario> buscarPorLogin(final String login) {
        return usuarioJpaRepository.findByLoginIgnoreCase(login).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Usuario> buscarPorId(final UUID id) {
        return usuarioJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<Usuario> buscarPorEmail(final String email) {
        return usuarioJpaRepository.findByEmailIgnoreCase(email).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<Usuario> listarAtivos() {
        return usuarioJpaRepository.findByAtivoTrue().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}
