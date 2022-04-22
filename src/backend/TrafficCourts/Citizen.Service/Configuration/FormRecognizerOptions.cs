using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Configuration;

public class FormRecognizerOptions : IValidatable
{
    public const string Section = "FormRecognizer";

    /// <summary>
    /// Azure FormRecognizer API KEY
    /// </summary>
    [Required]
    public string? ApiKey { get; set; } = string.Empty;

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

    public void Validate()
    {
        if (string.IsNullOrEmpty(ApiKey)) throw new SettingsValidationException(Section, nameof(ApiKey), "is required");
        if (Endpoint is null) throw new SettingsValidationException(Section, nameof(Endpoint), "is required");
        if (string.IsNullOrEmpty(ModelId)) throw new SettingsValidationException(Section, nameof(ModelId), "is required");
    }
}
