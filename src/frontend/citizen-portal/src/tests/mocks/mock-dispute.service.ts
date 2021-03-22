import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { Dispute } from '@shared/models/dispute.model';
import { Ticket } from '@shared/models/ticket.model';
import * as faker from 'faker';

import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;
  private _dispute: BehaviorSubject<Dispute>;
  private _disputes: BehaviorSubject<Dispute[]>;

  constructor() {
    const ticketA = this.createTicket();
    const ticketB = this.createTicket();

    this._ticket = new BehaviorSubject<Ticket>(ticketA);
    this._tickets = new BehaviorSubject<Ticket[]>([ticketA, ticketB]);

    const disputeA = this.createDispute();
    disputeA.statusCode = 'INP';
    disputeA.ticketType = 'eTicket';
    disputeA.status = 'Dispute Not Submitted';
    disputeA.note =
      'The dispute information is incomplete... click Continue to finish the process';

    const disputeB = this.createDispute();
    disputeB.statusCode = 'ACT';
    disputeB.ticketType = 'eTicket';
    disputeB.status = 'Dispute Submitted';
    disputeB.note = 'This is currently under review';

    const disputeC = this.createDispute();
    disputeC.statusCode = 'NEW';
    disputeC.ticketType = 'eTicket';
    disputeC.status = 'New';
    disputeC.note = '';

    const disputeD = this.createDispute();
    disputeD.statusCode = 'PAID';
    disputeD.ticketType = 'eTicket';
    disputeD.status = 'Paid';
    disputeD.note = 'This ticket has been paid';

    this._dispute = new BehaviorSubject<Dispute>(disputeA);
    this._disputes = new BehaviorSubject<Dispute[]>([
      disputeA,
      disputeB,
      disputeC,
      disputeD,
    ]);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }

  public get dispute$(): BehaviorSubject<Dispute> {
    return this._dispute;
  }

  public get dispute(): Dispute {
    return this._dispute.value;
  }

  public get tickets$(): BehaviorSubject<Ticket[]> {
    return this._tickets;
  }

  public get tickets(): Ticket[] {
    return this._tickets.value;
  }

  public get disputes$(): BehaviorSubject<Dispute[]> {
    return this._disputes;
  }

  public get disputes(): Dispute[] {
    return this._disputes.value;
  }

  public get httpTicket(): ApiHttpResponse<Ticket> {
    return new ApiHttpResponse(200, null, this._ticket.value);
  }

  public get rsiTicket(): any {
    return `{
      "violation_number": "EZ02000460",
      "violation_time": "09:54",
      "violation_date": "2020-09-18T09:54:00",
      "counts": [
        {
          "count_number": 1,
          "ticket_amount": 109,
          "amount_due": 83,
          "due_date": "2020-09-18T09:54",
          "description": "LOAD OR PROJECTION OVER 1M IN FRONT WITHOUT REQUIRED RED FLAG OR CLOTH"
        },
        {
          "count_number": 2,
          "ticket_amount": 109,
          "amount_due": 96,
          "due_date": "2020-09-18T09:54",
          "description": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED LAMP DURING TIME SPECIFIED IN MR SECTION 4.01"
        },
        {
          "count_number": 3,
          "ticket_amount": 109,
          "amount_due": 95,
          "due_date": "2020-09-18T09:54",
          "description": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED RED FLAG OR CLOTH"
        }
      ],
      "rawResponse": {
        "items": [
          {
            "selected_invoice": {
              "$ref": "https://wsgw.test.jag.gov.bc.ca/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/EZ020004601",
              "invoice": {
                "invoice_number": "EZ020004601",
                "pbc_ref_number": "10006",
                "party_number": "n/a",
                "party_name": "n/a",
                "account_number": "n/a",
                "site_number": "0",
                "cust_trx_type": "Traffic Violation Ticket",
                "term_due_date": "2020-09-18T09:54",
                "total": 109,
                "amount_due": 83,
                "attribute1": "LOAD OR PROJECTION OVER 1M IN FRONT WITHOUT REQUIRED RED FLAG OR CLOTH",
                "attribute2": "",
                "attribute3": "2020-09-18",
                "attribute4": "n/a"
              }
            },
            "open_invoices_for_site": null
          },
          {
            "selected_invoice": {
              "$ref": "https://wsgw.test.jag.gov.bc.ca/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/EZ020004602",
              "invoice": {
                "invoice_number": "EZ020004602",
                "pbc_ref_number": "10006",
                "party_number": "n/a",
                "party_name": "n/a",
                "account_number": "n/a",
                "site_number": "0",
                "cust_trx_type": "Traffic Violation Ticket",
                "term_due_date": "2020-09-18T09:54",
                "total": 109,
                "amount_due": 96,
                "attribute1": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED LAMP DURING TIME SPECIFIED IN MR SECTION 4.01",
                "attribute2": "",
                "attribute3": "2020-09-18",
                "attribute4": "n/a"
              }
            },
            "open_invoices_for_site": null
          },
          {
            "selected_invoice": {
              "$ref": "https://wsgw.test.jag.gov.bc.ca/paybc/vph/rest/PSSGVPHPAYBC/vph/invs/EZ020004603",
              "invoice": {
                "invoice_number": "EZ020004603",
                "pbc_ref_number": "10006",
                "party_number": "n/a",
                "party_name": "n/a",
                "account_number": "n/a",
                "site_number": "0",
                "cust_trx_type": "Traffic Violation Ticket",
                "term_due_date": "2020-09-18T09:54",
                "total": 109,
                "amount_due": 95,
                "attribute1": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED RED FLAG OR CLOTH",
                "attribute2": "",
                "attribute3": "2020-09-18",
                "attribute4": "n/a"
              }
            },
            "open_invoices_for_site": null
          }
        ]
      }
    }`;
  }

  private createTicket(): Ticket {
    // tslint:disable
    return {
      id: faker.random.number(),
      userId: faker.random.uuid(),
      violationTicketNumber:
        'EA' +
        faker.random
          .number({
            min: 10000000,
            max: 99999999,
          })
          .toString(),
      violationDate: faker.date.recent().toString(),
      violationTime:
        faker.random
          .number({
            min: 10,
            max: 23,
          })
          .toString() +
        ':' +
        faker.random
          .number({
            min: 10,
            max: 59,
          })
          .toString(),
      surname: faker.name.lastName(),
      givenNames: faker.name.firstName(),
      mailing: faker.address.streetAddress(),
      postal: 'V3P 4A5',
      city: faker.address.city(),
      province: 'BC',
      license: faker.random
        .number({
          min: 1000000,
          max: 9999999,
        })
        .toString(),
      provLicense: 'BC',
      homePhone: faker.phone.phoneNumber('250#######'),
      workPhone: faker.phone.phoneNumber('250#######'),
      birthdate: faker.date.past(60, new Date(2001, 0, 1)),
      counts: [
        {
          id: faker.random.number(),
          countNo: 1,
          statuteId: 19149,
          description: 'MVA 146(1) - Speed in outside municipality',
        },
        {
          id: faker.random.number(),
          countNo: 2,
          statuteId: 19742,
          description: 'MVA 73(1) - Fail to Stop for Police',
        },
      ],
    };
    // tslint:enable
  }

  private createDispute(): Dispute {
    const dispute: Dispute = {
      id: faker.random.number(),
      emailAddress: faker.internet.email(),
      courtLocation: faker.address.city(1),
      lawyerPresent: false,
      interpreterRequired: false,
      interpreterLanguage: null,
      callWitness: false,
      certifyCorrect: false,
      ticket: this.createTicket(),
      rsiTicket: JSON.parse(this.rsiTicket),
    };

    // dispute.counts = [...dispute.ticket.counts];

    dispute.rsiTicket?.counts.forEach((cnt) => {
      let dueDate = new Date(dispute.rsiTicket.violation_date);
      dueDate.setDate(dueDate.getDate() + 30);
      cnt.due_date = dueDate.toString();
    });

    dispute.counts = [...dispute.rsiTicket?.counts];

    dispute.counts.forEach((cnt) => {
      cnt.id = cnt.count_number;
      cnt.countNo = cnt.count_number;
      cnt.statuteId = cnt.count_number;
    });

    return dispute;
  }
}
