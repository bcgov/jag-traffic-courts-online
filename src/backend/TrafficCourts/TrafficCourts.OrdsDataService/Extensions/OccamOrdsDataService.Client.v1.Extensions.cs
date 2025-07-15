using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace TrafficCourts.OrdsDataService.Generated.OCCAM.Client.V1
{
    /// <summary>
    /// A custom contract resolver that can ignore the 'Required' property on JsonProperty attributes.
    /// This is useful when a generated client model marks properties as required, but the API
    /// sometimes omits them or sends null values. This resolver programmatically changes the
    /// contract for each property to make it not required, overriding the attribute's setting.
    /// </summary>
    public class TolerantContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            // The 'Required' property on the JsonProperty attribute overrides global settings.
            // If we set it back to Default, the global settings (like MissingMemberHandling) will be respected.
            if (property.Required != Required.Default)
            {
                property.Required = Required.Default;
            }

            // Also ensure NullValueHandling is set to Ignore for every property,
            // as this can also be specified on the attribute and cause issues.
            if (property.NullValueHandling != NullValueHandling.Ignore)
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            return property;
        }

        protected override JsonContract CreateContract(Type objectType)
        {
            var contract = base.CreateContract(objectType);

            // For object contracts, disable extension data handling to prevent additionalProperties
            if (contract is JsonObjectContract objectContract)
            {
                objectContract.ExtensionDataSetter = null;
                objectContract.ExtensionDataGetter = null;
            }

            return contract;
        }
    }

    public partial class OCCAMORDSDataServiceClientV1
    {
        // NSwag generates PrepareRequest partial methods that can be used to modify
        // the HttpRequestMessage before it is sent. Since base address and authorization
        // are now configured in the HttpClient setup, this method can be minimal.
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, StringBuilder url)
        {
            // Any additional request preparation can be done here if needed
        }

        static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings)
        {
            // Use a custom contract resolver to ignore the 'Required' setting on attributes.
            // This is the most effective way to handle this issue without modifying generated code.
            settings.ContractResolver = new TolerantContractResolver();

            // By default, the generated client requires all properties to be present in the JSON response.
            // However, some properties in the ORDS response may be null or omitted if they have no value.
            // This setting ensures that deserialization does not fail if a property is missing from the response.
            settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            settings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            // This setting handles cases where the API sends a null for a property that is a non-nullable value type in the C# model (e.g. long, int).
            // It instructs the deserializer to use the default value for the type (e.g. 0) instead of throwing an error.
            settings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
            
            // Prevent additional properties from being generated during serialization/deserialization
            // This ensures only defined properties are processed and unknown properties are ignored
            settings.MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Ignore;
            
            // Prevent serialization of default values to reduce additionalProperties generation
            settings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
        }
    }
}
