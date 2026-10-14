# System Overview

Traffic Courts Online (TCO) lets BC citizens dispute traffic violation tickets online. Court staff use a separate portal to process those disputes.

## Main components

| Component | Tech | Location |
|---|---|---|
| Citizen Portal | Angular 16 | `src/frontend/citizen-portal` |
| Staff Portal | Angular 21 | `src/frontend/staff-portal` |
| Citizen.Service (API) | .NET 8 | `src/backend/TrafficCourts/Citizen.Service` |
| Staff.Service (API) | .NET 8 | `src/backend/TrafficCourts/Staff.Service` |
| Workflow.Service | .NET 8, message consumers | `src/backend/TrafficCourts/Workflow.Service` |
| ARC Dispute Service | .NET 8 | `src/backend/TrafficCourts/TrafficCourts.Arc.Dispute.Service` |
| Oracle Data API (legacy) | Java 11 / Spring Boot | `src/backend/oracle-data-api` |
| ORDS data access | .NET client for Oracle REST Data Services | `src/backend/TrafficCourts/TrafficCourts.OrdsDataService` |
| Virus scan API | .NET | `src/backend/bc-gov-virus-scan` |

## Data flow (high level)

1. A citizen submits a dispute through the Citizen Portal, which calls Citizen.Service.
2. Citizen.Service publishes a message, and Workflow.Service consumes it and saves the dispute.
3. Disputes are stored in Oracle (the `OCCAM` and `TCO` schemas). The apps reach Oracle through ORDS endpoints, which call PL/SQL packages. Those packages and endpoints live in [related repositories](related-repositories.md).
4. Staff process disputes in the Staff Portal, which calls Staff.Service.

> TODO: Expand this with the messaging, JUSTIN/ARC integration, and document storage (COMS/CDOGS) flows. See also `docs/ords-data-access.md` and `docs/*.drawio`.
