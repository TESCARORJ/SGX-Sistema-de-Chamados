package br.geti.sistemachamado.aplicacao.chamado.portal;

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
import br.geti.sistemachamado.dominio.chamado.SituacaoChamado;
import br.geti.sistemachamado.dominio.chamado.TipoInteracao;
import br.geti.sistemachamado.dominio.chamado.repositorio.AnexoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.ChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.HistoricoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.repositorio.InteracaoChamadoRepositorio;
import br.geti.sistemachamado.dominio.chamado.servico.GeradorNumeroChamado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.UUID;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional(readOnly = true)
public class GerenciarChamadoPortalSolicitante {

    private static final int TAMANHO_MAXIMO_ANEXO_BYTES = 10 * 1024 * 1024;

    private final ChamadoRepositorio chamadoRepositorio;
    private final InteracaoChamadoRepositorio interacaoChamadoRepositorio;
    private final HistoricoChamadoRepositorio historicoChamadoRepositorio;
    private final AnexoChamadoRepositorio anexoChamadoRepositorio;
    private final UsuarioRepositorio usuarioRepositorio;
    private final DepartamentoRepositorio departamentoRepositorio;
    private final CategoriaRepositorio categoriaRepositorio;
    private final ServicoRepositorio servicoRepositorio;
    private final GeradorNumeroChamado geradorNumeroChamado;
    private final ArmazenadorAnexoChamado armazenadorAnexoChamado;

    public GerenciarChamadoPortalSolicitante(
            final ChamadoRepositorio chamadoRepositorio,
            final InteracaoChamadoRepositorio interacaoChamadoRepositorio,
            final HistoricoChamadoRepositorio historicoChamadoRepositorio,
            final AnexoChamadoRepositorio anexoChamadoRepositorio,
            final UsuarioRepositorio usuarioRepositorio,
            final DepartamentoRepositorio departamentoRepositorio,
            final CategoriaRepositorio categoriaRepositorio,
            final ServicoRepositorio servicoRepositorio,
            final GeradorNumeroChamado geradorNumeroChamado,
            final ArmazenadorAnexoChamado armazenadorAnexoChamado
    ) {
        this.chamadoRepositorio = chamadoRepositorio;
        this.interacaoChamadoRepositorio = interacaoChamadoRepositorio;
        this.historicoChamadoRepositorio = historicoChamadoRepositorio;
        this.anexoChamadoRepositorio = anexoChamadoRepositorio;
        this.usuarioRepositorio = usuarioRepositorio;
        this.departamentoRepositorio = departamentoRepositorio;
        this.categoriaRepositorio = categoriaRepositorio;
        this.servicoRepositorio = servicoRepositorio;
        this.geradorNumeroChamado = geradorNumeroChamado;
        this.armazenadorAnexoChamado = armazenadorAnexoChamado;
    }

    public CatalogoAberturaChamadoPortalDto consultarCatalogoAbertura() {
        final var departamentos = departamentoRepositorio.listarAtivos().stream()
                .map(dep -> new OpcaoCatalogoPortalDto(dep.id(), dep.nome()))
                .toList();
        final var categorias = categoriaRepositorio.listarAtivas().stream()
                .map(cat -> new OpcaoCatalogoPortalDto(cat.id(), cat.nome()))
                .toList();
        final var servicos = servicoRepositorio.listarAtivos().stream()
                .map(serv -> new OpcaoCatalogoPortalDto(serv.id(), serv.nome()))
                .toList();

        return new CatalogoAberturaChamadoPortalDto(departamentos, categorias, servicos);
    }

