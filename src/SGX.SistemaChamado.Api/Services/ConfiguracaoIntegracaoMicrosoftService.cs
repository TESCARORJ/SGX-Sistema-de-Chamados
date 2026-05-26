using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IConfiguracaoIntegracaoMicrosoftService
{
    Task<MicrosoftEntraIdIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default);
    Task<MicrosoftEntraIdIntegracaoResponse> AtualizarConfiguracaoAsync(
        AtualizarMicrosoftEntraIdIntegracaoRequest request,
        CancellationToken cancellationToken = default);
    Task<ProvedoresAutenticacaoResponse> ObterProvedoresAutenticacaoAsync(CancellationToken cancellationToken = default);
    Task<ConfiguracaoAutenticacaoEfetiva> ObterConfiguracaoAutenticacaoEfetivaAsync(CancellationToken cancellationToken = default);
}

public sealed record ConfiguracaoAutenticacaoEfetiva(
    bool MicrosoftHabilitado,
    bool LoginLocalSgxHabilitado,
    bool LoginLocalDevelopmentHabilitado,
    string ProvedorPrincipal,
    bool CriarUsuarioAutomaticamente,
    string PerfilPadraoUsuarioMicrosoft,
    IReadOnlyCollection<string> DominiosPermitidos,
    string TenantId,
    string ClientId,
    string Audience,
    string Issuer,
    string Authority,
    string ApiScope,
    string RedirectUri);

