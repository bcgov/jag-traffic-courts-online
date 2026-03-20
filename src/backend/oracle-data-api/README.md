# Oracle Data API

## Eclipse IDE Setup

- Lombok is required to be enabled in Eclipse for this project to build correctly
  - Follow the instructions to install [Lombok in Eclipse](https://www.baeldung.com/lombok-ide#eclipse), using a recent version like [lombok-1.18.38.jar](https://repo1.maven.org/maven2/org/projectlombok/lombok/1.18.38/lombok-1.18.38.jar) instead of 1.18.4 from the example

- Enable Annotation Processing for the project in Eclipse
  - Right-click the project, select Properties, then navigate to Java Compiler → Annotation Properties, check Enable project specific settings, and Apply

- Update Maven dependencies
  - Right-click the project, select Maven → Update Project, check Force Update of Snapshots/Releases, and hit OK

## Online Court Case Management ORDS Service OpenAPI Specification

The specification can be found in the src/main/resources directory here: [occm-openapi-spec.yaml](src/main/resources/occm-openapi-spec.yaml)

You can format the specification file using [openapi-format](https://www.npmjs.com/package/openapi-format)

```bash
npx openapi-format occm-openapi-spec.yaml --output occm-openapi-spec.yaml --lineWidth 140
```

TODO: determine if we can sort the schema elements. Having the file sorted helps prevent merge conflicts.

# Links

[OpenAPI Specification](https://swagger.io/specification/)

[OpenAPI.Tools](https://openapi.tools/)

[Swagger Definition Objects Generator](https://roger13.github.io/SwagDefGen/) - Converts JSON request/response mocks to Swagger definitions.
