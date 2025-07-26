# ReadMe:

## Scenario: 

RSI Ticket counts are the Source of truth
ViolationTickets needs to be adjusted based on the no.of counts returned for RSI tickets 
We need to either update ViolationTicket ticket counts data based on the RSI ticket counts or delete violation ticket count based on below rules

TODO:
- Flag if RSI Ticket doesnt have at least one count 
- Flag if RSI Ticket is missing previos counts, for example if count 1 doesnt exist when count 2 exists or if count 1 or 2 doesnt exist if count 3 exists 
    - Dont write any sql statement, just flag the RSI ticket number 

Violation Ticket Count 1 exists but RSI count doesnt exist 
- Delete sql statment if violationTicketCount.dispute count doesn't exist 
- Delete if RSI Count doesnt exist 
