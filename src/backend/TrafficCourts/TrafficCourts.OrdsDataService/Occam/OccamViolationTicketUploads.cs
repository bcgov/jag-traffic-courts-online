namespace TrafficCourts.OrdsDataService.Occam;

public partial class OccamViolationTicketUploads
{
    public OccamViolationTicketUploads()
    {
        Counts = new List<OccamViolationTicketCounts>();
    }

    /// <summary>
    /// The ticket counts
    /// </summary>
    public List<OccamViolationTicketCounts> Counts { get; set; }

    /// <summary>
    /// The dispute related information for this ticket.
    /// </summary>
    public OccamDisputes? Dispute { get; set; }


    /// <summary>
    /// Creates the series of insert operations in the correct order
    /// to insert this ticket and its related counts and disputes.
    /// </summary>
    /// <returns>The list of insert operations</returns>
    public IEnumerable<IDictionary<string, object?>> ToInsertDatabaseOperations()
    {
        BatchDatabaseOperation operations = new BatchDatabaseOperation();

        // required to add the operations in the correct order
        // because the FK constraints will be fetched by currval
        // of the sequence automatically in the triggers
        yield return this.ToInsertOperation();

        if (Dispute is not null)
        {
            yield return Dispute.ToInsertOperation();
        }

        foreach (var count in this.Counts)
        {
            yield return count.ToInsertOperation();
            if (count.Dispute is not null)
            {
                yield return count.Dispute.ToInsertOperation();
            }
        }
    }
}
