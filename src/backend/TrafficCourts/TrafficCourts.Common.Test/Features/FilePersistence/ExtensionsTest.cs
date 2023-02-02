using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TrafficCourts.Common.Features.FilePersistence;
using Xunit;

namespace TrafficCourts.Common.Test.Features.FilePersistence
{
    public class ExtensionsTest
    {
        [Fact]
        public void should_add_transient_IFilePersistenceService_with_type_InMemoryFilePersistenceService()
        {
            ServiceCollection services = new ServiceCollection();

            var actual = Common.Features.FilePersistence.Extensions.AddInMemoryFilePersistence(services);

            services.Single(ExistsTransient<IFilePersistenceService, InMemoryFilePersistenceService>);
            Assert.Same(services, actual);
        }

        [Fact]
        public void should_add_transient_IFilePersistenceService_with_type_MinioFilePersistenceService()
        {
            ServiceCollection services = new ServiceCollection();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();

            var actual = Common.Features.FilePersistence.Extensions.AddObjectStorageFilePersistence(services, configurationMock.Object);

            services.Single(ExistsTransient<IFilePersistenceService, MinioFilePersistenceService>);
            Assert.Same(services, actual);
        }

        [Fact]
        public void can_resolve_IObjectOperations()
        {
            Fixture fixture = new Fixture();

            // create services
            ServiceCollection services = new ServiceCollection();

            // create IConfiguration with required data
            var values = new Dictionary<string, string?>
            {
                { nameof(MinioClientConfiguration.Endpoint), fixture.Create<string>() },
                { nameof(MinioClientConfiguration.AccessKey), fixture.Create<string>() },
                { nameof(MinioClientConfiguration.SecretKey), fixture.Create<string>() }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

            // register types
            Common.Features.FilePersistence.Extensions.AddObjectStorageFilePersistence(services, configuration);

            // resolve IObjectOperations
            var actual = services.BuildServiceProvider().GetService<IObjectOperations>();

            Assert.NotNull(actual);
        }

        private bool ExistsTransient<TService, TImplementation>(ServiceDescriptor descriptor)
        {
            return descriptor.Lifetime == ServiceLifetime.Transient &&
                   descriptor.ServiceType == typeof(TService) &&
                   descriptor.ImplementationType == typeof(TImplementation);
        }
    }
}
