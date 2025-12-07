using DevHabit.Api;
using DevHabit.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationAsync();

    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

// Enable UseExceptionHandler middleware 
app.UseExceptionHandler();

// The order to implement this middleware are important
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
