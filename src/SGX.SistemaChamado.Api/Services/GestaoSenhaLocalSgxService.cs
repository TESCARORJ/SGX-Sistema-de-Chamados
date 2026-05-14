using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IGestaoSenhaLocalSgxService
{
    Task<MensagemAuthResponse> AlterarSenhaAsync(
        Guid usuarioId,
        AlterarSenhaLocalRequest request,
        CancellationToken cancellationToken = default);

    Task<MensagemAuthResponse> SolicitarRecuperacaoSenhaAsync(
        RecuperarSenhaSolicitacaoRequest request,
        string? ipSolicitacao,
        string? userAgentSolicitacao,
        CancellationToken cancellationToken = default);

    Task<MensagemAuthResponse> RedefinirSenhaAsync(
        RecuperarSenhaRedefinicaoRequest request,
        CancellationToken cancellationToken = default);

    Task<MensagemAuthResponse> RedefinirSenhaPorAdministradorAsync(
        Guid usuarioId,
        RedefinirSenhaUsuarioAdminRequest request,
        string usuarioResponsavel,
        CancellationToken cancellationToken = default);
}

public sealed class GestaoSenhaLocalSgxService(
    SGXSistemaChamadoDbContext dbContext,
    IPasswordHasher<Usuario> passwordHasher,
    IPoliticaSenhaService politicaSenhaService,
    IOptions<AuthOptions> authOptions,
    ITokenRecuperacaoSenhaService tokenRecuperacaoSenhaService,
    ILogger<GestaoSenhaLocalSgxService> logger) : IGestaoSenhaLocalSgxService
{
    private const string UsuarioTecnicoAlteracao = "auth.local.alteracao";
    private const string UsuarioTecnicoRecuperacao = "auth.local.recuperacao";
    private const string UsuarioTecnicoAdmin = "auth.local.admin";
    private const string MensagemRecuperacaoGenerica = "Se o e-mail estiver cadastrado, enviaremos as instruções para redefinição de senha.";
    private const string MensagemCredenciaisInvalidas = "Credenciais inválidas ou acesso temporariamente bloqueado.";

    public async Task<MensagemAuthResponse> AlterarSenhaAsync(
        Guid usuarioId,
        AlterarSenhaLocalRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentException("Payload de alteracao de senha invalido.");
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken)
            ?? throw new UnauthorizedAccessException("Autenticação inválida.");

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            throw new InvalidOperationException("Usuário inativo.");
        }

        if (string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            throw new InvalidOperationException("Usuário não possui senha local SGX configurada.");
        }

        var senhaAtual = request.SenhaAtual ?? string.Empty;
        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmacao = request.ConfirmacaoNovaSenha ?? string.Empty;

        if (!string.Equals(novaSenha, confirmacao, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmação da nova senha divergente.");
        }

        var verificacaoSenhaAtual = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHashLocal, senhaAtual);
        if (verificacaoSenhaAtual == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Falha na alteração de senha local por senha atual inválida. UsuarioId={UsuarioId}", usuario.Id);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidas);
        }

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha inválida.");
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, novaSenha), UsuarioTecnicoAlteracao);
        usuario.DefinirDeveAlterarSenha(false, UsuarioTecnicoAlteracao);
        usuario.LimparLockout(UsuarioTecnicoAlteracao);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Troca de senha local concluída. UsuarioId={UsuarioId}", usuario.Id);

        return new MensagemAuthResponse("Senha alterada com sucesso.");
    }

    public async Task<MensagemAuthResponse> SolicitarRecuperacaoSenhaAsync(
        RecuperarSenhaSolicitacaoRequest request,
        string? ipSolicitacao,
        string? userAgentSolicitacao,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return new MensagemAuthResponse(MensagemRecuperacaoGenerica);
        }

        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email))
        {
            return new MensagemAuthResponse(MensagemRecuperacaoGenerica);
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(x => x.Email == email || x.Login == email, cancellationToken);

        if (usuario is null || !usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo || string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            logger.LogInformation("Solicitação de recuperação de senha local recebida para identificador não elegível.");
            return new MensagemAuthResponse(MensagemRecuperacaoGenerica);
        }

        var tokensAtivos = await dbContext.TokensRecuperacaoSenha
            .Where(x => x.UsuarioId == usuario.Id && x.Ativo && !x.UtilizadoEm.HasValue)
            .ToListAsync(cancellationToken);

        foreach (var tokenAtivo in tokensAtivos)
        {
            tokenAtivo.Desativar(UsuarioTecnicoRecuperacao);
        }

        var token = tokenRecuperacaoSenhaService.GerarToken();
        var tokenHash = tokenRecuperacaoSenhaService.CalcularHash(token);
        var expiracaoMinutos = authOptions.Value.RecuperacaoSenha.ExpiracaoMinutos;

        await dbContext.TokensRecuperacaoSenha.AddAsync(
            new TokenRecuperacaoSenha(
                usuario.Id,
                tokenHash,
                DateTime.UtcNow.AddMinutes(expiracaoMinutos),
                UsuarioTecnicoRecuperacao,
                ipSolicitacao,
                userAgentSolicitacao),
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Solicitação de recuperação de senha local registrada. UsuarioId={UsuarioId}", usuario.Id);

        return new MensagemAuthResponse(MensagemRecuperacaoGenerica);
    }

    public async Task<MensagemAuthResponse> RedefinirSenhaAsync(
        RecuperarSenhaRedefinicaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new UnauthorizedAccessException("Solicitação de redefinição inválida.");
        }

        var token = (request.Token ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("Solicitação de redefinição inválida.");
        }

        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmacaoNovaSenha = request.ConfirmacaoNovaSenha ?? string.Empty;
        if (!string.Equals(novaSenha, confirmacaoNovaSenha, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmação da nova senha divergente.");
        }

        var tokenHash = tokenRecuperacaoSenhaService.CalcularHash(token);
        var registroToken = await dbContext.TokensRecuperacaoSenha
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.Ativo, cancellationToken);

        if (registroToken is null || registroToken.EstaUtilizado() || registroToken.EstaExpirado(DateTime.UtcNow))
        {
            throw new UnauthorizedAccessException("Token inválido ou expirado.");
        }

        var usuario = registroToken.Usuario;
        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            throw new InvalidOperationException("Usuário inativo.");
        }

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha inválida.");
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, novaSenha), UsuarioTecnicoRecuperacao);
        usuario.DefinirDeveAlterarSenha(false, UsuarioTecnicoRecuperacao);
        usuario.LimparLockout(UsuarioTecnicoRecuperacao);
        registroToken.MarcarUtilizado(DateTime.UtcNow, UsuarioTecnicoRecuperacao);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Redefinição de senha local concluída. UsuarioId={UsuarioId}", usuario.Id);

        return new MensagemAuthResponse("Senha redefinida com sucesso.");
    }

    public async Task<MensagemAuthResponse> RedefinirSenhaPorAdministradorAsync(
        Guid usuarioId,
        RedefinirSenhaUsuarioAdminRequest request,
        string usuarioResponsavel,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentException("Payload de redefinição de senha inválido.");
        }

        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmarNovaSenha = request.ConfirmarNovaSenha ?? string.Empty;
        if (!string.Equals(novaSenha, confirmarNovaSenha, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmação da nova senha divergente.");
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuário não encontrado.");

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha inválida.");
        }

        var usuarioTecnico = string.IsNullOrWhiteSpace(usuarioResponsavel)
            ? UsuarioTecnicoAdmin
            : $"{UsuarioTecnicoAdmin}:{usuarioResponsavel.Trim()}";

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, novaSenha), usuarioTecnico);
        usuario.DefinirDeveAlterarSenha(request.DeveAlterarSenha, usuarioTecnico);
        usuario.LimparLockout(usuarioTecnico);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Senha local redefinida por Administrador. UsuarioId={UsuarioId} Responsavel={Responsavel}",
            usuario.Id,
            string.IsNullOrWhiteSpace(usuarioResponsavel) ? "nao-informado" : usuarioResponsavel.Trim());

        return new MensagemAuthResponse("Senha redefinida com sucesso.");
    }

}
