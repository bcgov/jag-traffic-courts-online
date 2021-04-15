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
      discountAmount: 0,
      discountDueDate: null,
    };

    let dispute: Dispute = this.createDispute(
      ticket.violationTicketNumber,
      offence.offenceNumber
    );
    dispute.status = 1;
    dispute.interpreterRequired = true;
    dispute.interpreterLanguage = 'Spanish';
    dispute.informationCertified = true;
    offence.dispute = dispute;

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

    offence.notes = '';

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

    offence.notes = '';

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
    dispute.status = 2;
    dispute.informationCertified = true;
    offence.dispute = dispute;

    ticket.offences.push(offence);

    return ticket;
  }

  private createDispute(
    violationTicketNumber: string,
    offenceNumber: number
  ): Dispute {
    const dispute: Dispute = {
      violationTicketNumber,
      offenceNumber,
      status: 0,
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
