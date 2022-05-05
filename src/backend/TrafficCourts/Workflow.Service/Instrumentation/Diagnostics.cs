using System.Diagnostics;

namespace TrafficCourts.Workflow.Service;

public class Diagnostics
{
    public static readonly ActivitySource Source = new ActivitySource("workflow-service");
}
