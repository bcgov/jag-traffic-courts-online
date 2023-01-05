
# Usage:

Add configuration for accessing COMS. This is the structure,

```cs
public class ObjectManagementServiceConfiguration
{
    public string BaseUrl { get; set; } = string.Empty;        
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
```

Add call to `AddObjectManagementService` and pass the section name containing the configuration. The example assumes the configuration is in section COMS.

```
builder.Services.AddObjectManagementService("COMS");
```

Inject the service `IObjectManagementService` into your class.


# Client Code Generation

The [common-object-management-service](https://github.com/bcgov/common-object-management-service) 
OpenAPI specification is not friendly to code generators. I tried both [NSwag](https://github.com/RicoSuter/NSwag) and [
openapi-generator](https://github.com/OpenAPITools/openapi-generator). The use of deepObject type in the definition of 
[Query-TagSet](https://github.com/bcgov/common-object-management-service/blob/aa07302743730b0aaa131d6e4c8d0164498f67bc/app/src/docs/v1.api-spec.yaml#L992-L1003)
and additionally naming the parameter `tagset[*]` generates non-compilable code.

Tip: use [NSwagStudio](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio) on Windows.

The nswag definition checked in to the repo should only generate the model types `Models.g.cs`. To create the actual API service,
enable `generateClientClasses` and change the `output` to a different file name.  The file `ObjectManagementClient.g.cs` was created
and modified minimally to fix issues.  In partial class file `ObjectManagementClient.g.cs`, three helper methods were added to assist
adding tags and metadata fields to the appropriate operations.

Main Changes

* Changed metadata and tags parameters to be of type `IDictionary<string, string>?`
* In partial class file `ObjectManagementClient.g.cs`, added helper methods to assist adding tags and metadata fields to the appropriate operations
* Replaced code that adds tags and metadata to requests using helper methods

To reduce future maintenace when regenerating source from new specs, avoid:

* making cosmetic changes to the generated file
* changing or removing code that seems redundant or less efficient
* reorganizing the code
* changing parameter order