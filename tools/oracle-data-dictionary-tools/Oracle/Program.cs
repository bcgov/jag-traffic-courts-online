using Microsoft.EntityFrameworkCore;
using Oracle;
using Oracle.ManagedDataAccess.Client;
using Oracle.DataDictionary;
using Microsoft.Extensions.Configuration;

internal class Program
{
    private static AppConfiguration GetConfiguration()
    {
        // Build configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<Program>();

        var appConfig = new AppConfiguration();
        IConfiguration configuration = builder.Build();
        configuration.GetSection("AppConfiguration").Bind(appConfig);

        return appConfig;
    }

    private static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        OracleConfiguration.OracleDataSources.Add(configuration.sid, $"(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={configuration.host})(PORT=1521))(CONNECT_DATA=(SID={configuration.sid})(SERVER=dedicated)))");

        var builder = new DbmlGenerationOptionsBuilder()
            .OutputSchemaName(true)
            //.AddSchema("OCCAM")
            //.AddSchema("TCO")
            .AddSchema("JUSTIN")
            ;

        // add connections for all the sample schemas
        builder.UseDbContext("TCO", CreateDbContext(configuration.sid, configuration.tco.username, configuration.tco.password));
        builder.UseDbContext("OCCAM", CreateDbContext(configuration.sid, configuration.occam.username, configuration.occam.password));
        builder.UseDbContext("JUSTIN", CreateDbContext(configuration.sid, configuration.justin.username, configuration.justin.password));

        var options = builder.Build();

        var generator = new OracleToDbml(options);
        var dbml = generator.Generate();

        Console.WriteLine(dbml);

        OracleDataDictionaryDbContext CreateDbContext(string dataSource, string username, string password)
        {
            username = username.ToLower();

            var optionsBuilder = new DbContextOptionsBuilder<OracleDataDictionaryDbContext>();
            optionsBuilder.UseOracle($"Data Source={dataSource};User ID={username};Password={password};");

            var dbContext = new OracleDataDictionaryDbContext(optionsBuilder.Options);
            return dbContext;
        }
    }
}

public class AppConfiguration
{
    public string host { get; set; }
    public string sid { get; set; }
    public UsernamePassword occam { get; set; }
    public UsernamePassword tco { get; set; }
    public UsernamePassword justin { get; set; }
}

public class UsernamePassword
{
    public string username { get; set; }
    public string password { get; set; }
}
