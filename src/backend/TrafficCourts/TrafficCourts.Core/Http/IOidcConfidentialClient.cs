using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Core.Http;

public interface IOidcConfidentialClient
{
    Task<Token?> RequestAccessTokenAsync(CancellationToken cancellationToken);
}
