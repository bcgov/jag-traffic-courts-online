using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Mail.Templates;

public interface IRejectedDisputeEmailTemplate : IEmailTemplate<Dispute>
{
}
