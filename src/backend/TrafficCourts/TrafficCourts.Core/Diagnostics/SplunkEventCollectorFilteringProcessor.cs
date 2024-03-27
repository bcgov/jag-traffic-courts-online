using OpenTelemetry;
using System.Diagnostics;

namespace TrafficCourts.Diagnostics;

/// <summary>
/// Filters successful POST messages the Splunk HEC server.
/// </summary>
public sealed class SplunkEventCollectorFilteringProcessor : BaseProcessor<Activity>
{
    public override void OnEnd(Activity activity)
    {
        if (activity.Tags.Any(IsHecServer) &&
            activity.Tags.Any(IsPost) &&
            activity.Tags.Any(IsOk))
        {
            // drop the activity
            activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
        }
    }

    private static bool IsHecServer(KeyValuePair<string, string?> tag) => tag.Key == "server.address" && tag.Value == "hec.monitoring.ag.gov.bc.ca";
    private static bool IsPost(KeyValuePair<string, string?> tag) => tag.Key == "http.request.method" && tag.Value == "POST";
    private static bool IsOk(KeyValuePair<string, string?> tag) => tag.Key == "http.response.status_code" && tag.Value == "200";
}
