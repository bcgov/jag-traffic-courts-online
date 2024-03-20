using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Domain.Models;

[ExcludeFromCodeCoverage]
public record Language(
    string Code, 
    string Description
);
