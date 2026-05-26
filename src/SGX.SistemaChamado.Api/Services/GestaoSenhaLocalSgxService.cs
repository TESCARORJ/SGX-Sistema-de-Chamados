using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Application.Helpers;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
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
    ILogger<GestaoSenhaLocalSgxService> logger,
    IAuditoriaService? auditoriaService = null) : IGestaoSenhaLocalSgxService
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
            ?? throw new UnauthorizedAccessException("Autenticacao invalida.");

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            throw new InvalidOperationException("Usuario inativo.");
        }

        if (string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            throw new InvalidOperationException("Usuario nao possui senha local SGX configurada.");
        }

        var senhaAtual = request.SenhaAtual ?? string.Empty;
        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmacao = request.ConfirmacaoNovaSenha ?? string.Empty;

        if (!string.Equals(novaSenha, confirmacao, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmacao da nova senha divergente.");
        }

        var verificacaoSenhaAtual = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHashLocal, senhaAtual);
        if (verificacaoSenhaAtual == PasswordVerificationResult.Failed)
        {
            logger.LogWarning("Falha na alteracao de senha local por senha atual invalida. UsuarioId={UsuarioId}", usuario.Id);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidas);
        }

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha invalida.");
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, novaSenha), UsuarioTecnicoAlteracao);
        usuario.DefinirDeveAlterarSenha(false, UsuarioTecnicoAlteracao);
        usuario.LimparLockout(UsuarioTecnicoAlteracao);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Troca de senha local concluida. UsuarioId={UsuarioId}", usuario.Id);
        await RegistrarAuditoriaSenhaAsync(
            usuario,
            "Troca de senha local realizada pelo proprio usuario.",
            "AlteracaoSenha",
            cancellationToken);
        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.TrocaObrigatoriaSenhaConcluida,
            ResultadoEventoAutenticacao.Sucesso,
            "Troca de senha local concluida no primeiro acesso.",
            CodigoProvedorAutenticacao.LocalSgx,
            usuarioId: usuario.Id,
            usuarioNome: usuario.Nome,
            usuarioEmail: usuario.Email,
            usuarioLogin: usuario.Login,
            usuarioAlvoId: usuario.Id,
            usuarioAlvoEmail: usuario.Email,
            cancellationToken: cancellationToken);

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
            logger.LogInformation("Solicitacao de recuperacao de senha local recebida para identificador nao elegivel.");
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
        logger.LogInformation("Solicitacao de recuperacao de senha local registrada. UsuarioId={UsuarioId}", usuario.Id);
        await RegistrarAuditoriaSenhaAsync(
            usuario,
            "Solicitacao de recuperacao de senha local registrada.",
            "SolicitacaoRecuperacaoSenha",
            cancellationToken);
        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.RecuperacaoSenhaSolicitada,
            ResultadoEventoAutenticacao.Sucesso,
            "Solicitacao de recuperacao de senha local registrada.",
            CodigoProvedorAutenticacao.LocalSgx,
            usuarioId: usuario.Id,
            usuarioNome: usuario.Nome,
            usuarioEmail: usuario.Email,
            usuarioLogin: usuario.Login,
            usuarioAlvoId: usuario.Id,
            usuarioAlvoEmail: usuario.Email,
            cancellationToken: cancellationToken);

        return new MensagemAuthResponse(MensagemRecuperacaoGenerica);
    }

    public async Task<MensagemAuthResponse> RedefinirSenhaAsync(
        RecuperarSenhaRedefinicaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new UnauthorizedAccessException("Solicitacao de redefinicao invalida.");
        }

        var token = (request.Token ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new UnauthorizedAccessException("Solicitacao de redefinicao invalida.");
        }

        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmacaoNovaSenha = request.ConfirmacaoNovaSenha ?? string.Empty;
        if (!string.Equals(novaSenha, confirmacaoNovaSenha, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmacao da nova senha divergente.");
        }

        var tokenHash = tokenRecuperacaoSenhaService.CalcularHash(token);
        var registroToken = await dbContext.TokensRecuperacaoSenha
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.Ativo, cancellationToken);

        if (registroToken is null || registroToken.EstaUtilizado() || registroToken.EstaExpirado(DateTime.UtcNow))
        {
            throw new UnauthorizedAccessException("Token invalido ou expirado.");
        }

        var usuario = registroToken.Usuario;
        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            throw new InvalidOperationException("Usuario inativo.");
        }

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha invalida.");
        }

        usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, novaSenha), UsuarioTecnicoRecuperacao);
        usuario.DefinirDeveAlterarSenha(false, UsuarioTecnicoRecuperacao);
        usuario.LimparLockout(UsuarioTecnicoRecuperacao);
        registroToken.MarcarUtilizado(DateTime.UtcNow, UsuarioTecnicoRecuperacao);

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Redefinicao de senha local concluida. UsuarioId={UsuarioId}", usuario.Id);
        await RegistrarAuditoriaSenhaAsync(
            usuario,
            "Senha local redefinida por fluxo de recuperacao.",
            "RedefinicaoSenhaRecuperacao",
            cancellationToken);
        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.RedefinicaoSenhaConcluida,
            ResultadoEventoAutenticacao.Sucesso,
            "Redefinicao de senha local concluida por fluxo de recuperacao.",
            CodigoProvedorAutenticacao.LocalSgx,
            usuarioId: usuario.Id,
            usuarioNome: usuario.Nome,
            usuarioEmail: usuario.Email,
            usuarioLogin: usuario.Login,
            usuarioAlvoId: usuario.Id,
            usuarioAlvoEmail: usuario.Email,
            cancellationToken: cancellationToken);

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
            throw new ArgumentException("Payload de redefinicao de senha invalido.");
        }

        var novaSenha = request.NovaSenha ?? string.Empty;
        var confirmarNovaSenha = request.ConfirmarNovaSenha ?? string.Empty;
        if (!string.Equals(novaSenha, confirmarNovaSenha, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Confirmacao da nova senha divergente.");
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(x => x.Id == usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Usuario nao encontrado.");

        var validacao = politicaSenhaService.ValidarNovaSenha(usuario, novaSenha);
        if (!validacao.Valida)
        {
            throw new InvalidOperationException(validacao.Motivo ?? "Nova senha invalida.");
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

        await RegistrarAuditoriaSenhaAsync(
            usuario,
            "Senha local redefinida por administrador.",
            "RedefinicaoSenhaAdministrador",
            cancellationToken,
            observacao: string.IsNullOrWhiteSpace(usuarioResponsavel)
                ? "Responsavel: nao-informado"
                : $"Responsavel: {usuarioResponsavel.Trim()}");

        return new MensagemAuthResponse("Senha redefinida com sucesso.");
    }

    private Task RegistrarAuditoriaSenhaAsync(
        Usuario usuarioAfetado,
        string descricao,
        string operacao,
        CancellationToken cancellationToken,
        string? observacao = null)
    {
        if (auditoriaService is null)
        {
            return Task.CompletedTask;
        }

        return auditoriaService.RegistrarAsync(new RegistrarEventoAuditoriaRequest
        {
            Modulo = "Usuarios",
            Entidade = "Usuario",
            EntidadeId = usuarioAfetado.Id.ToString(),
            Acao = TipoAcaoAuditoria.Edicao,
            Descricao = descricao,
            Nivel = NivelAuditoria.Informacao,
            Sucesso = true,
            Metadados = AuditoriaDiffHelper.CriarMetadadosPadrao(
                origem: "api",
                modulo: "Usuarios",
                entidade: "Usuario",
                entidadeId: usuarioAfetado.Id.ToString(),
                codigo: usuarioAfetado.Login,
                nome: usuarioAfetado.Email,
                operacao: operacao,
                resultado: "Sucesso",
                observacao: observacao)
        }, cancellationToken);
    }
}
