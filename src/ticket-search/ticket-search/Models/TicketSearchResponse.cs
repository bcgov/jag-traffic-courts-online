using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Gov.TicketSearch.Models
{
    [ExcludeFromCodeCoverage(Justification = Justifications.Poco)]
    public class TicketSearchResponse
    {
        /// <summary>
        /// The traffic violation ticket number of the searching result
        /// </summary>
        public string ViolationTicketNumber { get; set; }

        /// <summary>
        /// The traffic violation time of the searching result in format of hh:mm
        /// </summary>
        public string ViolationTime { get; set; }

        /// <summary>
        /// The traffic violation date of the searching result in format of yyyy-mm-dd
        /// </summary>
        public string ViolationDate { get; set; }

        /// <summary>
        /// The list of all offences of the searching ticket
        /// </summary>
        public List<Offence> Offences { get; set; }
    }
}
