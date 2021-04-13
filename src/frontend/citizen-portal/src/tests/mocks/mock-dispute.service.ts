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
    const ticketA = this.createRsiTicket();
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
          ticketAmount: 109,
          amountDue: 77.76,
          dueDate: '2020-09-18T09:54',
          description:
            'LOAD OR PROJECTION OVER 1M IN FRONT WITHOUT REQUIRED RED FLAG OR CLOTH',
          dispute: null,
        },
        {
          offenceNumber: 2,
          ticketAmount: 109,
          amountDue: 89.76,
          dueDate: '2020-09-18T09:54',
          description:
            'LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED LAMP DURING TIME SPECIFIED IN MR SECTION 4.01',
          dispute: null,
        },
        {
          offenceNumber: 3,
          ticketAmount: 109,
          amountDue: 87.76,
          dueDate: '2020-09-18T09:54',
          description:
            'LOAD OR PROJECTION OVER 1.2M IN REAR WITHOUT REQUIRED RED FLAG OR CLOTH',
          dispute: null,
        },
      ],
    };

    let balance = 0;
    ticket.offences.forEach((offence) => {
      offence.earlyAmount = 0;
      offence.statusCode = 'UNPAID';
      offence.statusDesc = 'Outstanding Balance';
      offence.notes = '';
      balance +=
        offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;
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
      ticketAmount: 126,
      amountDue: 126,
      dueDate: faker.date.soon().toString(),
      description:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      dispute: null,
    };

    let dispute: Dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.interpreterRequired = true;
    dispute.interpreterLanguage = 'Spanish';
    dispute.informationCertified = true;
    offence.dispute = dispute;

    offence.earlyAmount = 0;
    offence.statusCode = 'DISPUTE';
    offence.statusDesc = 'Dispute Submitted';
    offence.notes =
      'The dispute has been filed. An email with the court information will be sent soon.';

    if (offence.amountDue > 0) {
      const todayDate = new Date();
      const dueDate = new Date(offence.dueDate);

      if (todayDate <= dueDate) {
        offence.earlyAmount = offence.ticketAmount - 25;
        offence.amountDue = offence.earlyAmount;
      }
    }

    balance +=
      offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 2,
      ticketAmount: 126,
      amountDue: 0,
      dueDate: faker.date.soon().toString(),
      description:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      dispute: null,
    };

    offence.earlyAmount = 0;
    offence.statusCode = 'PAID';
    offence.statusDesc = 'Paid';
    offence.notes = '';

    if (offence.amountDue > 0) {
      const todayDate = new Date();
      const dueDate = new Date(offence.dueDate);

      if (todayDate <= dueDate) {
        offence.earlyAmount = offence.ticketAmount - 25;
        offence.amountDue = offence.earlyAmount;
      }
    }

    balance +=
      offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 3,
      ticketAmount: 167,
      amountDue: 167,
      dueDate: faker.date.soon().toString(), // '2020-09-18T09:54',
      description: 'Operate Vehicle Without Seatbelts',
      dispute: null,
    };

    dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    offence.dispute = dispute;

    offence.earlyAmount = 0;
    offence.statusCode = 'UNPAID';
    offence.statusDesc = 'Outstanding Balance';
    offence.notes = '';

    if (offence.amountDue > 0) {
      const todayDate = new Date();
      const dueDate = new Date(offence.dueDate);

      if (todayDate <= dueDate) {
        offence.earlyAmount = offence.ticketAmount - 25;
        offence.amountDue = offence.earlyAmount;
      }
    }

    balance +=
      offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;

    ticket.offences.push(offence);
    // --------------------------
    offence = {
      offenceNumber: 4,
      ticketAmount: 126,
      amountDue: 126,
      dueDate: faker.date.soon().toString(),
      description: 'Using Electronic Device While Driving',
      dispute: null,
    };

    dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.informationCertified = true;
    offence.dispute = dispute;

    offence.earlyAmount = 0;
    offence.statusCode = 'COURT';
    offence.statusDesc = 'Dispute In Progress';
    offence.notes =
      'A court date has been set for this dispute. Check your email for more information.';

    if (offence.amountDue > 0) {
      const todayDate = new Date();
      const dueDate = new Date(offence.dueDate);

      if (todayDate <= dueDate) {
        offence.earlyAmount = offence.ticketAmount - 25;
        offence.amountDue = offence.earlyAmount;
      }
    }

    balance +=
      offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 5,
      ticketAmount: 126,
      amountDue: 0,
      dueDate: faker.date.soon().toString(),
      description:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      dispute: null,
    };

    dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.lawyerPresent = true;
    dispute.informationCertified = true;
    offence.dispute = dispute;

    offence.earlyAmount = 0;
    offence.statusCode = 'COMPLETE';
    offence.statusDesc = 'Dispute Settled';
    offence.notes = '';

    if (offence.amountDue > 0) {
      const todayDate = new Date();
      const dueDate = new Date(offence.dueDate);

      if (todayDate <= dueDate) {
        offence.earlyAmount = offence.ticketAmount - 25;
        offence.amountDue = offence.earlyAmount;
      }
    }

    balance +=
      offence.earlyAmount > 0 ? offence.earlyAmount : offence.amountDue;

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
    };

    return dispute;
  }
}
