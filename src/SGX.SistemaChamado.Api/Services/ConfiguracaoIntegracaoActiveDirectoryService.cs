using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Infrastructure.Persistence;

namespace SGX.SistemaChamado.Api.Services;

public interface IConfiguracaoIntegracaoActiveDirectoryService
{
    Task<ActiveDirectoryIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default);
    Task<ActiveDirectoryIntegracaoResponse> AtualizarConfiguracaoAsync(
        AtualizarActiveDirectoryIntegracaoRequest request,
        CancellationToken cancellationToken = default);
    Task<TestarConexaoActiveDirectoryResponse> TestarConexaoAsync(
        TestarConexaoActiveDirectoryRequest? request,
        CancellationToken cancellationToken = default);
    Task<TestarAutenticacaoActiveDirectoryResponse> TestarAutenticacaoAsync(
        TestarAutenticacaoActiveDirectoryRequest request,
        CancellationToken cancellationToken = default);
    Task<ActiveDirectoryOptions> ObterConfiguracaoEfetivaAsync(CancellationToken cancellationToken = default);
}

public interface IActiveDirectoryConnectivityTester
{
    Task<(bool Sucesso, string Mensagem)> TestarConexaoTcpAsync(
        ActiveDirectoryOptions options,
        CancellationToken cancellationToken = default);
}

public sealed class ActiveDirectoryConnectivityTester : IActiveDirectoryConnectivityTester
{
    public async Task<(bool Sucesso, string Mensagem)> TestarConexaoTcpAsync(
        ActiveDirectoryOptions options,
        CancellationToken cancellationToken = default)
    {
        var host = ExtrairHost(options.Servidor);
        if (string.IsNullOrWhiteSpace(host))
        {
            return (false, "Servidor LDAP/LDAPS invalido.");
        }

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(options.TimeoutConexaoSegundos));

        try
        {
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(host, options.Porta, timeoutCts.Token);
            return (true, $"Conexao TCP estabelecida com {host}:{options.Porta}.");
        }
        catch (OperationCanceledException)
        {
            return (false, "Timeout ao tentar conectar no servidor Active Directory/LDAP.");
        }
        catch (Exception ex)
        {
            return (false, $"Falha ao conectar no servidor Active Directory/LDAP: {ex.GetType().Name}.");
        }
    }

    private static string ExtrairHost(string servidor)
    {
        var valor = (servidor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        if (Uri.TryCreate(valor, UriKind.Absolute, out var uri))
        {
            return uri.Host;
        }

        var idx = valor.IndexOf("://", StringComparison.Ordinal);
        if (idx > -1 && idx < valor.Length - 3)
        {
            valor = valor[(idx + 3)..];
        }

        var host = valor.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        return host ?? string.Empty;
    }
}

