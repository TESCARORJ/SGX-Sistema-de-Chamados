using Serilog;
using SGX.SistemaChamado.Api.Extensions;
using SGX.SistemaChamado.Api.Middlewares;
using SGX.SistemaChamado.Api.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args)
    .AddStructuredLogging();

builder.Services.AddApiServices(builder.Configuration, builder.Environment);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DevelopmentSeedService>();
    await seeder.SeedAsync();
}

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
}

app.UseHttpsRedirection();
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
