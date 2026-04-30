package br.geti.sistemachamado.aplicacao.chamado.admin;

import br.geti.sistemachamado.dominio.administracao.Categoria;
import br.geti.sistemachamado.dominio.administracao.Departamento;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.CategoriaRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.DepartamentoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.ServicoRepositorio;
import br.geti.sistemachamado.dominio.administracao.repositorio.UsuarioRepositorio;
import br.geti.sistemachamado.dominio.chamado.AnexoChamado;
import br.geti.sistemachamado.dominio.chamado.Chamado;
import br.geti.sistemachamado.dominio.chamado.HistoricoChamado;
import br.geti.sistemachamado.dominio.chamado.InteracaoChamado;
import br.geti.sistemachamado.dominio.chamado.OrigemChamado;
import br.geti.sistemachamado.dominio.chamado.PrioridadeChamado;
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.TipoInteracao;
import br.geti.sistemachamado.dominio.chamado.repositorio.AnexoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.ChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.HistoricoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.InteracaoChamadoRepositorio;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.Comparator;
import java.util.List;
import java.util.Locale;
import java.util.UUID;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional(readOnly = true)
public class GerenciarChamadoAdministrativo {

    private final ChamadoRepositorio chamadoRepositorio;
    private final InteracaoChamadoRepositorio interacaoChamadoRepositorio;
    private final HistoricoChamadoRepositorio historicoChamadoRepositorio;
    private final AnexoChamadoRepositorio anexoChamadoRepositorio;
    private final UsuarioRepositorio usuarioRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;
    private final CategoriaRepositorio categoriaRepositorio;
    private final ServicoRepositorio servicoRepositorio;

    public GerenciarChamadoAdministrativo(
            final ChamadoRepositorio chamadoRepositorio,
            final InteracaoChamadoRepositorio interacaoChamadoRepositorio,
            final HistoricoChamadoRepositorio historicoChamadoRepositorio,
            final AnexoChamadoRepositorio anexoChamadoRepositorio,
            final UsuarioRepositorio usuarioRepositorio,
            final DepartamentoRepositorio departamentoRepositorio,
            final CategoriaRepositorio categoriaRepositorio,
            final ServicoRepositorio servicoRepositorio
    ) {
        this.chamadoRepositorio = chamadoRepositorio;
        this.interacaoChamadoRepositorio = interacaoChamadoRepositorio;
        this.historicoChamadoRepositorio = historicoChamadoRepositorio;
        this.anexoChamadoRepositorio = anexoChamadoRepositorio;
        this.usuarioRepositorio = usuarioRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
        this.categoriaRepositorio = categoriaRepositorio;
        this.servicoRepositorio = servicoRepositorio;
    }

    public CatalogoChamadoAdminDto consultarCatalogo() {
        final var departamentos = departamentoRepositorio.listarAtivos().stream()
                .map(dep -> new OpcaoCatalogoAdminDto(dep.id(), dep.nome()))
                .toList();
        final var categorias = categoriaRepositorio.listarAtivas().stream()
                .map(cat -> new OpcaoCatalogoAdminDto(cat.id(), cat.nome()))
                .toList();
        final var servicos = servicoRepositorio.listarAtivos().stream()
                .map(serv -> new OpcaoCatalogoAdminDto(serv.id(), serv.nome()))
                .toList();
        final var responsaveis = usuarioRepositorio.listarAtivos().stream()
                .filter(this::usuarioInterno)
                .sorted(Comparator.comparing(Usuario::nome, String.CASE_INSENSITIVE_ORDER))
                .map(usuario -> new ResponsavelChamadoAdminDto(
                        usuario.id(),
                        usuario.nome(),
                        usuario.login(),
                        usuario.perfilAcesso().nome()
                ))
                .toList();

        return new CatalogoChamadoAdminDto(
                departamentos,
                categorias,
                servicos,
                responsaveis,
                List.of(
                        SituacaoChamado.ABERTO.name(),
                        SituacaoChamado.EM_ATENDIMENTO.name(),
                        SituacaoChamado.AGUARDANDO_SOLICITANTE.name(),
                        SituacaoChamado.RESOLVIDO.name(),
                        SituacaoChamado.CANCELADO.name()
                ),
                List.of(
                        PrioridadeChamado.BAIXA.name(),
                        PrioridadeChamado.MEDIA.name(),
                        PrioridadeChamado.ALTA.name(),
                        PrioridadeChamado.CRITICA.name()
                ),
                List.of(OrigemChamado.PORTAL.name(), OrigemChamado.EMAIL.name())
        );
    }

