package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.PerfilAcesso;
import br.geti.sistemachamado.dominio.administracao.repositorio.PerfilAcessoRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.PerfilAcessoEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.PerfilAcessoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class PerfilAcessoRepositorioJpa implements PerfilAcessoRepositorio {

    private final PerfilAcessoJpaRepository perfilAcessoJpaRepository;

    public PerfilAcessoRepositorioJpa(final PerfilAcessoJpaRepository perfilAcessoJpaRepository) {
        this.perfilAcessoJpaRepository = perfilAcessoJpaRepository;
    }

    @Override
    @Transactional
    public PerfilAcesso salvar(final PerfilAcesso perfilAcesso) {
        final var entidade = new PerfilAcessoEntidadeJpa();
        entidade.setNome(perfilAcesso.nome());
        entidade.setDescricao(perfilAcesso.descricao());
        entidade.setAtivo(perfilAcesso.ativo());

        final var salva = perfilAcessoJpaRepository.save(entidade);
        return AdministracaoMapeadorJpa.paraDominio(salva);
    }

    @Override
    public Optional<PerfilAcesso> buscarPorId(final UUID id) {
        return perfilAcessoJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<PerfilAcesso> buscarPorNome(final String nome) {
        return perfilAcessoJpaRepository.findByNomeIgnoreCase(nome).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<PerfilAcesso> listarAtivos() {
        return perfilAcessoJpaRepository.findByAtivoTrue().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}