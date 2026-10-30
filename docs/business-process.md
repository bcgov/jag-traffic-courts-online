# Business process

Traffic Courts Online has two web applications:

- The **citizen portal** is public. Citizens use it to dispute traffic violation tickets they have received.
- The **staff portal** is used by Traffic Court staff and judicial justices (JJs) to validate and process those disputes.

## Dispute lifecycle

1. **Submit.** A citizen submits a dispute in the citizen portal.
2. **Validate.** The dispute appears in the **Staff Workbench** section of the staff portal. Traffic Court staff approve or reject it.
3. **Send to ARC.** An approved dispute is submitted to ARC, a separate system.
4. **Schedule.** Staff use the JUSTIN application to schedule the dispute for adjudication by a JJ. Once it is scheduled, the appearance information is copied into the TCO schema (see `database/README.md`).
5. **Adjudicate.** The JJ decides the dispute in one of two ways:
   - **Written reasons (WR):** used when the JJ does not need to hear from the disputant in person.
   - **Hearing:** a court date is scheduled and the dispute is heard.
6. **Record the decision.** In either case, the JJ submits the decision in the **JJ Workbench** section of the staff portal.
7. **Conclude.** Traffic Court staff validate the decision in the Staff Workbench, which concludes the dispute.

Terms used here are defined in [terminology.md](terminology.md).
