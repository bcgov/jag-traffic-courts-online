using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using NodaTime;
using System.Configuration;
using TrafficCourts.Common.Configuration;

namespace TrafficCourts.Common.Features.FilePersistence
{
    public static class Extensions
    {
        /// <summary>
        /// Adds the file persistence storage.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ConfigurationErrorsException"></exception>
        public static IServiceCollection AddFilePersistence(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            TicketStorageConfiguration storageConfiguration = new();
            configuration.GetSection(TicketStorageConfiguration.Section).Bind(storageConfiguration);

            if (storageConfiguration.Type == TicketStorageType.ObjectStore)
            {
                // These appear to be pre-requisites.
                services.AddSingleton<IClock>(SystemClock.Instance); 
                services.AddRecyclableMemoryStreams();

                // note MinioClientConfiguration.Section == ObjectBucketConfiguration.Section
                return AddObjectStorageFilePersistence(services, configuration.GetSection(MinioClientConfiguration.Section));
            }
            else if (storageConfiguration.Type == TicketStorageType.InMemory)
            {
                return AddInMemoryFilePersistence(services);
            }

            throw new ConfigurationErrorsException($"Invalid TicketStorageType specified {storageConfiguration.Type}");
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

            services.UseConfigurationValidation();
            services.ConfigureValidatableSetting<MinioClientConfiguration>(configuration);
            services.ConfigureValidatableSetting<ObjectBucketConfiguration>(configuration);

            services.AddTransient<IObjectOperations>(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<MinioClientConfiguration>();

                var client = new MinioClient()
                    .WithEndpoint(configuration.Endpoint)
                    .WithCredentials(configuration.AccessKey, configuration.SecretKey);

                if (configuration.Ssl)
                {                    
                    client = client.WithSSL(); // WithSSL returns the same object, but re-assign to make it less confusing
                }
                    
                // the build method will finalize the object's internal state and returns the same instance
                return client.Build();
            });

            services.AddTransient<IFilePersistenceService, MinioFilePersistenceService>();

            return services;
        }
    }
}
