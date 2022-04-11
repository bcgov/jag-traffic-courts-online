using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Citizen.Service.Configuration;

public class FormRecognizerConfigurationOptions
{
    /// <summary>
    /// Azure FormRecognizer API KEY
    /// </summary>
    [Required]
    public string? ApiKey { get; set; }

    /// <summary>
    /// Azure FormRecognizer URL
    /// </summary>
    [Required]
    public Uri? Endpoint { get; set; }

    /// <summary>
    /// Azure FormRecognizer ModelId, the name of the model used to perform OCR processing.
    /// </summary>
    [Required]
    public string? ModelId { get; set; } = "ViolationTicket_v3";

}
