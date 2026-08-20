BC will drop DST soon, we need to prepare for that:
Perform a technical scan of the TCO OpenShift applications to identify impacts from eliminating or changing daylight saving time.
 
Review application code, container configurations, scheduled jobs, integrations, database processes, time-zone libraries, and date/time conversions. Remediate confirmed issues and document the results.
 
**Acceptance Criteria**
- Application code and OpenShift configurations have been scanned for DST dependencies.
- Scheduled and batch jobs have been reviewed for timing impacts.
- Integrations and database processes using local time have been assessed.
- Identified issues have been documented and remediated.
- Testing confirms correct processing before, during, and after the affected time transition.
- Scan results, remediation details, and residual risks are documented.
 
tips: replace moment-timezone with TS native methods(timezone is not important for outage banner to work), check for ENV variables, dockerfile setup (alpine uses UTC be default), DateTime.Now and ToLocalTime or other timezone sensetive methods, use DateTimeOffset where appropriate