    public DashboardAdminChamadoDto consultarDashboard() {
        final var chamados = chamadoRepositorio.listarTodos();

        final var porSituacao = List.of(
                SituacaoChamado.ABERTO,
                SituacaoChamado.EM_ATENDIMENTO,
                SituacaoChamado.AGUARDANDO_SOLICITANTE,
                SituacaoChamado.RESOLVIDO,
                SituacaoChamado.CANCELADO
        ).stream().map(situacao -> new IndicadorAdminChamadoDto(
                situacao.name(),
                chamados.stream().filter(ch -> ch.situacao() == situacao).count()
        )).toList();

        final var porPrioridade = List.of(
                PrioridadeChamado.CRITICA,
                PrioridadeChamado.ALTA,
                PrioridadeChamado.MEDIA,
                PrioridadeChamado.BAIXA
        ).stream().map(prioridade -> new IndicadorAdminChamadoDto(
                prioridade.name(),
                chamados.stream().filter(ch -> ch.prioridade() == prioridade).count()
        )).toList();

        final var porDepartamento = chamados.stream()
                .collect(java.util.stream.Collectors.groupingBy(
                        chamado -> chamado.departamento().nome(),
                        java.util.stream.Collectors.counting()
                ))
                .entrySet().stream()
                .sorted((a, b) -> Long.compare(b.getValue(), a.getValue()))
                .map(entry -> new IndicadorAdminChamadoDto(entry.getKey(), entry.getValue()))
                .toList();

        final var pendentesRecentes = chamados.stream()
                .filter(chamado -> chamado.situacao() != SituacaoChamado.RESOLVIDO
                        && chamado.situacao() != SituacaoChamado.CANCELADO)
                .sorted(Comparator.comparing(Chamado::dataCriacao).reversed())
                .limit(10)
                .map(this::paraResumoDashboard)
                .toList();

        return new DashboardAdminChamadoDto(porSituacao, porPrioridade, porDepartamento, pendentesRecentes);
    }

    public List<ChamadoAdminFilaDto> listarFila(final ChamadoAdminFiltroFilaComando filtro) {
        final var filtroAplicado = filtro != null
                ? filtro
                : new ChamadoAdminFiltroFilaComando(null, null, null, null, null);

        return chamadoRepositorio.listarTodos().stream()
                .filter(chamado -> filtroAplicado.situacao() == null || chamado.situacao() == filtroAplicado.situacao())
                .filter(chamado -> filtroAplicado.prioridade() == null || chamado.prioridade() == filtroAplicado.prioridade())
                .filter(chamado -> filtroAplicado.departamentoId() == null
                        || chamado.departamento().id().equals(filtroAplicado.departamentoId()))
                .filter(chamado -> filtroAplicado.origem() == null || chamado.origem() == filtroAplicado.origem())
                .filter(chamado -> filtroAplicado.responsavelId() == null
                        || (chamado.responsavel() != null && chamado.responsavel().id().equals(filtroAplicado.responsavelId())))
                .sorted(Comparator.comparing(Chamado::dataCriacao).reversed())
                .map(this::paraFila)
                .toList();
    }

    public ChamadoAdminDetalheDto detalhar(final UUID chamadoId) {
        final var chamado = obterChamado(chamadoId);
        return montarDetalhe(chamado);
    }

