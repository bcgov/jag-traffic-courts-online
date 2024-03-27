using TrafficCourts.Domain.Models;

namespace TrafficCourts.Workflow.Service.Services.EmailTemplates;

public interface IDisputeSubmittedEmailTemplate : IEmailTemplate<Dispute>
{
}
