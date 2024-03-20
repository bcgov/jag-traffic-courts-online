using TrafficCourts.Domain.Models;

namespace TrafficCourts.Common.Features.Mail.Templates;

public interface IConfirmationEmailTemplate : IEmailTemplate<Dispute>
{
}
