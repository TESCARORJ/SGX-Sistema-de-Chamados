using Serilog;
using SGX.SistemaChamado.Api.Extensions;
using SGX.SistemaChamado.Api.Middlewares;
using SGX.SistemaChamado.Api.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args)
    .AddStructuredLogging();

builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();
var startupLogger = app.Logger;
var corsAllowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
var authModoLocalHabilitado = builder.Configuration.GetValue<bool>("Authentication:ModoLocalHabilitado");

startupLogger.LogInformation(
    "Ambiente da API: {Environment}. ModoLocalHabilitado={ModoLocalHabilitado}. CorsOriginsCarregadas={CorsOrigins}. PoliticaCors={PolicyName}",
    app.Environment.EnvironmentName,
    authModoLocalHabilitado,
    string.Join(", ", corsAllowedOrigins),
    ServiceCollectionExtensions.AppCorsPolicyName);

using (var scope = app.Services.CreateScope())
{
    try
    {
        startupLogger.LogInformation("Inicializando banco de dados do SGX Sistema de Chamados (startup).");

        var dbContext = scope.ServiceProvider.GetRequiredService<SGXSistemaChamadoDbContext>();
        if (dbContext.Database.IsRelational())
        {
            startupLogger.LogInformation(
                "Aplicando migrations pendentes para o contexto {DbContext} (provider relacional).",
                nameof(SGXSistemaChamadoDbContext));
            await dbContext.Database.MigrateAsync();
            startupLogger.LogInformation("MigrateAsync concluido com sucesso.");
        }
        else
        {
            startupLogger.LogInformation("Garantindo criacao do banco (provider nao relacional).");
            await dbContext.Database.EnsureCreatedAsync();
        }

        var administradorInicialService = scope.ServiceProvider.GetRequiredService<IAdministradorInicialService>();
        await administradorInicialService.SeedAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentSeedService>();
        await seeder.SeedAsync();

        startupLogger.LogInformation("Inicializacao de banco e seeds concluida com sucesso.");
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning", StringComparison.Ordinal))
    {
        startupLogger.LogCritical(
            ex,
            "Falha ao aplicar migrations: EF Core detectou PendingModelChangesWarning. " +
            "Checklist sugerido: (1) parar debugger/API em execucao, (2) limpar bin/obj e rebuild debug, " +
            "(3) executar scripts/check-ef-model.ps1. Evite criar migration sem diff real confirmado.");
        throw;
    }
    catch (Exception ex)
    {
        startupLogger.LogCritical(ex, "Falha na inicializacao de banco/seeds durante startup da API.");
        throw;
    }
}

app.UseForwardedHeaders();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

var swaggerOutsideDevelopmentHabilitado = builder.Configuration.GetValue<bool>("Swagger:EnableInNonDevelopment");
if (app.Environment.IsDevelopment() || swaggerOutsideDevelopmentHabilitado)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors(ServiceCollectionExtensions.AppCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapControllers();

app.Run();

public partial class Program;
