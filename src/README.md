# Application Source Code

This directory contains application source code


# Example of full data call stack
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

    


