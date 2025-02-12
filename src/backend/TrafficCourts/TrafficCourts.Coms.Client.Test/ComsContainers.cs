using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Npgsql;
using System.Reactive.Joins;
using System.Text.RegularExpressions;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace TrafficCourts.Coms.Client.Test
{
    public class ComsContainers : IAsyncDisposable
    {
        /// <summary>
        /// The version of COMS we want to test
        /// </summary>
        private readonly string _tag;

        private readonly List<string> _migrationTags;
        
        private string? _bucket;
        private INetwork? _network;

        private MinioContainer? _minioContainer;
        private PostgreSqlContainer? _postgresContainer;
        private IContainer? _comsContainer;

        public MinioContainer? Minio => _minioContainer;
        public PostgreSqlContainer? Postgres => _postgresContainer;
        public IContainer? Coms => _comsContainer;

        /// <summary>
        /// A composite collection of test containers used to test COMS integration.
        /// </summary>
        /// <param name="tag">The version of COMS to run</param>
        /// <param name="migrationTags">
        /// Optional list of migration tags to apply. The tags will be applied in the order supplied.
        /// The <paramref name="tag"/> will be appended to the end
        /// if it does not already exist in the supplied tags.
        /// </param>
        /// <remarks>
        /// The <paramref name="migrationTags"/> are only required if you are looking to test applying a series of migrations
        /// to simulate an upgrade path. One would assume the latest migration would handle bringing the database to the 
        /// latest version. However, there could be situations where previous versions 
        /// had errors that prevent newer migrations from completing successfully.
        /// </remarks>
        public ComsContainers(string tag, params string[] migrationTags)
        {
            _tag = tag;

            _migrationTags = new List<string>(migrationTags);
            if (!_migrationTags.Contains(tag))
            {
                _migrationTags.Add(tag);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_minioContainer is not null) await _minioContainer.DisposeAsync();
            if (_postgresContainer is not null) await _postgresContainer.DisposeAsync();
            if (_comsContainer is not null) await _comsContainer.DisposeAsync();
        }

        private async Task<INetwork> BuildAndCreateNetwork(CancellationToken cancellationToken)
        {
            var network = new NetworkBuilder()
              .WithName($"network-{Guid.NewGuid():n}")
              .Build();

            await network.CreateAsync(cancellationToken)
                .ConfigureAwait(false);

            return network;
        }

        private async Task<PostgreSqlContainer> BuildAndStartPostgreSqlContainer(CancellationToken cancellationToken)
        {
            var container = new PostgreSqlBuilder()
                .WithImage("postgres:15.1")
                .WithHostname("postgres")
                .WithPortBinding(5432, 5432)
                .WithNetwork(_network)
                .Build();

            await container.StartAsync(cancellationToken).ConfigureAwait(false);

            return container;
        }

        private async Task<MinioContainer> BuildAndStartMinioContainer(CancellationToken cancellationToken)
        {
            var container = new MinioBuilder()
                .WithImage("quay.io/minio/minio")
                .WithPortBinding(59000, 9000)
                .WithPortBinding(59001, 59001)
                .WithName("minio")
                .WithNetwork(_network)
                .WithUsername("username")
                .WithPassword("password")
                .WithCommand("--console-address", ":59001")
                .Build();

            await container.StartAsync(cancellationToken).ConfigureAwait(false);

            
            return container;
        }

        private async Task<string> CreateBucket(bool enableVersioning, CancellationToken cancellationToken)
        {
            string bucket = $"bucket-{Guid.NewGuid():n}";

            var client = GetMinioClient();
            var args = new MakeBucketArgs()
                .WithLocation("us-east-1")
                .WithBucket(bucket);

            await client.MakeBucketAsync(args, cancellationToken);

            if (enableVersioning)
            {
                var vArgs = new SetVersioningArgs()
                    .WithVersioningEnabled()
                    .WithBucket(bucket);

                await client.SetVersioningAsync(vArgs, cancellationToken);

            }

            return bucket;
        }

        public async Task BuildAndStartAsync(bool enableBucketVersioning, CancellationToken cancellationToken)
        {
            _network = await BuildAndCreateNetwork(cancellationToken);

            _postgresContainer = await BuildAndStartPostgreSqlContainer(cancellationToken);

            _minioContainer = await BuildAndStartMinioContainer(cancellationToken);
            _bucket = await CreateBucket(enableBucketVersioning, cancellationToken);

            // run the database migrations
            foreach (var migrationTag in _migrationTags)
            {
                var container = GetComsDbMigrationContainer(migrationTag, _network, _postgresContainer)
                    .Build();

                await container.StartAsync(cancellationToken);

                long rc = await container.GetExitCodeAsync(cancellationToken);
                var logs = await container.GetLogsAsync();

                Assert.Equal(0, rc);

            }

            // run the coms service
            _comsContainer = GetComsContainer(_network, _postgresContainer, _minioContainer)
                .WithPortBinding(3000, true)
                .Build();

            await _comsContainer.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public ObjectManagementClient GetObjectManagementClient()
        {
            if (_comsContainer is null) throw new InvalidOperationException("Containers not built and started");

            ObjectManagementClient client = new ObjectManagementClient(new HttpClient
            {
                BaseAddress = new Uri($"http://127.0.0.1:{_comsContainer.GetMappedPublicPort(3000)}")
            });

            return client;
        }

        internal ObjectManagementService GetObjectManagementService()
        {
            ObjectManagementClient client = GetObjectManagementClient();

            var service = new ObjectManagementService(
                client, 
                new MemoryStreamFactory(() => new MemoryStream()), 
                NSubstitute.Substitute.For<ILogger<ObjectManagementService>>());

            return service;
        }

        public IMinioClient GetMinioClient()
        {
            if (_minioContainer is null) throw new InvalidOperationException("Containers not built and started");

            var client = new Minio.MinioClient()
                .WithRegion("us-east-1")
                .WithEndpoint("127.0.0.1", 59000)
                .WithCredentials(_minioContainer.GetAccessKey(), _minioContainer.GetSecretKey())
                .Build();

            return client;
        }

        private static ContainerBuilder GetComsCoreContainer(string tag, INetwork network, PostgreSqlContainer postgres)
        {
            var containerBuilder = new ContainerBuilder()
                .WithNetwork(network)
                .WithImage($"docker.io/bcgovimages/common-object-management-service:{tag}")
                .WithPostgres(postgres);

            return containerBuilder;
        }


        private static ContainerBuilder GetComsDbMigrationContainer(string tag, INetwork network, PostgreSqlContainer postgres)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(postgres.GetConnectionString());

            var containerBuilder = GetComsCoreContainer(tag, network, postgres)
                .WithCommand("npm", "run", "migrate")
                .WaitForContainerExited();

            return containerBuilder;
        }

        private ContainerBuilder GetComsContainer(
            INetwork network,
            PostgreSqlContainer postgres,
            MinioContainer minio)
        {
            var containerBuilder = GetComsContainer(_tag, _bucket, network, postgres, minio);
            return containerBuilder;
        }

        private static ContainerBuilder GetComsContainer(
            string tag,
            string? bucket,
            INetwork network,
            PostgreSqlContainer postgres,
            MinioContainer minio)
        {
            var containerBuilder = GetComsCoreContainer(tag, network, postgres)
                .WithBasicAuthentication("username", "password")
                .WithEnvironment("OBJECTSTORAGE_ENABLED", "true")
                .WithMinioObjectStorage(minio, bucket);

            return containerBuilder;
        }
    }
}

