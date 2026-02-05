using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace OIG.Assessment.Api;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithDemoUser(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OIG Assessment API",
                Version = "v1"
            });

            c.AddSecurityDefinition("DemoUser", new OpenApiSecurityScheme
            {
                Name = "X-Demo-UserId",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description =
                    "Demo user id (Guid) used for permission checks.\n" +
                    "1) Отримай список користувачів: GET /api/users/selector\n" +
                    "2) Скопіюй Id потрібного користувача\n" +
                    "3) Натисни 'Authorize' вгорі праворуч і встав значення в поле 'X-Demo-UserId'."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "DemoUser"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}

