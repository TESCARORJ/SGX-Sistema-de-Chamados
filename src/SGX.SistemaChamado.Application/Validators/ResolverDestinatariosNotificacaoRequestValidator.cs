using FluentValidation;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.Validators;

public sealed class ResolverDestinatariosNotificacaoRequestValidator : AbstractValidator<ResolverDestinatariosNotificacaoRequest>
{
    public ResolverDestinatariosNotificacaoRequestValidator()
    {
        RuleFor(x => x.Evento)
            .NotNull()
            .WithMessage("O evento candidato e obrigatorio.");

        When(x => x.Evento is not null, () =>
        {
            RuleFor(x => x.Evento.TipoEvento)
                .IsInEnum()
                .WithMessage("O tipo de evento informado e invalido.");

            RuleFor(x => x.Evento.OcorridoEm)
                .Must(data => data != default)
                .WithMessage("A data de ocorrencia do evento e obrigatoria.");

            RuleFor(x => x.Evento.ChaveCorrelacao)
                .NotEmpty()
                .WithMessage("A chave de correlacao do evento e obrigatoria.");

            RuleFor(x => x.Evento.ChaveIdempotencia)
                .NotEmpty()
                .WithMessage("A chave de idempotencia do evento e obrigatoria.");

            RuleFor(x => x.Evento.UsuarioOriginadorId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("O usuario originador informado e invalido.");

            RuleFor(x => x.Evento.ChamadoId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("O chamado informado no evento e invalido.");
        });

        RuleFor(x => x.Participacoes)
            .NotNull()
            .Must(participacoes => participacoes is { Count: > 0 })
            .WithMessage("Ao menos uma participacao deve ser informada.");

        RuleForEach(x => x.Participacoes)
            .IsInEnum()
            .WithMessage("Uma participacao informada e invalida.");

        RuleFor(x => x.Participacoes)
            .Must(participacoes => participacoes is null || participacoes.Distinct().Count() == participacoes.Count)
            .WithMessage("Nao e permitido informar participacoes duplicadas.");

        RuleFor(x => x.AprovacaoChamadoId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("A aprovacao legada informada e invalida.");

        RuleFor(x => x.InstanciaAprovacaoChamadoId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("A instancia de aprovacao informada e invalida.");

        RuleFor(x => x.GrupoTecnicoId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("O grupo tecnico informado e invalido.");

        RuleFor(x => x.PerfilAcessoId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage("O perfil de acesso informado e invalido.");

        When(DependeDeChamado, () =>
        {
            RuleFor(x => x.Evento.ChamadoId)
                .NotNull()
                .WithMessage("O chamado do evento e obrigatorio para as participacoes solicitadas.");
        });

        When(x => x.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.AprovadorLegado), () =>
        {
            RuleFor(x => x.AprovacaoChamadoId)
                .NotNull()
                .WithMessage("A aprovacao legada e obrigatoria para resolver aprovador legado.");
        });

        When(x => x.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia), () =>
        {
            RuleFor(x => x.InstanciaAprovacaoChamadoId)
                .NotNull()
                .WithMessage("A instancia de aprovacao e obrigatoria para resolver aprovador da instancia.");
        });

        When(x => x.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico), () =>
        {
            RuleFor(x => x)
                .Must(request => request.GrupoTecnicoId.HasValue || request.Evento.ChamadoId.HasValue)
                .WithMessage("O grupo tecnico ou o chamado do evento deve ser informado para resolver membros de grupo tecnico.");
        });

        When(x => x.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.PerfilAcesso), () =>
        {
            RuleFor(x => x.PerfilAcessoId)
                .NotNull()
                .WithMessage("O perfil de acesso e obrigatorio para resolver destinatarios por perfil.");
        });
    }

    private static bool DependeDeChamado(ResolverDestinatariosNotificacaoRequest request)
        => request.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.Solicitante)
           || request.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual)
           || (request.Participacoes.Contains(TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico) && !request.GrupoTecnicoId.HasValue);
}