public sealed class ConfiguracaoIntegracaoActiveDirectoryService(
    SGXSistemaChamadoDbContext dbContext,
    IOptions<ActiveDirectoryOptions> activeDirectoryOptions,
    IActiveDirectoryConnectivityTester connectivityTester,
    IActiveDirectoryCredentialValidator credentialValidator) : IConfiguracaoIntegracaoActiveDirectoryService
{
    private const string UsuarioTecnico = "admin.integracoes.active-directory";

    private static class Chaves
    {
        public const string Ativo = "auth.active_directory.ativo";
        public const string Servidor = "auth.active_directory.servidor";
        public const string Porta = "auth.active_directory.porta";
        public const string UsarLdaps = "auth.active_directory.usar_ldaps";
        public const string PermitirLdapSemTls = "auth.active_directory.permitir_ldap_sem_tls";
        public const string Dominio = "auth.active_directory.dominio";
        public const string BaseDn = "auth.active_directory.base_dn";
        public const string UserSearchFilter = "auth.active_directory.user_search_filter";
        public const string PermitirAutoProvisionamento = "auth.active_directory.auto_provisionamento";
        public const string PerfilPadrao = "auth.active_directory.perfil_padrao";
        public const string TimeoutConexaoSegundos = "auth.active_directory.timeout_conexao_segundos";
    }

    public async Task<ActiveDirectoryIntegracaoResponse> ObterConfiguracaoAsync(CancellationToken cancellationToken = default)
    {
        var efetiva = await ObterConfiguracaoEfetivaAsync(cancellationToken);
        return MapearResposta(efetiva);
    }

    public async Task<ActiveDirectoryIntegracaoResponse> AtualizarConfiguracaoAsync(
        AtualizarActiveDirectoryIntegracaoRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentException("Payload invalido para atualizacao da integracao Active Directory/LDAP.");
        }

        var opcoes = NormalizarParaOpcoes(
            request.Ativo,
            request.Servidor,
            request.Porta,
            request.UsarLdaps,
            request.PermitirLdapSemTls,
            request.Dominio,
            request.BaseDn,
            request.UserSearchFilter,
            request.PermitirAutoProvisionamento,
            request.PerfilPadrao,
            request.TimeoutConexaoSegundos);

        ValidarConfiguracao(opcoes, request.ConfirmacaoPermitirLdapSemTls, strict: true);

        await UpsertParametroAsync(Chaves.Ativo, opcoes.Ativo ? "true" : "false", false, "Active Directory tecnico ativo.", cancellationToken);
        await UpsertParametroAsync(Chaves.Servidor, opcoes.Servidor, false, "Servidor LDAP/LDAPS.", cancellationToken);
        await UpsertParametroAsync(Chaves.Porta, opcoes.Porta.ToString(), false, "Porta LDAP/LDAPS.", cancellationToken);
        await UpsertParametroAsync(Chaves.UsarLdaps, opcoes.UsarLdaps ? "true" : "false", false, "Uso de LDAPS.", cancellationToken);
        await UpsertParametroAsync(Chaves.PermitirLdapSemTls, opcoes.PermitirLdapSemTls ? "true" : "false", false, "Permite LDAP sem TLS.", cancellationToken);
        await UpsertParametroAsync(Chaves.Dominio, opcoes.Dominio, false, "Dominio AD para bind.", cancellationToken);
        await UpsertParametroAsync(Chaves.BaseDn, opcoes.BaseDn, false, "Base DN de busca AD.", cancellationToken);
        await UpsertParametroAsync(Chaves.UserSearchFilter, opcoes.UserSearchFilter, false, "Filtro LDAP para busca de usuario.", cancellationToken);
        await UpsertParametroAsync(Chaves.PermitirAutoProvisionamento, opcoes.PermitirAutoProvisionamento ? "true" : "false", false, "Permite auto provisionamento AD.", cancellationToken);
        await UpsertParametroAsync(Chaves.PerfilPadrao, opcoes.PerfilPadrao, false, "Perfil padrao para auto provisionamento AD.", cancellationToken);
        await UpsertParametroAsync(Chaves.TimeoutConexaoSegundos, opcoes.TimeoutConexaoSegundos.ToString(), false, "Timeout de conexao AD em segundos.", cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await ObterConfiguracaoAsync(cancellationToken);
    }

    public async Task<TestarConexaoActiveDirectoryResponse> TestarConexaoAsync(
        TestarConexaoActiveDirectoryRequest? request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var opcoes = request is null
            ? await ObterConfiguracaoEfetivaAsync(cancellationToken)
            : NormalizarParaOpcoes(
                request.Ativo,
                request.Servidor,
                request.Porta,
                request.UsarLdaps,
                request.PermitirLdapSemTls,
                request.Dominio,
                request.BaseDn,
                request.UserSearchFilter,
                request.PermitirAutoProvisionamento,
                request.PerfilPadrao,
                request.TimeoutConexaoSegundos);

        var pendencias = ValidarConfiguracao(opcoes, request?.ConfirmacaoPermitirLdapSemTls ?? false, strict: false);
        if (pendencias.Count > 0)
        {
            stopwatch.Stop();
            return new TestarConexaoActiveDirectoryResponse(
                Sucesso: false,
                Mensagem: string.Join(" ", pendencias),
                DuracaoMs: stopwatch.ElapsedMilliseconds);
        }

        var resultado = await connectivityTester.TestarConexaoTcpAsync(opcoes, cancellationToken);
        stopwatch.Stop();
        return new TestarConexaoActiveDirectoryResponse(resultado.Sucesso, resultado.Mensagem, stopwatch.ElapsedMilliseconds);
    }

    public async Task<TestarAutenticacaoActiveDirectoryResponse> TestarAutenticacaoAsync(
        TestarAutenticacaoActiveDirectoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            throw new ArgumentException("Payload invalido para teste de autenticacao Active Directory.");
        }

        var usuario = (request.Usuario ?? string.Empty).Trim();
        var senha = request.Senha ?? string.Empty;
        if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException("Usuario e senha sao obrigatorios para teste de autenticacao controlada.");
        }

        var stopwatch = Stopwatch.StartNew();
        var opcoes = NormalizarParaOpcoes(
            request.Ativo,
            request.Servidor,
            request.Porta,
            request.UsarLdaps,
            request.PermitirLdapSemTls,
            request.Dominio,
            request.BaseDn,
            request.UserSearchFilter,
            permitirAutoProvisionamento: false,
            perfilPadrao: PerfisInternos.Solicitante,
            request.TimeoutConexaoSegundos);

        var pendencias = ValidarConfiguracao(opcoes, request.ConfirmacaoPermitirLdapSemTls, strict: false);
        if (pendencias.Count > 0)
        {
            stopwatch.Stop();
            return new TestarAutenticacaoActiveDirectoryResponse(
                Sucesso: false,
                Mensagem: string.Join(" ", pendencias),
                UsuarioSamAccountName: null,
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DuracaoMs: stopwatch.ElapsedMilliseconds);
        }

        var dominio = string.IsNullOrWhiteSpace(request.Dominio) ? opcoes.Dominio : request.Dominio.Trim();
        var validacao = await credentialValidator.ValidarCredenciaisAsync(
            usuario,
            senha,
            dominio,
            opcoes,
            cancellationToken);

        stopwatch.Stop();
        if (!validacao.Sucesso)
        {
            return new TestarAutenticacaoActiveDirectoryResponse(
                Sucesso: false,
                Mensagem: "Falha no teste de autenticacao controlada.",
                UsuarioSamAccountName: null,
                NomeCompleto: null,
                Email: null,
                UserPrincipalName: null,
                DuracaoMs: stopwatch.ElapsedMilliseconds);
        }

        return new TestarAutenticacaoActiveDirectoryResponse(
            Sucesso: true,
            Mensagem: "Autenticacao controlada concluida com sucesso.",
            UsuarioSamAccountName: validacao.UsuarioSamAccountName,
            NomeCompleto: validacao.NomeCompleto,
            Email: validacao.Email,
            UserPrincipalName: validacao.UserPrincipalName,
            DuracaoMs: stopwatch.ElapsedMilliseconds);
    }

    public async Task<ActiveDirectoryOptions> ObterConfiguracaoEfetivaAsync(CancellationToken cancellationToken = default)
    {
        var parametros = await CarregarParametrosAtivosAsync(cancellationToken);
        var fallback = activeDirectoryOptions.Value;

        return NormalizarParaOpcoes(
            ativo: ObterBoolean(parametros, Chaves.Ativo) ?? fallback.Ativo,
            servidor: ObterValor(parametros, Chaves.Servidor) ?? fallback.Servidor,
            porta: ObterInt(parametros, Chaves.Porta) ?? fallback.Porta,
            usarLdaps: ObterBoolean(parametros, Chaves.UsarLdaps) ?? fallback.UsarLdaps,
            permitirLdapSemTls: ObterBoolean(parametros, Chaves.PermitirLdapSemTls) ?? fallback.PermitirLdapSemTls,
            dominio: ObterValor(parametros, Chaves.Dominio) ?? fallback.Dominio,
            baseDn: ObterValor(parametros, Chaves.BaseDn) ?? fallback.BaseDn,
            userSearchFilter: ObterValor(parametros, Chaves.UserSearchFilter) ?? fallback.UserSearchFilter,
            permitirAutoProvisionamento: ObterBoolean(parametros, Chaves.PermitirAutoProvisionamento) ?? fallback.PermitirAutoProvisionamento,
            perfilPadrao: ObterValor(parametros, Chaves.PerfilPadrao) ?? fallback.PerfilPadrao,
            timeoutConexaoSegundos: ObterInt(parametros, Chaves.TimeoutConexaoSegundos) ?? fallback.TimeoutConexaoSegundos);
    }

    private static ActiveDirectoryIntegracaoResponse MapearResposta(ActiveDirectoryOptions efetiva)
    {
        var pendencias = ValidarConfiguracao(efetiva, confirmacaoPermitirLdapSemTls: efetiva.PermitirLdapSemTls, strict: false);
        var avisos = CalcularAvisosSeguranca(efetiva);
        var tecnicamenteConfigurado = efetiva.Ativo && pendencias.Count == 0;
        var status = !efetiva.Ativo
            ? "Inativo"
            : tecnicamenteConfigurado
                ? "Configurado"
                : "ConfiguracaoInvalida";

        return new ActiveDirectoryIntegracaoResponse(
            Ativo: efetiva.Ativo,
            Servidor: efetiva.Servidor,
            Porta: efetiva.Porta,
            UsarLdaps: efetiva.UsarLdaps,
            PermitirLdapSemTls: efetiva.PermitirLdapSemTls,
            Dominio: efetiva.Dominio,
            BaseDn: efetiva.BaseDn,
            UserSearchFilter: efetiva.UserSearchFilter,
            PermitirAutoProvisionamento: efetiva.PermitirAutoProvisionamento,
            PerfilPadrao: efetiva.PerfilPadrao,
            TimeoutConexaoSegundos: efetiva.TimeoutConexaoSegundos,
            TecnicamenteConfigurado: tecnicamenteConfigurado,
            StatusConfiguracao: status,
            PendenciasConfiguracao: pendencias,
            AvisosSeguranca: avisos);
    }

    private static List<string> ValidarConfiguracao(
        ActiveDirectoryOptions options,
        bool confirmacaoPermitirLdapSemTls,
        bool strict)
    {
        var erros = new List<string>();

        if (!options.Ativo)
        {
            return erros;
        }

        var servidor = (options.Servidor ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(servidor))
        {
            erros.Add("Servidor LDAP/LDAPS nao informado.");
        }
        else if (Uri.TryCreate(servidor, UriKind.Absolute, out var uri))
        {
            if (!string.Equals(uri.Scheme, "ldap", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(uri.Scheme, "ldaps", StringComparison.OrdinalIgnoreCase))
            {
                erros.Add("Servidor deve usar esquema ldap:// ou ldaps://.");
            }
        }
        else if (servidor.Contains("://", StringComparison.Ordinal))
        {
            erros.Add("Servidor LDAP/LDAPS invalido.");
        }

        if (options.Porta <= 0 || options.Porta > 65535)
        {
            erros.Add("Porta LDAP/LDAPS deve estar entre 1 e 65535.");
        }

        if (options.TimeoutConexaoSegundos <= 0 || options.TimeoutConexaoSegundos > 120)
        {
            erros.Add("Timeout de conexao deve estar entre 1 e 120 segundos.");
        }

        if (!options.UsarLdaps && !options.PermitirLdapSemTls)
        {
            erros.Add("LDAP sem TLS exige permissao explicita.");
        }

        if (!options.UsarLdaps && options.PermitirLdapSemTls && strict && !confirmacaoPermitirLdapSemTls)
        {
            erros.Add("Confirmacao explicita obrigatoria para habilitar LDAP sem TLS.");
        }

        if (string.IsNullOrWhiteSpace(options.BaseDn))
        {
            erros.Add("Base DN nao configurada.");
        }

        var filtro = (options.UserSearchFilter ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(filtro))
        {
            erros.Add("Filtro de busca do usuario nao configurado.");
        }
        else if (!filtro.Contains("{0}", StringComparison.Ordinal))
        {
            erros.Add("Filtro de busca do usuario deve conter o placeholder {0}.");
        }

        if (options.PermitirAutoProvisionamento)
        {
            var perfilPadrao = string.IsNullOrWhiteSpace(options.PerfilPadrao)
                ? PerfisInternos.Solicitante
                : options.PerfilPadrao.Trim();

            if (!PerfisInternos.EhPerfilValido(perfilPadrao))
            {
                erros.Add("Perfil padrao para auto provisionamento AD invalido.");
            }

            if (string.Equals(perfilPadrao, PerfisInternos.Administrador, StringComparison.OrdinalIgnoreCase))
            {
                erros.Add("Auto provisionamento AD nao pode conceder perfil Administrador.");
            }
        }

        return erros;
    }

    private static IReadOnlyCollection<string> CalcularAvisosSeguranca(ActiveDirectoryOptions options)
    {
        var avisos = new List<string>();

        if (!options.UsarLdaps)
        {
            avisos.Add("Prefira LDAPS em homologacao e producao.");
        }

        if (options.PermitirLdapSemTls)
        {
            avisos.Add("LDAP sem TLS aumenta risco de exposicao de credenciais em trafego de rede.");
        }

        return avisos;
    }

    private async Task<Dictionary<string, ParametroSistema>> CarregarParametrosAtivosAsync(CancellationToken cancellationToken)
    {
        var chaves = new[]
        {
            Chaves.Ativo,
            Chaves.Servidor,
            Chaves.Porta,
            Chaves.UsarLdaps,
            Chaves.PermitirLdapSemTls,
            Chaves.Dominio,
            Chaves.BaseDn,
            Chaves.UserSearchFilter,
            Chaves.PermitirAutoProvisionamento,
            Chaves.PerfilPadrao,
            Chaves.TimeoutConexaoSegundos
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

    private static ActiveDirectoryOptions NormalizarParaOpcoes(
        bool ativo,
        string servidor,
        int porta,
        bool usarLdaps,
        bool permitirLdapSemTls,
        string dominio,
        string baseDn,
        string userSearchFilter,
        bool permitirAutoProvisionamento,
        string perfilPadrao,
        int timeoutConexaoSegundos)
    {
        return new ActiveDirectoryOptions
        {
            Ativo = ativo,
            Servidor = (servidor ?? string.Empty).Trim(),
            Porta = porta,
            UsarLdaps = usarLdaps,
            PermitirLdapSemTls = permitirLdapSemTls,
            Dominio = (dominio ?? string.Empty).Trim(),
            BaseDn = (baseDn ?? string.Empty).Trim(),
            UserSearchFilter = string.IsNullOrWhiteSpace(userSearchFilter)
                ? "(&(objectClass=user)(sAMAccountName={0}))"
                : userSearchFilter.Trim(),
            PermitirAutoProvisionamento = permitirAutoProvisionamento,
            PerfilPadrao = string.IsNullOrWhiteSpace(perfilPadrao)
                ? PerfisInternos.Solicitante
                : perfilPadrao.Trim(),
            TimeoutConexaoSegundos = timeoutConexaoSegundos <= 0 ? 10 : timeoutConexaoSegundos
        };
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
        var valor = ObterValor(parametros, chave);
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        if (bool.TryParse(valor, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static int? ObterInt(IReadOnlyDictionary<string, ParametroSistema> parametros, string chave)
    {
        var valor = ObterValor(parametros, chave);
        if (string.IsNullOrWhiteSpace(valor))
        {
            return null;
        }

        if (int.TryParse(valor, out var parsed))
        {
            return parsed;
        }

        return null;
    }
}
