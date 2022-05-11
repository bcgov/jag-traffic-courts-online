namespace TrafficCourts.Messaging.MessageContracts
{
    public class DisputedCount
    {
        /// <summary>
        /// Represents the dispuant plea on count.
        /// </summary>
        public Plea Plea { get; set; }
        
        /// <summary>
        /// The count number. Must be unique within an individual dispute.
        /// </summary>
        public int Count { get; set; }
        
        /// <summary>
        /// The disputant is requesting time to pay the ticketed amount.
        /// </summary>
        public bool RequestTimeToPay { get; set; }
        
        /// <summary>
        /// The disputant is requesting a reduction of the ticketed amount.
        /// </summary>
        public bool RequestReduction { get; set; }

        /// <summary>
        /// Does the want to appear in court?
        /// </summary>
        public bool AppearInCourt { get; set; }
    }
}
