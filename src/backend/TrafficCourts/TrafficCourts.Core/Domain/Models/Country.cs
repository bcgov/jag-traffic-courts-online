using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Domain.Models;

[ExcludeFromCodeCoverage]
public record Country(
    string CtryId, 
    string CtryLongNm
);
