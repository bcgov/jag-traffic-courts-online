using System.Diagnostics;

namespace TrafficCourts.Hotfix.DataMigration;

public static class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("hotfix-data-migration-api");
}
