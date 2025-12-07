using DevHabit.Api.Database;
using DevHabit.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DevHabit.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext applicationDbContext = 
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await using ApplicationIdentityDbContext identityDbContext =
            scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

        try
        { 
            await applicationDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Application database migration applied successfully.");
            
            await identityDbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Identity database migration applied successfully.");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while applying the database migration.");
            throw;
        }
    }

    public static async Task SeedInitialDataAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        RoleManager<IdentityRole> roleManager =
            scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        try
        { 
            if(!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }
            if(!await roleManager.RoleExistsAsync(Roles.Member))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Member));
            }

            app.Logger.LogInformation("Successfully created roles");
        }
        catch (Exception e)
        {
            app.Logger.LogError(e, "An error occurred while seeding initial data.");
            throw;
        }
    }
}
