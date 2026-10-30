# AGENTS.md

## Business Overview

This repo contains the source code and database definitions for two related Traffic Courts Online web applications: the citizen portal and the staff portal.

The citizen portal is publicly available and citizens use it to submit disputes relating to traffic tickets they've received.

The staff portal is used by government staff to validate and process these disputes.

### Business Process

Citizens submit disputes using the citizen portal. These disputes appear in the Staff Workbench section of the staff portal, where Traffic Court staff approve or reject them. Once approved, disputes are submitted to a separate system, ARC. After doing this, Traffic Court staff use the JUSTIN application to schedule these disputes to be adjudicated by a judicial justice (JJ). If the dispute does not require the JJ to hear from the disputant in-person, the JJ adjudicates it as a written reason (WR) submission. Otherwise, a court date is scheduled and the dispute is adjudicated via a hearing. In either case, the JJ submits their decision using the JJ Workbench section of the staff portal. Traffic Court staff then validate the decision in the Staff Workbench section of the staff portal, concluding the dispute.

## Project Overview

### Angular Applications

- /src/frontend/citizen-portal
- /src/frontend/staff-portal

The citizen-portal front-end communicates with the Citizen.Service .NET web service.

The staff-portal front-end communicates with the Staff.Service .NET web service.

### .NET Services

- /src/backend/TrafficCourts/Citizen.Service
- /src/backend/TrafficCourts/Staff.Service
- /src/backend/TrafficCourts/Workflow.Service

The .NET services may contact external services, queue messages for RabbitMQ (consumed by Workflow.Service), or access the Oracle database.

### Java Service

- /src/backend/oracle-data-api

### Oracle Databases

The databases are accessed through ORDS directly via .NET or indirectly via Java.

- The .NET project TrafficCourts.OrdsDataService accesses ORDS directly.
- The .NET project TrafficCourts.OracleDataApi accesses the Java project oracle-data-api, which then accesses ORDS.

The databases' OpenAPI specs are used to generate C# code (through NSwag) in TrafficCourts.OrdsDataService or Java code (through openapi-generator-maven-plugin) in oracle-data-api:

- /src/backend/oracle-data-api/src/main/resources/occm-openapi-spec.yaml
- /src/backend/oracle-data-api/src/main/resources/tco-openapi-spec.yaml

The Java service is consumed only through /src/backend/TrafficCourts/TrafficCourts.OracleDataApi, which generates C# code for the Java service's OpenAPI spec using NSwag:

- /src/backend/TrafficCourts/TrafficCourts.OracleDataApi/Client/V1/OracleDataApi.v1_0.json

## Database Details

There are 5 database schema.

- /src/database/occam
- /src/database/tco
- /src/database/occamords
- /src/database/tcoords
- /src/database/justin

Each folder contains a .ddl file for the schema and may contain a definitions folder for packages, procedures, and triggers.

### occam

The occam database stores information about disputes submitted by citizens.

### tco

The tco database stores information about dispute adjudication. Once dispute adjudication has been scheduled in the JUSTIN application, information about the appearance is copied into the tco database, using data from both the tco and justin databases.

### occamords

The occamords database is the only way data from the occam database can be accessed by the citizen and staff portals.

### tcoords

The tcoords database is the only way data from the tco database can be accessed by the citizen and staff portals.

### justin

The justin database is primarily used by the JUSTIN application and deals with all types of court appearances. It is not under the control of this project.
