# Application Source Code

This directory contains application source code.

## Components

| Component | Location | Talks to |
| --------- | -------- | -------- |
| citizen-portal | `frontend/citizen-portal` | citizen-api (`backend/TrafficCourts/Citizen.Service`) |
| staff-portal | `frontend/staff-portal` | staff-api (`backend/TrafficCourts/Staff.Service`) |
| citizen-api, staff-api | `backend/TrafficCourts` | External services, RabbitMQ, and Oracle (see below) |
| workflow-service | `backend/TrafficCourts/Workflow.Service` | Consumes RabbitMQ messages; reads and writes Oracle |
| arc-dispute-api | `backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service` | Builds ARC files and uploads them over SFTP |
| oracle-data-api | `backend/oracle-data-api` | ORDS (Java, Spring Boot) |

The .NET services may call external services, queue messages on RabbitMQ for workflow-service, or read and write Oracle.

## Paths to Oracle

The .NET services never connect to Oracle directly. They go through ORDS, by one of two paths:

- **Direct:** `TrafficCourts.OrdsDataService` calls ORDS from .NET.
- **Through Java:** `TrafficCourts.OracleDataApi` calls the Java `oracle-data-api`, which then calls ORDS.

ORDS exposes the OCCAM and TCO schemas only through the `occamords` and `tcoords` schemas (see `../database/README.md`). Both paths use clients generated from OpenAPI specs, described under "Generated clients" in [backend/README.md](backend/README.md#generated-clients). Query conventions for ORDS (filters, paging, includes) are in [ords-data-access.md](../docs/ords-data-access.md).

## Example of full data call stack
- Staff Portal (tco) - Ticket Validation Tab (pageload)
- calls `/api/dispute/disputes?...` 
 - which routes to `TrafficCourts.Staff.Service.DisputeController.GetDisputesAsync()`
    - which calls to `TrafficCourts.Staff.Service.DisputeService.GetAllDisputesAsync()`
        - which gets Cached Disputes
        - OR pulls fresh disputes if no cache, via `OracleDataApiService.GetAllDisputesAsync()`
            - some minor filtering happens here
            - then maps these into `TrafficCourts.Domain.Models.DisputeListItem`
        - Then gets Agencies lookups
        - then filters
        - then sorts
        - then pages
        - then returns `new PagedDisputeListItemCollection(results)`
    - then returns results
