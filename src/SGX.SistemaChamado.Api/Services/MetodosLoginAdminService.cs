using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.Contracts.Admin;
using SGX.SistemaChamado.Api.Contracts.Auth;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Domain.Enums;
using SGX.SistemaChamado.Infrastructure.Persistence;
using System.Text.Json;

namespace SGX.SistemaChamado.Api.Services;

public interface IMetodosLoginAdminService
{
    Task<MetodosLoginAdminResponse> ObterConfiguracaoAdminAsync(CancellationToken cancellationToken = default);
    Task<MetodosLoginAdminResponse> AtualizarConfiguracaoAdminAsync(
        AtualizarMetodosLoginAdminRequest request,
        CancellationToken cancellationToken = default);
    Task<ProvedoresAutenticacaoResponse> ObterProvedoresPublicosAsync(CancellationToken cancellationToken = default);
    Task<bool> ProvedorHabilitadoAsync(string codigoProvedor, CancellationToken cancellationToken = default);
    Task<MetodoLoginEfetivo?> ObterMetodoEfetivoAsync(string codigoProvedor, CancellationToken cancellationToken = default);
}

public sealed record MetodoLoginEfetivo(
    string Codigo,
    string Nome,
    string Descricao,
    bool Configurado,
    bool Habilitado,
    bool Principal,
    int Ordem,
    bool PermiteAutoProvisionamento,
    string PerfilPadraoAutoProvisionamento,
    string RotuloExibicao,
    bool Funcional,
    bool PodeHabilitar,
    string? MotivoBloqueioHabilitar);

