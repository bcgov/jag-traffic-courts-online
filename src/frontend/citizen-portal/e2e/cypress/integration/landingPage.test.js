///<reference types="Cypress"/>

//These tests for the basic test coverage

before(() => {
    // runs once before all tests in the block
    cy.visit("/");
  })


describe("Testing the landing page components", () => {
    context("Testing the welcome components of the landing page", () => {
        it("Test the welcome header message", () => {
            cy.get('h1').should('have.text', 'Ticket information in British Columbia');
        })
        
        it("Test the paragraph element", () => {
            cy.get('p').should('have.text', ' Looking for information on your violation or traffic ticket? Understand your ticket options and find out how to pay fines or dispute. ');
        })
    })

    context("Testing the did you know component", () => {
        it("Test the did you know message", () => {
            cy.get('h4').should('contain.text', 'Did you know?');
        })

        it("Test the did you know list", () => {
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
                cy.get('.landing-card-text').should('contain.text', ' Governing drivers, putting road safety policies in place, and working with partners to help reach the goal of zero traffic fatalities. ')
                cy.get('.useful-links.road-safety-bc').should('contain.text', 'View site').should('have.attr', 'href', 'https://www2.gov.bc.ca/gov/content/transportation/driving-and-cycling/roadsafetybc/intersection-safety-cameras')
                cy.contains('View site').click();
             });      
        })
    })
})