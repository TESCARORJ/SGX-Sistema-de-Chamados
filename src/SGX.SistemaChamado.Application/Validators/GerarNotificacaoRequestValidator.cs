using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class GerarNotificacaoRequestValidator : AbstractValidator<GerarNotificacaoRequest>
{
    private const int MaximoDestinatarioEndereco = 320;
    private const int MaximoAssunto = 300;
    private const int MaximoConteudo = 10000;

    public GerarNotificacaoRequestValidator()
    {
        RuleFor(x => x.Evento)
            .NotNull()
            .WithMessage("O evento candidato da notificacao e obrigatorio.");

        When(x => x.Evento is not null, () =>
        {
            RuleFor(x => x.Evento.TipoEvento)
                .IsInEnum()
                .WithMessage("O tipo de evento da notificacao e invalido.");

            RuleFor(x => x.Evento.ChaveIdempotencia)
                .NotEmpty()
                .WithMessage("A chave de idempotencia da notificacao e obrigatoria.");

            RuleFor(x => x.Evento.OcorridoEm)
                .Must(data => data != default)
                .WithMessage("A data de ocorrencia do evento e obrigatoria.");

            RuleFor(x => x.Evento.ChaveCorrelacao)
                .NotEmpty()
                .WithMessage("A chave de correlacao do evento e obrigatoria.");
        });

        RuleFor(x => x.Canal)
            .IsInEnum()
            .WithMessage("O canal da notificacao e invalido.");

        RuleFor(x => x.DestinatarioUsuarioId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("O destinatario usuario informado e invalido.");

        RuleFor(x => x.DestinatarioEndereco)
            .MaximumLength(MaximoDestinatarioEndereco)
            .WithMessage($"O valor informado deve possuir no maximo {MaximoDestinatarioEndereco} caracteres.");

        RuleFor(x => x.Assunto)
            .MaximumLength(MaximoAssunto)
            .WithMessage($"O valor informado deve possuir no maximo {MaximoAssunto} caracteres.");

        RuleFor(x => x.Conteudo)
            .NotEmpty()
            .WithMessage("O conteudo da notificacao e obrigatorio.")
            .MaximumLength(MaximoConteudo)
            .WithMessage($"O valor informado deve possuir no maximo {MaximoConteudo} caracteres.");

        RuleFor(x => x)
            .Must(x => x.DestinatarioUsuarioId.HasValue || !string.IsNullOrWhiteSpace(x.DestinatarioEndereco))
            .WithName(nameof(GerarNotificacaoRequest.DestinatarioEndereco))
            .WithMessage("A notificacao deve possuir destinatario por usuario ou endereco.");

        RuleFor(x => x.AgendadaEm)
            .Must(data => !data.HasValue || data.Value != default)
            .WithMessage("A data de agendamento informada e invalida.");
    }
}
