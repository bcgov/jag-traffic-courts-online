# Backend Source Code

This directory contains the application's backend source code

## Services

After running `docker-compose up` from the project root, these services should be available:

| Name                  | URL                                          | Notes
| --------------------- | -------------------------------------------- | --------------------------------------------
| oracle-data-api       | http://localhost:5010/swagger-ui/index.html  | The tool to query the API
|                       | http://localhost:5010/h2-console             | A database tool to execute manual SQL queries
| staff-api             | http://localhost:5005/swagger/index.html     | A bearer token is required to access the api
| TrafficCourts         | http://localhost:5000/swagger/index.html     | 
| Splunk                | http://localhost:8000                        | login with admin/password

## Description

### oracle-data-api
An API that acts as an interface between Oracle and the TrafficCourts API 

There is a swagger-ui tool useful to query the API endpoints.

There is an H2 console that is useful to execute manual SQL queries against the database.  
Set the JDBC URL parameter to "jdbc:h2:file:./data/h2" to connect, username is "sa", no password.

#### Configuration

| Key                         | Description
| --------------------------- | ----------------------------------------------------------------------------------------
| CODETABLE_REFRESH_ENABLED   | If enabled, this refresh will trigger a pull from JUSTIN to populate a cached copy of the lookup data in redis.
| CODETABLE_REFRESH_CRON      | A cron-like expression (defaulting to once per day at 3am), extending the usual UN*X definition to include triggers on the second, minute, hour, day of month, month, and day of week. 
|                             | 
|                             | For example, "0 * * * * MON-FRI" means once per minute on weekdays(at the top of the minute - the 0th second). 
|                             | 
|                             | The fields read from left to right are interpreted as follows:
|                             |  ┌───────────── second (0-59)
|                             |  │ ┌───────────── minute (0 - 59)
|                             |  │ │ ┌───────────── hour (0 - 23)
|                             |  │ │ │ ┌───────────── day of the month (1 - 31)
|                             |  │ │ │ │ ┌───────────── month (1 - 12) (or JAN-DEC)
|                             |  │ │ │ │ │ ┌───────────── day of the week (0 - 7)
|                             |  │ │ │ │ │ │          (0 or 7 is Sunday, or MON-SUN)
|                             |  │ │ │ │ │ │
|                             |  * * * * * *
| REDIS_HOST                  | When redis is configured in a standalone configuration, the hostname of a redis server used to cache JUSTIN lookup data. Not used in sentinal configuration.
| REDIS_PORT                  | When redis is configured in a standalone configuration, the port of redis server. Not used in sentinal configuration.
| REDIS_PASSWORD              | The password of redis server (either configuration).
| REDIS_SENTINAL_MASTER       | When redis is configured in a master-slave-sentinel configuration, the name of the master node, ie. "mymaster"
| REDIS_SENTINAL_NODES        | When redis is configured in a master-slave-sentinel configuration, a comma-separated list of host:port sentinal nodes. ie "redis-sentinel-1:26379,redis-sentinel-2:26380,redis-sentinel-3:26381"
| SPLUNK_URL                  | 
| SPLUNK_TOKEN                | 
| ORDS_API_TIMEOUT            | Connection timeout limit in MS.
| ORDS_API_RETRY_COUNT        | The maximum number of retry attempts to allow.
| ORDS_API_RETRY_DELAY        | The Duration of the fixed delays.
| ORDS_API_AUTH_ENABLED       | If enabled, this will add basic authentication using the credentials below for all ORDS API calls.
| ORDS_API_OCCAM_URL          | The URL to connect to ORDS OCCAM. (Needs to be set in .env file to connect to ORDS successfully when running through docker-compose)
| ORDS_API_OCCAM_USERNAME     | Basic auth username for calling ORDS OCCAM.
| ORDS_API_OCCAM_PASSWORD     | Basic auth password for calling ORDS OCCAM.
| ORDS_API_TCO_URL            | The URL to connect to ORDS TCO. (Needs to be set in .env file to connect to ORDS successfully when running through docker-compose)
| ORDS_API_TCO_USERNAME       | Basic auth username for calling ORDS TCO.
| ORDS_API_TCO_PASSWORD       | Basic auth password for calling ORDS TCO.
| JAVA_OPTS                   | JVM parameters to be passed to the container. ie, "-Dlogging.level.ca.bc.gov.open.jag.tco.oracledataapi=DEBUG"

Note: Redis Standalone and Sentinel modes are mutually exclusive.  Either use host/port variables or master/nodes.

Code tables can be refreshed manually at any time by hitting the "/codetable/refresh" endpoint.

### TrafficCourts
An API for creating violation ticket disputes

#### Configuration

| Key                   | Description                                  | Notes
| --------------------- | -------------------------------------------- | --------------------------------------------
| RabbitMQ:Host         |                       | 
| RabbitMQ:Port         |                       | Defaults to 5672
| RabbitMQ:Username     |                       | 
| RabbitMQ:Password     |                       | 
| MassTransit:Transport |                       | 
| FormRecognizer        |                       | 
| TicketSearchClient    |                       | 
| Splunk                |                       | 
| TicketStorage         | InMemory or ObjectStore                      | In memory can be used for development. ObjectStore is S3 storage, minio locally.
| ObjectStorage:Endpoint |
| ObjectStorage:AccessKey |
| ObjectStorage:SecretKey |
| ObjectStorage:BucketName |

Local object store example. Run `docker-compose up minio createbuckets`

```
{
  "TicketStorage": "ObjectStore",
  "ObjectStorage": {
    "Endpoint": "localhost:9000",
    "AccessKey": "username",
    "SecretKey": "password",
    "BucketName": "traffic-ticket-dev"
  }
}
```

### Splunk
A logging collector tool to capture, index, and correlate logs in a searchable repository.

A sample query might be (to find all logs produced from the Oracle Data API:
`source="oracle-data-api"`

