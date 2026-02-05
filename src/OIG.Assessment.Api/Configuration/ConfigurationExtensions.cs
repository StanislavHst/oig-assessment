using Npgsql;
using OIG.Assessment.Api.Configuration.Models;

namespace OIG.Assessment.Api.Configuration;

public static class ConfigurationExtensions
{
    public static string GetPostgreSQLConnectionString(this IConfiguration configuration)
    {
        var config = configuration.GetDbConnectionConfiguration();

        NpgsqlConnectionStringBuilder builder = new ()
        {
            Host = config.Host,
            Port = config.Port,
            Database = config.Name,
            Username = config.User,
            Password = config.Password,
        };

        return builder.ToString();
    }

    public static PostgreSqlConnectionConfiguration? GetDbConnectionConfiguration(this IConfiguration configuration) =>
        configuration.GetSection("Database").Get<PostgreSqlConnectionConfiguration>();
}