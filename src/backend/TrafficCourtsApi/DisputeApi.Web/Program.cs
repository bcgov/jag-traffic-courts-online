using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using TrafficCourts.Common.Configuration;

namespace DisputeApi.Web
{
    public class Program
    {
        public static int Main(string[] args)
        {
            // create default logger to ensure any configuration issues are appropriately logged
            Log.Logger = SerilogLogging.GetDefaultLogger<Program>();

            try
            {
                Log.Debug("Building web host");
                IHost host = CreateHostBuilder(args).Build();

                Log.Information("Starting web host");
                host.Run();
                return 0;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Web host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(SplunkEventCollector.Configure)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
