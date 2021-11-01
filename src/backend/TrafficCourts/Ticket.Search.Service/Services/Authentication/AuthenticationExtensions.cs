using TrafficCourts.Common.Configuration;
using TrafficCourts.Ticket.Search.Service.Configuration;

namespace TrafficCourts.Ticket.Search.Service.Services.Authentication
{
    public static class AuthenticationExtensions
    {
        public static void UseAuthenticationClient(this WebApplicationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Configuration.Add<TicketSearchServiceConfigurationProvider>();
            var configuration = builder.Configuration.Get<TicketSearchServiceConfiguration>();

            builder.Services.AddHttpClient<AuthenticationClient>(client =>
            {
                client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
                client.DefaultRequestHeaders.Add("return-client-request-id", "true");
            });

            builder.Services.AddTransient<IAuthenticationClient, AuthenticationClient>();

            builder.Services.AddSingleton(configuration.Credentials);

        }
    }
}
