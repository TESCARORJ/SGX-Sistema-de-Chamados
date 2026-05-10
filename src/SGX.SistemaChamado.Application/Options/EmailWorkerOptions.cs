namespace SGX.SistemaChamado.Application.Options;

public sealed class EmailWorkerOptions
{
    public const string SectionName = "EmailWorker";

    public string ImapHost { get; init; } = string.Empty;
    public int ImapPorta { get; init; } = 993;
    public string Usuario { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
    public string Pasta { get; init; } = "INBOX";
    public bool SslHabilitado { get; init; } = true;
    public bool TlsHabilitado { get; init; } = false;
    public int IntervaloSegundos { get; init; } = 60;
    public int MaxMensagensPorCiclo { get; init; } = 20;
    public Guid? CategoriaPadraoId { get; init; }
    public Guid? PrioridadePadraoId { get; init; }
    public Guid? DepartamentoPadraoId { get; init; }
    public string[] DominiosPermitidos { get; init; } = [];
    public int TamanhoMaximoAnexoMb { get; init; } = 10;
    public string[] ExtensoesPermitidas { get; init; } = [];
    public bool MarcarComoLidaAoProcessar { get; init; } = true;
    public bool MoverProcessadas { get; init; }
    public string PastaProcessadas { get; init; } = "Processadas";
    public bool MoverComErro { get; init; }
    public string PastaErro { get; init; } = "Erro";

    public bool Configurado =>
        !string.IsNullOrWhiteSpace(ImapHost) &&
        ImapPorta > 0 &&
        !string.IsNullOrWhiteSpace(Usuario) &&
        !string.IsNullOrWhiteSpace(Senha) &&
        !string.IsNullOrWhiteSpace(Pasta);
}
