package br.geti.sistemachamado.infraestrutura.persistencia.entidade.chamado;

import br.geti.sistemachamado.infraestrutura.persistencia.entidade.EntidadeBaseJpa;
import br.geti.sistemachamado.infraestrutura.persistencia.entidade.UsuarioEntidadeJpa;
import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import jakarta.persistence.Table;

@Entity
@Table(name = "anexos_chamado")
public class AnexoChamadoEntidadeJpa extends EntidadeBaseJpa {

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "chamado_id", nullable = false)
    private ChamadoEntidadeJpa chamado;

    @Column(name = "nome_arquivo", nullable = false, length = 255)
    private String nomeArquivo;

    @Column(name = "nome_armazenado", nullable = false, length = 255)
    private String nomeArmazenado;

    @Column(name = "caminho_armazenamento", nullable = false, length = 500)
    private String caminhoArmazenamento;

    @Column(name = "tipo_conteudo", nullable = false, length = 150)
    private String tipoConteudo;

    @Column(name = "tamanho_bytes", nullable = false)
    private long tamanhoBytes;

    @ManyToOne(fetch = FetchType.LAZY, optional = false)
    @JoinColumn(name = "autor_usuario_id", nullable = false)
    private UsuarioEntidadeJpa autor;

    public ChamadoEntidadeJpa getChamado() {
        return chamado;
    }

    public void setChamado(final ChamadoEntidadeJpa chamado) {
        this.chamado = chamado;
    }

    public String getNomeArquivo() {
        return nomeArquivo;
    }

    public void setNomeArquivo(final String nomeArquivo) {
        this.nomeArquivo = nomeArquivo;
    }

    public String getNomeArmazenado() {
        return nomeArmazenado;
    }

    public void setNomeArmazenado(final String nomeArmazenado) {
        this.nomeArmazenado = nomeArmazenado;
    }

    public String getCaminhoArmazenamento() {
        return caminhoArmazenamento;
    }

    public void setCaminhoArmazenamento(final String caminhoArmazenamento) {
        this.caminhoArmazenamento = caminhoArmazenamento;
    }

    public String getTipoConteudo() {
        return tipoConteudo;
    }

    public void setTipoConteudo(final String tipoConteudo) {
        this.tipoConteudo = tipoConteudo;
    }

    public long getTamanhoBytes() {
        return tamanhoBytes;
    }

    public void setTamanhoBytes(final long tamanhoBytes) {
        this.tamanhoBytes = tamanhoBytes;
    }

    public UsuarioEntidadeJpa getAutor() {
        return autor;
    }

    public void setAutor(final UsuarioEntidadeJpa autor) {
        this.autor = autor;
    }
}
