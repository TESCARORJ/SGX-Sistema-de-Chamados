package br.geti.sistemachamado.dominio.administracao;

public enum PerfilUsuario {
    ADMINISTRADOR("Administrador"),
    ATENDENTE("Atendente"),
    SOLICITANTE("Solicitante"),
    SUPERVISOR("Supervisor");

    private final String nomePerfilAcesso;

    PerfilUsuario(final String nomePerfilAcesso) {
        this.nomePerfilAcesso = nomePerfilAcesso;
    }

    public String nomePerfilAcesso() {
        return nomePerfilAcesso;
    }
}
