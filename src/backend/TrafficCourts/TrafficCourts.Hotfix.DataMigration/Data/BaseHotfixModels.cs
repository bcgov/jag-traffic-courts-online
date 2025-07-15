using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Hotfix.DataMigration.Data
{

    public class BaseHotfix
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string? BeforeHotfixDataJson { get; set; }
        public string? HotfixUpdateDataJson { get; set; }
        public string? AfterHotfixDataJson { get; set; }
        public Boolean IsHotfixApplied { get; set; } = false;
        public Boolean IsIntegrityCheckPassed { get; set; } = false;
        public string? IntegrityCheckFailureReason { get; set; }
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Base class for ticket search cache entries used across hotfixes
    /// </summary>
    public class HotfixRSITicketSearch : BaseHotfix
    {
    }

    public class HotfixOccamDispute : BaseHotfix
    {
        public int DisputeId { get; set; }
    }

    public class HotfixViolationTicket : BaseHotfix
    {

        public int DisputeId { get; set; }
    }


}
