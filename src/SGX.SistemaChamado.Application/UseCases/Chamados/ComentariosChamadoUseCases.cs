using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Chamados;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Chamados;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Chamados;

public sealed class ListarComentariosChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService) : IListarComentariosChamadoUseCase
{
    public async Task<IReadOnlyCollection<ComentarioChamadoResponse>> ExecutarAsync(Guid chamadoId, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        var query = comentarioRepository.Query()
            .AsNoTracking()
            .Include(x => x.Usuario)
            .Where(x => x.Ativo && x.ChamadoId == chamadoId);

        if (ComentariosChamadoUseCaseHelpers.EhSolicitante(usuario))
        {
            query = query.Where(x => !x.Interno);
        }

        var comentarios = await query
            .OrderBy(x => x.CriadoEm)
            .Select(x => new ComentarioChamadoResponse(
                x.Id,
                x.UsuarioId,
                x.Usuario.Nome,
                x.Mensagem,
                x.Interno,
                x.CriadoEm))
            .ToArrayAsync(cancellationToken);

        return comentarios;
    }
}

public sealed class AdicionarComentarioChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<ComentarioChamado> comentarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAdicionarComentarioChamadoUseCase
{
    public async Task<ComentarioChamadoResponse> ExecutarAsync(Guid chamadoId, CriarComentarioChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        ComentariosChamadoUseCaseHelpers.ValidarPerfilPermitido(usuario);

        var chamado = await chamadoRepository.Query()
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        if (!ComentariosChamadoUseCaseHelpers.PodeAcessarChamado(usuario, chamado))
        {
            throw new KeyNotFoundException("Chamado nao encontrado.");
        }

        if (request.Interno && ComentariosChamadoUseCaseHelpers.EhSolicitante(usuario))
        {
            throw new UnauthorizedAccessException("Solicitante nao pode criar comentario interno.");
        }

        var comentario = new ComentarioChamado(
            chamado.Id,
            usuario.Id,
            request.Mensagem,
            request.Interno,
            usuario.Login);

        await comentarioRepository.AddAsync(comentario, cancellationToken);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ComentarioAdicionado,
            ComentariosChamadoUseCaseHelpers.ObterDescricaoHistorico(usuario, request.Interno),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);

        if (!request.Interno && ComentariosChamadoUseCaseHelpers.EhAdministradorOuAtendente(usuario))
        {
            await slaService.RegistrarPrimeiraRespostaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        }

        chamado.AtualizarAuditoria(usuario.Login);
        chamadoRepository.Update(chamado);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ComentarioChamadoResponse(
            comentario.Id,
            comentario.UsuarioId,
            usuario.Nome,
            comentario.Mensagem,
            comentario.Interno,
            comentario.CriadoEm);
    }
}

internal static class ComentariosChamadoUseCaseHelpers
{
    private const string PerfilAdministrador = "Administrador";
    private const string PerfilAtendente = "Atendente";
    private const string PerfilSolicitante = "Solicitante";

    public static bool EhSolicitante(UsuarioContextoAplicacao usuario)
        => usuario.PossuiPerfil(PerfilSolicitante)
           && !usuario.PossuiQualquerPerfil(PerfilAdministrador, PerfilAtendente);

    public static bool EhAdministradorOuAtendente(UsuarioContextoAplicacao usuario)
        => usuario.PossuiQualquerPerfil(PerfilAdministrador, PerfilAtendente);

    public static bool PodeAcessarChamado(UsuarioContextoAplicacao usuario, Chamado chamado)
        => EhAdministradorOuAtendente(usuario) || chamado.SolicitanteId == usuario.Id;

    public static void ValidarPerfilPermitido(UsuarioContextoAplicacao usuario)
    {
        if (!usuario.PossuiQualquerPerfil(PerfilAdministrador, PerfilAtendente, PerfilSolicitante))
        {
            throw new UnauthorizedAccessException("Perfil sem permissao para atendimento de chamados.");
        }
    }

    public static string ObterDescricaoHistorico(UsuarioContextoAplicacao usuario, bool interno)
    {
        if (interno)
        {
            return "Comentario interno adicionado";
        }

        if (EhAdministradorOuAtendente(usuario))
        {
            return "Comentario publico administrativo adicionado";
        }

        return "Comentario adicionado";
    }
}
