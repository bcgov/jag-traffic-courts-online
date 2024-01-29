using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Models;

[ExcludeFromCodeCoverage]
public record Country(
    string CtryId, 
    string CtryLongNm
);
