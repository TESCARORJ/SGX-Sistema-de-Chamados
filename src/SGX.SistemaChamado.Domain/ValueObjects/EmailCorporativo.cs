using System.Net.Mail;

namespace SGX.SistemaChamado.Domain.ValueObjects;

public sealed class EmailCorporativo
{
    public string Valor { get; }

    private EmailCorporativo(string valor)
    {
        Valor = valor;
    }

    public static EmailCorporativo Criar(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("O e-mail e obrigatorio.", nameof(email));
        }

        try
        {
            var normalizado = new MailAddress(email.Trim()).Address.ToLowerInvariant();
            return new EmailCorporativo(normalizado);
        }
        catch (FormatException ex)
        {
            throw new ArgumentException("O e-mail informado e invalido.", nameof(email), ex);
        }
    }
}
