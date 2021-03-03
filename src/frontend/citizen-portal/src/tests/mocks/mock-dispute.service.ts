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

  private createTicket(): Ticket {
    return {
      id: faker.random.number(),
      userId: faker.random.uuid(),
      violationTicketNumber:
        'AE' +
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
      lawyerPresent: false,
      interpreterRequired: false,
      interpreterLanguage: null,
      callWitness: false,
      counts: [
        {
          id: faker.random.number(),
          countNo: 1,
          statuteId: 19149,
          description: 'MVA 146(1) Speed in outside municipality',
        },
        {
          id: faker.random.number(),
          countNo: 2,
          statuteId: 19742,
          description: 'MVA 73(1) Fail to Stop for Police',
        },
      ],
    };
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
    };

    dispute.counts = [...dispute.ticket.counts];
    return dispute;
  }
}
