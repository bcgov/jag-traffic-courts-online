using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace TrafficCourts.Cdogs.Client.Test.DocumentGenerationService
{
    public abstract class DocumentGenerationServiceTests
    {
        protected static IConfiguration BuildConfiguration<T>(string section, T configuration, bool includeNullValues = false)
        {
            var items = ToDictionary(section, configuration, includeNullValues);

            return new ConfigurationBuilder()
                .AddUserSecrets<DocumentGenerationServiceTests>()
                .AddInMemoryCollection(items)
                .Build();
        }

        protected static Dictionary<string, string?> ToDictionary<T>(string section, T value, bool includeNullValues)
        {
            Dictionary<string, string?> dictionary = new();

            // Get all public properties of the object using reflection
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                // Get the property value from the object
                object? propertyValue = property.GetValue(value);

                if (propertyValue is not null)
                {
                    string? propertyStringValue = propertyValue.ToString();
                    dictionary.Add($"{section}:{property.Name}", propertyStringValue);
                }
                else if (includeNullValues)
                {
                    dictionary.Add($"{section}:{property.Name}", null);
                }
            }

            return dictionary;
        }

    }
}
