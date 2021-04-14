import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { Dispute } from '@shared/models/dispute.model';
import { Offence } from '@shared/models/offence.model';
import { Ticket } from '@shared/models/ticket.model';
import * as faker from 'faker';

import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;

  constructor() {
    const ticketA = this.createTicket();
    const ticketB = this.createTicket();

    this._ticket = new BehaviorSubject<Ticket>(ticketA);
    this._tickets = new BehaviorSubject<Ticket[]>([ticketA, ticketB]);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }

  public get tickets$(): BehaviorSubject<Ticket[]> {
    return this._tickets;
  }

  public get tickets(): Ticket[] {
    return this._tickets.value;
  }

  public get httpTicket(): ApiHttpResponse<Ticket> {
    return new ApiHttpResponse(200, null, this._ticket.value);
  }

  private createRsiTicket(): Ticket {
    const ticket: Ticket = {
      violationTicketNumber: 'EZ02000461',
      violationTime: '09:55',
      violationDate: '2020-09-18',
      offences: [
        {
          offenceNumber: 1,
          ticketedAmount: 109,
          amountDue: 77.76,
          violationDateTime: '2020-09-18T09:54',
          offenceDescription:
            'LOAD OR PROJECTION OVER 1M IN FRONT WITHOUT REQUIRED RED FLAG OR CLOTH',
          dispute: null,
          invoiceType: 'Traffic Violation Ticket',
          vehicleDescription: 'Toyota Prius',
          discountAmount: 25,
          discountDueDate: faker.date.soon().toString(),
        },
        {
          offenceNumber: 2,
          ticketedAmount: 109,
          amountDue: 89.76,
          violationDateTime: '2020-09-18T09:54',
          offenceDescription:
            'LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED LAMP DURING TIME SPECIFIED IN MR SECTION 4.01',
          dispute: null,
          invoiceType: 'Traffic Violation Ticket',
          vehicleDescription: 'Toyota Prius',
          discountAmount: 0,
          discountDueDate: null,
        },
        {
          offenceNumber: 3,
          ticketedAmount: 109,
          amountDue: 87.76,
          violationDateTime: '2020-09-18T09:54',
          offenceDescription:
            'LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED RED FLAG OR CLOTH',
          dispute: null,
          invoiceType: 'Traffic Violation Ticket',
          vehicleDescription: 'Ford Focus',
          discountAmount: 25,
          discountDueDate: faker.date.soon().toString(),
        },
      ],
    };

    let balance = 0;
    ticket.offences.forEach((offence) => {
      offence.statusCode = 'UNPAID';
      offence.statusDesc = 'Outstanding Balance';
      offence.notes = '';
      balance += offence.amountDue;
    });

    // ------------------------------------
    ticket.outstandingBalance = balance;

    return ticket;
  }

  private createTicket(): Ticket {
    const ticket: Ticket = {
      violationTicketNumber:
        'EA' +
        faker.random
          .number({
            min: 10000000,
            max: 99999999,
          })
          .toString(),
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
      violationDate: null,
      offences: [],
    };

    let balance = 0;

    // --------------------------
    let offence: Offence = {
      offenceNumber: 1,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.soon().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      dispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: faker.date.soon().toString(),
    };

    let dispute: Dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.interpreterRequired = true;
    dispute.interpreterLanguage = 'Spanish';
    dispute.informationCertified = true;
    offence.dispute = dispute;

    offence.statusCode = 'DISPUTE';
    offence.statusDesc = 'Dispute Submitted';
    offence.notes =
      'The dispute has been filed. An email with the court information will be sent soon.';

    balance += offence.amountDue;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 2,
      ticketedAmount: 126,
      amountDue: 0,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      dispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
    };

    offence.statusCode = 'PAID';
    offence.statusDesc = 'Paid';
    offence.notes = '';

    balance += offence.amountDue;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 3,
      ticketedAmount: 167,
      amountDue: 167,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      dispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: faker.date.soon().toString(),
    };

    dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    offence.dispute = dispute;

    offence.statusCode = 'UNPAID';
    offence.statusDesc = 'Outstanding Balance';
    offence.notes = '';

    balance += offence.amountDue;

    ticket.offences.push(offence);
    // --------------------------
    offence = {
      offenceNumber: 4,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Using Electronic Device While Driving',
      dispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Ford Focus',
      discountAmount: 25,
      discountDueDate: faker.date.soon().toString(),
    };

    dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.informationCertified = true;
    offence.dispute = dispute;

    offence.statusCode = 'COURT';
    offence.statusDesc = 'Dispute In Progress';
    offence.notes =
      'A court date has been set for this dispute. Check your email for more information.';

    balance += offence.amountDue;

    ticket.offences.push(offence);

    // ------------------------------------
    ticket.outstandingBalance = balance;

    return ticket;
  }

  private createDispute(
    violationTicketNumber: string,
    offenceNumber: number
  ): Dispute {
    const dispute: Dispute = {
      violationTicketNumber,
      offenceNumber,
      emailAddress: faker.internet.email(),
      lawyerPresent: null,
      interpreterRequired: null,
      interpreterLanguage: null,
      witnessPresent: null,
      informationCertified: null,
      offenceAgreementStatus: null,
      requestReduction: null,
      requestMoreTime: null,
      reductionReason: null,
      moreTimeReason: null,
      status: null,
    };

    return dispute;
  }
}
