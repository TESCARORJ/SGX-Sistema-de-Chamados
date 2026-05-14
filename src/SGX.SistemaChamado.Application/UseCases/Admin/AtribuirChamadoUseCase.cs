using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Application.DTOs.Admin;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Admin;
using SGX.SistemaChamado.Application.Interfaces.Persistence;
using SGX.SistemaChamado.Application.Interfaces.Sla;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;

namespace SGX.SistemaChamado.Application.UseCases.Admin;

public sealed class AtribuirChamadoUseCase(
    IRepository<Chamado> chamadoRepository,
    IRepository<Usuario> usuarioRepository,
    IRepository<HistoricoChamado> historicoRepository,
    ISlaService slaService,
    IUsuarioContextoAplicacaoService usuarioContextoAplicacaoService,
    IUnitOfWork unitOfWork) : IAtribuirChamadoUseCase
{
    public async Task<ChamadoAdminDetalheResponse> ExecutarAsync(Guid chamadoId, AtribuirChamadoRequest request, CancellationToken cancellationToken = default)
    {
        if (chamadoId == Guid.Empty)
        {
            throw new ArgumentException("Id do chamado invalido.", nameof(chamadoId));
        }

        var usuario = await usuarioContextoAplicacaoService.ObterAsync(cancellationToken);
        if (!AdminUseCaseHelpers.PodeOperarAdmin(usuario))
        {
            throw new UnauthorizedAccessException("Acesso administrativo negado.");
        }

        if (!AdminUseCaseHelpers.EhAdministrador(usuario))
        {
            throw new InvalidOperationException("Atribuicao de responsavel permitida apenas para Administrador nesta sprint.");
        }

        var chamado = await chamadoRepository.Query()
            .Include(x => x.ChamadoSla)
            .FirstOrDefaultAsync(x => x.Id == chamadoId && x.Ativo, cancellationToken)
            ?? throw new KeyNotFoundException("Chamado nao encontrado.");

        var responsavel = await usuarioRepository.Query()
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso)
            .FirstOrDefaultAsync(x => x.Id == request.ResponsavelId && x.Ativo && x.Situacao == SituacaoUsuario.Ativo, cancellationToken)
            ?? throw new InvalidOperationException("Responsavel informado nao encontrado ou inativo.");

        var possuiPerfilAtendimento = responsavel.UsuarioPerfis.Any(p =>
            p.PerfilAcesso.TipoPerfil == TipoPerfil.Administrador ||
            p.PerfilAcesso.TipoPerfil == TipoPerfil.Atendente);

        if (!possuiPerfilAtendimento)
        {
            throw new InvalidOperationException("Usuario destino nao possui perfil de atendimento.");
        }

        chamado.AtribuirResponsavel(responsavel.Id, usuario.Login);
        await slaService.RegistrarPrimeiraRespostaAsync(chamado, usuario.Login, DateTime.UtcNow, cancellationToken);
        chamadoRepository.Update(chamado);

        var historico = new HistoricoChamado(
            chamado.Id,
            TipoHistoricoChamado.ResponsavelAlterado,
            AdminUseCaseHelpers.ObterDescricaoHistorico(TipoHistoricoChamado.ResponsavelAlterado, $"Responsavel alterado para {responsavel.Nome}"),
            usuario.Id,
            usuario.Login);

        await historicoRepository.AddAsync(historico, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var atualizado = await AdminChamadoLoader.QueryDetalhe(chamadoRepository.Query().AsNoTracking())
            .FirstAsync(x => x.Id == chamadoId, cancellationToken);

        return AdminUseCaseHelpers.MapDetalhe(atualizado);
    }
}
