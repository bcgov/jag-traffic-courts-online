namespace TrafficCourts.Common.OpenAPIs.OracleDataApi.v1_0;

/// <summary>
/// A class that contains the byte[] data (raw image) that is retrieved from the object store.
/// </summary>
public class ViolationTicketImage
{
    public ViolationTicketImage(byte[] image, string mimeType)
    {
        Image = image ?? throw new ArgumentNullException(nameof(image));
        MimeType = mimeType ?? throw new ArgumentNullException(nameof(mimeType));
    }

    /// <summary>
    /// The byte[] of the ViolationTicket image.
    /// </summary>
    public byte[] Image { get; set; }

    /// <summary>
    /// The MimeType of the image.
    /// </summary>
    public string MimeType { get; set; }
}