    @Transactional
    public ChamadoPortalDetalheDto abrirChamado(final AberturaChamadoPortalComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados para abertura do chamado sao obrigatorios");

        final var solicitante = obterSolicitanteAtivo(comando.solicitanteId());
        final var departamento = obterDepartamentoAtivo(comando.departamentoId());
        final var categoria = obterCategoriaAtiva(comando.categoriaId());
        final var servico = obterServicoAtivo(comando.servicoId());

        final var agora = LocalDateTime.now();
        final var chamadoSalvo = chamadoRepositorio.salvar(new Chamado(
                UUID.randomUUID(),
                geradorNumeroChamado.gerarNumero(),
                ValidadorDominio.textoObrigatorio(comando.titulo(), "titulo do chamado e obrigatorio"),
                ValidadorDominio.textoObrigatorio(comando.descricao(), "descricao do chamado e obrigatoria"),
                SituacaoChamado.ABERTO,
                ValidadorDominio.obrigatorio(comando.prioridade(), "prioridade do chamado e obrigatoria"),
                OrigemChamado.PORTAL,
                solicitante,
                null,
                departamento,
                categoria,
                servico,
                agora,
                null
        ));

        interacaoChamadoRepositorio.salvar(new InteracaoChamado(
                UUID.randomUUID(),
                chamadoSalvo.id(),
                TipoInteracao.ABERTURA,
                "Chamado aberto pelo solicitante no portal.",
                true,
                solicitante,
                agora,
                null
        ));

        historicoChamadoRepositorio.salvar(new HistoricoChamado(
                UUID.randomUUID(),
                chamadoSalvo.id(),
                "Chamado aberto via portal.",
                null,
                chamadoSalvo.situacao(),
                true,
                agora,
                null
        ));

        return buscarDetalheDoSolicitante(solicitante.id(), chamadoSalvo.id());
    }

    public List<ChamadoPortalResumoDto> listarChamadosDoSolicitante(final UUID solicitanteId) {
        ValidadorDominio.obrigatorio(solicitanteId, "solicitante e obrigatorio");
        return chamadoRepositorio.listarPorSolicitante(solicitanteId).stream()
                .map(this::paraResumo)
                .toList();
    }

    public ChamadoPortalDetalheDto buscarDetalheDoSolicitante(final UUID solicitanteId, final UUID chamadoId) {
        ValidadorDominio.obrigatorio(solicitanteId, "solicitante e obrigatorio");
        ValidadorDominio.obrigatorio(chamadoId, "chamado e obrigatorio");

        final var chamado = chamadoRepositorio.buscarPorIdESolicitante(chamadoId, solicitanteId)
                .orElseThrow(() -> new ErroDeDominio("Chamado nao encontrado para o solicitante informado."));

        final var interacoes = interacaoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .filter(InteracaoChamado::visivelSolicitante)
                .map(this::paraInteracao)
                .toList();
        final var historicos = historicoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .filter(HistoricoChamado::visivelSolicitante)
                .map(this::paraHistorico)
                .toList();
        final var anexos = anexoChamadoRepositorio.listarPorChamado(chamado.id()).stream()
                .map(this::paraAnexo)
                .toList();

        return paraDetalhe(chamado, interacoes, historicos, anexos);
    }

    @Transactional
    public AnexoChamadoPortalDto anexarArquivo(
            final UUID solicitanteId,
            final UUID chamadoId,
            final AnexoChamadoPortalComando comando
    ) {
        ValidadorDominio.obrigatorio(comando, "anexo do chamado e obrigatorio");
        final var solicitante = obterSolicitanteAtivo(solicitanteId);
        final var chamado = chamadoRepositorio.buscarPorIdESolicitante(chamadoId, solicitanteId)
                .orElseThrow(() -> new ErroDeDominio("Chamado nao encontrado para o solicitante informado."));

        final var nomeArquivo = ValidadorDominio.textoObrigatorio(comando.nomeArquivo(), "nome do anexo e obrigatorio");
        final var conteudo = ValidadorDominio.obrigatorio(comando.conteudo(), "conteudo do anexo e obrigatorio");
        if (conteudo.length == 0) {
            throw new ErroDeDominio("conteudo do anexo nao pode ser vazio");
        }
        if (conteudo.length > TAMANHO_MAXIMO_ANEXO_BYTES) {
            throw new ErroDeDominio("anexo excede o limite de 10MB");
        }

        final var anexoId = UUID.randomUUID();
        final var arquivoArmazenado = armazenadorAnexoChamado.armazenar(chamado.id(), anexoId, nomeArquivo, conteudo);
        final var agora = LocalDateTime.now();
        final var tipoConteudo = comando.tipoConteudo() == null || comando.tipoConteudo().isBlank()
                ? "application/octet-stream"
                : comando.tipoConteudo().trim();

        final var anexoSalvo = anexoChamadoRepositorio.salvar(new AnexoChamado(
                anexoId,
                chamado.id(),
                nomeArquivo,
                arquivoArmazenado.nomeArmazenado(),
                arquivoArmazenado.caminhoArmazenamento(),
                tipoConteudo,
                conteudo.length,
                solicitante,
                agora,
                null
        ));

        interacaoChamadoRepositorio.salvar(new InteracaoChamado(
                UUID.randomUUID(),
                chamado.id(),
                TipoInteracao.ANEXO,
                "Anexo enviado pelo solicitante: " + anexoSalvo.nomeArquivo(),
                true,
                solicitante,
                agora,
                null
        ));

        return paraAnexo(anexoSalvo);
    }

