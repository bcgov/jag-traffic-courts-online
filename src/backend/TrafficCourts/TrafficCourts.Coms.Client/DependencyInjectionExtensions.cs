using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using TrafficCourts.Coms.Client.Data;

namespace TrafficCourts.Coms.Client;

public static class DependencyInjectionExtensions
{
    public static void AddObjectManagementRepository(this IServiceCollection services)
    {
        NpgsqlConnectionStringBuilder connectionString = new NpgsqlConnectionStringBuilder();
        connectionString.Host = Environment.GetEnvironmentVariable("COMS_DB_HOST");
        connectionString.Database = Environment.GetEnvironmentVariable("COMS_DB_DATABASE");
        connectionString.Username = Environment.GetEnvironmentVariable("COMS_DB_USER");
        connectionString.Password = Environment.GetEnvironmentVariable("COMS_DB_PASSWORD");

        services.AddDbContext<ObjectManagementContext>(builder => builder.UseNpgsql(connectionString.ConnectionString));
        services.AddTransient<IObjectManagementRepository, ObjectManagementRepository>();
    }
}
