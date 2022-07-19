using System.ComponentModel.DataAnnotations;
using TrafficCourts.Common.Configuration.Validation;

namespace TrafficCourts.Citizen.Service.Configuration;

public class FormRecognizerOptions : IValidatable
{
    public const string Section = "FormRecognizer";
    public const string v2_1 = "2.1";
    public const string v2022_06_30_preview = "2022-06-30-preview";

    /// <summary>
    /// Azure FormRecognizer API KEY
    /// </summary>
    [Required]
    public string? ApiVersion { get; set; } = v2022_06_30_preview;

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
    public string? ModelId { get; set; } = "ViolationTicket_v4";

    public void Validate()
    {
        if (string.IsNullOrEmpty(ApiVersion)) throw new SettingsValidationException(Section, nameof(ApiVersion), "is required");
        if (string.IsNullOrEmpty(ApiKey)) throw new SettingsValidationException(Section, nameof(ApiKey), "is required");
        if (Endpoint is null) throw new SettingsValidationException(Section, nameof(Endpoint), "is required");
        if (string.IsNullOrEmpty(ModelId)) throw new SettingsValidationException(Section, nameof(ModelId), "is required");
    }

    public static FormRecognizerOptions Get(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new FormRecognizerOptions();
        configuration.GetSection(Section).Bind(options);
        return options;
    }
}