    @Transactional
    public ChamadoAdminDetalheDto atribuir(final AtribuicaoChamadoAdminComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados de atribuicao do chamado sao obrigatorios");
        final var chamado = obterChamado(comando.chamadoId());
        final var agente = obterUsuarioInternoAtivo(comando.agenteId(), "agente da atribuicao");
        final var responsavel = obterUsuarioInternoAtivo(comando.responsavelId(), "responsavel da atribuicao");

        if (chamado.responsavel() != null && chamado.responsavel().id().equals(responsavel.id())) {
            throw new ErroDeDominio("Chamado ja esta atribuido ao responsavel informado.");
        }

        final var atualizado = atualizarChamado(chamado, chamado.situacao(), responsavel, chamado.departamento(),
                chamado.categoria(), chamado.servico());
        final var salvo = chamadoRepositorio.salvar(atualizado);

        registrarInteracao(
                salvo.id(),
                TipoInteracao.ATRIBUICAO,
                "Atribuicao do chamado para " + responsavel.nome() + " por " + agente.nome() + ".",
                false,
                agente
        );
        registrarHistorico(
                salvo.id(),
                "Atribuicao registrada para " + responsavel.nome() + ".",
                salvo.situacao(),
                salvo.situacao(),
                false
        );

        return montarDetalhe(salvo);
    }

    @Transactional
    public ChamadoAdminDetalheDto alterarSituacao(final AlteracaoSituacaoChamadoAdminComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados da alteracao de situacao sao obrigatorios");
        final var chamado = obterChamado(comando.chamadoId());
        final var agente = obterUsuarioInternoAtivo(comando.agenteId(), "agente da alteracao de situacao");
        final var novaSituacao = ValidadorDominio.obrigatorio(comando.novaSituacao(), "nova situacao e obrigatoria");

        if (chamado.situacao() == novaSituacao) {
            throw new ErroDeDominio("Chamado ja esta na situacao informada.");
        }

        final var situacaoAnterior = chamado.situacao();
        final var atualizado = atualizarChamado(
                chamado,
                novaSituacao,
                chamado.responsavel(),
                chamado.departamento(),
                chamado.categoria(),
                chamado.servico()
        );
        final var salvo = chamadoRepositorio.salvar(atualizado);

        registrarInteracao(
                salvo.id(),
                TipoInteracao.ALTERACAO_SITUACAO,
                "Situacao alterada de " + situacaoAnterior.name() + " para " + novaSituacao.name() + " por "
                        + agente.nome() + ".",
                true,
                agente
        );
        registrarHistorico(
                salvo.id(),
                "Situacao alterada pela equipe administrativa.",
                situacaoAnterior,
                novaSituacao,
                true
        );

        return montarDetalhe(salvo);
    }

    @Transactional
    public ChamadoAdminDetalheDto encaminhar(final EncaminhamentoChamadoAdminComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados do encaminhamento sao obrigatorios");
        final var chamado = obterChamado(comando.chamadoId());
        final var agente = obterUsuarioInternoAtivo(comando.agenteId(), "agente do encaminhamento");
        final var departamento = obterDepartamentoAtivo(comando.departamentoId());
        final var categoria = obterCategoriaAtiva(comando.categoriaId());
        final var servico = obterServicoAtivo(comando.servicoId());

        final var semAlteracao = chamado.departamento().id().equals(departamento.id())
                && chamado.categoria().id().equals(categoria.id())
                && chamado.servico().id().equals(servico.id());
        if (semAlteracao) {
            throw new ErroDeDominio("Encaminhamento nao alterou departamento/categoria/servico do chamado.");
        }

        final var atualizado = atualizarChamado(
                chamado,
                chamado.situacao(),
                null,
                departamento,
                categoria,
                servico
        );
        final var salvo = chamadoRepositorio.salvar(atualizado);

        registrarInteracao(
                salvo.id(),
                TipoInteracao.ENCAMINHAMENTO,
                "Chamado encaminhado por " + agente.nome() + " para departamento " + departamento.nome()
                        + ", categoria " + categoria.nome() + ", servico " + servico.nome() + ".",
                false,
                agente
        );
        registrarHistorico(
                salvo.id(),
                "Encaminhamento administrativo registrado.",
                salvo.situacao(),
                salvo.situacao(),
                false
        );

        return montarDetalhe(salvo);
    }

    @Transactional
    public ChamadoAdminDetalheDto comentarPublicamente(final ComentarioChamadoAdminComando comando) {
        return comentar(comando, true, TipoInteracao.COMENTARIO_PUBLICO, true, "Comentario publico registrado.");
    }

    @Transactional
    public ChamadoAdminDetalheDto comentarInternamente(final ComentarioChamadoAdminComando comando) {
        return comentar(comando, false, TipoInteracao.COMENTARIO_INTERNO, false, "Comentario interno registrado.");
    }

