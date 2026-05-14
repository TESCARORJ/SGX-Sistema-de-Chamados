using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
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
    ILogger<AutenticacaoLocalSgxService> logger) : IAutenticacaoLocalSgxService
{
    private const string OrigemAutenticacaoLocalSgx = "LocalSgx";
    private const string UsuarioTecnico = "auth.local";
    private const string MensagemCredenciaisInvalidasOuBloqueio = "Credenciais inválidas ou acesso temporariamente bloqueado.";

    public async Task<LocalLoginResponse> LoginAsync(LocalLoginRequest request, CancellationToken cancellationToken = default)
    {
        var options = authOptions.Value;
        if (!options.LoginLocalHabilitado || !options.UsaLoginLocalSgxComoPrincipalOuHibrido())
        {
            throw new InvalidOperationException("Login local SGX desabilitado pela configuração atual.");
        }

        var identificador = NormalizarIdentificador(request.Email);
        var senhaInformada = request.Senha ?? string.Empty;

        if (string.IsNullOrWhiteSpace(identificador) || string.IsNullOrWhiteSpace(senhaInformada))
        {
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        var usuario = await dbContext.Usuarios
            .FirstOrDefaultAsync(
                x => x.Email == identificador || x.Login == identificador,
                cancellationToken);

        if (usuario is null || string.IsNullOrWhiteSpace(usuario.SenhaHashLocal))
        {
            logger.LogWarning("Falha no login local SGX. Identificador não localizado ou sem senha local.");
            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            throw new AcessoNegadoException("Usuário inativo no SGX Sistema de Chamados.");
        }

        var agoraUtc = DateTime.UtcNow;
        if (usuario.BloqueadoAte.HasValue && usuario.BloqueadoAte.Value > agoraUtc)
        {
            logger.LogWarning("Login local SGX bloqueado temporariamente. UsuarioId={UsuarioId}", usuario.Id);
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

            throw new UnauthorizedAccessException(MensagemCredenciaisInvalidasOuBloqueio);
        }

        usuario.RegistrarLoginLocalBemSucedido(agoraUtc, UsuarioTecnico);

        if (verificacao == PasswordVerificationResult.SuccessRehashNeeded)
        {
            usuario.DefinirSenhaHashLocal(passwordHasher.HashPassword(usuario, senhaInformada), UsuarioTecnico);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Login local SGX concluído com sucesso. UsuarioId={UsuarioId}", usuario.Id);

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
}
