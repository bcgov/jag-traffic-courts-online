using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;

namespace TrafficCourts.Common.Configuration
{
    public static class SerilogLogging
    {
        /// <summary>
        /// Creates the default logger used for the entry point program.
        /// </summary>
        public static ILogger GetDefaultLogger<TProgram>() where TProgram : class
        {
            IConfiguration configuration = GetConfiguration<TProgram>();

            // we are creating the default logger for the entry point, write to the console as well
            Logger logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            return logger;
        }

        /// <summary>
        /// Get the configuration in the same way WebHost.CreateDefaultBuilder builds the configuration.
        /// Configuration is required before the WebHost is built, so we need to duplicate the logic.
        /// </summary>
        private static IConfiguration GetConfiguration<TProgram>() where TProgram : class
        {
            // based on the Serilog EarlyInitializationSample
            // https://github.com/serilog/serilog-aspnetcore/blob/dev/samples/EarlyInitializationSample/Program.cs
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment ?? "Production"}.json", optional: true);

            if (environment == "Development")
            {
                configurationBuilder.AddUserSecrets<TProgram>(optional: true);
            }

            configurationBuilder.AddEnvironmentVariables();


            IConfiguration configuration = configurationBuilder.Build();
            return configuration;
        }
    }
}