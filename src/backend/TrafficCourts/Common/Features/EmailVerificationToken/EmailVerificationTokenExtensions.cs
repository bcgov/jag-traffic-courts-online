using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Features.EmailVerificationToken;

namespace TrafficCourts.Common;

[ExcludeFromCodeCoverage]
public static class EmailVerificationTokenExtensions
{
    public static IServiceCollection AddEmailVerificationTokens(this IServiceCollection services)
    {
        services.AddTransient<IDisputeEmailVerificationTokenEncoder, DisputeEmailVerificationTokenEncoder>();
        return services;
    }
}
