using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using NodaTime;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Common.Features.FilePersistence
{
    public static class Extensions
    {
        public static IServiceCollection AddFilePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            TicketStorageConfiguration ticketStorageConfiguration = new();
            configuration.GetSection(TicketStorageConfiguration.Section).Bind(ticketStorageConfiguration);

            if (ticketStorageConfiguration.Type == TicketStorageType.ObjectStore)
            {
                // These appear to be pre-requisites.
                services.AddSingleton<IClock>(SystemClock.Instance); 
                services.AddRecyclableMemoryStreams();

                return AddObjectStorageFilePersistence(services, configuration.GetSection("ObjectStorage"));
            }
            else
            {
                return AddInMemoryFilePersistence(services);
            }
        }

        public static IServiceCollection AddInMemoryFilePersistence(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);
            services.AddMemoryCache();
            services.AddTransient<IFilePersistenceService, InMemoryFilePersistenceService>();
            return services;
        }

        public static IServiceCollection AddObjectStorageFilePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.AddOptions<ObjectBucketConfiguration>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.AddOptions<MinioClientConfiguration>()
                .Bind(configuration)
                .ValidateDataAnnotations();

            services.AddTransient<IObjectOperations>(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<MinioClientConfiguration>>();
                var configuration = options.Value;
                
                var client = new MinioClient()
                    .WithEndpoint(configuration.Endpoint)
                    .WithCredentials(configuration.AccessKey, configuration.SecretKey)
                    .Build();
                
                return client;
            });

            services.AddTransient<IFilePersistenceService, MinioFilePersistenceService>();

            return services;
        }
    }
}
