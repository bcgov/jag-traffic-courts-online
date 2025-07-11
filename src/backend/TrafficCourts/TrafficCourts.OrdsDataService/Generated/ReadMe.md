# ORDS Client Generation with NSwag

This project uses [NSwag](https://github.com/RicoSuter/NSwag) and [NSwag Studio](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio) to generate the ORDS (Oracle REST Data Services) client code.

## OpenAPI Specification Location

Currently, the OpenAPI specifications used for client generation are maintained within the Java Oracle Data API project at:

- `src/backend/oracle-data-api/src/main/resources/occm-openapi-spec.yaml`
- `src/backend/oracle-data-api/src/main/resources/tco-openapi-spec.yaml`

The location of these specs is also referenced in the NSwag configuration file. In the future, it may be preferable to move these specifications to a common location in the repository hierarchy to facilitate reuse and easier maintenance across multiple services or clients.

## How It Works

- **NSwag Configuration**: The NSwag configuration file defines how the client code is generated from the OpenAPI/Swagger specification provided by ORDS.
- **NSwag Studio**: NSwag Studio is a graphical tool that allows you to visually configure and generate client code. You can open the NSwag config file in NSwag Studio, adjust settings as needed, and generate the client code for C# or TypeScript.

## Steps to Generate the ORDS Client

1. Open the NSwag configuration file (e.g., `nswag.json`) in [NSwag Studio](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio).
2. Ensure the OpenAPI/Swagger URL or file path is correct in the config.
3. Adjust any client generation settings as needed (e.g., namespace, output path).
4. Click **Generate Outputs** in NSwag Studio to produce the client code.
5. The generated client code will be placed in the specified output directory, ready for use in the project.

For more information, see the [NSwag documentation](https://github.com/RicoSuter/NSwag/wiki) and [NSwag Studio guide](https://github.com/RicoSuter/NSwag/wiki/NSwagStudio).

---

## Questions

- Why was the ORDS client not generated previously in the same way as the OracleDataApi client? Was there a technical or process-related reason for this difference?

