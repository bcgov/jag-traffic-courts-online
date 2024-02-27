using Microsoft.Extensions.Logging;
using System.Net;
using TrafficCourts.Core.Http.Models;

namespace TrafficCourts.Http;

/// <summary>
/// Provides logging tags for 
/// </summary>
internal static partial class TagProvider
{
    public static void RecordTags(ITagCollector collector, Token token)
    {
        if (token is not null)
        {
            collector.Add(nameof(token.AccessToken), token.AccessToken);
            collector.Add(nameof(token.RefreshExpiresIn), token.RefreshExpiresIn);
            collector.Add(nameof(token.RefreshToken), token.RefreshToken);
            collector.Add(nameof(token.Scope), token.Scope);
            collector.Add(nameof(token.SessionState), token.SessionState);
            collector.Add(nameof(token.TokenType), token.TokenType);
        }
    }

    public static void RecordExpiresAtTag(ITagCollector collector, DateTimeOffset expiresAt)
    {
        collector.Add("ExpiresAt", expiresAt);
    }

    public static void RecordCacheKeyTag(ITagCollector collector, string cacheKey)
    {
        if (cacheKey is not null)
        {
            collector.Add("CacheKey", cacheKey);
        }
    }

    public static void RecordHttpStatusCodeTag(ITagCollector collector, HttpStatusCode httpStatus)
    {
        collector.Add("HttpStatusCode", httpStatus);
    }

    public static void RecordDurationTag(ITagCollector collector, TimeSpan duration)
    {
        collector.Add("Duration", duration);
    }

    public static void RecordFailureCountTag(ITagCollector collector, int failureCount)
    {
        collector.Add("FailureCount", failureCount);
    }
}