public static class ContainerExtensions
{
    public static System.Version ContainerVersion(this IContainer container)
    {
        var tag = container.Image.Tag;

        if (tag == "latest")
        {
            return new System.Version(0, 0, 0, 0);
        }

        try
        {
            return new System.Version(tag);
        }
        catch (FormatException)
        {
            // probably non-version type string like 2022-latest or RELEASE.2022-10-24T18-35-07Z
            return new System.Version(-1, -1, -1, -1);
        }
    }
}

public static class ContainerBuilderExtensions
{
    public static ContainerBuilder WithBasicAuthentication(this ContainerBuilder builder, string username, string password)
    {
        builder = builder
            .WithEnvironment("BASICAUTH_USERNAME", username)
            .WithEnvironment("BASICAUTH_PASSWORD", password);

        return builder;
    }

    public static ContainerBuilder WithPostgres(this ContainerBuilder builder, PostgreSqlContainer postgres)
    {
        var connectionString = new NpgsqlConnectionStringBuilder(postgres.GetConnectionString());

        builder = builder
            .WithEnvironment("DB_ENABLED", "true")
            .WithEnvironment("DB_DATABASE", connectionString.Database)
            .WithEnvironment("DB_HOST", postgres.Name[1..])
            .WithEnvironment("DB_PORT", "5432")
            .WithEnvironment("DB_USERNAME", connectionString.Username)
            .WithEnvironment("DB_PASSWORD", connectionString.Password);

        return builder;
    }

    public static ContainerBuilder WithMinioObjectStorage(this ContainerBuilder builder, MinioContainer minio, string? bucket)
    {
        builder = builder
            .WithEnvironment("OBJECTSTORAGE_ENABLED", "true")
            .WithEnvironment("OBJECTSTORAGE_ACCESSKEYID", minio.GetAccessKey())
            .WithEnvironment("OBJECTSTORAGE_SECRETACCESSKEY", minio.GetSecretKey())
            .WithEnvironment("OBJECTSTORAGE_BUCKET", bucket)
            .WithEnvironment("OBJECTSTORAGE_ENDPOINT", $"http://{minio.Name[1..]}:9000")
            .WithEnvironment("OBJECTSTORAGE_KEY", "/");

        return builder;
    }

    public static ContainerBuilder WaitForContainerExited(this ContainerBuilder builder)
    {
        builder = builder
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new ContainerExited()));

        return builder;
    }

    private sealed class ContainerExited : IWaitUntil
    {
        // The Flyway container will exit after executing the database migration. We do not
        // check if the migration was successful. To verify its success, we can either
        // check the exit code of the container or the console output, respectively the
        // standard output (stdout) or error output (stderr).
        public Task<bool> UntilAsync(IContainer container)
        {
            return Task.FromResult(TestcontainersStates.Exited.Equals(container.State));
        }
    }
}
