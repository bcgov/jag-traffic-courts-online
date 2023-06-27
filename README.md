[![Maintainability](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/maintainability)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/test_coverage)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/test_coverage)

[![img](https://img.shields.io/badge/Lifecycle-Experimental-339999)](https://github.com/bcgov/repomountie/blob/master/doc/lifecycle-badges.md)  ![Cucumber Tests](https://github.com/bcgov/jag-traffic-courts-online/workflows/Cucumber%20Tests/badge.svg)

# jag-traffic-courts-online

Welcome to Traffic Courts Online

## Project Structure

```
    ├── .docker
    │   └── keycloak-realms
    ├── .github
    │   └── workflows
    ├── .gitops
    │   ├── azure
    │   └── charts
    │       ├── jaeger-aio
    │       └── traffic-court-online
    ├── docs
    ├── infrastructure
    │   ├── certificates
    │   ├── keycloak
    │   └── openshift
    ├── splunk-dash-board
    ├── src
    │   ├── backend
    │   │   ├── TrafficCourts
    │   │   └── oracle-data-api
    │   └── frontend
    │       ├── citizen-portal
    │       └── staff-portal
    └── tools
    │   ├── form-recognizer
    │   ├── minio
    │   ├── network-tester
    │   ├── performance-testing
    │   └── ticket-generator
    ├── COMPLIANCE.yaml
    ├── CONTRIBUTING.md
    ├── docker-compose.yml
    ├── LICENSE                                  # Apache License
    └── README.md                                # This file
```
## Configuration

### Configuration Sources
The dotnet based projects can read and combine configuration from multiple sources. The configuration uses the default [Configuration in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0) and is summarized below. The configuration for each app is provided in the following order, from highest to lowest priority:

1. Command-line arguments using the Command-line configuration provider.
1. Non-prefixed environment variables using the Non-prefixed environment variables configuration provider.
1. User secrets when the app runs in the Development environment.
1. `appsettings.{Environment}.json` using the JSON configuration provider. For example, `appsettings.Production.json` and `appsettings.Development.json`.
1. appsettings.json using the JSON configuration provider.

### Translating configuration between providers

While the rest of this document will showcase configuration examples in environment variable format, the same configuration can be expressed via any of the other configuration sources.
For example, the serilog logging configuration in environment variable format can be expressed as:

```
Serilog__Using__0=Serilog.Sinks.Splunk
Serilog__WriteTo__0__Name=EventCollector
Serilog__WriteTo__0__Args__eventCollectorToken=00000000-0000-0000-0000-000000000000
Serilog__WriteTo__0__Args__splunkHost=http://localhost:8000
```
Environment variables use `__`(double underscore) as the hierarchical separator.

The same configuration can be expressed via structured json format in user secrets or appsettings.json files as,

```json
{
    "Serilog": {
        "Using": [
          "Serilog.Sinks.Splunk"
        ],
        "WriteTo": [
            { 
                "Name": "EventCollector",
                "Args": [
                    "eventCollectorToken": "00000000-0000-0000-0000-000000000000",
                    "splunkHost": "http://localhost:8000"
                ]
            }
        ]
    }
}
```

or a flat json format,

```json
{
    "Serilog__Using__0": "Serilog.Sinks.Splunk",
    "Serilog__WriteTo__0__Name": "EventCollector",
    "Serilog__WriteTo__0__Args__eventCollectorToken": "00000000-0000-0000-0000-000000000000",
    "Serilog__WriteTo__0__Args__splunkHost": "http://localhost:8000"
}
```

## arc-dispute-api 

| Key | Secret | Default | Description |
|--- | ------ |----------- |----------- |
| ASPNETCORE_ENVIRONMENT | | Production | |
| ASPNETCORE_URLS | | http://\*:8080 | |
| OTEL_EXPORTER_JAEGER_ENDPOINT | | http://jaeger-collector:14268/api/traces | |
| OTEL_EXPORTER_JAEGER_PROTOCOL | | http/thrift.binary | |
| Sftp__Host | Yes | 
| Sftp__SshPrivateKey | Yes | 
| Sftp__Username | Yes | 
| Serilog__Using__0 | | Serilog.Sinks.Splunk | |
| Serilog__WriteTo__0__Args__eventCollectorToken | Yes | 
| Serilog__WriteTo__0__Args__splunkHost | Yes | 
| Serilog__WriteTo__0__Name | | EventCollector | |
| Swagger__Enabled |

## citizen-api  

| Key | Secret | Default | Description |
|--- | ------ |--------- |----------- |
| ASPNETCORE_ENVIRONMENT | | Production | |
| ASPNETCORE_URLS | | http://\*:8080 | |
| FormRecognizer__ApiVersion
| FormRecognizer__ApiKey | Yes | 
| FormRecognizer__Endpoint | Yes | 
| FormRecognizer__ModelId | | |
| ObjectStorage__AccessKey | Yes | 
| ObjectStorage__BucketName | Yes | 
| ObjectStorage__Endpoint | Yes | 
| ObjectStorage__SecretKey | Yes | 
| RabbitMQ__Host
| RabbitMQ__Password | Yes | 
| RabbitMQ__Port
| RabbitMQ__Username | Yes | 
| RabbitMQ__VirtualHost | | / | |
| Redis__ConnectionString
| OTEL_EXPORTER_JAEGER_ENDPOINT | | http://jaeger-collector:14268/api/traces | |
| OTEL_EXPORTER_JAEGER_PROTOCOL | | http/thrift.binary | |
| Serilog__Using__0 | | Serilog.Sinks.Splunk | |
| Serilog__WriteTo__0__Args__eventCollectorToken | Yes | 
| Serilog__WriteTo__0__Args__splunkHost | Yes | 
| Serilog__WriteTo__0__Name | | EventCollector | |
| Swagger__Enabled | | false | |
| MassTransit__Transport | | RabbitMq | MassTransit transport. If not configured, will use RabbitMq |

## citizen-web

app.config.json

## oracle-data-api

| Key | Secret | Default | Description |
|--- | ------ |----------- |----------- |
| CODETABLE_REFRESH_CRON
| CODETABLE_REFRESH_ENABLED
| H2_DATASOURCE
| JAVA_OPTS
| OTEL_EXPORTER_JAEGER_ENDPOINT
| REDIS_HOST
| REDIS_PORT
| SPLUNK_TOKEN | Yes | 
| SPLUNK_URL | Yes | 
| UNASSIGN_DISPUTES_CRON
| UNASSIGN_DISPUTES_ENABLED

## staff-api

| Key | Secret | Default | Description |
|--- | ------ |--------- |----------- |
| ASPNETCORE_ENVIRONMENT | | Production | |
| ASPNETCORE_URLS | | http://\*:8080 | |
| ObjectStorage__AccessKey | Yes | 
| ObjectStorage__BucketName | Yes | 
| ObjectStorage__Endpoint | Yes | 
| ObjectStorage__SecretKey | Yes |
| OracleDataApi__BaseUrl | |
| OTEL_EXPORTER_JAEGER_ENDPOINT | | http://jaeger-collector:14268/api/traces | |
| OTEL_EXPORTER_JAEGER_PROTOCOL | | http/thrift.binary | |
| Serilog__Using__0 | | Serilog.Sinks.Splunk | |
| Serilog__WriteTo__0__Args__eventCollectorToken | Yes | 
| Serilog__WriteTo__0__Args__splunkHost | Yes | 
| Serilog__WriteTo__0__Name | | EventCollector | |
| Swagger__Enabled
| TicketStorage__Type
| MassTransit__Transport
| Jwt__Audience
| Jwt__Authority
| KeycloakAdminApi__BaseUri
| KeycloakAdminApi__Realm
| OAuth__TokenUri
| OAuth__ClientId
| OAuth__ClientSecret | Yes | 
| 

## staff-web

keycloak.config.json

## workflow-service

| Key | Secret | Default | Description |
|--- | ------ |--------- |----------- |
| ASPNETCORE_ENVIRONMENT | | Production | |
| ASPNETCORE_URLS | | http://\*:8080 | |
| OracleDataApi__BaseUrl | |
| OTEL_EXPORTER_JAEGER_ENDPOINT | | http://jaeger-collector:14268/api/traces | |
| OTEL_EXPORTER_JAEGER_PROTOCOL | | http/thrift.binary | |
| RabbitMQ__ClientProvidedName
| RabbitMQ__Host
| RabbitMQ__Password | Yes | 
| RabbitMQ__Port
| RabbitMQ__Username | Yes | 
| RabbitMQ__VirtualHost
| Serilog__Using__0 | | Serilog.Sinks.Splunk | |
| Serilog__WriteTo__0__Args__eventCollectorToken | Yes | 
| Serilog__WriteTo__0__Args__splunkHost | Yes | 
| Serilog__WriteTo__0__Name | | EventCollector | |
| Swagger__Enabled
| MassTransit__Transport

## Docker

[Download](https://www.docker.com/products/docker-desktop) and install Docker

## Run docker-compose

Copy the `.env.template` to `.env` and then run docker-compose up.
Add the configuration for token and password for splunk.
Default user is `admin`. Password is what is configured in `.env`

REDIS__HOST will be `redis` which is the service name.
```
docker-compose up
```

### Examples

Develop the `citizen-portal`. Run the associated API, `citizen-api`.  This starts the required services:

* citizen-api
* rabbitmq

```
docker-compose -f docker-compose.yml up -d citizen-api
```

Run `citizen-api` and have the backend logs go to [local Splunk](http://localhost:8000)

Note, this is currently getting an error: "Error response from daemon: network ... not found"

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.splunk.yml up -d citizen-api
```

To stop when running Splunk,

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.splunk.yml down
```

The frontend app citizen-portal will be accessible in the browser at http://localhost:8080 

To remove services run (all services and networking)
```
docker-compose down
```

## Application Services


| Service | URL      | Notes |
| ------------------------------- | -------------------------------------------- | ----- |
| citizen-portal | http://localhost:8080/ | |
| citizen-api | http://localhost:5000/swagger/index.html | |
| staff-portal | http://localhost:8081/ | |
| staff-api |  | |
| oracle-data-api | http://localhost:5010/ | |

## infrastructure Services
| Service | URL      | Notes |
| ------------------------------- | -------------------------------------------- | ----- |
| rabbitmq | localhost:5672, localhost:15672 | |
| minio | http://localhost:9001/login | |
| redis | localhost:6379 | |
| redis-commander | http://localhost:8082 | |
| splunk | http://localhost:8000 | |
| jaeger | http://localhost:16686 | |
| form-recognizer | http://localhost:5200 | |

### Logging

Developers can choose how logging is configured in the running apps. Developer can choose Splunk or Seq.

#### Splunk

The default `docker-compose.yaml` file does NOT contain Splunk configuration. To use/test with Splunk,
you can include a docker-compose override to configure Splunk for the requested services, ie,

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.splunk.yml up
```

Open [Local Splunk](http://localhost:8000) to view logs.  Login with username: admin, password: password.

#### Splunk 
A custom configuration file, `./.docker/splunk-dev-config.yaml`
is used to adjust the default settings. A key setting is disabling the SSL on the HEC endpoint.

See [Splunk Docker examples](https://splunk.github.io/docker-splunk/EXAMPLES.html#create-standalone-with-hec) for more information.

#### Seq

[Seq](https://datalust.co/seq) is an alternative logging source that is more developer friendly, especially those unfamilar with Splunk.

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.seq.yml up
```

Open [Local Seq](http://localhost:8001) to view logs.

### Redis

By default, redis runs in Standalone mode (a single container).
To run the project where redis is configured to run in sentinel mode (a high-availability failover configuration), specify the redis override file and run:
```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.redis.yml up -d
```

### Form Recognizer

A local containerized instance of Azure's Form Recognizer is available, consisting of 4 containers:
* azure-cognitive-service-proxy (an nginx router to the below services)
* azure-cognitive-service-custom-api (main api to the service)
* azure-cognitive-service-custom-layout (the container that does all the work)
* azure-cognitive-service-custom-supervised (tracks billing, submits to Azure)

```
docker-compose -f docker-compose.yml -f .docker/docker-compose-ocr.yml up -d
```

#### Configuration

| Environment variable | Description                                         | Default |
| ------------------------------- | --------------------------------------------------------------------------------------- | --------------------------------|
| FORMRECOGNIZER__APIVERSION | One of "2.1" or "2022-06-30-preview"                | 2022-06-30-preview |
| FORMRECOGNIZER__APIKEY | Billing key of Azure Form Recognizer (32 character GUID, excluding hypens) | |
| FORMRECOGNIZER__BILLING_URL | Azure endpoint for billing purposes (where the apikey is used) | |
| FORMRECOGNIZER__ENDPOINT | API endpoint                                        | |
| FORMRECOGNIZER__MODELID | If API version is 2.1, this is the 36 character GUID of the model id (including hypens). If API version is 2022-06-30-preview, this is the model name, ie. ViolationTicket | |

Use 2.1 as the FORMRECOGNIZER__APIVERSION to target Form Recognizer running locally in Docker (or in OpenShift).
Use 2022-06-30-preview to target Form Recognizer running in Azure cloud (which runs the latest api version, not 2.1).

The API key and Billing URL are found in the Azure Portal. Either Key 1 or Key 2 can be used for the API key.

![ Form Recognizer](docs/form-recognizer-api-key-and-billing-url.png)

See [Form Recognizer Containers (2.1)](https://learn.microsoft.com/en-us/azure/applied-ai-services/form-recognizer/containers/form-recognizer-container-install-run?view=form-recog-2.1.0&preserve-view=true&tabs=read) and [Form Recognizer Containers (3.0)](https://learn.microsoft.com/en-us/azure/applied-ai-services/form-recognizer/containers/form-recognizer-container-install-run?view=form-recog-3.0.0&tabs=read) for information.

## Debugging

A Developer can debug the application by attaching debugging instances to the processes running inside the docker containers.

### Oracle Data API

To debug a running java application inside a docker container:
1. Ensure the codebase is the same in Eclipse as what is running in the Docker container.
2. For security reasons, java applications do not run in debug mode by default. To launch the oracle-data-api in debug mode (a mode that can accept debugging sessions), set the necessary -X flags 
   on the container's entrypoint. This can be accomplish by simply having the JAVA_OPTS environment variable set in the .env file when lauching docker-compose. (The .env file is a file for custom env 
   variables and should be next to the project's docker-compose.yaml file).
   
   `JAVA_OPTS=-Xdebug -Xrunjdwp:server=y,transport=dt_socket,address=0.0.0.0:8000,server=y,suspend=n -Dlogging.level.ca.bc.gov.open.jag.tco.oracledataapi=DEBUG`

   The JAVA_OPTS environment variable is passed to the oracle-data-api Dockerfile when launching the container. The above configuration will enable the jvm to listen on port 8000 for debugging sessions. 
   If suspend=y, the application will pause before launching for any attaching debug instances so a developer can debug the application's startup process.
   If suspend=n, the application will launch as normal and one can attach a debugging instance later.
3. Create a Remote Java Application launch task in Eclipse.
   Run -> Debug Configurations -> add Remote Java Application
   Project: oracle-data-api
   Connection Type: Standard (Socket Attach)
   Connection Properties:
     Host: localhost
     Port: 8000
4. Done. The application should suspend at any breakpoints in the project.

### .NET projects (staff-api, citizen-api, workflow-service, etc)

To debug a running .NET application inside a docker container:
1. Ensure the codebase is the same in Visual Studio as what is running in the Docker container.
2. Navigate to Debug -> Attach to Process ...
3. Connection type: Docker (Linux Container)
                    Connection target: staff-api
   Click Attach
4. Done. The application should suspend at any breakpoints in the project.
   