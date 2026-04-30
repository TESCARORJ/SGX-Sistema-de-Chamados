package br.geti.sistemachamado.aplicacao.chamado.email;

import br.geti.sistemachamado.aplicacao.acesso.ComandoSincronizacaoUsuarioAutenticado;
import br.geti.sistemachamado.aplicacao.acesso.SincronizarUsuarioAutenticado;
import br.geti.sistemachamado.aplicacao.chamado.portal.ArmazenadorAnexoChamado;
import br.geti.sistemachamado.dominio.administracao.CaixaDeEmail;
import br.geti.sistemachamado.dominio.administracao.Servico;
import br.geti.sistemachamado.dominio.administracao.Usuario;
import br.geti.sistemachamado.dominio.administracao.repositorio.CaixaDeEmailRepositorio;
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
import br.geti.sistemachamado.dominio.chamado.servico.GeradorNumeroChamado;
import br.geti.sistemachamado.dominio.compartilhado.ErroDeDominio;
import br.geti.sistemachamado.dominio.compartilhado.ValidadorDominio;
import java.time.LocalDateTime;
import java.util.List;
import java.util.Locale;
import java.util.UUID;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
@Transactional(readOnly = true)
public class GerenciarChamadoPorEmail {

    private static final int TAMANHO_MAXIMO_ANEXO_BYTES = 10 * 1024 * 1024;
    private static final String TITULO_SEM_ASSUNTO = "Chamado aberto por e-mail sem assunto";
    private static final String DESCRICAO_SEM_CORPO = "Mensagem recebida sem corpo textual.";
    private static final String DESCRICAO_HISTORICO_RESPOSTA_EMAIL = "Interacao registrada via resposta de e-mail.";

    private final ChamadoRepositorio chamadoRepositorio;
    private final InteracaoChamadoRepositorio interacaoChamadoRepositorio;
    private final HistoricoChamadoRepositorio historicoChamadoRepositorio;
    private final AnexoChamadoRepositorio anexoChamadoRepositorio;
    private final CaixaDeEmailRepositorio caixaDeEmailRepositorio;
    private final ServicoRepositorio servicoRepositorio;
    private final UsuarioRepositorio usuarioRepositorio;
    private final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado;
    private final GeradorNumeroChamado geradorNumeroChamado;
    private final ArmazenadorAnexoChamado armazenadorAnexoChamado;

    public GerenciarChamadoPorEmail(
            final ChamadoRepositorio chamadoRepositorio,
            final InteracaoChamadoRepositorio interacaoChamadoRepositorio,
            final HistoricoChamadoRepositorio historicoChamadoRepositorio,
            final AnexoChamadoRepositorio anexoChamadoRepositorio,
            final CaixaDeEmailRepositorio caixaDeEmailRepositorio,
            final ServicoRepositorio servicoRepositorio,
            final UsuarioRepositorio usuarioRepositorio,
            final SincronizarUsuarioAutenticado sincronizarUsuarioAutenticado,
            final GeradorNumeroChamado geradorNumeroChamado,
            final ArmazenadorAnexoChamado armazenadorAnexoChamado
    ) {
        this.chamadoRepositorio = chamadoRepositorio;
        this.interacaoChamadoRepositorio = interacaoChamadoRepositorio;
        this.historicoChamadoRepositorio = historicoChamadoRepositorio;
        this.anexoChamadoRepositorio = anexoChamadoRepositorio;
        this.caixaDeEmailRepositorio = caixaDeEmailRepositorio;
        this.servicoRepositorio = servicoRepositorio;
        this.usuarioRepositorio = usuarioRepositorio;
        this.sincronizarUsuarioAutenticado = sincronizarUsuarioAutenticado;
        this.geradorNumeroChamado = geradorNumeroChamado;
        this.armazenadorAnexoChamado = armazenadorAnexoChamado;
    }

