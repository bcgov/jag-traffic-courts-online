using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TrafficCourts.Citizen.Service.Services;

public static class DistributedCacheExtensions
{
    public static async Task<TData?> GetJsonAsync<TData>(this IDistributedCache cache, string key, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(key);

        var json = await cache.GetAsync(key, cancellationToken).ConfigureAwait(false);

        if (json is null)
        {
            return default;
        }

        // can throw JsonException
        var data = JsonSerializer.Deserialize<TData>(json);
        return data;
    }
}