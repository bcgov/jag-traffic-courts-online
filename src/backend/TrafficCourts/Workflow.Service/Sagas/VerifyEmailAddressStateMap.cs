using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TrafficCourts.Workflow.Service.Sagas;

public class VerifyEmailAddressStateMap : SagaClassMap<VerifyEmailAddressState>
{
    protected override void Configure(EntityTypeBuilder<VerifyEmailAddressState> entity, ModelBuilder model)
    {
        // TODO: this duplicates the attributes stored on VerifyEmailAddressState, how can this be consolidated with the EF migrations?
        entity.Property(x => x.CorrelationId).HasMaxLength(64);
        entity.Property(x => x.EmailAddress).HasMaxLength(100); // occam_disputes.email_address_txt
        entity.Property(x => x.TicketNumber).HasMaxLength(50); // occam_violation_ticket_uploads.ticket_number_txt
    }
}
