# Common Document Generation Service (CDOGS)

# Credentials

Use the `Request Access` link on the [Common Service Showcase](https://bcgov.github.io/common-service-showcase/services/cdogs.html) page.

# Example Configuration (json)

```
{
  "Cdogs": {
    "ClientId": "",
    "ClientSecret": "",
    "TokenEndpoint": "https://dev.loginproxy.gov.bc.ca/auth/realms/comsvcauth/protocol/openid-connect/token",
    "Endpoint": "https://cdogs-dev.api.gov.bc.ca/api"
  }
}
```

# Usage 

## Adding to project

```c#
// pass the configuration object and the section the config is contained in
services.AddDocumentGenerationService("Cdogs");
```

##

```c#
// inject IDocumentGenerationService into your class
IDocumentGenerationService documentGenerationService;

// right now, we only accept a string template, we can extend to Word and Html as well
string template = "Hello {d.firstName} {d.lastName}!";

// data can be any class that will serialize to json
RenderedReport actual = await sut.UploadTemplateAndRenderReportAsync(
    template, 
    TemplateType.Text, 
    ConvertTo.Pdf, 
    "unit-test", 
    new { firstName = "Phil", lastName = "Bolduc"}, 
    CancellationToken.None);
```
