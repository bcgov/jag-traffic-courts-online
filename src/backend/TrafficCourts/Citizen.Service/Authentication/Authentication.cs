using Microsoft.AspNetCore.Authentication.JwtBearer;
using TrafficCourts.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Authentication;

public static class AuthenticationExtensions
{
    private const string _jwtBearerOptionsSection = "Jwt";

    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Note: AddJwtBearer does not use IConfigureOptions<JwtBearerOptions>
        var jwtOptions = configuration.GetSection(_jwtBearerOptionsSection).Get<JwtBearerOptions>();

        if (jwtOptions is null)
        {
            throw SettingsValidationException.RequiredSectionNotFound(_jwtBearerOptionsSection);
        }

        if (string.IsNullOrEmpty(jwtOptions.Authority)) throw new SettingsValidationException(_jwtBearerOptionsSection, nameof(JwtBearerOptions.Authority), "is required");
        if (string.IsNullOrEmpty(jwtOptions.Audience)) throw new SettingsValidationException(_jwtBearerOptionsSection, nameof(JwtBearerOptions.Audience), "is required");

        // Note: Authority must have trailing slash
        var authority = jwtOptions.Authority.EndsWith("/") ? jwtOptions.Authority : jwtOptions.Authority + "/";
        var audience = jwtOptions.Audience;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
            });

        return services;
    }
}
