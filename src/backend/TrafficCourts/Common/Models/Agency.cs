using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Models;

[ExcludeFromCodeCoverage]
public record Agency(
    string Id, 
    string Name,
    string TypeCode
);
