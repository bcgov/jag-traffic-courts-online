using Microsoft.Extensions.Configuration;

namespace TrafficCourts.Common.Configuration
{
    public class SwaggerConfiguration
    {
        public const string Section = "Swagger";

        public bool Enabled { get; set; } = false;

        public static SwaggerConfiguration Get(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var swagger = new SwaggerConfiguration();
            configuration.GetSection(Section).Bind(swagger);
            return swagger;
        }
    }
}
