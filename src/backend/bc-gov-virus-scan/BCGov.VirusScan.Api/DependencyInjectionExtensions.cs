using Microsoft.IO;
using BCGov.VirusScan.Api.Network;
using BCGov.VirusScan.Api.Services;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddVirusScan(this IServiceCollection services)
    {
        services.AddSingleton<RecyclableMemoryStreamManager>();
        services.AddTransient<IRecyclableMemoryStreamManager, DefaultRecyclableMemoryStreamManager>();

        services.AddTransient<ITcpClient, TcpClientWrapper>();

        services.AddTransient<IVirusScanService, VirusScanService>();
        return services;
    }
}
