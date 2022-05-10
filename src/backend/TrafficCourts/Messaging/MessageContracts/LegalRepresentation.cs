namespace TrafficCourts.Messaging.MessageContracts
{
    public class LegalRepresentation
    {
        /// <summary>
        /// Name of the law firm that will represent the disputant at the hearing.
        /// </summary>
        public string LawFirmName { get; set; } = String.Empty;

        /// <summary>
        /// Full name of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        public string LawyerFullName { get; set; } = String.Empty;

        /// <summary>
        /// Email address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        public string LawyerEmail { get; set; } = String.Empty;

        /// <summary>
        /// Address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        public string LawyerAddress { get; set; } = String.Empty;

        /// <summary>
        /// Address of the lawyer who will represent the disputant at the hearing.
        /// </summary>
        public string LawyerPhoneNumber { get; set; } = String.Empty;
    }
}