public sealed class ConfiguracaoIntegracaoMicrosoftService(
    SGXSistemaChamadoDbContext dbContext,
    IOptions<AuthOptions> authOptions,
    IOptions<AzureAdOptions> azureAdOptions,
    IHostEnvironment environment) : IConfiguracaoIntegracaoMicrosoftService
{
    private const string UsuarioTecnico = "admin.integracoes.microsoft";
    private const string MensagemSemProvedorAtivo = "Ao menos um provedor de autenticação deve permanecer habilitado.";
    private const string MensagemModoLocalSemLoginLocal = "Login local SGX deve permanecer habilitado quando o modo Local estiver selecionado.";

    private static class Chaves
    {
        public const string Habilitado = "auth.microsoft.habilitado";
        public const string ProvedorPrincipal = "auth.provedor_principal";
        public const string LoginLocalHabilitado = "auth.login_local_habilitado";
        public const string TenantId = "auth.microsoft.tenant_id";
        public const string ClientId = "auth.microsoft.client_id";
        public const string Audience = "auth.microsoft.audience";
        public const string Issuer = "auth.microsoft.issuer";
        public const string Authority = "auth.microsoft.authority";
        public const string ApiScope = "auth.microsoft.api_scope";
        public const string RedirectUri = "auth.microsoft.redirect_uri";
        public const string DominiosPermitidos = "auth.microsoft.dominios_permitidos";
        public const string CriarUsuarioAutomaticamente = "auth.microsoft.criar_usuario_automaticamente";
        public const string PerfilPadraoUsuarioMicrosoft = "auth.microsoft.perfil_padrao_usuario";
    }

    public async Task<MicrosoftEntraIdIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default)
    {
        var configuracao = await ObterConfiguracaoAutenticacaoEfetivaAsync(cancellationToken);
        var pendencias = CalcularPendencias(configuracao);
        var status = ObterStatusConfiguracao(configuracao, pendencias);

        return new MicrosoftEntraIdIntegracaoResponse(
            configuracao.MicrosoftHabilitado,
            configuracao.ProvedorPrincipal,
            configuracao.LoginLocalSgxHabilitado,
            configuracao.TenantId,
            configuracao.ClientId,
            configuracao.Audience,
            configuracao.Issuer,
            configuracao.Authority,
            configuracao.ApiScope,
            configuracao.RedirectUri,
            configuracao.DominiosPermitidos,
            configuracao.CriarUsuarioAutomaticamente,
            configuracao.PerfilPadraoUsuarioMicrosoft,
            status,
            pendencias);
    }

    public async Task<MicrosoftEntraIdIntegracaoResponse> AtualizarConfiguracaoAsync(
        AtualizarMicrosoftEntraIdIntegracaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentException("Payload inválido para atualização da integração Microsoft Entra ID.");
        }

        var provedorPrincipal = NormalizarProvedorPrincipal(request.ProvedorPrincipal);
        var loginLocalHabilitado = request.LoginLocalHabilitado;
        var usaMicrosoftNoFluxo = UsaMicrosoftNoFluxo(provedorPrincipal);
        var usaLocalNoFluxo = UsaLocalNoFluxo(provedorPrincipal);
        var microsoftNoFluxo = request.Habilitado && usaMicrosoftNoFluxo;
        var localNoFluxo = loginLocalHabilitado && usaLocalNoFluxo;

        if (string.Equals(provedorPrincipal, ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase)
            && !loginLocalHabilitado)
        {
            throw new InvalidOperationException(MensagemModoLocalSemLoginLocal);
        }

        if (!microsoftNoFluxo && !localNoFluxo)
        {
            throw new InvalidOperationException(MensagemSemProvedorAtivo);
        }

        if (microsoftNoFluxo)
        {
            ValidarObrigatorio(request.TenantId, "Tenant ID é obrigatório quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.ClientId, "Client ID é obrigatório quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.Audience, "Audience é obrigatória quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.Issuer, "Issuer é obrigatório quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.Authority, "Authority é obrigatória quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.ApiScope, "API Scope é obrigatório quando a integração Microsoft está habilitada.");
            ValidarObrigatorio(request.RedirectUri, "Redirect URI é obrigatório quando a integração Microsoft está habilitada.");
        }

        await UpsertParametroAsync(Chaves.Habilitado, request.Habilitado ? "true" : "false", false, "Integração Microsoft Entra ID habilitada.", cancellationToken);
        await UpsertParametroAsync(Chaves.ProvedorPrincipal, provedorPrincipal, false, "Provedor principal de autenticação.", cancellationToken);
        await UpsertParametroAsync(Chaves.LoginLocalHabilitado, loginLocalHabilitado ? "true" : "false", false, "Login local SGX habilitado.", cancellationToken);
        await UpsertParametroAsync(Chaves.TenantId, request.TenantId, false, "Tenant ID do Microsoft Entra ID.", cancellationToken);
        await UpsertParametroAsync(Chaves.ClientId, request.ClientId, false, "Client ID do Microsoft Entra ID.", cancellationToken);
        await UpsertParametroAsync(Chaves.Audience, request.Audience, false, "Audience da API para tokens Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.Issuer, request.Issuer, false, "Issuer aceito para tokens Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.Authority, request.Authority, false, "Authority configurada para autenticação Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.ApiScope, request.ApiScope, false, "Escopo da API para frontend Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.RedirectUri, request.RedirectUri, false, "Redirect URI do frontend Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.DominiosPermitidos, string.Join(';', request.DominiosPermitidos ?? []), false, "Domínios permitidos para acesso Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.CriarUsuarioAutomaticamente, request.CriarUsuarioAutomaticamente ? "true" : "false", false, "Criação automática de usuário Microsoft.", cancellationToken);
        await UpsertParametroAsync(Chaves.PerfilPadraoUsuarioMicrosoft, request.PerfilPadraoUsuarioMicrosoft, false, "Perfil padrão para usuário criado via Microsoft.", cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await ObterConfiguracaoAsync(cancellationToken);
    }

    public async Task<ProvedoresAutenticacaoResponse> ObterProvedoresAutenticacaoAsync(CancellationToken cancellationToken = default)
    {
        var configuracao = await ObterConfiguracaoAutenticacaoEfetivaAsync(cancellationToken);
        var auth = authOptions.Value;

        var configurados = auth.ObterCodigosProvedoresConfiguradosNormalizados();
        var habilitados = auth.ObterCodigosProvedoresHabilitadosNormalizados();
        var principal = auth.ObterCodigoProvedorPrincipalNormalizado();
        var possuiHabilitadosExplicitos = auth.PossuiConfiguracaoExplicitaProvedoresHabilitados();

        var microsoftConfigurado = configurados.Contains(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparer.OrdinalIgnoreCase)
            || UsaMicrosoftNoFluxo(configuracao.ProvedorPrincipal);
        var localSgxConfigurado = configurados.Contains(CodigoProvedorAutenticacao.LocalSgx, StringComparer.OrdinalIgnoreCase)
            || UsaLocalNoFluxo(configuracao.ProvedorPrincipal);
        var activeDirectoryConfigurado = configurados.Contains(CodigoProvedorAutenticacao.ActiveDirectory, StringComparer.OrdinalIgnoreCase);
        var localDevelopmentConfigurado = configurados.Contains(CodigoProvedorAutenticacao.LocalDevelopment, StringComparer.OrdinalIgnoreCase)
            || auth.ModoLocalHabilitado;

        var microsoftHabilitado = microsoftConfigurado
            && (possuiHabilitadosExplicitos
                ? habilitados.Contains(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparer.OrdinalIgnoreCase)
                : configuracao.MicrosoftHabilitado)
            && configuracao.MicrosoftHabilitado;

        var localSgxHabilitado = localSgxConfigurado
            && (possuiHabilitadosExplicitos
                ? habilitados.Contains(CodigoProvedorAutenticacao.LocalSgx, StringComparer.OrdinalIgnoreCase)
                : configuracao.LoginLocalSgxHabilitado)
            && configuracao.LoginLocalSgxHabilitado;

        var activeDirectoryHabilitado = activeDirectoryConfigurado
            && habilitados.Contains(CodigoProvedorAutenticacao.ActiveDirectory, StringComparer.OrdinalIgnoreCase);

        var localDevelopmentHabilitado = localDevelopmentConfigurado
            && environment.IsDevelopment()
            && (possuiHabilitadosExplicitos
                ? habilitados.Contains(CodigoProvedorAutenticacao.LocalDevelopment, StringComparer.OrdinalIgnoreCase)
                : configuracao.LoginLocalDevelopmentHabilitado);

        var itens = new List<ProvedorAutenticacaoDto>();

        AdicionarSeHabilitado(
            itens,
            CodigoProvedorAutenticacao.MicrosoftEntraId,
            "Microsoft Entra ID",
            "Login corporativo federado pelo Microsoft Entra ID.",
            microsoftHabilitado,
            auth.ObterOrdemProvedor(CodigoProvedorAutenticacao.MicrosoftEntraId, 10));

        AdicionarSeHabilitado(
            itens,
            CodigoProvedorAutenticacao.ActiveDirectory,
            "Active Directory",
            "Login corporativo integrado ao Active Directory do cliente.",
            activeDirectoryHabilitado,
            auth.ObterOrdemProvedor(CodigoProvedorAutenticacao.ActiveDirectory, 20));

        AdicionarSeHabilitado(
            itens,
            CodigoProvedorAutenticacao.LocalSgx,
            "Local SGX",
            "Login local SGX com e-mail corporativo e senha.",
            localSgxHabilitado,
            auth.ObterOrdemProvedor(CodigoProvedorAutenticacao.LocalSgx, 30));

        AdicionarSeHabilitado(
            itens,
            CodigoProvedorAutenticacao.LocalDevelopment,
            "Local Development",
            "Login técnico de desenvolvimento exclusivo para ambiente Development.",
            localDevelopmentHabilitado,
            auth.ObterOrdemProvedor(CodigoProvedorAutenticacao.LocalDevelopment, 40));

        if (itens.Count == 0)
        {
            return new ProvedoresAutenticacaoResponse([]);
        }

        var principalEfetivo = itens.Any(x => string.Equals(x.Codigo, principal, StringComparison.OrdinalIgnoreCase))
            ? principal
            : itens.OrderBy(x => x.Ordem).First().Codigo;

        var provedores = itens
            .OrderBy(x => x.Ordem)
            .Select(x => x with
            {
                Principal = string.Equals(x.Codigo, principalEfetivo, StringComparison.OrdinalIgnoreCase)
            })
            .ToArray();

        return new ProvedoresAutenticacaoResponse(provedores);
    }

    public async Task<ConfiguracaoAutenticacaoEfetiva> ObterConfiguracaoAutenticacaoEfetivaAsync(CancellationToken cancellationToken = default)
    {
        var parametros = await CarregarParametrosAtivosAsync(cancellationToken);
        var auth = authOptions.Value;
        var azure = azureAdOptions.Value;
        var provedoresHabilitados = auth.ObterCodigosProvedoresHabilitadosNormalizados();
        var possuiHabilitadosExplicitos = auth.PossuiConfiguracaoExplicitaProvedoresHabilitados();

        var provedorPrincipal = NormalizarProvedorPrincipal(
            ObterValor(parametros, Chaves.ProvedorPrincipal) ?? auth.ObterProvedorPrincipalNormalizado());

        var loginLocalHabilitado = ObterBoolean(parametros, Chaves.LoginLocalHabilitado) ?? auth.LoginLocalHabilitado;
        var tenantId = ObterValor(parametros, Chaves.TenantId) ?? (azure.TenantId ?? string.Empty);
        var clientId = ObterValor(parametros, Chaves.ClientId) ?? (azure.ClientId ?? string.Empty);
        var audience = ObterValor(parametros, Chaves.Audience) ?? (azure.Audience ?? string.Empty);
        var issuer = ObterValor(parametros, Chaves.Issuer) ?? (azure.Issuer ?? string.Empty);
        var authority = ObterValor(parametros, Chaves.Authority) ?? azure.BuildAuthority();
        var apiScope = ObterValor(parametros, Chaves.ApiScope) ?? string.Empty;
        var redirectUri = ObterValor(parametros, Chaves.RedirectUri) ?? string.Empty;
        var dominiosPermitidos = ObterDominios(parametros, auth);
        var criarUsuarioAutomaticamente = ObterBoolean(parametros, Chaves.CriarUsuarioAutomaticamente) ?? auth.CriarUsuarioAutomaticamente;
        var perfilPadraoUsuarioMicrosoft = ObterValor(parametros, Chaves.PerfilPadraoUsuarioMicrosoft) ?? auth.PerfilPadraoUsuarioMicrosoft;

        var habilitadoBruto = ObterBoolean(parametros, Chaves.Habilitado);
        var microsoftNoFluxo = UsaMicrosoftNoFluxo(provedorPrincipal)
            || (possuiHabilitadosExplicitos
                && provedoresHabilitados.Contains(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparer.OrdinalIgnoreCase));
        var localNoFluxo = UsaLocalNoFluxo(provedorPrincipal)
            || (possuiHabilitadosExplicitos
                && provedoresHabilitados.Contains(CodigoProvedorAutenticacao.LocalSgx, StringComparer.OrdinalIgnoreCase));
        var microsoftHabilitadoBase = habilitadoBruto
            ?? (possuiHabilitadosExplicitos
                ? provedoresHabilitados.Contains(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparer.OrdinalIgnoreCase)
                : auth.UsaMicrosoftComoPrincipalOuHibrido());
        var microsoftConfigurado = !string.IsNullOrWhiteSpace(tenantId)
            && !string.IsNullOrWhiteSpace(clientId)
            && !string.IsNullOrWhiteSpace(audience)
            && !string.IsNullOrWhiteSpace(issuer)
            && !string.IsNullOrWhiteSpace(authority)
            && !string.IsNullOrWhiteSpace(apiScope)
            && !string.IsNullOrWhiteSpace(redirectUri);

        var microsoftHabilitado = microsoftHabilitadoBase && microsoftNoFluxo && microsoftConfigurado;
        var localSgxHabilitado = loginLocalHabilitado
            && localNoFluxo
            && (!possuiHabilitadosExplicitos
                || provedoresHabilitados.Contains(CodigoProvedorAutenticacao.LocalSgx, StringComparer.OrdinalIgnoreCase));
        var localDevelopmentHabilitado = environment.IsDevelopment()
            && auth.ModoLocalHabilitado
            && (!possuiHabilitadosExplicitos
                || provedoresHabilitados.Contains(CodigoProvedorAutenticacao.LocalDevelopment, StringComparer.OrdinalIgnoreCase));

        return new ConfiguracaoAutenticacaoEfetiva(
            MicrosoftHabilitado: microsoftHabilitado,
            LoginLocalSgxHabilitado: localSgxHabilitado,
            LoginLocalDevelopmentHabilitado: localDevelopmentHabilitado,
            ProvedorPrincipal: provedorPrincipal,
            CriarUsuarioAutomaticamente: criarUsuarioAutomaticamente,
            PerfilPadraoUsuarioMicrosoft: (perfilPadraoUsuarioMicrosoft ?? string.Empty).Trim(),
            DominiosPermitidos: dominiosPermitidos,
            TenantId: (tenantId ?? string.Empty).Trim(),
            ClientId: (clientId ?? string.Empty).Trim(),
            Audience: (audience ?? string.Empty).Trim(),
            Issuer: (issuer ?? string.Empty).Trim(),
            Authority: (authority ?? string.Empty).Trim(),
            ApiScope: (apiScope ?? string.Empty).Trim(),
            RedirectUri: (redirectUri ?? string.Empty).Trim());
    }

    private static IReadOnlyCollection<string> CalcularPendencias(ConfiguracaoAutenticacaoEfetiva configuracao)
    {
        var pendencias = new List<string>();

        if (UsaMicrosoftNoFluxo(configuracao.ProvedorPrincipal))
        {
            if (string.IsNullOrWhiteSpace(configuracao.TenantId))
            {
                pendencias.Add("Tenant ID não configurado.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.ClientId))
            {
                pendencias.Add("Client ID não configurado.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.Audience))
            {
                pendencias.Add("Audience não configurada.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.Issuer))
            {
                pendencias.Add("Issuer não configurado.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.Authority))
            {
                pendencias.Add("Authority não configurada.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.ApiScope))
            {
                pendencias.Add("API Scope não configurado.");
            }

            if (string.IsNullOrWhiteSpace(configuracao.RedirectUri))
            {
                pendencias.Add("Redirect URI não configurada.");
            }
        }

        if (!configuracao.MicrosoftHabilitado && !configuracao.LoginLocalSgxHabilitado)
        {
            pendencias.Add("Nenhum provedor ativo para autenticação de produção.");
        }

        return pendencias;
    }

    private static string ObterStatusConfiguracao(ConfiguracaoAutenticacaoEfetiva configuracao, IReadOnlyCollection<string> pendencias)
    {
        if (!UsaMicrosoftNoFluxo(configuracao.ProvedorPrincipal))
        {
            return "Desabilitado";
        }

        return pendencias.Count == 0 ? "Configurado" : "PendenteConfiguracao";
    }

    private async Task<Dictionary<string, ParametroSistema>> CarregarParametrosAtivosAsync(CancellationToken cancellationToken)
    {
        var chaves = new[]
        {
            Chaves.Habilitado,
            Chaves.ProvedorPrincipal,
            Chaves.LoginLocalHabilitado,
            Chaves.TenantId,
            Chaves.ClientId,
            Chaves.Audience,
            Chaves.Issuer,
            Chaves.Authority,
            Chaves.ApiScope,
            Chaves.RedirectUri,
            Chaves.DominiosPermitidos,
            Chaves.CriarUsuarioAutomaticamente,
            Chaves.PerfilPadraoUsuarioMicrosoft
        };

        var itens = await dbContext.ParametrosSistema
            .Where(x => x.Ativo && chaves.Contains(x.Chave))
            .ToListAsync(cancellationToken);

        return itens.ToDictionary(x => x.Chave, x => x, StringComparer.OrdinalIgnoreCase);
    }

    private async Task UpsertParametroAsync(
        string chave,
        string valor,
        bool sensivel,
        string descricao,
        CancellationToken cancellationToken)
    {
        var parametro = await dbContext.ParametrosSistema
            .FirstOrDefaultAsync(x => x.Chave == chave, cancellationToken);

        if (string.IsNullOrWhiteSpace(valor))
        {
            if (parametro is not null && parametro.Ativo)
            {
                parametro.Desativar(UsuarioTecnico);
                parametro.DefinirDescricao(descricao, UsuarioTecnico);
                parametro.DefinirSensivel(sensivel, UsuarioTecnico);
            }

            return;
        }

        if (parametro is null)
        {
            await dbContext.ParametrosSistema.AddAsync(
                new ParametroSistema(chave, valor, descricao, sensivel, UsuarioTecnico),
                cancellationToken);
            return;
        }

        parametro.DefinirChave(chave);
        parametro.Ativar(UsuarioTecnico);
        parametro.AtualizarValor(valor, UsuarioTecnico);
        parametro.DefinirDescricao(descricao, UsuarioTecnico);
        parametro.DefinirSensivel(sensivel, UsuarioTecnico);
    }

    private static void AdicionarSeHabilitado(
        ICollection<ProvedorAutenticacaoDto> itens,
        string codigo,
        string nome,
        string descricao,
        bool habilitado,
        int ordem)
    {
        if (!habilitado)
        {
            return;
        }

        itens.Add(new ProvedorAutenticacaoDto(
            Codigo: codigo,
            Nome: nome,
            Descricao: descricao,
            Habilitado: true,
            Principal: false,
            Ordem: ordem));
    }

    private static void ValidarObrigatorio(string valor, string mensagem)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new InvalidOperationException(mensagem);
        }
    }

    private static bool UsaMicrosoftNoFluxo(string provedorPrincipal)
    {
        return string.Equals(provedorPrincipal, ProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(provedorPrincipal, ProvedorAutenticacao.Hibrido, StringComparison.OrdinalIgnoreCase);
    }

    private static bool UsaLocalNoFluxo(string provedorPrincipal)
    {
        return string.Equals(provedorPrincipal, ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase)
            || string.Equals(provedorPrincipal, ProvedorAutenticacao.Hibrido, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizarProvedorPrincipal(string? provedorPrincipal)
    {
        var valor = (provedorPrincipal ?? string.Empty).Trim();
        if (valor.Equals(ProvedorAutenticacao.Local, StringComparison.OrdinalIgnoreCase))
        {
            return ProvedorAutenticacao.Local;
        }

        if (valor.Equals(ProvedorAutenticacao.Hibrido, StringComparison.OrdinalIgnoreCase))
        {
            return ProvedorAutenticacao.Hibrido;
        }

        return ProvedorAutenticacao.MicrosoftEntraId;
    }

    private static string? ObterValor(IReadOnlyDictionary<string, ParametroSistema> parametros, string chave)
    {
        if (!parametros.TryGetValue(chave, out var parametro))
        {
            return null;
        }

        var valor = (parametro.Valor ?? string.Empty).Trim();
        return string.IsNullOrWhiteSpace(valor) ? null : valor;
    }

    private static bool? ObterBoolean(IReadOnlyDictionary<string, ParametroSistema> parametros, string chave)
    {
        if (!parametros.TryGetValue(chave, out var parametro))
        {
            return null;
        }

        var valor = (parametro.Valor ?? string.Empty).Trim();
        if (bool.TryParse(valor, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static IReadOnlyCollection<string> ObterDominios(
        IReadOnlyDictionary<string, ParametroSistema> parametros,
        AuthOptions authOptions)
    {
        var bruto = ObterValor(parametros, Chaves.DominiosPermitidos);
        if (string.IsNullOrWhiteSpace(bruto))
        {
            return authOptions.DominiosPermitidos
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        return bruto
            .Split([';', ',', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}


