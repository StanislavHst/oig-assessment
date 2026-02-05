using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OIG.Assessment.Application.Permissions;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain.Services;

namespace OIG.Assessment.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(assembly);

        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<IPermissionChecker, PermissionChecker>();
        services.AddScoped<IGlobalAdminChecker, GlobalAdminChecker>();

        return services;
    }
}
