using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        services.AddScoped<IActiveDirectoryCredentialValidator, ActiveDirectoryCredentialValidator>();
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
                            "http://127.0.0.1:5173",
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
