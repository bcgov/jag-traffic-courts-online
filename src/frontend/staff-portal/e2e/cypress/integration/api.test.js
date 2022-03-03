const ticketResultProperties = ["violationTicketNumber", "violationTime", 
                "violationDate", "informationCertified", "disputant", "additional", "offences"]
        
const offenceProperties = ["offenceNumber", "ticketedAmount", "amountDue", "violationDateTime",
                                   "offenceDescription", "vehicleDescription", "offenceDisputeDetail", 
                                   "discountAmount", "discountDueDate", "invoiceType"]

describe("Perform API testing of RSI ticket search endpoint", () => {
    context("Make a GET request to the endpoint", () => {
        
        /**concatenationg url and parameters as the qs option not working  &
           : in the time parameters is getting encoded to %3A**/
        const api_url = Cypress.env('api_url') + '?ticketNumber=EZ02000460&time=09:54'
        it("Test that the endpoint returns correct status code and body", () => {
            cy.request(api_url).then(response => {
                expect(response.status).to.eq(200)
                expect(response.body).to.have.property('result')
                
                cy.log("Test that Ticket has correct properties")
                ticketResultProperties.forEach((item) => {
                    expect(response.body.result).to.have.property(item)
                })

                cy.log("Test that offence has correct details")
                offenceProperties.forEach((item) => {
                    response.body.result.offences.forEach((offence) => {
                        expect(offence).to.have.property(item)
                    })
                })
            })
        })  
    })
})
