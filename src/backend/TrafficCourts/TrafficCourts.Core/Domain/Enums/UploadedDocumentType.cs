using System.ComponentModel;

namespace TrafficCourts.Domain.Enums;

/// <summary>
/// The type of a document uploaded by staff or citizens to the document management service.
/// The <see cref="DescriptionAttribute"/> holds the exact value stored in the
/// document metadata, use <see cref="EnumExtensions.GetDescription(Enum)"/> to get it.
/// </summary>
public enum UploadedDocumentType
{
    [Description("Other")]
    Other = 1,

    [Description("Adjournment")]
    Adjournment = 2,

    [Description("Written Reasons")]
    WrittenReasons = 3,

    [Description("Certified Extract")]
    CertifiedExtract = 4,
}
