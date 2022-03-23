using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;

namespace TrafficCourts.Common.Features.FilePersistence
{
    public static class Extensions
    {
        public static IServiceCollection AddInMemoryFilePersistence(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddTransient<IFilePersistenceService, InMemoryFilePersistenceService>();
            return services;
        }

        public static IServiceCollection AddObjectStorageFilePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddOptions<MinioClientConfiguration>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.AddOptions<MinioConfiguration>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.AddSingleton<MinioConfiguration>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MinioConfiguration>>();
                var configuration = options.Value;
                return configuration;
            });

            services.AddTransient<MinioClient>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MinioClientConfiguration>>();
                var configuration = options.Value;

                var client = new MinioClient(configuration.Endpoint, configuration.AccessKey, configuration.SecretKey);
                return client;
            });

            services.AddTransient<IFilePersistenceService, MinioFilePersistenceService>();

            return services;
        }
    }
}
