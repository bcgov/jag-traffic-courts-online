using System.ComponentModel.DataAnnotations;

namespace TrafficCourts.Citizen.Service.Configuration
{
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
    }
}
