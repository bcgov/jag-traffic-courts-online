using Microsoft.Extensions.Configuration;

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
}
