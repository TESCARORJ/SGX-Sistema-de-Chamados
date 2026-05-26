using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IAutenticacaoLocalSgxService
{
    Task<LocalLoginResponse> LoginAsync(LocalLoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AutenticacaoLocalSgxService(
    SGXSistemaChamadoDbContext dbContext,
    IPasswordHasher<Usuario> passwordHasher,
    IOptions<AuthOptions> authOptions,
    IMetodosLoginAdminService metodosLoginAdminService,
    ILogger<AutenticacaoLocalSgxService> logger,
    IAuditoriaService? auditoriaService = null) : IAutenticacaoLocalSgxService
{
    private const string OrigemAutenticacaoLocalSgx = "LocalSgx";
    private const string UsuarioTecnico = "auth.local";
    private const string MensagemCredenciaisInvalidasOuBloqueio = "Credenciais inválidas ou acesso temporariamente bloqueado.";

    public async Task<LocalLoginResponse> LoginAsync(LocalLoginRequest request, CancellationToken cancellationToken = default)
    {
        var options = authOptions.Value;
        var localSgxHabilitado = await metodosLoginAdminService
            .ProvedorHabilitadoAsync(CodigoProvedorAutenticacao.LocalSgx, cancellationToken);

        if (!options.LoginLocalHabilitado || !localSgxHabilitado)
        {
            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.ProvedorDesabilitadoTentativaLogin,
                ResultadoEventoAutenticacao.Negado,
                "Tentativa de login local SGX com provedor desabilitado.",
                CodigoProvedorAutenticacao.LocalSgx,
                mensagemTecnica: "Login local SGX desabilitado pela configuracao atual.",
                cancellationToken: cancellationToken);

            throw new InvalidOperationException("Login local SGX desabilitado pela configuração atual.");
        }

        var identificador = NormalizarIdentificador(request.Email);
        var senhaInformada = request.Senha ?? string.Empty;

        if (string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(senhaInformada))
        {
            await RegistrarFalhaLoginAsync(
                "Falha de login local SGX por credenciais ausentes.",
                MensagemCredenciaisInvalidasOuBloqueio,
                identificador,
                cancellationToken);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(
                x => x.Email == identificador || x.Login == identificador,
                cancellationToken);

        if (usuario is null || string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            logger.LogWarning("Falha no login local SGX. Identificador não localizado ou sem senha local.");
            await RegistrarFalhaLoginAsync(
                "Falha de login local SGX por usuario inexistente ou sem senha local.",
                MensagemCredenciaisInvalidasOuBloqueio,
                identificador,
                cancellationToken);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            await RegistrarFalhaLoginAsync(
                "Falha de login local SGX para usuario inativo.",
                "Usuario inativo no SGX Sistema de Chamados.",
                identificador,
                cancellationToken,
                usuario);

            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.UsuarioInativoBloqueado,
                ResultadoEventoAutenticacao.Bloqueado,
                "Tentativa de login local SGX bloqueada para usuario inativo.",
                CodigoProvedorAutenticacao.LocalSgx,
                usuarioId: usuario.Id,
                usuarioNome: usuario.Nome,
                usuarioEmail: usuario.Email,
                usuarioLogin: usuario.Login,
                usuarioAlvoId: usuario.Id,
                usuarioAlvoEmail: usuario.Email,
                cancellationToken: cancellationToken);

            throw new AcessoNegadoException("Usuário inativo no SGX Sistema de Chamados.");
        }

        var agoraUtc = DateTime.UtcNow;
        if (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte.Value > agoraUtc)
        {
            logger.LogWarning("Login local SGX bloqueado temporariamente. UsuarioId={UsuarioId}", usuario.Id);
            await RegistrarFalhaLoginAsync(
                "Falha de login local SGX por lockout ativo.",
                MensagemCredenciaisInvalidasOuBloqueio,
                identificador,
                cancellationToken,
                usuario);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        var verificacao = passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHashLocal, senhaInformada);
        if (verificacao == PasswordVerificationResult.Failed)
        {
            var lockoutOptions = options.Lockout;
            usuario.RegistrarFalhaLoginLocal(
                lockoutOptions.TentativasMaximas,
                TimeSpan.FromMinutes(lockoutOptions.MinutosBloqueio),
                agoraUtc,
                UsuarioTecnico);

            await dbContext.SaveChangesAsync(cancellationToken);

            if (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte.Value > agoraUtc)
            {
                logger.LogWarning("Usuário bloqueado por tentativas inválidas no login local SGX. UsuarioId={UsuarioId}", usuario.Id);
            }
            else
            {
                logger.LogWarning("Falha no login local SGX por senha inválida. UsuarioId={UsuarioId}", usuario.Id);
            }

            await RegistrarFalhaLoginAsync(
                "Falha de login local SGX por senha invalida.",
                MensagemCredenciaisInvalidasOuBloqueio,
                identificador,
                cancellationToken,
                usuario);
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        usuario.RegistrarLoginLocalBemSucedido(agoraUtc, UsuarioTecnico);

        if (verificacao == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, senhaInformada), UsuarioTecnico);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Login local SGX concluído com sucesso. UsuarioId={UsuarioId}", usuario.Id);
        if (auditoriaService is not null)
        {
            try
            {
                await auditoriaService.RegistrarLoginAsync(
                    true,
                    "Login local SGX realizado com sucesso.",
                    usuarioId: usuario.Id,
                    usuarioNome: usuario.Nome,
                    usuarioEmail: usuario.Email,
                    usuarioLogin: usuario.Login,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao registrar evento de auditoria legada de login local SGX (sucesso).");
            }
        }

        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.LoginLocalSgxSucesso,
            ResultadoEventoAutenticacao.Sucesso,
            "Login local SGX bem-sucedido.",
            CodigoProvedorAutenticacao.LocalSgx,
            usuarioId: usuario.Id,
            usuarioNome: usuario.Nome,
            usuarioEmail: usuario.Email,
            usuarioLogin: usuario.Login,
            usuarioAlvoId: usuario.Id,
            usuarioAlvoEmail: usuario.Email,
            cancellationToken: cancellationToken);

        return GerarRespostaLogin(usuario, options);
    }

    private static string NormalizarIdentificador(string? valor)
        => (valor ?? string.Empty).Trim().ToLowerInvariant();

    private static LocalLoginResponse GerarRespostaLogin(Usuario usuario, AuthOptions options)
    {
        var agoraUtc = DateTime.UtcNow;
        var expiracao = agoraUtc.AddMinutes(options.JwtLocalExpiracaoMinutos <= 0 ? 120 : options.JwtLocalExpiracaoMinutos);
        var chave = ObterChaveAssinatura(options);
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new("name", usuario.Nome),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new("email", usuario.Email),
            new("preferred_username", usuario.Login),
            new("upn", usuario.Email),
            new("auth_provider", OrigemAutenticacaoLocalSgx),
            new("autenticado_por", OrigemAutenticacaoLocalSgx)
        };

        var token = new JwtSecurityToken(
            issuer: options.JwtLocalIssuer,
            audience: options.JwtLocalAudience,
            claims: claims,
            notBefore: agoraUtc,
            expires: expiracao,
            signingCredentials: credenciais);

        var tokenSerializado = new JwtSecurityTokenHandler().WriteToken(token);

        return new LocalLoginResponse(
            AccessToken: tokenSerializado,
            TokenType: "Bearer",
            ExpiresIn: (int)Math.Floor((expiracao - agoraUtc).TotalSeconds),
            AutenticadoPor: OrigemAutenticacaoLocalSgx,
            DeveAlterarSenha: usuario.DeveAlterarSenha);
    }

    private static SymmetricSecurityKey ObterChaveAssinatura(AuthOptions options)
    {
        var chaveTexto = (options.JwtLocalChaveAssinatura ?? string.Empty).Trim();
        if (chaveTexto.Length < 32)
        {
            throw new InvalidOperationException(
                "Authentication:JwtLocalChaveAssinatura deve possuir ao menos 32 caracteres para login local SGX.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveTexto));
    }

    private async Task RegistrarFalhaLoginAsync(
        string descricao,
        string mensagemErro,
        string? identificador,
        CancellationToken cancellationToken,
        Usuario? usuario = null)
    {
        if (auditoriaService is null)
        {
            return;
        }

        try
        {
            await auditoriaService.RegistrarLoginAsync(
                false,
                descricao,
                mensagemErro: mensagemErro,
                usuarioId: usuario?.Id,
                usuarioNome: usuario?.Nome,
                usuarioEmail: usuario?.Email,
                usuarioLogin: usuario?.Login ?? identificador,
                metadados: string.IsNullOrWhiteSpace(identificador)
                    ? null
                    : JsonSerializer.Serialize(new { identificador }),
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao registrar evento de auditoria legada de login local SGX.");
        }

        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.LoginLocalSgxNegado,
            ResultadoEventoAutenticacao.Falha,
            "Falha de login local SGX.",
            CodigoProvedorAutenticacao.LocalSgx,
            mensagemTecnica: mensagemErro,
            usuarioId: usuario?.Id,
            usuarioNome: usuario?.Nome,
            usuarioEmail: usuario?.Email,
            usuarioLogin: usuario?.Login ?? identificador,
            usuarioAlvoId: usuario?.Id,
            usuarioAlvoEmail: usuario?.Email,
            observacao: string.IsNullOrWhiteSpace(identificador) ? null : $"Identificador: {identificador}",
            cancellationToken: cancellationToken);
    }
}
