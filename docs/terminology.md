# Terminology

Domain terms and acronyms used across Traffic Courts Online code, data, and tickets.

## Business terms

- **TCO:** Traffic Courts Online, this system. Also the name of one of its Oracle schemas (see `docs/oracle-data-model/tco.dbml`).
- **Violation ticket:** The ticket a person receives and may dispute. Identified by its ticket number.
- **Dispute:** A disputant's request to contest one or more counts on a violation ticket.
- **Disputant:** The person disputing a ticket.
- **VTC:** Violation Ticket Centre, the staff group that validates and processes disputes in the staff portal.
- **JJ:** Judicial Justice, the judicial officer who decides disputes. A *JJ dispute* is the dispute record as seen in the JJ workspace. A JJ can have disputes assigned to them and can reopen them for review.
- **Staff Workbench:** The staff portal section where staff approve or reject submitted disputes and later validate JJ decisions.
- **JJ Workbench:** The staff portal section where JJs record their decisions.
- **WR:** Written reasons. A dispute the JJ decides without hearing from the disputant in person. The alternative is a hearing on a scheduled court date. See `docs/business-process.md`.

## Systems and data stores

- **OCCAM:** The Oracle schema that holds dispute records. Backend code refers to `OccamDisputeId`. See `docs/oracle-data-model/occam.dbml`.
- **JUSTIN:** The justice system of record that supplies code tables (agencies, statutes, cities, provinces, countries, languages) to TCO via ORDS (`TrafficCourts.OrdsDataService.Justin`).
- **ORDS:** Oracle REST Data Services, the REST layer over the Oracle schemas. See `docs/ords-data-access.md`.
- **oracle-data-api:** The Java service in `src/backend/oracle-data-api` that sits between the .NET services and ORDS.
- **ARC:** A downstream system that receives submitted disputes. When staff submit a dispute, `arc-dispute-api` (`TrafficCourts.Arc.Dispute.Service`) builds a fixed-format ARC file and uploads it over SFTP. The audit log records this as "Dispute submitted to ARC".
- **COMS:** Common Object Management Service, used for storing ticket images and documents (`TrafficCourts.Coms.Client`).
- **CDOGS:** Common Document Generation Service, used to render documents from templates (`TrafficCourts.Cdogs.Client`).
- **Form Recognizer:** Azure AI service that extracts fields from uploaded ticket images.

## Applications

- **citizen-portal / citizen-api:** The public Angular app and its .NET API (`Citizen.Service`).
- **staff-portal / staff-api:** The staff and JJ Angular app and its .NET API (`Staff.Service`).
- **workflow-service:** The .NET service that reacts to MassTransit/RabbitMQ messages to run dispute workflows and send email (`Workflow.Service`).

## Agent context

- **Router:** The root `AGENTS.md` file that directs an agent to more context.
- **Route:** A condition plus a project-relative path in the router.
- **Shared project truth:** Documentation useful to both humans and agents. It stays in visible locations (`README.md`, `CONTRIBUTING.md`, `docs/`).
- **Agent-specific resource:** A persona or skill under `.agents/` written specifically for agent workflows.
- **Persona:** A reviewer perspective an agent adopts for a particular activity.
- **Skill:** A task-specific instruction package that follows the [Agent Skills](https://agentskills.io) format.
