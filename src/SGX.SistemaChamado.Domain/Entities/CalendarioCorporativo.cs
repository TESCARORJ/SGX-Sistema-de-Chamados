using SGX.SistemaChamado.Domain.Abstractions;

namespace SGX.SistemaChamado.Domain.Entities;

public sealed class CalendarioCorporativo : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public bool Padrao { get; private set; }
    public string TimeZone { get; private set; } = "America/Sao_Paulo";

    public ICollection<HorarioAtendimentoCalendario> HorariosAtendimento { get; private set; } = [];
    public ICollection<ExcecaoCalendarioCorporativo> Excecoes { get; private set; } = [];

    private CalendarioCorporativo()
    {
    }

    public CalendarioCorporativo(string nome, string? descricao, bool padrao, string timeZone, string criadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirTimeZone(timeZone);
        Padrao = padrao;
        DefinirCriacao(criadoPor);
    }

    public void Atualizar(string nome, string? descricao, string timeZone, string atualizadoPor)
    {
        DefinirNome(nome);
        DefinirDescricao(descricao);
        DefinirTimeZone(timeZone);
        AtualizarAuditoria(atualizadoPor);
    }

    public void DefinirComoPadrao(string atualizadoPor)
    {
        Padrao = true;
        Ativar(atualizadoPor);
    }

    public void RemoverPadrao(string atualizadoPor)
    {
        Padrao = false;
        AtualizarAuditoria(atualizadoPor);
    }

    private void DefinirNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new ArgumentException("O nome do calendario corporativo e obrigatorio.", nameof(nome));
        }

        Nome = nome.Trim();
    }

    private void DefinirDescricao(string? descricao)
    {
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
    }

    private void DefinirTimeZone(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
        {
            throw new ArgumentException("O fuso horario do calendario corporativo e obrigatorio.", nameof(timeZone));
        }

        TimeZone = timeZone.Trim();
    }
}
