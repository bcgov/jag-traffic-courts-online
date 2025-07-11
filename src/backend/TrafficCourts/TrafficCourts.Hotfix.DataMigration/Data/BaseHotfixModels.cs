using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Hotfix.DataMigration.Data
{
    /// <summary>
    /// Base class for dispute cache entries used across hotfixes
    /// </summary>
    public class HotfixOccamDispute
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Stores JSON data as a string in the database.
        /// </summary>
        public string? DataJson { get; set; }
    }

    public class HotfixViolationTicket
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Stores JSON data as a string in the database.
        /// </summary>
        public string? DataJson { get; set; }
    }
    
    /// <summary>
    /// Base class for ticket search cache entries used across hotfixes
    /// </summary>
    public class HotfixRSITicketSearch
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;

        public DateTime CachedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Stores JSON data as a string in the database.
        /// </summary>
        public string? DataJson { get; set; }
    }
}
