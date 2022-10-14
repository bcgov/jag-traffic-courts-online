before(() => {
    // runs once before all tests in the block
    cy.visit("/");
  })


describe("Testing the landing page components", () => {
    context("Testing the welcome section", () => {
        it("Test the welcome header message", () => {
            cy.get('h1').should('have.text', 'Ticket information in British Columbia');
        })
        
        it("Test the paragraph element", () => {
            cy.get('p').should('have.text', ' Looking for information on your violation or traffic ticket? Understand your ticket options and find out how to pay fines or dispute. ');
        })
    })

    
    context("Testing the understand your ticket section", () => {
        it("Clicking on section should take you to Understand Your Ticket page", () => {
            cy.get(".understand-your-ticket").should('have.attr', 'href', 'https://understandmyticket.gov.bc.ca/')
            // cy.get(".understand-your-ticket").click({ multiple: true })
        })
        
        it("Testing the Understand Icon Image", () => {
            cy.get(".understand-your-ticket .landing-card__image > img").should('have.attr', 'src', '/assets/understand-icon.svg')
        })

        it("Testing the Understand Your Ticket Header Text", () => {
            cy.get(".understand-your-ticket h3").should('contain.text', 'Understand');
            cy.get(".understand-your-ticket h3").should('contain.text', 'Your Ticket');
        })

        it("Test the Explore ticket options button", () => {
            cy.get(".understand-your-ticket button").should('contain.text', 'Explore ticket options')
        })
    })
    
    context("Testing the pay options section", () => {
        it("Clicking on section should take you to Pay a provincial violation ticket page", () => {
            cy.get(".payment-options").should('have.attr', 'href', 'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/pay-ticket')
            // cy.get(".payment-options").click({ multiple: true })
        })
        
        it("Testing the Understand Icon Image", () => {
            cy.get(".payment-options .landing-card__image > img").should('have.attr', 'src', '/assets/pay-icon.svg')
        })

        it("Testing the Pay Options Header Text", () => {
            cy.get(".payment-options h3").should('contain.text', 'Pay');
            cy.get(".payment-options h3").should('contain.text', 'Options');
        })

        it("Test the Find ways to pay button", () => {
            cy.get(".payment-options button").should('contain.text', 'Find ways to pay')
        })
    })

    context("Testing the dispute ticket section", () => {
        it("Clicking on section should take you to Dispute a Provincial Violation ticket page", () => {
            cy.get(".resolution-options").should('have.attr', 'href', 'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments/pay-dispute-ticket/prov-violation-tickets/dispute-ticket')
            // cy.get(".resolution-options").click({ multiple: true })
        })
        
        it("Testing the Dispute Ticket Icon Image", () => {
            cy.get(".resolution-options .landing-card__image > img").should('have.attr', 'src', '/assets/resolution-icon.svg')
        })
        
        it("Testing the Dispute Ticket Header Text", () => {
            cy.get(".resolution-options h3").should('contain.text', 'Resolution');
            cy.get(".resolution-options h3").should('contain.text', 'Options');
        })

        it("Test the Learn your options button", () => {
            cy.get(".resolution-options button").should('contain.text', 'Learn your options')
        })
    })
        
    context("Testing the did you know section", () => {
        it("Test the \"Did you know?\" message", () => {
            cy.get('h4').should('contain.text', 'Did you know?');
        })

        it("Test the \"Did you know?\" list", () => {
            cy.get('.flex-last-info-content ul>li').should((items) => {
                expect(items[0]).to.have.text(' Minor errors on the ticket, such as a misspelled word, may not be grounds for dismissing a ticket. ')
                expect(items[1]).to.have.text(' The officer does not need to write the speed on the ticket. ')
                expect(items[2]).to.have.text('You do not need to sign the ticket.')
                expect(items[3]).to.have.text(' Officers (including RCMP, municipal detachments, and Integrated Road Safety Unit members) can issue tickets across different municipalities. ')
                expect(items[4]).to.have.text(' Officer schedules are checked before setting court hearing dates. ')
            })
        })
    })
    
        
    context("Testing the useful links section", () => {
        it("Testing the RoadSafety link and subsection", () => {
                cy.contains('RoadSafetyBC').click().then(() => {
                cy.get('#cdk-accordion-child-0 .landing-card-text').should('contain.text', ' Governing drivers, putting road safety policies in place, and working with partners to help reach the goal of zero traffic fatalities. ')
                cy.get('.useful-links.road-safety-bc').should('contain.text', 'View site').should('have.attr', 'href', 'https://www2.gov.bc.ca/gov/content/transportation/driving-and-cycling/roadsafetybc/intersection-safety-cameras')
                // cy.contains('View site').click();
                cy.contains('RoadSafetyBC').click();
            });      
        })

        it("Testing the ICBC link and subsection", () => {
            cy.contains('Insurance Corporation of BC (ICBC)').click().then(() => {
                cy.get('#cdk-accordion-child-1 .landing-card-text').should('contain.text', 'Learn how to pay or dispute a ticket, and more.')
                cy.get('.useful-links.icbc').should('contain.text', 'View site').should('have.attr', 'href', 'https://www.icbc.com/driver-licensing/tickets/Pages/default.aspx')
            //    cy.contains('View site').click({ force: true });
                cy.contains('Insurance Corporation of BC (ICBC)').click();
            }); 
        })

        it("Testing the Provincial Court of BC link and subsection", () => {
            cy.contains('Provincial Court of BC').click().then(() => {
            cy.get('#cdk-accordion-child-2 .landing-card-text').should('contain.text', 'Deals with three types of tickets: Provincial Violation Tickets (including traffic tickets), Federal Contravention Tickets and Municipal Tickets.')
            cy.get('.useful-links.provincial-court-of-bc').should('contain.text', 'View site').should('have.attr', 'href', 'https://www.provincialcourt.bc.ca/types-of-cases/traffic-and-bylaw-matters')
            // cy.contains('View site').click({ force: true });
            cy.contains('Provincial Court of BC').click();
            });      
        })

        it("Testing the CourtHouse Services of BC link and subsection", () => {
            cy.contains('Courthouse Services of BC').click().then(() => {
            cy.get('#cdk-accordion-child-3 .landing-card-text').should('contain.text', 'Pay or dispute provincial violation or Federal Contravention Tickets.')
            cy.get('.useful-links.courthouse-services-of-bc').should('contain.text', 'View site').should('have.attr', 'href', 'https://www2.gov.bc.ca/gov/content/justice/courthouse-services/fines-payments')
            // cy.contains('View site').click({ force: true });
            cy.contains('Courthouse Services of BC').click()
            });      
        })
    })
})