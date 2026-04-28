package br.geti.sistemachamado.infraestrutura.persistencia.adaptador;

import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
import br.geti.sistemachamado.infraestrutura.persistencia.adaptador.mapeador.AdministracaoMapeadorJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.CaixaDeEmailEntidadeJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.CaixaDeEmailJpaRepository;
import br.geti.sistemachamado.infraestrutura.persistencia.repositorio.DepartamentoJpaRepository;
import java.util.List;
import java.util.Optional;
import java.util.UUID;
import org.springframework.stereotype.Repository;
import org.springframework.transaction.annotation.Transactional;

@Repository
@Transactional(readOnly = true)
public class CaixaDeEmailRepositorioJpa implements CaixaDeEmailRepositorio {

    private final CaixaDeEmailJpaRepository caixaDeEmailJpaRepository;
    private final DepartamentoJpaRepository departamentoJpaRepository;

    public CaixaDeEmailRepositorioJpa(
            final CaixaDeEmailJpaRepository caixaDeEmailJpaRepository,
            final DepartamentoJpaRepository departamentoJpaRepository
    ) {
        this.caixaDeEmailJpaRepository = caixaDeEmailJpaRepository;
        this.departamentoJpaRepository = departamentoJpaRepository;
    }

    @Override
    @Transactional
    public CaixaDeEmail salvar(final CaixaDeEmail caixaDeEmail) {
        final var entidade = new CaixaDeEmailEntidadeJpa();
        entidade.setEnderecoEmail(caixaDeEmail.enderecoEmail());
        entidade.setNomeExibicao(caixaDeEmail.nomeExibicao());
        entidade.setAtiva(caixaDeEmail.ativa());
        entidade.setDepartamento(departamentoJpaRepository.getReferenceById(caixaDeEmail.departamento().id()));

        final var salva = caixaDeEmailJpaRepository.save(entidade);
        return AdministracaoMapeadorJpa.paraDominio(salva);
    }

    @Override
    public Optional<CaixaDeEmail> buscarPorId(final UUID id) {
        return caixaDeEmailJpaRepository.findById(id).map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public Optional<CaixaDeEmail> buscarPorEnderecoEmail(final String enderecoEmail) {
        return caixaDeEmailJpaRepository.findByEnderecoEmailIgnoreCase(enderecoEmail)
                .map(AdministracaoMapeadorJpa::paraDominio);
    }

    @Override
    public List<CaixaDeEmail> listarAtivas() {
        return caixaDeEmailJpaRepository.findByAtivaTrue().stream()
                .map(AdministracaoMapeadorJpa::paraDominio)
                .toList();
    }
}