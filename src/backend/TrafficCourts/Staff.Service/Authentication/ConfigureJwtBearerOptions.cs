using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        // https://github.com/dotnet/aspnetcore/blob/v6.0.2/src/Security/Authentication/JwtBearer/samples/JwtBearerSample/Startup.cs
        _configuration.Bind("Jwt", options);

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context => await OnTokenValidatedAsync(context)
        };
    }

    private static Task OnTokenValidatedAsync(TokenValidatedContext context)
    {
        if (context.SecurityToken is JwtSecurityToken accessToken
                && context?.Principal?.Identity is ClaimsIdentity identity
                && identity.IsAuthenticated)
        {
            identity.AddClaim(new Claim(ClaimTypes.Name, accessToken.Subject));
            FlattenRealmAccessRoles(identity);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Flattens the Realm Access claim, as Microsoft Identity Model doesn't support nested claims
    /// </summary>
    private static void FlattenRealmAccessRoles(ClaimsIdentity identity)
    {
        var realmAccessClaim = identity.Claims
            .SingleOrDefault(claim => claim.Type == Claims.RealmAccess)
            ?.Value;

        if (realmAccessClaim != null)
        {
            var realmAccess = JsonSerializer.Deserialize<RealmAccess>(realmAccessClaim);
            if (realmAccess?.Roles is not null)
            {
                identity.AddClaims(realmAccess.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }
        }
    }

    internal class RealmAccess
    {
        public string[]? Roles { get; set; }
    }
}
