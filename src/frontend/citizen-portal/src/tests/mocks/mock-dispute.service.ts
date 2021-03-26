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
      "violation_date": "2021-03-18T09:54:00",
      "counts": [
        {
          "count_number": 1,
          "ticket_amount": 167,
          "amount_due": 95,
          "due_date": "2021-03-18T09:54",
          "description": "OPERATE VEHICLE WITHOUT SEATBELTS"
        },
        {
          "count_number": 2,
          "ticket_amount": 126,
          "amount_due": 96,
          "due_date": "2021-03-18T09:54",
          "description": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED LAMP DURING TIME SPECIFIED IN MR SECTION 4.01"
        },
        {
          "count_number": 3,
          "ticket_amount": 97,
          "amount_due": 0,
          "due_date": "2021-03-18T09:54",
          "description": "LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED RED FLAG OR CLOTH"
        },
        {
          "count_number": 4,
          "ticket_amount": 253,
          "amount_due": 253,
          "due_date": "2021-03-18T09:54",
          "description": "SPEED IN SCHOOL ZONE"
        },
        {
          "count_number": 5,
          "ticket_amount": 368,
          "amount_due": 368,
          "due_date": "2021-03-18T09:54",
          "description": "USING ELECTRONIC DEVICE WHILE DRIVING"
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

    dispute.ticket.violationTicketNumber = dispute.rsiTicket?.violation_number;
    dispute.ticket.violationTime = dispute.rsiTicket?.violation_time;
    dispute.ticket.violationDate = dispute.rsiTicket?.violation_date;

    dispute.rsiTicket?.counts.forEach((cnt) => {
      cnt.early_amount = 0;
      if (cnt.amount_due > 0) {
        const todayDate = new Date();
        const dueDate = new Date(cnt.due_date);
        dueDate.setDate(dueDate.getDate() + 30);
        cnt.due_date = dueDate.toString();

        if (todayDate <= dueDate) {
          cnt.early_amount = cnt.ticket_amount - 25;
          cnt.amount_due = cnt.early_amount;
        }
      }
    });

    dispute.counts = [...dispute.rsiTicket?.counts];

    let balance = 0;
    dispute.counts.forEach((cnt) => {
      cnt.id = cnt.count_number;
      cnt.countNo = cnt.count_number;
      cnt.statuteId = cnt.count_number;

      if (cnt.countNo === 1) {
        cnt.statusCode = 'UNPAID';
        cnt.statusDesc = 'Outstanding Balance';
      } else if (cnt.countNo === 2) {
        cnt.statusCode = 'DISPUTE';
        cnt.statusDesc = 'Dispute Submitted';
        cnt.notes =
          'The dispute has been filed. An email with the court information will be sent soon.';
      } else if (cnt.countNo === 3) {
        cnt.statusCode = 'PAID';
        cnt.statusDesc = 'Paid';
      } else if (cnt.countNo === 4) {
        cnt.statusCode = 'COURT';
        cnt.statusDesc = 'Dispute In Progress';
        cnt.early_amount = 0;
        cnt.notes =
          'A court date has been set for this dispute. Check your email for more information.';
      } else if (cnt.countNo === 5) {
        cnt.amount_due = 0;
        cnt.statusCode = 'COMPLETE';
        cnt.statusDesc = 'Dispute Settled';
        cnt.early_amount = 0;
      } else if (cnt.countNo === 6) {
        cnt.statusCode = 'UNPAID';
        cnt.statusDesc = 'Outstanding Balance';
      }
      balance += cnt.early_amount > 0 ? cnt.early_amount : cnt.amount_due;
    });

    dispute.ticket.outstandingBalance = balance;

    return dispute;
  }
}
