using Winista.Mime;

namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// A class that contains the byte[] data (raw image) that is retrieved from the object store.
/// </summary>
public class ViolationTicketImage
{
    public ViolationTicketImage(byte[] Image, MimeType MimeType)
    {
        ArgumentNullException.ThrowIfNull(Image);
        ArgumentNullException.ThrowIfNull(MimeType);
        this.Image = Image;
        this.MimeType = MimeType;
    }

    /// <summary>
    /// The byte[] of the ViolationTicket image.
    /// </summary>
    public byte[] Image { get; set; }

    /// <summary>
    /// The MimeType of the image.
    /// </summary>
    public MimeType MimeType { get; set; }
}
