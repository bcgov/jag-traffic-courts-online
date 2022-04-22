using Microsoft.Extensions.Options;
using Renci.SshNet;
using System.Text;
using System.Text.Json;
using TrafficCourts.Arc.Dispute.Service;
using TrafficCourts.Arc.Dispute.Service.Configuration;
using TrafficCourts.Arc.Dispute.Service.Mappings;
using TrafficCourts.Arc.Dispute.Service.Services;
using TrafficCourts.Common;
using TrafficCourts.Common.Configuration;

var builder = WebApplication.CreateBuilder(args);
// determine if swagger is enabled or not
var swagger = SwaggerConfiguration.Get(builder.Configuration);

// Add services to the container.

builder.Services
    .AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
    .AddJsonOptions(options =>
    {
        options.UseDateOnlyTimeOnlyStringConverters();
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

if (swagger.Enabled)
{
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Registering and Initializing AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRecyclableMemoryStreams();

builder.Services.AddTransient<IArcFileService, ArcFileService>();

builder.Services.Configure<SftpConfig>(builder.Configuration.GetRequiredSection(SftpConfig.Section));

builder.Services.AddTransient<ISftpService, SftpService>();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger logger = loggerFactory.CreateLogger<Startup>();

builder.Services.AddTransient<SftpClient>(services =>
{
    var configurationOptions = services.GetRequiredService<IOptions<SftpConfig>>();
    var configuration = configurationOptions.Value;
    try
    {
        var sshPrivateKeyPath = configuration.SshPrivateKeyPath;

        if (!string.IsNullOrEmpty(sshPrivateKeyPath) && File.Exists(sshPrivateKeyPath))
        {
            var privateKey = new PrivateKeyFile(sshPrivateKeyPath);
            return new SftpClient(configuration.Host, configuration.Port, configuration.Username, new[] { privateKey });
        }

        return new SftpClient(configuration.Host, configuration.Port, configuration.Username, configuration.Password);
    }
    catch (System.IO.DirectoryNotFoundException exception)
    {
        logger.LogInformation(exception, "SSH key for the provided path has not been found. Using default password authentication.");
        return new SftpClient(configuration.Host, configuration.Port, configuration.Username, configuration.Password);
    }    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

if (swagger.Enabled)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => JsonUtils.ToSnakeCase(name);
}

public class JsonUtils
{

    private enum SeparatedCaseState
    {
        Start,
        Lower,
        Upper,
        NewWord
    }

    public static string ToSnakeCase(string s) => ToSeparatedCase(s, '_');

    private static string ToSeparatedCase(string s, char separator)
    {
        if (string.IsNullOrEmpty(s))
        {
            return s;
        }

        StringBuilder sb = new StringBuilder();
        SeparatedCaseState state = SeparatedCaseState.Start;

        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == ' ')
            {
                if (state != SeparatedCaseState.Start)
                {
                    state = SeparatedCaseState.NewWord;
                }
            }
            else if (char.IsUpper(s[i]))
            {
                switch (state)
                {
                    case SeparatedCaseState.Upper:
                        bool hasNext = (i + 1 < s.Length);
                        if (i > 0 && hasNext)
                        {
                            char nextChar = s[i + 1];
                            if (!char.IsUpper(nextChar) && nextChar != separator)
                            {
                                sb.Append(separator);
                            }
                        }
                        break;
                    case SeparatedCaseState.Lower:
                    case SeparatedCaseState.NewWord:
                        sb.Append(separator);
                        break;
                }

                char c;
                c = char.ToLowerInvariant(s[i]);
                sb.Append(c);

                state = SeparatedCaseState.Upper;
            }
            else if (s[i] == separator)
            {
                sb.Append(separator);
                state = SeparatedCaseState.Start;
            }
            else
            {
                if (state == SeparatedCaseState.NewWord)
                {
                    sb.Append(separator);
                }

                sb.Append(s[i]);
                state = SeparatedCaseState.Lower;
            }
        }

        return sb.ToString();
    }
}