    private Usuario obterSolicitanteAtivo(final UUID solicitanteId) {
        ValidadorDominio.obrigatorio(solicitanteId, "solicitante e obrigatorio");
        final var usuario = usuarioRepositorio.buscarPorId(solicitanteId)
                .orElseThrow(() -> new ErroDeDominio("Solicitante autenticado nao encontrado."));
        if (!usuario.ativo()) {
            throw new ErroDeDominio("Solicitante autenticado esta inativo.");
        }
        return usuario;
    }

    private Departamento obterDepartamentoAtivo(final UUID departamentoId) {
        ValidadorDominio.obrigatorio(departamentoId, "departamento e obrigatorio");
        final var departamento = departamentoRepositorio.buscarPorId(departamentoId)
                .orElseThrow(() -> new ErroDeDominio("Departamento informado nao encontrado."));
        if (!departamento.ativo()) {
            throw new ErroDeDominio("Departamento informado esta inativo.");
        }
        return departamento;
    }

    private Categoria obterCategoriaAtiva(final UUID categoriaId) {
        ValidadorDominio.obrigatorio(categoriaId, "categoria e obrigatoria");
        final var categoria = categoriaRepositorio.buscarPorId(categoriaId)
                .orElseThrow(() -> new ErroDeDominio("Categoria informada nao encontrada."));
        if (!categoria.ativo()) {
            throw new ErroDeDominio("Categoria informada esta inativa.");
        }
        return categoria;
    }

    private Servico obterServicoAtivo(final UUID servicoId) {
        ValidadorDominio.obrigatorio(servicoId, "servico e obrigatorio");
        final var servico = servicoRepositorio.buscarPorId(servicoId)
                .orElseThrow(() -> new ErroDeDominio("Servico informado nao encontrado."));
        if (!servico.ativo()) {
            throw new ErroDeDominio("Servico informado esta inativo.");
        }
        return servico;
    }

    private ChamadoPortalResumoDto paraResumo(final Chamado chamado) {
        return new ChamadoPortalResumoDto(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.situacao().name(),
                chamado.prioridade().name(),
                chamado.categoria().nome(),
                chamado.servico().nome(),
                chamado.dataCriacao(),
                chamado.dataAtualizacao()
        );
    }

    private InteracaoChamadoPortalDto paraInteracao(final InteracaoChamado interacao) {
        return new InteracaoChamadoPortalDto(
                interacao.id(),
                interacao.tipoInteracao().name(),
                interacao.mensagem(),
                interacao.autor().nome(),
                interacao.dataCriacao()
        );
    }

    private HistoricoChamadoPortalDto paraHistorico(final HistoricoChamado historico) {
        return new HistoricoChamadoPortalDto(
                historico.id(),
                historico.descricao(),
                historico.situacaoAnterior() != null ? historico.situacaoAnterior().name() : null,
                historico.situacaoNova().name(),
                historico.dataCriacao()
        );
    }

    private AnexoChamadoPortalDto paraAnexo(final AnexoChamado anexo) {
        return new AnexoChamadoPortalDto(
                anexo.id(),
                anexo.nomeArquivo(),
                anexo.tipoConteudo(),
                anexo.tamanhoBytes(),
                anexo.dataCriacao()
        );
    }

    private ChamadoPortalDetalheDto paraDetalhe(
            final Chamado chamado,
            final List<InteracaoChamadoPortalDto> interacoes,
            final List<HistoricoChamadoPortalDto> historicos,
            final List<AnexoChamadoPortalDto> anexos
    ) {
        return new ChamadoPortalDetalheDto(
                chamado.id(),
                chamado.numero(),
                chamado.titulo(),
                chamado.descricao(),
                chamado.situacao().name(),
                chamado.prioridade().name(),
                chamado.origem().name(),
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
}
