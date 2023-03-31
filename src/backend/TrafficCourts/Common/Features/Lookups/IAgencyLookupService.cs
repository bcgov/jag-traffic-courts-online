using System.Diagnostics.CodeAnalysis;
using TrafficCourts.Common.Models;

namespace TrafficCourts.Common.Features.Lookups;

public interface IAgencyLookupService : ICachedLookupService<Agency>
{
}
