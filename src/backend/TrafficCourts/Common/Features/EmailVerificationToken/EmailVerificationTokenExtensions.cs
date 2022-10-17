using Microsoft.Extensions.DependencyInjection;
using TrafficCourts.Common.Features.EmailVerificationToken;

namespace TrafficCourts.Common;

public static class EmailVerificationTokenExtensions
{
    public static IServiceCollection AddEmailVerificationTokens(this IServiceCollection services)
    {
        services.AddTransient<IDisputeEmailVerificationTokenEncoder, DisputeEmailVerificationTokenEncoder>();
        return services;
    }
}
