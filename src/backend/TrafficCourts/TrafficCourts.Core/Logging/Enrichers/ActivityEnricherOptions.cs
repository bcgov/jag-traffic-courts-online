namespace TrafficCourts.Logging.Enrichers;

public sealed record ActivityEnricherOptions(bool LogTraceId, bool LogSpanId, bool LogParentId);
