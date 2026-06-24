using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Notificacoes;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Notificacoes;

public sealed class ResolverDestinatariosNotificacaoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<AprovacaoChamado> aprovacaoChamadoRepository,
    IRepository<InstanciaAprovacaoChamado> instanciaAprovacaoChamadoRepository,
    IRepository<MembroGrupoTecnico> membroGrupoTecnicoRepository,
    IRepository<PerfilAcesso> perfilAcessoRepository,
    IRepository<UsuarioPerfilAcesso> usuarioPerfilAcessoRepository) : IResolverDestinatariosNotificacaoUseCase
{
    public async Task<ResolverDestinatariosNotificacaoResponse> ExecutarAsync(
        ResolverDestinatariosNotificacaoRequest request,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var validator = new ResolverDestinatariosNotificacaoRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var avisos = new List<string>();
        var chamadosPorId = new Dictionary<Guid, Chamado>();
        var usuariosPorId = new Dictionary<Guid, Usuario>();
        var destinatarios = new Dictionary<Guid, DestinatarioAcumulado>();

        var chamado = await ObterChamadoSeNecessarioAsync(request, chamadosPorId, cancellationToken);

        foreach (var participacao in request.Participacoes)
        {
            cancellationToken.ThrowIfCancellationRequested();

            switch (participacao)
            {
                case TipoParticipacaoDestinatarioNotificacao.Solicitante:
                    await AdicionarUsuarioDoChamadoAsync(
                        chamado,
                        participacao,
                        "Solicitante do chamado nao encontrado ou nao elegivel.",
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        x => x.SolicitanteId,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.ResponsavelAtual:
                    await AdicionarUsuarioDoChamadoAsync(
                        chamado,
                        participacao,
                        "Responsavel atual do chamado nao encontrado ou nao elegivel.",
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        x => x.ResponsavelId,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.UsuarioOriginador:
                    await AdicionarUsuarioAsync(
                        request.Evento.UsuarioOriginadorId,
                        participacao,
                        "Usuario originador nao informado, nao encontrado ou nao elegivel.",
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.AprovadorLegado:
                    await ResolverAprovadorLegadoAsync(
                        request,
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia:
                    await ResolverAprovadorInstanciaAsync(
                        request,
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico:
                    await ResolverGrupoTecnicoAsync(
                        request,
                        chamado,
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        cancellationToken);
                    break;

                case TipoParticipacaoDestinatarioNotificacao.PerfilAcesso:
                    await ResolverPerfilAcessoAsync(
                        request,
                        avisos,
                        usuariosPorId,
                        destinatarios,
                        cancellationToken);
                    break;

                default:
                    avisos.Add($"Participacao {participacao} ainda nao suportada.");
                    break;
            }
        }

        if (request.ExcluirUsuarioOriginador && request.Evento.UsuarioOriginadorId.HasValue)
        {
            destinatarios.Remove(request.Evento.UsuarioOriginadorId.Value);
        }

        var resolvidos = destinatarios.Values
            .OrderBy(x => x.Nome, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.UsuarioId)
            .Select(x => new DestinatarioNotificacaoResolvido(
                x.UsuarioId,
                x.Nome,
                x.Email,
                x.Origens.OrderBy(y => (int)y).ToArray()))
            .ToArray();

        return new ResolverDestinatariosNotificacaoResponse(
            resolvidos,
            avisos.Distinct(StringComparer.Ordinal).ToArray());
    }

    private async Task<Chamado?> ObterChamadoSeNecessarioAsync(
        ResolverDestinatariosNotificacaoRequest request,
        Dictionary<Guid, Chamado> chamadosPorId,
        CancellationToken cancellationToken)
    {
        if (!request.Evento.ChamadoId.HasValue)
        {
            return null;
        }

        var chamadoId = request.Evento.ChamadoId.Value;
        if (chamadosPorId.TryGetValue(chamadoId, out var chamadoExistente))
        {
            return chamadoExistente;
        }

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId, cancellationToken);

        if (chamado is not null)
        {
            chamadosPorId[chamadoId] = chamado;
        }

        return chamado;
    }

    private async Task AdicionarUsuarioDoChamadoAsync(
        Chamado? chamado,
        TipoParticipacaoDestinatarioNotificacao participacao,
        string avisoQuandoAusente,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        Func<Chamado, Guid?> seletorUsuarioId,
        CancellationToken cancellationToken)
    {
        if (chamado is null)
        {
            avisos.Add("Chamado do evento nao encontrado para resolver destinatarios por participacao.");
            return;
        }

        await AdicionarUsuarioAsync(
            seletorUsuarioId(chamado),
            participacao,
            avisoQuandoAusente,
            avisos,
            usuariosPorId,
            destinatarios,
            cancellationToken);
    }

    private async Task ResolverAprovadorLegadoAsync(
        ResolverDestinatariosNotificacaoRequest request,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        CancellationToken cancellationToken)
    {
        var aprovacao = await aprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == request.AprovacaoChamadoId!.Value && x.Ativo)
            .Select(x => new { x.AprovadorId })
            .FirstOrDefaultAsync(cancellationToken);

        if (aprovacao?.AprovadorId is null)
        {
            avisos.Add("Aprovador legado nao resolvido para a aprovacao informada.");
            return;
        }

        await AdicionarUsuarioAsync(
            aprovacao.AprovadorId,
            TipoParticipacaoDestinatarioNotificacao.AprovadorLegado,
            "Aprovador legado nao encontrado ou nao elegivel.",
            avisos,
            usuariosPorId,
            destinatarios,
            cancellationToken);
    }

    private async Task ResolverAprovadorInstanciaAsync(
        ResolverDestinatariosNotificacaoRequest request,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        CancellationToken cancellationToken)
    {
        var instancia = await instanciaAprovacaoChamadoRepository.Query()
            .AsNoTracking()
            .Where(x => x.Id == request.InstanciaAprovacaoChamadoId!.Value && x.Ativo)
            .Select(x => new { x.AprovadorResolvidoUsuarioId })
            .FirstOrDefaultAsync(cancellationToken);

        if (instancia?.AprovadorResolvidoUsuarioId is null)
        {
            avisos.Add("Aprovador da instancia nao resolvido para a aprovacao informada.");
            return;
        }

        await AdicionarUsuarioAsync(
            instancia.AprovadorResolvidoUsuarioId,
            TipoParticipacaoDestinatarioNotificacao.AprovadorInstancia,
            "Aprovador da instancia nao encontrado ou nao elegivel.",
            avisos,
            usuariosPorId,
            destinatarios,
            cancellationToken);
    }

    private async Task ResolverGrupoTecnicoAsync(
        ResolverDestinatariosNotificacaoRequest request,
        Chamado? chamado,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        CancellationToken cancellationToken)
    {
        var grupoTecnicoId = request.GrupoTecnicoId ?? chamado?.GrupoTecnicoId;
        if (!grupoTecnicoId.HasValue)
        {
            avisos.Add("Grupo tecnico nao informado nem encontrado no chamado para resolucao de destinatarios.");
            return;
        }

        var grupoAtivo = await membroGrupoTecnicoRepository.Query()
            .AsNoTracking()
            .Where(x => x.GrupoTecnicoId == grupoTecnicoId.Value && x.Ativo && x.GrupoTecnico.Ativo)
            .Select(x => x.UsuarioId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (grupoAtivo.Length == 0)
        {
            avisos.Add("Grupo tecnico sem membros ativos elegiveis para notificacao.");
            return;
        }

        var usuarios = await CarregarUsuariosElegiveisAsync(grupoAtivo, usuariosPorId, cancellationToken);
        if (usuarios.Count == 0)
        {
            avisos.Add("Grupo tecnico sem usuarios elegiveis para notificacao.");
            return;
        }

        foreach (var usuario in usuarios)
        {
            AdicionarDestinatario(destinatarios, usuario, TipoParticipacaoDestinatarioNotificacao.MembroGrupoTecnico);
        }
    }

    private async Task ResolverPerfilAcessoAsync(
        ResolverDestinatariosNotificacaoRequest request,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        CancellationToken cancellationToken)
    {
        var perfilAcessoId = request.PerfilAcessoId!.Value;
        var perfilAtivo = await perfilAcessoRepository.Query()
            .AsNoTracking()
            .AnyAsync(x => x.Id == perfilAcessoId && x.Ativo, cancellationToken);

        if (!perfilAtivo)
        {
            avisos.Add("Perfil de acesso nao encontrado ou inativo para resolucao de destinatarios.");
            return;
        }

        var usuarioIds = await usuarioPerfilAcessoRepository.Query()
            .AsNoTracking()
            .Where(x => x.PerfilAcessoId == perfilAcessoId && x.PerfilAcesso.Ativo)
            .Select(x => x.UsuarioId)
            .Distinct()
            .ToArrayAsync(cancellationToken);

        if (usuarioIds.Length == 0)
        {
            avisos.Add("Perfil de acesso sem usuarios vinculados para notificacao.");
            return;
        }

        var usuarios = await CarregarUsuariosElegiveisAsync(usuarioIds, usuariosPorId, cancellationToken);
        if (usuarios.Count == 0)
        {
            avisos.Add("Perfil de acesso sem usuarios elegiveis para notificacao.");
            return;
        }

        foreach (var usuario in usuarios)
        {
            AdicionarDestinatario(destinatarios, usuario, TipoParticipacaoDestinatarioNotificacao.PerfilAcesso);
        }
    }

    private async Task AdicionarUsuarioAsync(
        Guid? usuarioId,
        TipoParticipacaoDestinatarioNotificacao participacao,
        string avisoQuandoAusente,
        List<string> avisos,
        Dictionary<Guid, Usuario> usuariosPorId,
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        CancellationToken cancellationToken)
    {
        if (!usuarioId.HasValue)
        {
            avisos.Add(avisoQuandoAusente);
            return;
        }

        var usuario = await ObterUsuarioElegivelAsync(usuarioId.Value, usuariosPorId, cancellationToken);
        if (usuario is null)
        {
            avisos.Add(avisoQuandoAusente);
            return;
        }

        AdicionarDestinatario(destinatarios, usuario, participacao);
    }

    private async Task<Usuario?> ObterUsuarioElegivelAsync(
        Guid usuarioId,
        Dictionary<Guid, Usuario> usuariosPorId,
        CancellationToken cancellationToken)
    {
        if (usuariosPorId.TryGetValue(usuarioId, out var usuario))
        {
            return usuario;
        }

        usuario = await usuarioRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Id == usuarioId
                     && x.Ativo
                     && x.Situacao == SituacaoUsuario.Ativo
                     && (!x.BloqueadoAte.HasValue || x.BloqueadoAte.Value <= DateTime.UtcNow),
                cancellationToken);

        if (usuario is not null)
        {
            usuariosPorId[usuarioId] = usuario;
        }

        return usuario;
    }

    private async Task<IReadOnlyCollection<Usuario>> CarregarUsuariosElegiveisAsync(
        IReadOnlyCollection<Guid> usuarioIds,
        Dictionary<Guid, Usuario> usuariosPorId,
        CancellationToken cancellationToken)
    {
        var idsPendentes = usuarioIds
            .Where(id => !usuariosPorId.ContainsKey(id))
            .Distinct()
            .ToArray();

        if (idsPendentes.Length > 0)
        {
            var usuarios = await usuarioRepository.Query()
                .AsNoTracking()
                .Where(
                    x => idsPendentes.Contains(x.Id)
                         && x.Ativo
                         && x.Situacao == SituacaoUsuario.Ativo
                         && (!x.BloqueadoAte.HasValue || x.BloqueadoAte.Value <= DateTime.UtcNow))
                .ToListAsync(cancellationToken);

            foreach (var usuario in usuarios)
            {
                usuariosPorId[usuario.Id] = usuario;
            }
        }

        return usuarioIds
            .Where(usuariosPorId.ContainsKey)
            .Select(id => usuariosPorId[id])
            .ToArray();
    }
    private static void AdicionarDestinatario(
        Dictionary<Guid, DestinatarioAcumulado> destinatarios,
        Usuario usuario,
        TipoParticipacaoDestinatarioNotificacao participacao)
    {
        if (!destinatarios.TryGetValue(usuario.Id, out var acumulado))
        {
            acumulado = new DestinatarioAcumulado(usuario.Id, usuario.Nome, usuario.Email);
            destinatarios[usuario.Id] = acumulado;
        }

        acumulado.Origens.Add(participacao);
    }

    private sealed record DestinatarioAcumulado(Guid UsuarioId, string Nome, string? Email)
    {
        public HashSet<TipoParticipacaoDestinatarioNotificacao> Origens { get; } = [];
    }
}
