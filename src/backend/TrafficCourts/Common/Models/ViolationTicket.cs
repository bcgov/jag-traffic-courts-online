using System.Diagnostics.CodeAnalysis;

namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// An extension of the ViolationTicket object to include a ViolationTicketImage object.
/// </summary>
[ExcludeFromCodeCoverage]
public partial class ViolationTicket
{
    /// <summary>
    /// A non-persistent object (not in the Oracle database model) that wraps an uploaded image (the actual byte[])
    /// of the object store image reference in the ocrViolationTicket json property.
    /// </summary>
    public ViolationTicketImage? ViolationTicketImage { get; set; }
    public OcrViolationTicket? OcrViolationTicket { get; set;  }
}
