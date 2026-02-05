using Microsoft.Extensions.DependencyInjection;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<IPermissionService, PermissionService>();
        return services;
    }
}

