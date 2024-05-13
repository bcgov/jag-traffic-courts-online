using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Npgsql;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;

namespace TrafficCourts.Coms.Client.Test
{
    public class ComsContainers : IAsyncDisposable
    {
        private readonly string _tag;
        
        private string? _bucket;
        private INetwork? _network;

        private MinioContainer? _minioContainer;
        private PostgreSqlContainer? _postgresContainer;
        private IContainer? _comsContainer;

        public MinioContainer? Minio => _minioContainer;
        public PostgreSqlContainer? Postgres => _postgresContainer;
        public IContainer? Coms => _comsContainer;

        public ComsContainers(string tag = "0.4.2")
        {
            _tag = tag;
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
            var initContainer = GetComsContainer(_network, _postgresContainer, _minioContainer)
                                .WithCommand("npm", "run", "migrate")
                                .Build();

            await initContainer.StartAsync(cancellationToken);

            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            long rc = await initContainer.GetExitCodeAsync(cancellationToken);
            Assert.Equal(0, rc);

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

        private ContainerBuilder GetComsContainer(
            INetwork network,
            PostgreSqlContainer postgres, 
            MinioContainer minio)
        {
            var connectionString = new NpgsqlConnectionStringBuilder(postgres.GetConnectionString());

            var containerBuilder = new ContainerBuilder()
                .WithNetwork(network)
                .WithImage($"docker.io/bcgovimages/common-object-management-service:{_tag}")
                .WithEnvironment("DB_ENABLED", "true")
                .WithEnvironment("DB_DATABASE", connectionString.Database)
                .WithEnvironment("DB_HOST", postgres.Name[1..])
                .WithEnvironment("DB_PORT", "5432")
                .WithEnvironment("DB_USERNAME", connectionString.Username)
                .WithEnvironment("DB_PASSWORD", connectionString.Password)
                .WithEnvironment("BASICAUTH_USERNAME", "username")
                .WithEnvironment("BASICAUTH_PASSWORD", "password")
                .WithEnvironment("OBJECTSTORAGE_ENABLED", "true")
                .WithEnvironment("OBJECTSTORAGE_ACCESSKEYID", minio.GetAccessKey())
                .WithEnvironment("OBJECTSTORAGE_SECRETACCESSKEY", minio.GetSecretKey())
                .WithEnvironment("OBJECTSTORAGE_BUCKET", _bucket)
                .WithEnvironment("OBJECTSTORAGE_ENDPOINT", $"http://{minio.Name[1..]}:9000")
                .WithEnvironment("OBJECTSTORAGE_KEY", "/");

            return containerBuilder;
        }
    }
}
