using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace TrafficCourts.Staff.Service.Authentication;

public class ConfigureJwtBearerOptions : IConfigureOptions<JwtBearerOptions>
{
    private readonly IConfiguration _configuration;

    public ConfigureJwtBearerOptions(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public void Configure(JwtBearerOptions options)
    {
        options.Authority = _configuration["Jwt:Authority"];
        options.Audience = _configuration["Jwt:Audience"];
    }
}
