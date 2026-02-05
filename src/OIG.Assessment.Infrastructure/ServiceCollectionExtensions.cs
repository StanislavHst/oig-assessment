using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OIG.Assessment.Domain.Repositories;
using OIG.Assessment.Infrastructure.Data;
using OIG.Assessment.Infrastructure.Repositories;

namespace OIG.Assessment.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string 'DefaultConnection' is not configured.", nameof(connectionString));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}

