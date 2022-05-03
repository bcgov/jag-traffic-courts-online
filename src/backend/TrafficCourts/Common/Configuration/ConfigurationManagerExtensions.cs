using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace TrafficCourts.Common.Configuration;

public static class ConfigurationManagerExtensions
{
    /// <summary>
    /// Adds ini formatted Vault Secrets to the configuration
    /// </summary>
    /// <param name="configurationManager"></param>
    public static void AddVaultSecrets(this ConfigurationManager configurationManager, Serilog.ILogger logger)
    {
        // standard directory Vault stores secrets
        const string vaultSecrets = "/vault/secrets";

        if (!Directory.Exists(vaultSecrets))
        {
            logger.Information("Vault {Directory} does not exist, will not load Vault secrets", vaultSecrets);
            return;
        }

        foreach (var file in Directory.EnumerateFiles(vaultSecrets, "*.ini", SearchOption.TopDirectoryOnly))
        {
            logger.Debug("Loading secrets from {File}", file);
            configurationManager.AddIniFile(file, optional: false, reloadOnChange: false); // assume we can read
        }
    }


    /// <summary>
    /// Adds Redis connection.
    /// </summary>
    /// <param name="builder"></param>
    public static void AddRedis(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.ConfigureValidatableSetting<RedisOptions>(builder.Configuration.GetSection(RedisOptions.Section));

        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<RedisOptions>();
            ConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration.ConnectionString);
            return connectionMultiplexer;
        });
    }
}
