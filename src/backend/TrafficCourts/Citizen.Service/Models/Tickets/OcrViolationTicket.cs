namespace TrafficCourts.Citizen.Service.Models.Tickets;

/// <summary>
/// A model representation of the extracted OCR results.
/// </summary>
public class OcrViolationTicket
{
    // This list of static variables are the labels used in the Azure Form Recognizer
    public readonly static string TicketTitle = "Violation Ticket Label";
    public readonly static string ViolationTicketNumber = "Violation Ticket Number";
    public readonly static string Surname = "Surname";
    public readonly static string GivenName = "Given Name";
    public readonly static string ViolationDate = "Violation Date";
    public readonly static string ViolationTime = "Violation Time";
    public readonly static string OffenseIsMVA = "Offense is MVA";
    public readonly static string OffenseIsMCA = "Offense is MCA";
    public readonly static string OffenseIsCTA = "Offense is CTA";
    public readonly static string OffenseIsWLA = "Offense is WLA";
    public readonly static string OffenseIsFAA = "Offense is FAA";
    public readonly static string OffenseIsLCA = "Offense is LCA";
    public readonly static string OffenseIsTCR = "Offense is TCR";
    public readonly static string OffenseIsOther = "Offense is Other";
    public readonly static string Count1Description = "Count 1 Description";
    public readonly static string Count1ActRegs = "Count 1 Act/Regs";
    public readonly static string Count1IsACT = "Count 1 Is ACT";
    public readonly static string Count1IsREGS = "Count 1 Is REGS";
    public readonly static string Count1Section = "Count 1 Section";
    public readonly static string Count1TicketAmount = "Count 1 Ticket Amount";    
    public readonly static string Count2Description = "Count 2 Description";
    public readonly static string Count2ActRegs = "Count 2 Act/Regs";
    public readonly static string Count2IsACT = "Count 2 Is ACT";
    public readonly static string Count2IsREGS = "Count 2 Is REGS";
    public readonly static string Count2Section = "Count 2 Section";
    public readonly static string Count2TicketAmount = "Count 2 Ticket Amount";    
    public readonly static string Count3Description = "Count 3 Description";
    public readonly static string Count3ActRegs = "Count 3 Act/Regs";
    public readonly static string Count3IsACT = "Count 3 Is ACT";
    public readonly static string Count3IsREGS = "Count 3 Is REGS";
    public readonly static string Count3Section = "Count 3 Section";
    public readonly static string Count3TicketAmount = "Count 3 Ticket Amount";
    public readonly static string DetachmentLocation = "Detachment Location";
    public readonly static string DriverLicenceNumber = "Drivers Licence Number";

    /// Not all fields on the handwritten ticket are of value.
    /// This is a list of fields that are to be saved in ARC
    public readonly static List<string> FIELDS = new List<string>() {
        TicketTitle,
        ViolationTicketNumber,
        Surname,
        GivenName,
        ViolationDate,
        ViolationTime,
        OffenseIsMVA,
        OffenseIsMCA,
        OffenseIsCTA,
        OffenseIsWLA,
        OffenseIsFAA,
        OffenseIsLCA,
        OffenseIsTCR,
        OffenseIsOther,
        Count1Description,
        Count1ActRegs,
        Count1IsACT,
        Count1IsREGS,
        Count1Section,
        Count1TicketAmount,
        Count2Description,
        Count2ActRegs,
        Count2IsACT,
        Count2IsREGS,
        Count2Section,
        Count2TicketAmount,
        Count3Description,
        Count3ActRegs,
        Count3IsACT,
        Count3IsREGS,
        Count3Section,
        Count3TicketAmount,
        DetachmentLocation,
        DriverLicenceNumber
    };

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
    /// TODO: convert to a Dictionary or actual class Properties
    public List<Field> Fields { get; set; } = new List<Field>();

    /// <summary>Finds the field with the given name</summary>
    public OcrViolationTicket.Field? GetField(String name)
    {
        return Fields.Find(_ => name.Equals(_.Name));
    }

    public class Field
    {
        public String? Name { get; set; }
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
