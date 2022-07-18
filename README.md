[![Maintainability](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/maintainability)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/maintainability) [![Test Coverage](https://api.codeclimate.com/v1/badges/1b10997fdfad5bc3f42c/test_coverage)](https://codeclimate.com/github/bcgov/jag-traffic-courts-online/test_coverage)

[![img](https://img.shields.io/badge/Lifecycle-Experimental-339999)](https://github.com/bcgov/repomountie/blob/master/doc/lifecycle-badges.md)  ![Cucumber Tests](https://github.com/bcgov/jag-traffic-courts-online/workflows/Cucumber%20Tests/badge.svg)

# jag-traffic-courts-online

Welcome to Traffic Courts Online

## Project Structure

```
    ├── .github                                 # Contains GitHub Related sources
    ├── docs                                    # docs and images
    │   └── data-model                          # Oracle designer data model
    ├── splunk-dash-board
    ├── src                                      # application source files
    │   ├── backend                              # Backend code
    │       └── oracle-data-interface            # An Oracle Interface API
    │   └── frontend                             # Frontend code
    ├── tools
    ├── COMPLIANCE.yaml
    ├── CONTRIBUTING.md
    ├── docker-compose.yml
    ├── LICENSE                                  # Apache License
    └── README.md                                # This file
```
## Apps

| Name                | Description                                  | Doc                             |
| ------------------- | -------------------------------------------- | --------------------------------|
| backend             | all server side services                     |                                 |
| citizen api         | base citizen api                             | [README](src/backend/README.md)|
| ticket search api   | ticket search api called from citizen api    | [README](src/backend/README.md)|
| frontend            | all client side applications                 | [README](src/frontend/README.md)|

### Citizen Service (API)

#### Configuration

| Key                             | Description                                  | Default                         |
| ------------------------------- | -------------------------------------------- | --------------------------------|
| RabbitMQ:Host                   |                                              |                                 |
| RabbitMQ:Port                   |                                              |                                 |
| RabbitMQ:Username               |                                              |                                 |
| RabbitMQ:Password               |                                              |                                 |
| MassTransit:Transport           | InMemory or RabbitMQ                         |                                 |
| FormRecognizer:ApiKey           |                                              |                                 |
| FormRecognizer:Endpoint         |                                              |                                 |
| TicketSearchClient:Address      |                                              |                                 |

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
* ticket-search

```
docker-compose -f docker-compose.yml up -d citizen-api
```

Run `citizen-api` and have the backend logs go to [local Seq](http://localhost:8001),

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.seq.yml up -d citizen-api
```

To stop when running Seq,

```
docker-compose -f docker-compose.yml -f ./.docker/docker-compose.seq.yml down
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

## Services

| Service                         | URL                                          | Notes |
| ------------------------------- | -------------------------------------------- | ----- |
| citizen-portal                  | http://localhost:8080/                       |       |
| citizen-api                     | http://localhost:5000/swagger/index.html     |       |
| staff-portal                    | http://localhost:8081/                       |       |
| ticket-search                   | n/a                                          | grpc  |
| oraface-api                     | http://localhost:5010/                       |       |
| rabbitmq                        | localhost:5672, localhost:15672              |       |
| minio                           | http://localhost:9001/login                  |       |
| redis                           | localhost:6379                               |       |
| redis-commander                 | http://localhost:8082                        |       |
| splunk                          | http://localhost:8000                        |       |
| seq                             | http://localhost:8001                        |       |
| jaeger                          | http://localhost:16686                       |       |
| form-recognizer                 | http://localhost:5200                        |       |

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

| Environment variable            | Description                                                                             | Default                         |
| ------------------------------- | --------------------------------------------------------------------------------------- | --------------------------------|
| FORMRECOGNIZER__APIVERSION      | One of "2.1" or "2022-06-30-preview"                                                    | 2022-06-30-preview              |
| FORMRECOGNIZER__APIKEY          | Billing key of Azure Form Recognizer (32 character GUID, excluding hypens)              |                                 |
| FORMRECOGNIZER__BILLING_URL     | Azure endpoint for billing purposes (where the apikey is used)                          |                                 |
| FORMRECOGNIZER__ENDPOINT        | API endpoint                                                                            |                                 |
| FORMRECOGNIZER__MODELID         | If API version is 2.1, this is the 36 character GUID of the model id (including hypens). If API version is 2022-06-30-preview, this is the model name, ie. ViolationTicket | |

Use 2.1 as the FORMRECOGNIZER__APIVERSION to target Form Recognizer running locally in Docker (or in OpenShift).
Use 2022-06-30-preview to target Form Recognizer running in Azure cloud (which runs the latest api version, not 2.1).
