using TrafficCourts.Domain.Models;

namespace TrafficCourts.Staff.Service.Models.Disputes;

public record GetDisputeCountResponse(DisputeStatus Status, int Count);
