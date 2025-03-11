using AutoFixture;
using System.Text.Json;
using TrafficCourts.OrdsDataService.Occam;

namespace TrafficCourts.OrdsDataService.Test;

public class BatchOperationTest
{
    private AutoFixture.Fixture _fixture;

    public BatchOperationTest()
    {
        _fixture = new AutoFixture.Fixture();
        _fixture.Customize(new YnPropertyCustomization());

        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketUploads>(_ => _.ViolationTicketUploadId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketUploads>(_ => _.UpdUserId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketUploads>(_ => _.Counts));

        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketCounts>(_ => _.ViolationTicketCountId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketCounts>(_ => _.ViolationTicketUploadId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamViolationTicketCounts>(_ => _.UpdUserId));

        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputes>(_ => _.DisputeId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputes>(_ => _.ViolationTicketUploadId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputes>(_ => _.UpdUserId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputes>(_ => _.ViolationTicketUpload));

        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputeCounts>(_ => _.DisputeCountId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputeCounts>(_ => _.DisputeId));
        _fixture.Customize(new IgnorePropertyCustomization<OccamDisputeCounts>(_ => _.UpdUserId));
    }

    [Fact]
    public void can_serialize_new_dispute_insert_operation()
    {
        OccamViolationTicketUploads ticket = Create();

        BatchDatabaseOperation operations = [.. ticket.ToInsertDatabaseOperations()];

        var json = JsonSerializer.Serialize(operations, options: new JsonSerializerOptions { WriteIndented = true });
    }


    [Fact]
    public void can_serialize_new_dispute_update_operation()
    {
        OccamViolationTicketUploads ticket = Create();

        BatchDatabaseOperation operations = new BatchDatabaseOperation();
        operations.Update(ticket);
        var json = JsonSerializer.Serialize(operations, options: new JsonSerializerOptions { WriteIndented = true });
    }

    [Fact]
    public void can_serialize_new_dispute_delete_operation()
    {
        OccamViolationTicketUploads ticket = Create();

        BatchDatabaseOperation operations = new BatchDatabaseOperation();
        operations.Delete(ticket);
        var json = JsonSerializer.Serialize(operations, options: new JsonSerializerOptions { WriteIndented = true });
    }

    [Fact]
    public void can_serialize_batch_request()
    {
        OccamDisputes dispute = new OccamDisputes()
        {
            DisputeId = Random.Shared.Next()
        };

        OccamDisputeCounts count = new OccamDisputeCounts()
        {
            DisputeCountId = Random.Shared.Next()
        };

        OccamAuditLogEntries auditLogEntry = new OccamAuditLogEntries
        {
            DisputeId = dispute.DisputeId,
            AuditLogEntryTypeCd = "SADM",
            ActionByApplicationUser = "John Doe"
        };


        BatchDatabaseOperation operations = new BatchDatabaseOperation();

        operations.Update(dispute);
        operations.Delete(count);
        operations.Insert(auditLogEntry);

        var json = JsonSerializer.Serialize(operations, options: new JsonSerializerOptions { WriteIndented = true });
    }


    private OccamViolationTicketUploads Create()
    {
        OccamViolationTicketUploads ticket = _fixture.Create<OccamViolationTicketUploads>();

        ticket.Dispute = _fixture.Create<OccamDisputes>();

        for (short count = 1; count <= 3; count++)
        {
            OccamViolationTicketCounts ticketCount = _fixture.Create<OccamViolationTicketCounts>();
            ticketCount.CountNo = count;
            ticketCount.Dispute = _fixture.Create<OccamDisputeCounts>();
            ticket.Counts.Add(ticketCount);
        }


        return ticket;
    }

}