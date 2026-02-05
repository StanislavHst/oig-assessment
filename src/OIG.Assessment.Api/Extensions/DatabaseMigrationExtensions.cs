using Microsoft.EntityFrameworkCore;
using OIG.Assessment.Infrastructure.Data;

namespace OIG.Assessment.Api.Extensions;

public static class DatabaseMigrationExtensions
{
    public static async Task MigrateAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        await DbSeeder.SeedAsync(db);
    }
}
