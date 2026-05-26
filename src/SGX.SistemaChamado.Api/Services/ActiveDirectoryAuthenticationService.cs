using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Exceptions;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IActiveDirectoryAuthenticationService
{
    Task<LocalLoginResponse> LoginAsync(LoginActiveDirectoryRequest request, CancellationToken cancellationToken = default);
}

public interface IActiveDirectoryCredentialValidator
{
    Task<ActiveDirectoryValidacaoResultado> ValidarCredenciaisAsync(
        string usuario,
        string senha,
        string dominio,
        ActiveDirectoryOptions options,
        CancellationToken cancellationToken = default);
}

public sealed record ActiveDirectoryValidacaoResultado(
    bool Sucesso,
    string UsuarioSamAccountName,
    string? NomeCompleto,
    string? Email,
    string? UserPrincipalName,
    string? DistinguishedName,
    string? DetalheErro = null);

public sealed class ActiveDirectoryCredentialValidator(
    ILogger<ActiveDirectoryCredentialValidator> logger) : IActiveDirectoryCredentialValidator
{
    public Task<ActiveDirectoryValidacaoResultado> ValidarCredenciaisAsync(
        string usuario,
        string senha,
        string dominio,
        ActiveDirectoryOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var identificador = CriarIdentificadorLdap(options);
            using var connection = new LdapConnection(identificador);
            connection.AuthType = AuthType.Negotiate;
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.SecureSocketLayer = options.UsarLdaps;
            connection.Timeout = TimeSpan.FromSeconds(options.TimeoutConexaoSegundos <= 0 ? 10 : options.TimeoutConexaoSegundos);

            var usuarioParaBind = MontarUsuarioBind(usuario, dominio);
            connection.Bind(new NetworkCredential(usuarioParaBind, senha));

            var searchFilter = string.Format(
                options.UserSearchFilter,
                EscapeLdapFilterValue(NormalizarUsuario(usuario)));
            var searchRequest = new SearchRequest(
                options.BaseDn,
                searchFilter,
                SearchScope.Subtree,
                ["displayName", "mail", "userPrincipalName", "sAMAccountName", "distinguishedName"]);

            var response = (SearchResponse)connection.SendRequest(searchRequest);
            var entry = response.Entries.Cast<SearchResultEntry>().FirstOrDefault();
            if (entry is null)
            {
                return Task.FromResult(new ActiveDirectoryValidacaoResultado(
                    Sucesso: true,
                    UsuarioSamAccountName: NormalizarUsuario(usuario),
                    NomeCompleto: null,
                    Email: null,
                    UserPrincipalName: null,
                    DistinguishedName: null));
            }

            var usuarioSam = ObterAtributo(entry, "sAMAccountName") ?? NormalizarUsuario(usuario);
            var nomeCompleto = ObterAtributo(entry, "displayName");
            var email = ObterAtributo(entry, "mail");
            var userPrincipalName = ObterAtributo(entry, "userPrincipalName");
            var distinguishedName = ObterAtributo(entry, "distinguishedName");

            return Task.FromResult(new ActiveDirectoryValidacaoResultado(
                Sucesso: true,
                UsuarioSamAccountName: usuarioSam,
                NomeCompleto: nomeCompleto,
                Email: email,
                UserPrincipalName: userPrincipalName,
                DistinguishedName: distinguishedName));
        }
        catch (LdapException ex)
        {
            logger.LogWarning(ex, "Falha LDAP/LDAPS no bind Active Directory para o usuario informado.");
            return Task.FromResult(new ActiveDirectoryValidacaoResultado(
                Sucesso: false,
                UsuarioSamAccountName: NormalizarUsuario(usuario),
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DistinguishedName: null,
                DetalheErro: "Falha de bind LDAP."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado durante validacao Active Directory.");
            return Task.FromResult(new ActiveDirectoryValidacaoResultado(
                Sucesso: false,
                UsuarioSamAccountName: NormalizarUsuario(usuario),
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DistinguishedName: null,
                DetalheErro: "Erro de comunicacao com Active Directory."));
        }
    }

    private static LdapDirectoryIdentifier CriarIdentificadorLdap(ActiveDirectoryOptions options)
    {
        var servidor = (options.Servidor ?? string.Empty).Trim();
        if (Uri.TryCreate(servidor, UriKind.Absolute, out var uri))
        {
            return new LdapDirectoryIdentifier(uri.Host, options.Porta, fullyQualifiedDnsHostName: true, connectionless: false);
        }

        return new LdapDirectoryIdentifier(servidor, options.Porta, fullyQualifiedDnsHostName: true, connectionless: false);
    }

    private static string MontarUsuarioBind(string usuario, string dominio)
    {
        var usuarioNormalizado = (usuario ?? string.Empty).Trim();
        if (usuarioNormalizado.Contains('\\') || usuarioNormalizado.Contains('@'))
        {
            return usuarioNormalizado;
        }

        var dominioNormalizado = (dominio ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(dominioNormalizado)
            ? usuarioNormalizado
            : $"{dominioNormalizado}\\{usuarioNormalizado}";
    }

    private static string NormalizarUsuario(string? usuario)
        => (usuario ?? string.Empty).Trim().ToLowerInvariant();

    private static string EscapeLdapFilterValue(string value)
    {
        return value
            .Replace("\\", "\\5c", StringComparison.Ordinal)
            .Replace("*", "\\2a", StringComparison.Ordinal)
            .Replace("(", "\\28", StringComparison.Ordinal)
            .Replace(")", "\\29", StringComparison.Ordinal)
            .Replace("\0", "\\00", StringComparison.Ordinal);
    }

    private static string? ObterAtributo(SearchResultEntry entry, string atributo)
    {
        if (!entry.Attributes.Contains(atributo))
        {
            return null;
        }

        var valor = entry.Attributes[atributo][0]?.ToString();
        return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
    }
}

public sealed class ActiveDirectoryAuthenticationService(
    SGXSistemaChamadoDbContext dbContext,
    IOptions<AuthOptions> authOptions,
    IConfiguracaoIntegracaoActiveDirectoryService configuracaoIntegracaoActiveDirectoryService,
    IMetodosLoginAdminService metodosLoginAdminService,
    IActiveDirectoryCredentialValidator credentialValidator,
    ILogger<ActiveDirectoryAuthenticationService> logger,
    IAuditoriaService? auditoriaService = null) : IActiveDirectoryAuthenticationService
{
    private const string OrigemAutenticacaoActiveDirectory = "ActiveDirectory";
    private const string UsuarioTecnico = "auth.ad";
    private const string MensagemErroGenerica = "Credenciais inválidas ou acesso temporariamente bloqueado.";

    public async Task<LocalLoginResponse> LoginAsync(LoginActiveDirectoryRequest request, CancellationToken cancellationToken = default)
    {
        var auth = authOptions.Value;
        var ad = await configuracaoIntegracaoActiveDirectoryService.ObterConfiguracaoEfetivaAsync(cancellationToken);
        var metodoAd = await metodosLoginAdminService
            .ObterMetodoEfetivoAsync(CodigoProvedorAutenticacao.ActiveDirectory, cancellationToken);
        if (metodoAd is null || !metodoAd.Habilitado || !metodoAd.Funcional)
        {
            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.ProvedorDesabilitadoTentativaLogin,
                ResultadoEventoAutenticacao.Negado,
                "Tentativa de login Active Directory com provedor desabilitado.",
                CodigoProvedorAutenticacao.ActiveDirectory,
                mensagemTecnica: "Login Active Directory desabilitado pela configuracao atual.",
                cancellationToken: cancellationToken);

            throw new InvalidOperationException("Login Active Directory desabilitado pela configuracao atual.");
        }

        if (!ad.EstaConfigurado())
        {
            logger.LogError("Falha de configuracao Active Directory: secao ActiveDirectory incompleta.");
            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.FalhaConfiguracaoProvedor,
                ResultadoEventoAutenticacao.Falha,
                "Falha de configuracao do provedor Active Directory.",
                CodigoProvedorAutenticacao.ActiveDirectory,
                mensagemTecnica: "Secao ActiveDirectory incompleta.",
                cancellationToken: cancellationToken);
            throw new InvalidOperationException("Configuracao Active Directory invalida.");
        }

        var usuarioInformado = (request?.Usuario ?? string.Empty).Trim();
        var senhaInformada = request?.Senha ?? string.Empty;
        var dominioInformado = string.IsNullOrWhiteSpace(request?.Dominio) ? ad.Dominio : request.Dominio.Trim();

        if (string.IsNullOrWhiteSpace(usuarioInformado) || string.IsNullOrWhiteSpace(senhaInformada))
        {
            await RegistrarFalhaAsync(
                "Falha de login Active Directory por credenciais ausentes.",
                usuarioInformado,
                cancellationToken,
                tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                detalheTecnico: "Usuario/senha ausentes.");
            throw new UnauthorizedAccessException(MensagemErroGenerica);
        }

        var validacao = await credentialValidator.ValidarCredenciaisAsync(
            usuarioInformado,
            senhaInformada,
            dominioInformado,
            ad,
            cancellationToken);

        if (!validacao.Sucesso)
        {
            await RegistrarFalhaAsync(
                "Falha de login Active Directory por credenciais invalidas.",
                usuarioInformado,
                cancellationToken,
                tipoEvento: TipoEventoAutenticacao.FalhaCredencialInvalida,
                detalheTecnico: validacao.DetalheErro);
            throw new UnauthorizedAccessException(MensagemErroGenerica);
        }

        var loginNormalizado = NormalizarLogin(validacao.UsuarioSamAccountName, usuarioInformado);
        var emailNormalizado = NormalizarEmail(validacao.Email, validacao.UserPrincipalName);

        var usuario = await CarregarUsuarioInternoAsync(loginNormalizado, emailNormalizado, cancellationToken);
        if (usuario is null)
        {
            if (!metodoAd.PermiteAutoProvisionamento)
            {
                await RegistrarFalhaAsync(
                    "Falha de login Active Directory por usuario interno nao provisionado.",
                    loginNormalizado,
                    cancellationToken,
                    tipoEvento: TipoEventoAutenticacao.LoginActiveDirectoryNegado);
                throw new UnauthorizedAccessException(MensagemErroGenerica);
            }

            usuario = await AutoProvisionarUsuarioAsync(validacao, loginNormalizado, emailNormalizado, metodoAd.PerfilPadraoAutoProvisionamento, cancellationToken);
            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.AutoProvisionamentoUsuario,
                ResultadoEventoAutenticacao.Sucesso,
                "Usuario auto provisionado por autenticacao Active Directory.",
                CodigoProvedorAutenticacao.ActiveDirectory,
                usuarioId: usuario.Id,
                usuarioNome: usuario.Nome,
                usuarioEmail: usuario.Email,
                usuarioLogin: usuario.Login,
                usuarioAlvoId: usuario.Id,
                usuarioAlvoEmail: usuario.Email,
                cancellationToken: cancellationToken);
        }

        if (!usuario.Ativo || usuario.Situacao != SituacaoUsuario.Ativo)
        {
            await RegistrarFalhaAsync(
                "Falha de login Active Directory para usuario inativo.",
                loginNormalizado,
                cancellationToken,
                tipoEvento: TipoEventoAutenticacao.UsuarioInativoBloqueado,
                detalheTecnico: "Usuario inativo no SGX.",
                usuarioAlvo: usuario);

            throw new AcessoNegadoException("Usuário inativo no SGX Sistema de Chamados.");
        }

        usuario.AtualizarUltimoAcesso(DateTime.UtcNow, UsuarioTecnico);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (auditoriaService is not null)
        {
            try
            {
                await auditoriaService.RegistrarLoginAsync(
                    true,
                    "Login Active Directory realizado com sucesso.",
                    usuarioId: usuario.Id,
                    usuarioNome: usuario.Nome,
                    usuarioEmail: usuario.Email,
                    usuarioLogin: usuario.Login,
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao registrar evento de auditoria legada de login Active Directory (sucesso).");
            }
        }

        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            TipoEventoAutenticacao.LoginActiveDirectorySucesso,
            ResultadoEventoAutenticacao.Sucesso,
            "Login Active Directory bem-sucedido.",
            CodigoProvedorAutenticacao.ActiveDirectory,
            usuarioId: usuario.Id,
            usuarioNome: usuario.Nome,
            usuarioEmail: usuario.Email,
            usuarioLogin: usuario.Login,
            usuarioAlvoId: usuario.Id,
            usuarioAlvoEmail: usuario.Email,
            cancellationToken: cancellationToken);

        return GerarRespostaLogin(usuario, auth);
    }

    private async Task<Usuario?> CarregarUsuarioInternoAsync(string login, string? email, CancellationToken cancellationToken)
    {
        var query = dbContext.Usuarios
            .Include(x => x.UsuarioPerfis)
            .ThenInclude(x => x.PerfilAcesso);

        var usuarioPorLogin = await query.FirstOrDefaultAsync(x => x.Login == login, cancellationToken);
        if (usuarioPorLogin is not null)
        {
            return usuarioPorLogin;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        return await query.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    private async Task<Usuario> AutoProvisionarUsuarioAsync(
        ActiveDirectoryValidacaoResultado validacao,
        string loginNormalizado,
        string? emailNormalizado,
        string perfilPadraoConfigurado,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(emailNormalizado))
        {
            logger.LogWarning("Auto provisionamento AD sem e-mail foi bloqueado para usuario {Usuario}.", loginNormalizado);
            throw new UnauthorizedAccessException(MensagemErroGenerica);
        }

        var perfilPadraoNome = string.IsNullOrWhiteSpace(perfilPadraoConfigurado)
            ? PerfisInternos.Solicitante
            : perfilPadraoConfigurado.Trim();
        var perfilPadrao = await dbContext.PerfisAcesso
            .FirstOrDefaultAsync(x => x.Ativo && x.Nome == perfilPadraoNome, cancellationToken);
        if (perfilPadrao is null)
        {
            throw new InvalidOperationException("Perfil padrão para auto provisionamento AD não encontrado.");
        }

        var nomeUsuario = string.IsNullOrWhiteSpace(validacao.NomeCompleto)
            ? loginNormalizado
            : validacao.NomeCompleto!.Trim();
        var usuario = new Usuario(nomeUsuario, emailNormalizado, loginNormalizado, UsuarioTecnico);
        await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.UsuariosPerfisAcesso.AddAsync(
            new UsuarioPerfilAcesso(usuario.Id, perfilPadrao.Id, UsuarioTecnico),
            cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return usuario;
    }

    private static string NormalizarLogin(string? loginRetornado, string usuarioInformado)
    {
        var login = (loginRetornado ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(login))
        {
            login = usuarioInformado;
        }

        var idxBarra = login.LastIndexOf('\\');
        if (idxBarra >= 0 && idxBarra < login.Length - 1)
        {
            login = login[(idxBarra + 1)..];
        }

        var idxArroba = login.IndexOf('@');
        if (idxArroba > 0)
        {
            login = login[..idxArroba];
        }

        return login.Trim().ToLowerInvariant();
    }

    private static string? NormalizarEmail(string? email, string? upn)
    {
        var valor = (email ?? string.Empty).Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(valor))
        {
            return valor;
        }

        var upnNormalizado = (upn ?? string.Empty).Trim().ToLowerInvariant();
        return upnNormalizado.Contains('@', StringComparison.Ordinal) ? upnNormalizado : null;
    }

    private static LocalLoginResponse GerarRespostaLogin(Usuario usuario, AuthOptions options)
    {
        var agoraUtc = DateTime.UtcNow;
        var expiracao = agoraUtc.AddMinutes(options.JwtLocalExpiracaoMinutos <= 0 ? 120 : options.JwtLocalExpiracaoMinutos);
        var credenciais = new SigningCredentials(ObterChaveAssinatura(options), SecurityAlgorithms.HmacSha256);

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
            new("auth_provider", OrigemAutenticacaoActiveDirectory),
            new("autenticado_por", OrigemAutenticacaoActiveDirectory)
        };

        var token = new JwtSecurityToken(
            issuer: options.JwtLocalIssuer,
            audience: options.JwtLocalAudience,
            claims: claims,
            notBefore: agoraUtc,
            expires: expiracao,
            signingCredentials: credenciais);

        return new LocalLoginResponse(
            AccessToken: new JwtSecurityTokenHandler().WriteToken(token),
            TokenType: "Bearer",
            ExpiresIn: (int)Math.Floor((expiracao - agoraUtc).TotalSeconds),
            AutenticadoPor: OrigemAutenticacaoActiveDirectory,
            DeveAlterarSenha: usuario.DeveAlterarSenha);
    }

    private static SymmetricSecurityKey ObterChaveAssinatura(AuthOptions options)
    {
        var chaveTexto = (options.JwtLocalChaveAssinatura ?? string.Empty).Trim();
        if (chaveTexto.Length < 32)
        {
            throw new InvalidOperationException(
                "Authentication:JwtLocalChaveAssinatura deve possuir ao menos 32 caracteres para login AD.");
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveTexto));
    }

    private async Task RegistrarFalhaAsync(
        string descricao,
        string? usuarioLogin,
        CancellationToken cancellationToken,
        TipoEventoAutenticacao tipoEvento,
        string? detalheTecnico = null,
        Usuario? usuarioAlvo = null)
    {
        if (auditoriaService is not null)
        {
            try
            {
                await auditoriaService.RegistrarLoginAsync(
                    false,
                    descricao,
                    mensagemErro: MensagemErroGenerica,
                    usuarioLogin: usuarioLogin,
                    metadados: string.IsNullOrWhiteSpace(detalheTecnico)
                        ? null
                        : JsonSerializer.Serialize(new { detalheTecnico }),
                    cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falha ao registrar evento de auditoria legada de login Active Directory.");
            }
        }

        await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
            auditoriaService,
            logger,
            tipoEvento,
            tipoEvento == TipoEventoAutenticacao.UsuarioInativoBloqueado
                ? ResultadoEventoAutenticacao.Bloqueado
                : ResultadoEventoAutenticacao.Falha,
            "Falha de login Active Directory.",
            CodigoProvedorAutenticacao.ActiveDirectory,
            mensagemTecnica: MensagemErroGenerica,
            usuarioId: usuarioAlvo?.Id,
            usuarioNome: usuarioAlvo?.Nome,
            usuarioEmail: usuarioAlvo?.Email,
            usuarioLogin: usuarioAlvo?.Login ?? usuarioLogin,
            usuarioAlvoId: usuarioAlvo?.Id,
            usuarioAlvoEmail: usuarioAlvo?.Email,
            observacao: string.IsNullOrWhiteSpace(detalheTecnico) ? null : detalheTecnico,
            cancellationToken: cancellationToken);

        if (tipoEvento != TipoEventoAutenticacao.LoginActiveDirectoryNegado)
        {
            await AuditoriaAutenticacaoHelper.RegistrarEventoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.LoginActiveDirectoryNegado,
                tipoEvento == TipoEventoAutenticacao.UsuarioInativoBloqueado
                    ? ResultadoEventoAutenticacao.Bloqueado
                    : ResultadoEventoAutenticacao.Falha,
                "Login Active Directory negado.",
                CodigoProvedorAutenticacao.ActiveDirectory,
                mensagemTecnica: MensagemErroGenerica,
                usuarioId: usuarioAlvo?.Id,
                usuarioNome: usuarioAlvo?.Nome,
                usuarioEmail: usuarioAlvo?.Email,
                usuarioLogin: usuarioAlvo?.Login ?? usuarioLogin,
                usuarioAlvoId: usuarioAlvo?.Id,
                usuarioAlvoEmail: usuarioAlvo?.Email,
                observacao: string.IsNullOrWhiteSpace(detalheTecnico) ? null : detalheTecnico,
                cancellationToken: cancellationToken);
        }
    }
}