    private ChamadoAdminDetalheDto comentar(
            final ComentarioChamadoAdminComando comando,
            final boolean visivelSolicitante,
            final TipoInteracao tipoInteracao,
            final boolean historicoVisivelSolicitante,
            final String descricaoHistorico
    ) {
        ValidadorDominio.obrigatorio(comando, "dados de comentario sao obrigatorios");
        final var chamado = obterChamado(comando.chamadoId());
        final var autor = obterUsuarioInternoAtivo(comando.autorId(), "autor do comentario");
        final var mensagem = ValidadorDominio.textoObrigatorio(comando.mensagem(), "mensagem do comentario e obrigatoria");

        registrarInteracao(chamado.id(), tipoInteracao, mensagem, visivelSolicitante, autor);
        registrarHistorico(
                chamado.id(),
                descricaoHistorico,
                chamado.situacao(),
                chamado.situacao(),
                historicoVisivelSolicitante
        );
        return montarDetalhe(obterChamado(chamado.id()));
    }

    private ChamadoAdminDetalheDto montarDetalhe(final Chamado chamado) {
        final var interacoes = interacaoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .map(this::paraInteracao)
                .toList();
        final var historicos = historicoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .map(this::paraHistorico)
                .toList();
        final var anexos = anexoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .map(this::paraAnexo)
                .toList();

        return new ChamadoAdminDetalheDto(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.descricao(),
                chamado.situacao().name(),
                chamado.prioridade().name(),
                chamado.origem().name(),
                chamado.solicitante().id(),
                chamado.solicitante().nome(),
                chamado.solicitante().login(),
                chamado.solicitante().email(),
                chamado.responsavel() != null ? chamado.responsavel().id() : null,
                chamado.responsavel() != null ? chamado.responsavel().nome() : null,
                chamado.departamento().id(),
                chamado.departamento().nome(),
                chamado.categoria().id(),
                chamado.categoria().nome(),
                chamado.servico().id(),
                chamado.servico().nome(),
                chamado.dataCriacao(),
                chamado.dataAtualizacao(),
                interacoes,
                historicos,
                anexos
        );
    }

    private ChamadoAdminFilaDto paraFila(final Chamado chamado) {
        return new ChamadoAdminFilaDto(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.situacao().name(),
                chamado.prioridade().name(),
                chamado.origem().name(),
                chamado.solicitante().nome(),
                chamado.departamento().nome(),
                chamado.categoria().nome(),
                chamado.servico().nome(),
                chamado.responsavel() != null ? chamado.responsavel().id() : null,
                chamado.responsavel() != null ? chamado.responsavel().nome() : null,
                chamado.dataCriacao(),
                chamado.dataAtualizacao()
        );
    }

    private ChamadoAdminResumoDashboardDto paraResumoDashboard(final Chamado chamado) {
        return new ChamadoAdminResumoDashboardDto(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.situacao().name(),
                chamado.prioridade().name(),
                chamado.departamento().nome(),
                chamado.responsavel() != null ? chamado.responsavel().nome() : "Sem responsavel",
                chamado.dataCriacao()
        );
    }

    private InteracaoChamadoAdminDto paraInteracao(final InteracaoChamado interacao) {
        return new InteracaoChamadoAdminDto(
                interacao.id(),
                interacao.tipoInteracao().name(),
                interacao.mensagem(),
                interacao.visivelSolicitante(),
                interacao.autor().id(),
                interacao.autor().nome(),
                interacao.dataCriacao()
        );
    }

    private HistoricoChamadoAdminDto paraHistorico(final HistoricoChamado historico) {
        return new HistoricoChamadoAdminDto(
                historico.id(),
                historico.descricao(),
                historico.situacaoAnterior() != null ? historico.situacaoAnterior().name() : null,
                historico.situacaoNova().name(),
                historico.visivelSolicitante(),
                historico.dataCriacao()
        );
    }

    private AnexoChamadoAdminDto paraAnexo(final AnexoChamado anexo) {
        return new AnexoChamadoAdminDto(
                anexo.id(),
                anexo.nomeArquivo(),
                anexo.tipoConteudo(),
                anexo.tamanhoBytes(),
                anexo.autor().id(),
                anexo.autor().nome(),
                anexo.dataCriacao()
        );
    }

