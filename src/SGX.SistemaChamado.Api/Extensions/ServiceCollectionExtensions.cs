using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SGX.SistemaChamado.Api.Authentication;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.HealthChecks;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Domain.Entities;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Interfaces.Auditoria;
using SGX.SistemaChamado.Application.Options;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Infrastructure;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace SGX.SistemaChamado.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public const string AppCorsPolicyName = "AppCorsPolicy";

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        ConfigurarCors(services, configuration, environment);
        ConfigurarHealthChecks(services);
        ConfigurarForwardedHeaders(services, configuration);

        services.AddValidatorsFromAssemblyContaining<ApiInfoRequestValidator>();

        services.AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidator>();
        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<AzureAdOptions>, AzureAdOptionsValidator>();
        services.AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SectionName))
            .ValidateOnStart();

        services.AddSingleton<IValidateOptions<ActiveDirectoryOptions>, ActiveDirectoryOptionsValidator>();
        services.AddOptions<ActiveDirectoryOptions>()
            .Bind(configuration.GetSection(ActiveDirectoryOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<SlaMonitoringOptions>()
            .Bind(configuration.GetSection(SlaMonitoringOptions.SectionName))
            .Validate(options => options.IntervalMinutes > 0, "IntervalMinutes deve ser maior que zero.");

        services.AddHttpContextAccessor();
        services.AddScoped<IAuditoriaContextProvider, AuditoriaContextProvider>();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();
        services.AddScoped<IUsuarioContextoAplicacaoService, UsuarioContextoAplicacaoService>();
        services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
        services.AddScoped<IPoliticaSenhaService, PoliticaSenhaService>();
        services.AddScoped<ITokenRecuperacaoSenhaService, TokenRecuperacaoSenhaService>();
        services.AddScoped<IAutenticacaoLocalSgxService, AutenticacaoLocalSgxService>();
        services.AddScoped<IActiveDirectoryConnectivityTester, ActiveDirectoryConnectivityTester>();
        services.AddScoped<IActiveDirectoryCredentialValidator, ActiveDirectoryCredentialValidator>();
        services.AddScoped<IConfiguracaoIntegracaoActiveDirectoryService, ConfiguracaoIntegracaoActiveDirectoryService>();
        services.AddScoped<IActiveDirectoryAuthenticationService, ActiveDirectoryAuthenticationService>();
        services.AddScoped<IGestaoSenhaLocalSgxService, GestaoSenhaLocalSgxService>();
        services.AddScoped<IConfiguracaoIntegracaoMicrosoftService, ConfiguracaoIntegracaoMicrosoftService>();
        services.AddScoped<IMetodosLoginAdminService, MetodosLoginAdminService>();
        services.AddScoped<IAdministradorInicialService, AdministradorInicialService>();
        services.AddScoped<DevelopmentSeedService>();
        services.AddHostedService<SlaMonitoringBackgroundService>();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthSchemes.Default;
                options.DefaultChallengeScheme = AuthSchemes.Default;
            })
            .AddPolicyScheme(AuthSchemes.Default, "SGX authentication policy", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
                    var authOptions = context.RequestServices.GetRequiredService<IOptions<AuthOptions>>().Value;

                    var authorizationText = context.Request.Headers.Authorization.ToString();
                    var hasBearerToken = authorizationText.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase);

                    var hasDevHeaders =
                        context.Request.Headers.ContainsKey("X-Dev-User-Email") ||
                        context.Request.Headers.ContainsKey("X-Dev-User-Name") ||
                        context.Request.Headers.ContainsKey("X-Dev-User-Role");

                    if (env.IsDevelopment() && authOptions.ModoLocalHabilitado && (hasDevHeaders || !hasBearerToken))
                    {
                        return AuthSchemes.LocalDevelopment;
                    }

                    if (hasBearerToken)
                    {
                        var token = authorizationText["Bearer ".Length..].Trim();
                        if (EhTokenLocalSgx(token, authOptions))
                        {
                            return AuthSchemes.BearerLocalSgx;
                        }
                    }

                    return AuthSchemes.BearerMicrosoft;
                };
            })
            .AddJwtBearer(AuthSchemes.BearerMicrosoft, _ => { })
            .AddJwtBearer(AuthSchemes.BearerLocalSgx, _ => { })
            .AddScheme<AuthenticationSchemeOptions, DevLocalAuthenticationHandler>(AuthSchemes.LocalDevelopment, _ => { });

        services.AddOptions<JwtBearerOptions>(AuthSchemes.BearerMicrosoft)
            .Configure<IOptions<AzureAdOptions>>((options, azureAdOptionsWrapper) =>
            {
                var azureAdOptions = azureAdOptionsWrapper.Value;
                options.MapInboundClaims = false;
                options.Authority = azureAdOptions.BuildAuthority();
                var metadataAddress = azureAdOptions.BuildMetadataAddress();
                if (!string.IsNullOrWhiteSpace(metadataAddress))
                {
                    options.MetadataAddress = metadataAddress;
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = azureAdOptions.Issuer,
                    ValidAudience = azureAdOptions.Audience,
                    NameClaimType = "name"
                };

                if (!string.IsNullOrWhiteSpace(azureAdOptions.ClientId))
                {
                    options.TokenValidationParameters.ValidAudiences =
                    [
                        azureAdOptions.Audience,
                        azureAdOptions.ClientId
                    ];
                }
            });

        services.AddOptions<JwtBearerOptions>(AuthSchemes.BearerLocalSgx)
            .Configure<IOptions<AuthOptions>>((options, authOptionsWrapper) =>
            {
                var authOptions = authOptionsWrapper.Value;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authOptions.JwtLocalIssuer,
                    ValidAudience = authOptions.JwtLocalAudience,
                    IssuerSigningKey = ObterChaveJwtLocal(authOptions),
                    NameClaimType = "name"
                };
            });

        services.AddScoped<IAuthorizationHandler, PerfilRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.Administrador, policy => policy.Requirements.Add(
                new PerfilRequirement(PerfisInternos.Administrador)));

            options.AddPolicy(Policies.Atendente, policy => policy.Requirements.Add(
                new PerfilRequirement(PerfisInternos.Atendente)));

            options.AddPolicy(Policies.Solicitante, policy => policy.Requirements.Add(
                new PerfilRequirement(PerfisInternos.Solicitante)));

            options.AddPolicy(Policies.AdminOuAtendente, policy => policy.Requirements.Add(
                new PerfilRequirement(PerfisInternos.Administrador, PerfisInternos.Atendente)));
        });

        services.AddInfrastructure(configuration);
        return services;
    }

    public static WebApplicationBuilder AddStructuredLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("Application", "SGX.SistemaChamado.Api")
                .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
                .Enrich.FromLogContext()
                .WriteTo.Console());

        return builder;
    }

    private static void ConfigurarCors(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        var configuredOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
        var normalizedOrigins = configuredOrigins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim().TrimEnd('/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(AppCorsPolicyName, policy =>
            {
                if (environment.IsDevelopment() && normalizedOrigins.Length == 0)
                {
                    policy
                        .WithOrigins(
                            "http://localhost:5173",
                            "https://localhost:5173",
                            "http://127.0.0.1:5173",
                            "https://127.0.0.1:5173",
                            "http://localhost:8081",
                            "http://127.0.0.1:8081")
                        .AllowAnyHeader()
                        .AllowAnyMethod();

                    return;
                }

                if (normalizedOrigins.Length > 0)
                {
                    policy
                        .WithOrigins(normalizedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });
    }

    private static void ConfigurarHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("api-live", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck<DatabaseReadyHealthCheck>("postgresql-ready", tags: ["ready"]);
    }

    private static void ConfigurarForwardedHeaders(IServiceCollection services, IConfiguration configuration)
    {
        var proxiesConfiaveisConfigurados = configuration.GetSection("ReverseProxy:TrustedProxies").Get<string[]>() ?? [];
        var redesConfiaveisConfiguradas = configuration.GetSection("ReverseProxy:TrustedNetworks").Get<string[]>() ?? [];

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 1;

            RemoverRedeConhecida(options, IPAddress.Parse("10.0.0.0"), 8);
            RemoverRedeConhecida(options, IPAddress.Parse("172.16.0.0"), 12);
            RemoverRedeConhecida(options, IPAddress.Parse("192.168.0.0"), 16);
            RemoverRedeConhecida(options, IPAddress.Parse("127.0.0.0"), 8);
            RemoverRedeConhecida(options, IPAddress.Parse("::1"), 128);

            AdicionarProxyConhecidoSeNecessario(options, IPAddress.Loopback);
            AdicionarProxyConhecidoSeNecessario(options, IPAddress.IPv6Loopback);
            AdicionarRedeConhecidaSeNecessario(options, IPAddress.Parse("127.0.0.0"), 8);
            AdicionarRedeConhecidaSeNecessario(options, IPAddress.IPv6Loopback, 128);

            foreach (var proxyConfigurado in proxiesConfiaveisConfigurados)
            {
                if (!IPAddress.TryParse(proxyConfigurado, out var enderecoProxy))
                {
                    continue;
                }

                AdicionarProxyConhecidoSeNecessario(options, enderecoProxy);
            }

            foreach (var redeConfigurada in redesConfiaveisConfiguradas)
            {
                if (!TryParseCidr(redeConfigurada, out var prefixoRede, out var tamanhoPrefixo))
                {
                    continue;
                }

                AdicionarRedeConhecidaSeNecessario(options, prefixoRede, tamanhoPrefixo);
            }
        });
    }

    private static void RemoverRedeConhecida(ForwardedHeadersOptions options, IPAddress prefixo, int tamanhoPrefixo)
    {
        for (var indice = options.KnownNetworks.Count - 1; indice >= 0; indice--)
        {
            var rede = options.KnownNetworks[indice];
            if (rede.Prefix.Equals(prefixo) && rede.PrefixLength == tamanhoPrefixo)
            {
                options.KnownNetworks.RemoveAt(indice);
            }
        }
    }

    private static void AdicionarProxyConhecidoSeNecessario(ForwardedHeadersOptions options, IPAddress proxy)
    {
        if (!options.KnownProxies.Contains(proxy))
        {
            options.KnownProxies.Add(proxy);
        }
    }

    private static void AdicionarRedeConhecidaSeNecessario(ForwardedHeadersOptions options, IPAddress prefixo, int tamanhoPrefixo)
    {
        var redeJaConhecida = options.KnownNetworks.Any(rede =>
            rede.Prefix.Equals(prefixo) &&
            rede.PrefixLength == tamanhoPrefixo);

        if (!redeJaConhecida)
        {
            options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(prefixo, tamanhoPrefixo));
        }
    }

    private static bool TryParseCidr(string valor, out IPAddress prefixo, out int tamanhoPrefixo)
    {
        prefixo = IPAddress.Loopback;
        tamanhoPrefixo = 0;

        if (string.IsNullOrWhiteSpace(valor))
        {
            return false;
        }

        var partes = valor.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (partes.Length != 2)
        {
            return false;
        }

        if (!IPAddress.TryParse(partes[0], out var prefixoParseado) || prefixoParseado is null)
        {
            return false;
        }

        prefixo = prefixoParseado;

        if (!int.TryParse(partes[1], out tamanhoPrefixo))
        {
            return false;
        }

        var tamanhoMaximo = prefixo.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128;
        return tamanhoPrefixo >= 0 && tamanhoPrefixo <= tamanhoMaximo;
    }

    private static SymmetricSecurityKey ObterChaveJwtLocal(AuthOptions authOptions)
    {
        var chave = (authOptions.JwtLocalChaveAssinatura ?? string.Empty).Trim();
        if (chave.Length < 32)
        {
            return new SymmetricSecurityKey(Guid.NewGuid().ToByteArray());
        }

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
    }

    private static bool EhTokenLocalSgx(string token, AuthOptions authOptions)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return false;
        }

        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var provedor = jwt.Claims.FirstOrDefault(x => x.Type == "auth_provider")?.Value;
            if (string.Equals(provedor, "LocalSgx", StringComparison.Ordinal))
            {
                return true;
            }

            return string.Equals(jwt.Issuer, authOptions.JwtLocalIssuer, StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
    }
}