public sealed class MetodosLoginAdminService(
    SGXSistemaChamadoDbContext dbContext,
    IHostEnvironment environment,
    IOptions<AuthOptions> authOptions,
    IConfiguracaoIntegracaoActiveDirectoryService configuracaoIntegracaoActiveDirectoryService,
    IConfiguracaoIntegracaoMicrosoftService configuracaoIntegracaoMicrosoftService,
    ILogger<MetodosLoginAdminService> logger,
    IAuditoriaService? auditoriaService = null) : IMetodosLoginAdminService
{
    private const string UsuarioTecnico = "admin.auth.provedores";
    private const string Prefixo = "auth.provedores";
    private const string ChavePrincipal = Prefixo + ".principal";
    private const string ChaveHabilitado = Prefixo + ".{0}.habilitado";
    private const string ChaveOrdem = Prefixo + ".{0}.ordem";
    private const string ChaveAutoProvisionamento = Prefixo + ".{0}.auto_provisionamento";
    private const string ChavePerfilPadrao = Prefixo + ".{0}.perfil_padrao";
    private const string ChaveRotuloExibicao = Prefixo + ".{0}.rotulo_exibicao";
    private const string ChaveFallbackMicrosoftAutoProvisionamento = "auth.microsoft.criar_usuario_automaticamente";
    private const string ChaveFallbackMicrosoftPerfilPadrao = "auth.microsoft.perfil_padrao_usuario";

    public async Task<MetodosLoginAdminResponse> ObterConfiguracaoAdminAsync(CancellationToken cancellationToken = default)
    {
        var metodos = await ObterMetodosEfetivosAsync(cancellationToken);
        var response = metodos
            .Select(x =>
            {
                var motivoBloqueioDesabilitar = ObterMotivoBloqueioDesabilitar(x, metodos);
                return MapearAdmin(x, motivoBloqueioDesabilitar);
            })
            .OrderBy(x => x.Ordem)
            .ToArray();
        return new MetodosLoginAdminResponse(response);
    }

        public async Task<MetodosLoginAdminResponse> AtualizarConfiguracaoAdminAsync(
        AtualizarMetodosLoginAdminRequest request,
        CancellationToken cancellationToken = default)
    {
        var configuracaoAntes = await ObterConfiguracaoAdminAsync(cancellationToken);

        if (request is null || request.Provedores.Count == 0)
        {
            throw new InvalidOperationException("Nenhuma configuracao de provedor foi informada.");
        }

        try
        {
            var disponiveis = ObterCatalogo()
                .Where(x => PodeAparecerNoAmbiente(x.Codigo))
                .ToDictionary(x => x.Codigo, StringComparer.OrdinalIgnoreCase);

            var recebidos = request.Provedores
                .GroupBy(x => NormalizarCodigo(x.Codigo), StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Last())
                .ToDictionary(x => x.Codigo, StringComparer.OrdinalIgnoreCase);

            var codigosInvalidos = recebidos.Keys
                .Where(x => !disponiveis.ContainsKey(x))
                .ToArray();
            if (codigosInvalidos.Length > 0)
            {
                throw new InvalidOperationException($"Provedores invalidos: {string.Join(", ", codigosInvalidos)}.");
            }

            var principal = recebidos.Values.Where(x => x.Principal).ToArray();
            if (principal.Length != 1)
            {
                throw new InvalidOperationException("O provedor principal deve ser unico.");
            }

            var principalCodigo = principal[0].Codigo;
            if (!recebidos.TryGetValue(principalCodigo, out var principalItem) || !principalItem.Habilitado)
            {
                throw new InvalidOperationException("O provedor principal precisa estar habilitado.");
            }

            foreach (var item in recebidos.Values)
            {
                if (item.Ordem <= 0)
                {
                    throw new InvalidOperationException($"A ordem do provedor '{item.Codigo}' deve ser maior que zero.");
                }

                if (!PerfilPadraoValido(item.PerfilPadraoAutoProvisionamento))
                {
                    throw new InvalidOperationException(
                        $"Perfil padrao invalido para auto provisionamento no provedor '{item.Codigo}'.");
                }

                if (PermiteAutoProvisionamento(item.Codigo)
                    && string.Equals(item.PerfilPadraoAutoProvisionamento?.Trim(), PerfisInternos.Administrador, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        $"Perfil Administrador nao pode ser atribuido automaticamente pelo provedor '{item.Codigo}'.");
                }

                if (item.Codigo.Equals(CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase)
                    && !environment.IsDevelopment()
                    && item.Habilitado)
                {
                    throw new InvalidOperationException("LocalDevelopment so pode ser habilitado em ambiente Development.");
                }
            }

            var simulacao = await SimularMetodosEfetivosAsync(recebidos, principalCodigo, cancellationToken);
            ValidarConfiguracaoViavel(simulacao);

            await PersistirConfiguracaoAsync(recebidos, principalCodigo, cancellationToken);
            var configuracaoDepois = await ObterConfiguracaoAdminAsync(cancellationToken);
            await RegistrarAuditoriaAlteracoesAsync(configuracaoAntes, configuracaoDepois, cancellationToken);
            return configuracaoDepois;
        }
        catch (InvalidOperationException ex)
        {
            await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                auditoriaService,
                logger,
                TipoEventoAutenticacao.BloqueioConfiguracaoInsegura,
                ResultadoEventoAutenticacao.Bloqueado,
                "Configuracao administrativa de metodos de login bloqueada por regra de seguranca.",
                "Administracao",
                entidadeId: "metodos-login",
                mensagemTecnica: ex.Message,
                dadosDepois: JsonSerializer.Serialize(request),
                cancellationToken: cancellationToken);

            throw;
        }
    }

    public async Task<ProvedoresAutenticacaoResponse> ObterProvedoresPublicosAsync(CancellationToken cancellationToken = default)
    {
        var metodos = await ObterMetodosEfetivosAsync(cancellationToken);
        var ativos = metodos
            .Where(x => x.Habilitado && x.Funcional && PodeAparecerNoAmbiente(x.Codigo))
            .OrderBy(x => x.Ordem)
            .ToArray();

        if (ativos.Length == 0)
        {
            return new ProvedoresAutenticacaoResponse([]);
        }

        var principal = ativos.FirstOrDefault(x => x.Principal) ?? ativos[0];
        var provedores = ativos
            .Select(x => new ProvedorAutenticacaoDto(
                Codigo: x.Codigo,
                Nome: x.RotuloExibicao,
                Descricao: x.Descricao,
                Habilitado: true,
                Principal: string.Equals(x.Codigo, principal.Codigo, StringComparison.OrdinalIgnoreCase),
                Ordem: x.Ordem))
            .ToArray();

        return new ProvedoresAutenticacaoResponse(provedores);
    }

    public async Task<bool> ProvedorHabilitadoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
    {
        var metodo = await ObterMetodoEfetivoAsync(codigoProvedor, cancellationToken);
        return metodo is not null && metodo.Habilitado && metodo.Funcional;
    }

    public async Task<MetodoLoginEfetivo?> ObterMetodoEfetivoAsync(string codigoProvedor, CancellationToken cancellationToken = default)
    {
        var codigo = NormalizarCodigo(codigoProvedor);
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return null;
        }

        var metodos = await ObterMetodosEfetivosAsync(cancellationToken);
        return metodos.FirstOrDefault(x => string.Equals(x.Codigo, codigo, StringComparison.OrdinalIgnoreCase));
    }

    private async Task<IReadOnlyCollection<MetodoLoginEfetivo>> ObterMetodosEfetivosAsync(CancellationToken cancellationToken)
    {
        var parametros = await CarregarParametrosAsync(cancellationToken);
        var itens = new List<MetodoLoginEfetivo>();
        var auth = authOptions.Value;
        var configuracaoMicrosoft = await configuracaoIntegracaoMicrosoftService
            .ObterConfiguracaoAutenticacaoEfetivaAsync(cancellationToken);
        var configuracaoActiveDirectory = await configuracaoIntegracaoActiveDirectoryService
            .ObterConfiguracaoEfetivaAsync(cancellationToken);

        foreach (var item in ObterCatalogo())
        {
            if (!PodeAparecerNoAmbiente(item.Codigo))
            {
                continue;
            }

            var configurado = ProvedorConfigurado(item.Codigo, auth);
            var habilitadoFallback = ProvedorHabilitadoFallback(item.Codigo, auth, configuracaoMicrosoft);
            var habilitado = ObterBoolean(parametros, Formatar(ChaveHabilitado, item.Codigo)) ?? habilitadoFallback;
            var ordem = ObterInt(parametros, Formatar(ChaveOrdem, item.Codigo)) ?? item.OrdemPadrao;
            var principalFallback = auth.ObterCodigoProvedorPrincipalNormalizado();
            var principalConfigurado = ObterValor(parametros, ChavePrincipal) ?? principalFallback;
            var principal = string.Equals(principalConfigurado, item.Codigo, StringComparison.OrdinalIgnoreCase);
            var autoProvisionamento = ObterAutoProvisionamentoEfetivo(
                parametros,
                item.Codigo,
                configuracaoMicrosoft,
                configuracaoActiveDirectory);
            var perfilPadrao = ObterPerfilPadraoEfetivo(
                parametros,
                item.Codigo,
                configuracaoMicrosoft,
                configuracaoActiveDirectory);
            var rotulo = ObterValor(parametros, Formatar(ChaveRotuloExibicao, item.Codigo));
            var rotuloExibicao = string.IsNullOrWhiteSpace(rotulo) ? item.NomePadrao : rotulo.Trim();
            var (podeHabilitar, motivoBloqueioHabilitar) = AvaliarPossibilidadeHabilitacao(
                item.Codigo,
                configuracaoMicrosoft,
                configuracaoActiveDirectory);
            var funcional = habilitado && podeHabilitar;

            itens.Add(new MetodoLoginEfetivo(
                Codigo: item.Codigo,
                Nome: item.NomePadrao,
                Descricao: item.DescricaoPadrao,
                Configurado: configurado,
                Habilitado: habilitado,
                Principal: principal,
                Ordem: ordem > 0 ? ordem : item.OrdemPadrao,
                PermiteAutoProvisionamento: autoProvisionamento,
                PerfilPadraoAutoProvisionamento: perfilPadrao,
                RotuloExibicao: rotuloExibicao,
                Funcional: funcional,
                PodeHabilitar: podeHabilitar,
                MotivoBloqueioHabilitar: motivoBloqueioHabilitar));
        }

        var principalEfetivo = itens.FirstOrDefault(x => x.Principal && x.Habilitado && x.Funcional)
            ?? itens.Where(x => x.Habilitado && x.Funcional).OrderBy(x => x.Ordem).FirstOrDefault();
        if (principalEfetivo is not null)
        {
            itens = itens.Select(x => x with { Principal = x.Codigo == principalEfetivo.Codigo }).ToList();
        }

        return itens.OrderBy(x => x.Ordem).ToArray();
    }

    private async Task<IReadOnlyCollection<MetodoLoginEfetivo>> SimularMetodosEfetivosAsync(
        IReadOnlyDictionary<string, MetodoLoginAdminAtualizacaoDto> recebidos,
        string principalCodigo,
        CancellationToken cancellationToken)
    {
        var atuais = await ObterMetodosEfetivosAsync(cancellationToken);
        return atuais
            .Select(x =>
            {
                if (!recebidos.TryGetValue(x.Codigo, out var novo))
                {
                    return x with { Principal = string.Equals(x.Codigo, principalCodigo, StringComparison.OrdinalIgnoreCase) };
                }

                return x with
                {
                    Habilitado = novo.Habilitado,
                    Principal = string.Equals(novo.Codigo, principalCodigo, StringComparison.OrdinalIgnoreCase),
                    Ordem = novo.Ordem,
                    PermiteAutoProvisionamento = novo.PermiteAutoProvisionamento,
                    PerfilPadraoAutoProvisionamento = string.IsNullOrWhiteSpace(novo.PerfilPadraoAutoProvisionamento)
                        ? PerfisInternos.Solicitante
                        : novo.PerfilPadraoAutoProvisionamento.Trim(),
                    RotuloExibicao = string.IsNullOrWhiteSpace(novo.RotuloExibicao)
                        ? x.Nome
                        : novo.RotuloExibicao.Trim(),
                    Funcional = novo.Habilitado && x.PodeHabilitar
                };
            })
            .ToArray();
    }

    private void ValidarConfiguracaoViavel(IReadOnlyCollection<MetodoLoginEfetivo> metodos)
    {
        var localSgx = metodos.FirstOrDefault(x => x.Codigo == CodigoProvedorAutenticacao.LocalSgx);
        if (localSgx is not null && !localSgx.Habilitado)
        {
            var alternativasAdministrativas = metodos
                .Where(x => x.Codigo != CodigoProvedorAutenticacao.LocalSgx)
                .Where(x => x.Habilitado && x.Funcional)
                .ToArray();
            if (alternativasAdministrativas.Length == 0)
            {
                throw new InvalidOperationException(
                    "Nao e permitido desabilitar LocalSgx sem alternativa de acesso administrativo viavel.");
            }
        }

        var habilitadosFuncionais = metodos
            .Where(x => x.Habilitado && x.Funcional)
            .ToArray();

        if (habilitadosFuncionais.Length == 0)
        {
            throw new InvalidOperationException(
                "Ao menos um metodo de login viavel deve permanecer habilitado.");
        }
    }

    private async Task PersistirConfiguracaoAsync(
        IReadOnlyDictionary<string, MetodoLoginAdminAtualizacaoDto> recebidos,
        string principalCodigo,
        CancellationToken cancellationToken)
    {
        await UpsertParametroAsync(
            ChavePrincipal,
            principalCodigo,
            "Provedor principal de autenticaÃ§Ã£o definido administrativamente.",
            cancellationToken);

        foreach (var item in recebidos.Values)
        {
            var codigo = item.Codigo;
            await UpsertParametroAsync(
                Formatar(ChaveHabilitado, codigo),
                item.Habilitado ? "true" : "false",
                $"HabilitaÃ§Ã£o administrativa do provedor {codigo}.",
                cancellationToken);
            await UpsertParametroAsync(
                Formatar(ChaveOrdem, codigo),
                item.Ordem.ToString(),
                $"Ordem administrativa do provedor {codigo}.",
                cancellationToken);
            await UpsertParametroAsync(
                Formatar(ChaveRotuloExibicao, codigo),
                string.IsNullOrWhiteSpace(item.RotuloExibicao) ? codigo : item.RotuloExibicao.Trim(),
                $"RÃ³tulo de exibiÃ§Ã£o do provedor {codigo}.",
                cancellationToken);

            if (PermiteAutoProvisionamento(codigo))
            {
                await UpsertParametroAsync(
                    Formatar(ChaveAutoProvisionamento, codigo),
                    item.PermiteAutoProvisionamento ? "true" : "false",
                    $"Auto provisionamento administrativo do provedor {codigo}.",
                    cancellationToken);
                await UpsertParametroAsync(
                    Formatar(ChavePerfilPadrao, codigo),
                    string.IsNullOrWhiteSpace(item.PerfilPadraoAutoProvisionamento)
                        ? PerfisInternos.Solicitante
                        : item.PerfilPadraoAutoProvisionamento.Trim(),
                    $"Perfil padrÃ£o para auto provisionamento do provedor {codigo}.",
                    cancellationToken);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Dictionary<string, ParametroSistema>> CarregarParametrosAsync(CancellationToken cancellationToken)
    {
        var parametros = await dbContext.ParametrosSistema
            .Where(x => x.Ativo && (x.Chave.StartsWith(Prefixo) || x.Chave == ChaveFallbackMicrosoftAutoProvisionamento || x.Chave == ChaveFallbackMicrosoftPerfilPadrao))
            .ToListAsync(cancellationToken);

        return parametros.ToDictionary(x => x.Chave, x => x, StringComparer.OrdinalIgnoreCase);
    }

    private async Task UpsertParametroAsync(
        string chave,
        string valor,
        string descricao,
        CancellationToken cancellationToken)
    {
        var parametro = await dbContext.ParametrosSistema
            .FirstOrDefaultAsync(x => x.Chave == chave, cancellationToken);

        if (parametro is null)
        {
            await dbContext.ParametrosSistema.AddAsync(
                new ParametroSistema(chave, valor, descricao, false, UsuarioTecnico),
                cancellationToken);
            return;
        }

        parametro.Ativar(UsuarioTecnico);
        parametro.AtualizarValor(valor, UsuarioTecnico);
        parametro.DefinirDescricao(descricao, UsuarioTecnico);
        parametro.DefinirSensivel(false, UsuarioTecnico);
    }

    private (bool PodeHabilitar, string? MotivoBloqueio) AvaliarPossibilidadeHabilitacao(
        string codigo,
        ConfiguracaoAutenticacaoEfetiva configuracaoMicrosoft,
        ActiveDirectoryOptions configuracaoActiveDirectory)
    {
        if (string.Equals(codigo, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            if (!configuracaoMicrosoft.MicrosoftHabilitado)
            {
                return (false, "Microsoft Entra ID nao esta tecnicamente configurado.");
            }

            return (true, null);
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
        {
            if (!configuracaoActiveDirectory.EstaConfigurado())
            {
                return (false, "Active Directory nao esta tecnicamente configurado.");
            }

            return (true, null);
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase))
        {
            if (!environment.IsDevelopment())
            {
                return (false, "LocalDevelopment so pode ser habilitado em ambiente Development.");
            }

            return (true, null);
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase))
        {
            if (!authOptions.Value.LoginLocalHabilitado)
            {
                return (false, "Login Local SGX esta desabilitado na configuracao tecnica do ambiente.");
            }

            return (true, null);
        }

        return (false, "Provedor nao suportado no ambiente atual.");
    }

    private string? ObterMotivoBloqueioDesabilitar(
        MetodoLoginEfetivo metodo,
        IReadOnlyCollection<MetodoLoginEfetivo> metodos)
    {
        if (!metodo.Habilitado)
        {
            return null;
        }

        var simulacao = metodos
            .Select(x => x.Codigo == metodo.Codigo
                ? x with { Habilitado = false, Funcional = false }
                : x)
            .ToArray();

        try
        {
            ValidarConfiguracaoViavel(simulacao);
            return null;
        }
        catch (InvalidOperationException ex)
        {
            return ex.Message;
        }
    }

    private static MetodoLoginAdminDto MapearAdmin(MetodoLoginEfetivo x, string? motivoBloqueioDesabilitar)
    {
        return new MetodoLoginAdminDto(
            Codigo: x.Codigo,
            Nome: x.Nome,
            Descricao: x.Descricao,
            Configurado: x.Configurado,
            Habilitado: x.Habilitado,
            Principal: x.Principal,
            Ordem: x.Ordem,
            PermiteAutoProvisionamento: x.PermiteAutoProvisionamento,
            PerfilPadraoAutoProvisionamento: x.PerfilPadraoAutoProvisionamento,
            RotuloExibicao: x.RotuloExibicao,
            Funcional: x.Funcional,
            PodeHabilitar: x.PodeHabilitar,
            MotivoBloqueioHabilitar: x.MotivoBloqueioHabilitar,
            PodeDesabilitar: string.IsNullOrWhiteSpace(motivoBloqueioDesabilitar),
            MotivoBloqueioDesabilitar: motivoBloqueioDesabilitar);
    }

    private static bool ProvedorConfigurado(string codigo, AuthOptions auth)
    {
        return auth.ObterCodigosProvedoresConfiguradosNormalizados()
            .Contains(codigo, StringComparer.OrdinalIgnoreCase);
    }

    private static bool ProvedorHabilitadoFallback(
        string codigo,
        AuthOptions auth,
        ConfiguracaoAutenticacaoEfetiva configuracaoMicrosoft)
    {
        if (string.Equals(codigo, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            return configuracaoMicrosoft.MicrosoftHabilitado;
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase))
        {
            return configuracaoMicrosoft.LoginLocalSgxHabilitado;
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase))
        {
            return configuracaoMicrosoft.LoginLocalDevelopmentHabilitado;
        }

        return auth.ObterCodigosProvedoresHabilitadosNormalizados()
            .Contains(codigo, StringComparer.OrdinalIgnoreCase);
    }

    private bool PodeAparecerNoAmbiente(string codigo)
    {
        return !string.Equals(codigo, CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase)
            || environment.IsDevelopment();
    }

    private static bool PermiteAutoProvisionamento(string codigo)
    {
        return string.Equals(codigo, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase)
            || string.Equals(codigo, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase);
    }

    private bool ObterAutoProvisionamentoEfetivo(
        IReadOnlyDictionary<string, ParametroSistema> parametros,
        string codigo,
        ConfiguracaoAutenticacaoEfetiva configuracaoMicrosoft,
        ActiveDirectoryOptions configuracaoActiveDirectory)
    {
        var admin = ObterBoolean(parametros, Formatar(ChaveAutoProvisionamento, codigo));
        if (admin.HasValue)
        {
            return admin.Value;
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return configuracaoActiveDirectory.PermitirAutoProvisionamento;
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            var legado = ObterBoolean(parametros, ChaveFallbackMicrosoftAutoProvisionamento);
            return legado ?? configuracaoMicrosoft.CriarUsuarioAutomaticamente;
        }

        return false;
    }

    private string ObterPerfilPadraoEfetivo(
        IReadOnlyDictionary<string, ParametroSistema> parametros,
        string codigo,
        ConfiguracaoAutenticacaoEfetiva configuracaoMicrosoft,
        ActiveDirectoryOptions configuracaoActiveDirectory)
    {
        var admin = ObterValor(parametros, Formatar(ChavePerfilPadrao, codigo));
        if (!string.IsNullOrWhiteSpace(admin))
        {
            return admin.Trim();
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return string.IsNullOrWhiteSpace(configuracaoActiveDirectory.PerfilPadrao)
                ? PerfisInternos.Solicitante
                : configuracaoActiveDirectory.PerfilPadrao.Trim();
        }

        if (string.Equals(codigo, CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            var legado = ObterValor(parametros, ChaveFallbackMicrosoftPerfilPadrao);
            if (!string.IsNullOrWhiteSpace(legado))
            {
                return legado.Trim();
            }

            return string.IsNullOrWhiteSpace(configuracaoMicrosoft.PerfilPadraoUsuarioMicrosoft)
                ? PerfisInternos.Solicitante
                : configuracaoMicrosoft.PerfilPadraoUsuarioMicrosoft.Trim();
        }

        return PerfisInternos.Solicitante;
    }

    private static bool PerfilPadraoValido(string perfilPadrao)
    {
        return PerfisInternos.EhPerfilValido(
            string.IsNullOrWhiteSpace(perfilPadrao)
                ? PerfisInternos.Solicitante
                : perfilPadrao.Trim());
    }

    private static string Formatar(string template, string codigo)
    {
        return string.Format(template, NormalizarCodigo(codigo).ToLowerInvariant());
    }

    private static string NormalizarCodigo(string? codigo)
    {
        var valor = (codigo ?? string.Empty).Trim();
        if (valor.Equals(CodigoProvedorAutenticacao.MicrosoftEntraId, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.MicrosoftEntraId;
        }

        if (valor.Equals(CodigoProvedorAutenticacao.ActiveDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.ActiveDirectory;
        }

        if (valor.Equals(CodigoProvedorAutenticacao.LocalSgx, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.LocalSgx;
        }

        if (valor.Equals(CodigoProvedorAutenticacao.LocalDevelopment, StringComparison.OrdinalIgnoreCase))
        {
            return CodigoProvedorAutenticacao.LocalDevelopment;
        }

        return valor;
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

    private async Task RegistrarAuditoriaAlteracoesAsync(
        MetodosLoginAdminResponse antes,
        MetodosLoginAdminResponse depois,
        CancellationToken cancellationToken)
    {
        var mapaAntes = antes.Provedores.ToDictionary(x => x.Codigo, StringComparer.OrdinalIgnoreCase);
        var mapaDepois = depois.Provedores.ToDictionary(x => x.Codigo, StringComparer.OrdinalIgnoreCase);

        foreach (var par in mapaDepois)
        {
            var codigo = par.Key;
            var atual = par.Value;
            if (!mapaAntes.TryGetValue(codigo, out var anterior))
            {
                continue;
            }

            var dadosAntes = JsonSerializer.Serialize(anterior);
            var dadosDepois = JsonSerializer.Serialize(atual);

            if (anterior.Habilitado != atual.Habilitado)
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoProvedorHabilitado,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao de habilitacao do provedor {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }

            if (anterior.Principal != atual.Principal)
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoProvedorPrincipal,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao de provedor principal para {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }

            if (anterior.Ordem != atual.Ordem)
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoOrdemExibicao,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao da ordem de exibicao do provedor {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }

            if (anterior.PermiteAutoProvisionamento != atual.PermiteAutoProvisionamento)
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoAutoProvisionamento,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao de auto provisionamento do provedor {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }

            if (!string.Equals(
                    anterior.PerfilPadraoAutoProvisionamento,
                    atual.PerfilPadraoAutoProvisionamento,
                    StringComparison.OrdinalIgnoreCase))
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoPerfilPadraoProvisionamento,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao de perfil padrao de auto provisionamento do provedor {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }

            if (!string.Equals(anterior.RotuloExibicao, atual.RotuloExibicao, StringComparison.Ordinal))
            {
                await AuditoriaAutenticacaoHelper.RegistrarEventoAdministrativoAsync(
                    auditoriaService,
                    logger,
                    TipoEventoAutenticacao.AlteracaoRotuloExibicao,
                    ResultadoEventoAutenticacao.Sucesso,
                    $"Alteracao de rotulo de exibicao do provedor {codigo}.",
                    codigo,
                    entidadeId: codigo,
                    dadosAntes: dadosAntes,
                    dadosDepois: dadosDepois,
                    cancellationToken: cancellationToken);
            }
        }
    }
    private static IReadOnlyCollection<ProvedorCatalogo> ObterCatalogo()
    {
        return
        [
            new ProvedorCatalogo(
                CodigoProvedorAutenticacao.MicrosoftEntraId,
                "Microsoft Entra ID",
                "Login corporativo federado pelo Microsoft Entra ID.",
                10),
            new ProvedorCatalogo(
                CodigoProvedorAutenticacao.ActiveDirectory,
                "Active Directory",
                "Login corporativo integrado ao Active Directory do cliente.",
                20),
            new ProvedorCatalogo(
                CodigoProvedorAutenticacao.LocalSgx,
                "Local SGX",
                "Login local SGX com e-mail corporativo e senha.",
                30),
            new ProvedorCatalogo(
                CodigoProvedorAutenticacao.LocalDevelopment,
                "Local Development",
                "Login tÃ©cnico de desenvolvimento exclusivo para ambiente Development.",
                40)
        ];
    }

    private sealed record ProvedorCatalogo(
        string Codigo,
        string NomePadrao,
        string DescricaoPadrao,
        int OrdemPadrao);
}