    @Transactional
    public ChamadoAbertoPorEmailDto abrirChamadoPorEmail(final AberturaChamadoEmailComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados da abertura de chamado por email sao obrigatorios");
        final var caixaDeEmail = obterCaixaAtiva(comando.caixaDeEmailId());
        final var servico = obterPrimeiroServicoAtivoDoDepartamento(caixaDeEmail.departamento().id());
        final var solicitante = resolverSolicitante(comando.remetenteNome(), comando.remetenteEmail());
        final var agora = LocalDateTime.now();

        final var chamadoSalvo = chamadoRepositorio.salvar(new Chamado(
                UUID.randomUUID(),
                geradorNumeroChamado.gerarNumero(),
                resolverTitulo(comando.assunto()),
                resolverDescricao(comando.corpoMensagem()),
                SituacaoChamado.ABERTO,
                comando.prioridade() != null ? comando.prioridade() : PrioridadeChamado.MEDIA,
                OrigemChamado.EMAIL,
                solicitante,
                null,
                caixaDeEmail.departamento(),
                servico.categoria(),
                servico,
                agora,
                null
        ));

        interacaoChamadoRepositorio.salvar(new InteracaoChamado(
                UUID.randomUUID(),
                chamadoSalvo.id(),
                TipoInteracao.ABERTURA,
                "Chamado aberto automaticamente a partir de e-mail recebido em " + caixaDeEmail.enderecoEmail() + ".",
                true,
                solicitante,
                agora,
                null
        ));

        historicoChamadoRepositorio.salvar(new HistoricoChamado(
                UUID.randomUUID(),
                chamadoSalvo.id(),
                "Chamado aberto automaticamente via integracao de e-mail.",
                null,
                chamadoSalvo.situacao(),
                true,
                agora,
                null
        ));

        salvarAnexos(comando.anexos(), chamadoSalvo, solicitante, agora);
        return new ChamadoAbertoPorEmailDto(chamadoSalvo.id(), chamadoSalvo.numero());
    }

    @Transactional
    public InteracaoChamadoPorEmailDto registrarRespostaEmChamado(final RespostaChamadoEmailComando comando) {
        ValidadorDominio.obrigatorio(comando, "dados da resposta de chamado por email sao obrigatorios");
        final var chamado = chamadoRepositorio.buscarPorId(
                        ValidadorDominio.obrigatorio(comando.chamadoId(), "chamado da resposta por email e obrigatorio")
                )
                .orElseThrow(() -> new ErroDeDominio("Chamado nao encontrado para correlacao da resposta por email."));
        final var autor = resolverSolicitante(comando.remetenteNome(), comando.remetenteEmail());
        final var agora = LocalDateTime.now();

        final var mensagemInteracao = montarMensagemInteracaoResposta(comando);
        final var interacao = interacaoChamadoRepositorio.salvar(new InteracaoChamado(
                UUID.randomUUID(),
                chamado.id(),
                TipoInteracao.MENSAGEM_SOLICITANTE,
                mensagemInteracao,
                true,
                autor,
                agora,
                null
        ));

        historicoChamadoRepositorio.salvar(new HistoricoChamado(
                UUID.randomUUID(),
                chamado.id(),
                DESCRICAO_HISTORICO_RESPOSTA_EMAIL,
                chamado.situacao(),
                chamado.situacao(),
                true,
                agora,
                null
        ));

        salvarAnexos(comando.anexos(), chamado, autor, agora);
        return new InteracaoChamadoPorEmailDto(chamado.id(), interacao.id());
    }

    private CaixaDeEmail obterCaixaAtiva(final UUID caixaDeEmailId) {
        ValidadorDominio.obrigatorio(caixaDeEmailId, "caixa de email de destino e obrigatoria");
        final var caixaDeEmail = caixaDeEmailRepositorio.buscarPorId(caixaDeEmailId)
                .orElseThrow(() -> new ErroDeDominio("Caixa de e-mail de destino nao encontrada."));
        if (!caixaDeEmail.ativa()) {
            throw new ErroDeDominio("Caixa de e-mail de destino esta inativa.");
        }
        return caixaDeEmail;
    }

    private Servico obterPrimeiroServicoAtivoDoDepartamento(final UUID departamentoId) {
        return servicoRepositorio.listarPorDepartamento(departamentoId).stream()
                .filter(Servico::ativo)
                .findFirst()
                .orElseThrow(() -> new ErroDeDominio(
                        "Nao existe servico ativo no departamento da caixa de e-mail para abertura automatica."
                ));
    }

