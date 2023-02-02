using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.Models;

[ExcludeFromCodeCoverage]
public record Language(
    string Code, 
    string Description
);