    private void registrarInteracao(
            final UUID chamadoId,
            final TipoInteracao tipoInteracao,
            final String mensagem,
            final boolean visivelSolicitante,
            final Usuario autor
    ) {
        interacaoChamadoRepositorio.salvar(new InteracaoChamado(
                UUID.randomUUID(),
                chamadoId,
                tipoInteracao,
                mensagem,
                visivelSolicitante,
                autor,
                LocalDateTime.now(),
                null
        ));
    }

    private void registrarHistorico(
            final UUID chamadoId,
            final String descricao,
            final SituacaoChamado situacaoAnterior,
            final SituacaoChamado situacaoNova,
            final boolean visivelSolicitante
    ) {
        historicoChamadoRepositorio.salvar(new HistoricoChamado(
                UUID.randomUUID(),
                chamadoId,
                descricao,
                situacaoAnterior,
                situacaoNova,
                visivelSolicitante,
                LocalDateTime.now(),
                null
        ));
    }

    private Chamado atualizarChamado(
            final Chamado chamado,
            final SituacaoChamado situacao,
            final Usuario responsavel,
            final Departamento departamento,
            final Categoria categoria,
            final Servico servico
    ) {
        return new Chamado(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.descricao(),
                situacao,
                chamado.prioridade(),
                chamado.origem(),
                chamado.solicitante(),
                responsavel,
                departamento,
                categoria,
                servico,
                chamado.dataCriacao(),
                LocalDateTime.now()
        );
    }

    private Chamado obterChamado(final UUID chamadoId) {
        ValidadorDominio.obrigatorio(chamadoId, "chamado e obrigatorio");
        return chamadoRepositorio.buscarPorId(chamadoId)
                .orElseThrow(() -> new ErroDeDominio("Chamado nao encontrado."));
    }

    private Usuario obterUsuarioInternoAtivo(final UUID usuarioId, final String contexto) {
        ValidadorDominio.obrigatorio(usuarioId, contexto + " e obrigatorio");
        final var usuario = usuarioRepositorio.buscarPorId(usuarioId)
                .orElseThrow(() -> new ErroDeDominio("Usuario nao encontrado para " + contexto + "."));
        if (!usuario.ativo()) {
            throw new ErroDeDominio("Usuario inativo para " + contexto + ".");
        }
        if (!usuarioInterno(usuario)) {
            throw new ErroDeDominio("Usuario informado nao possui perfil interno para " + contexto + ".");
        }
        return usuario;
    }

    private boolean usuarioInterno(final Usuario usuario) {
        final var perfil = usuario.perfilAcesso().nome().trim().toLowerCase(Locale.ROOT);
        return !perfil.equals("solicitante");
    }

    private Departamento obterDepartamentoAtivo(final UUID departamentoId) {
        ValidadorDominio.obrigatorio(departamentoId, "departamento do encaminhamento e obrigatorio");
        final var departamento = departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio("Departamento informado para encaminhamento nao encontrado."));
        if (!departamento.ativo()) {
            throw new ErroDeDominio("Departamento informado para encaminhamento esta inativo.");
        }
        return departamento;
    }

    private Categoria obterCategoriaAtiva(final UUID categoriaId) {
        ValidadorDominio.obrigatorio(categoriaId, "categoria do encaminhamento e obrigatoria");
        final var categoria = categoriaRepositorio.buscarPorId(categoriaId)
                .orElseThrow(() -> new ErroDeDominio("Categoria informada para encaminhamento nao encontrada."));
        if (!categoria.ativo()) {
            throw new ErroDeDominio("Categoria informada para encaminhamento esta inativa.");
        }
        return categoria;
    }

    private Servico obterServicoAtivo(final UUID servicoId) {
        ValidadorDominio.obrigatorio(servicoId, "servico do encaminhamento e obrigatorio");
        final var servico = servicoRepositorio.buscarPorId(servicoId)
                .orElseThrow(() -> new ErroDeDominio("Servico informado para encaminhamento nao encontrado."));
        if (!servico.ativo()) {
            throw new ErroDeDominio("Servico informado para encaminhamento esta inativo.");
        }
        return servico;
    }
}
