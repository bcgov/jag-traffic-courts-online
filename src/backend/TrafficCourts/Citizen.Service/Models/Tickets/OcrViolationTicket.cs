using System.Text.Json.Serialization;

namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class OcrViolationTicket
{

    public static readonly string ViolationTicketTitle = "violationTicketTitle";
    public static readonly string ViolationTicketNumber = "violationTicketNumber";
    public static readonly string Surname = "surname";
    public static readonly string GivenName = "givenName";
    public static readonly string DriverLicenceNumber = "driverLicenceNumber";
    public static readonly string ViolationDate = "violationDate";
    public static readonly string ViolationTime = "violationTime";
    public static readonly string OffenseIsMVA = "offenseIsMVA";
    public static readonly string OffenseIsMCA = "offenseIsMCA";
    public static readonly string OffenseIsCTA = "offenseIsCTA";
    public static readonly string OffenseIsWLA = "offenseIsWLA";
    public static readonly string OffenseIsFAA = "offenseIsFAA";
    public static readonly string OffenseIsLCA = "offenseIsLCA";
    public static readonly string OffenseIsTCR = "offenseIsTCR";
    public static readonly string OffenseIsOther = "offenseIsOther";
    public static readonly string Count1Description = "count1Description";
    public static readonly string Count1ActRegs = "count1ActRegs";
    public static readonly string Count1IsACT = "count1IsACT";
    public static readonly string Count1IsREGS = "count1IsREGS";
    public static readonly string Count1Section = "count1Section";
    public static readonly string Count1TicketAmount = "count1TicketAmount";
    public static readonly string Count2Description = "count2Description";
    public static readonly string Count2ActRegs = "count2ActRegs";
    public static readonly string Count2IsACT = "count2IsACT";
    public static readonly string Count2IsREGS = "count2IsREGS";
    public static readonly string Count2Section = "count2Section";
    public static readonly string Count2TicketAmount = "count2TicketAmount";
    public static readonly string Count3Description = "count3Description";
    public static readonly string Count3ActRegs = "count3ActRegs";
    public static readonly string Count3IsACT = "count3IsACT";
    public static readonly string Count3IsREGS = "count3IsREGS";
    public static readonly string Count3Section = "count3Section";
    public static readonly string Count3TicketAmount = "count3TicketAmount";
    public static readonly string DetachmentLocation = "detachmentLocation";

    /// <summary>
    /// A global confidence of correctly extracting the document. This value will be low if the title of this 
    /// Violation Ticket form is not found (or of low confidence itself) or if the main ticket number is missing or invalid.
    /// </summary>
    public float Confidence { get; set; }

    /// <summary>
    /// A list of global reasons why the global Confidence may be low (ie, missing ticket number, not a Violation Ticket, etc.)
    /// </summary>
    public List<string> ValidationErrors { get; set; } = new List<string>();

    /// <summary>
    /// An enumeration of all fields in this Violation Ticket.
    /// </summary>
    public Dictionary<string, Field> Fields { get; set; } = new Dictionary<string, Field>();

    public class Field
    {
        public String? Value { get; set; }
        public float? Confidence { get; set; }
        /// <summary>
        /// A list of field-specific reasons why the field Confidence may be low
        /// </summary>
        public List<string> Validation { get; set; } = new List<string>();
        public String? Type { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; } = new List<BoundingBox>();
    }

    public class BoundingBox
    {
        public List<Point> Points { get; set; } = new List<Point>();
    }

    public class Point
    {
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }
    }

}