    private Usuario resolverSolicitante(final String remetenteNome, final String remetenteEmail) {
        final var emailNormalizado = ValidadorDominio.textoObrigatorio(
                remetenteEmail,
                "email do remetente e obrigatorio"
        ).toLowerCase(Locale.ROOT);
        final var nomeNormalizado = normalizarNomeRemetente(remetenteNome, emailNormalizado);

        sincronizarUsuarioAutenticado.sincronizar(new ComandoSincronizacaoUsuarioAutenticado(
                nomeNormalizado,
                emailNormalizado,
                emailNormalizado,
                null
        ));

        return usuarioRepositorio.buscarPorEmail(emailNormalizado)
                .orElseThrow(() -> new ErroDeDominio("Solicitante por email nao encontrado apos sincronizacao."));
    }

    private void salvarAnexos(
            final List<AnexoChamadoEmailComando> anexos,
            final Chamado chamado,
            final Usuario solicitante,
            final LocalDateTime agora
    ) {
        if (anexos == null || anexos.isEmpty()) {
            return;
        }

        for (final var anexo : anexos) {
            if (anexo == null) {
                continue;
            }
            final var nomeArquivo = ValidadorDominio.textoObrigatorio(
                    anexo.nomeArquivo(),
                    "nome do anexo do email e obrigatorio"
            );
            final var conteudo = ValidadorDominio.obrigatorio(anexo.conteudo(), "conteudo do anexo do email e obrigatorio");
            if (conteudo.length == 0) {
                throw new ErroDeDominio("conteudo do anexo do email nao pode ser vazio");
            }
            if (conteudo.length > TAMANHO_MAXIMO_ANEXO_BYTES) {
                throw new ErroDeDominio("anexo do email excede o limite de 10MB");
            }

            final var anexoId = UUID.randomUUID();
            final var arquivoArmazenado = armazenadorAnexoChamado.armazenar(
                    chamado.id(),
                    anexoId,
                    nomeArquivo,
                    conteudo
            );

            final var tipoConteudo = (anexo.tipoConteudo() == null || anexo.tipoConteudo().isBlank())
                    ? "application/octet-stream"
                    : anexo.tipoConteudo().trim();

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
                    "Anexo recebido via e-mail: " + anexoSalvo.nomeArquivo(),
                    true,
                    solicitante,
                    agora,
                    null
            ));
        }
    }

    private String resolverTitulo(final String assunto) {
        if (assunto == null || assunto.trim().isEmpty()) {
            return TITULO_SEM_ASSUNTO;
        }
        return assunto.trim();
    }

    private String resolverDescricao(final String corpoMensagem) {
        if (corpoMensagem == null || corpoMensagem.trim().isEmpty()) {
            return DESCRICAO_SEM_CORPO;
        }
        return corpoMensagem.trim();
    }

    private String montarMensagemInteracaoResposta(final RespostaChamadoEmailComando comando) {
        final var corpo = resolverDescricao(comando.corpoMensagem());
        final var assunto = comando.assunto() == null || comando.assunto().isBlank()
                ? "Sem assunto"
                : comando.assunto().trim();

        final var cabecalho = new StringBuilder("Resposta recebida por e-mail. Assunto: ")
                .append(assunto)
                .append(".");

        if (comando.messageId() != null && !comando.messageId().isBlank()) {
            cabecalho.append(" Message-Id: ").append(comando.messageId().trim()).append(".");
        }
        if (comando.inReplyTo() != null && !comando.inReplyTo().isBlank()) {
            cabecalho.append(" In-Reply-To: ").append(comando.inReplyTo().trim()).append(".");
        }

        return cabecalho.append(System.lineSeparator()).append(System.lineSeparator()).append(corpo).toString();
    }

    private String normalizarNomeRemetente(final String remetenteNome, final String remetenteEmail) {
        if (remetenteNome != null && !remetenteNome.isBlank()) {
            return remetenteNome.trim();
        }

        final var separador = remetenteEmail.indexOf('@');
        if (separador > 0) {
            return remetenteEmail.substring(0, separador);
        }
        return remetenteEmail;
    }
}
