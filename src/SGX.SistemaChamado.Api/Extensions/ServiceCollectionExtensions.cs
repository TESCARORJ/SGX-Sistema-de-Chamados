using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SGX.SistemaChamado.Api.Authentication;
using SGX.SistemaChamado.Api.Authorization;
using SGX.SistemaChamado.Api.HealthChecks;
using SGX.SistemaChamado.Api.Options;
using SGX.SistemaChamado.Api.Services;
using SGX.SistemaChamado.Application.Interfaces;
using SGX.SistemaChamado.Application.Validators;
using SGX.SistemaChamado.Infrastructure;

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

        services.AddOptions<AuthOptions>()
            .Bind(configuration.GetSection(AuthOptions.SectionName));

        services.AddSingleton<IValidateOptions<AzureAdOptions>, AzureAdOptionsValidator>();
        services.AddOptions<AzureAdOptions>()
            .Bind(configuration.GetSection(AzureAdOptions.SectionName))
            .ValidateOnStart();

        services.AddHttpContextAccessor();
        services.AddScoped<IUsuarioAtualService, UsuarioAtualService>();
        services.AddScoped<IUsuarioContextoAplicacaoService, UsuarioContextoAplicacaoService>();
        services.AddScoped<DevelopmentSeedService>();

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

                    return JwtBearerDefaults.AuthenticationScheme;
                };
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, _ => { })
            .AddScheme<AuthenticationSchemeOptions, DevLocalAuthenticationHandler>(AuthSchemes.LocalDevelopment, _ => { });

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<AzureAdOptions>>((options, azureAdOptionsWrapper) =>
            {
                var azureAdOptions = azureAdOptionsWrapper.Value;
                options.MapInboundClaims = false;
                options.Authority = azureAdOptions.BuildAuthority();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = azureAdOptions.Issuer,
                    ValidAudience = azureAdOptions.Audience
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

        services.AddScoped<IAuthorizationHandler, PerfilRequirementHandler>();
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
}
