using DevHabit.Api;
using DevHabit.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationAsync();
}

app.UseHttpsRedirection();

// Enable UseExceptionHandler middleware 
app.UseExceptionHandler();

app.MapControllers();

await app.RunAsync();
