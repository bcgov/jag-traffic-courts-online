using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Mail.Templates;

public interface ICancelledDisputeEmailTemplate : IEmailTemplate<Dispute>
{
}
