using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using TrafficCourts.Common.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using TrafficCourts.Common.Configuration.Validation;

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
            throw new SettingsValidationException($"Required configuration section not found {_jwtBearerOptionsSection}");
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
