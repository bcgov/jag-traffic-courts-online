import { ApiHttpResponse } from '@core/models/api-http-response.model';
import { OffenceDispute } from '@shared/models/offenceDispute.model';
import { Disputant } from '@shared/models/disputant.model';
import { Offence } from '@shared/models/offence.model';
import { Ticket } from '@shared/models/ticket.model';
import * as faker from 'faker';

import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<Ticket>;
  private _tickets: BehaviorSubject<Ticket[]>;

  constructor() {
    // const ticket = this.createTicketWithoutDisputes();
    const ticket = this.createTicketWithDispute();

    this._ticket = new BehaviorSubject<Ticket>(ticket);
    // this._tickets = new BehaviorSubject<Ticket[]>([ticketA, ticketB]);
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }

  // public get tickets$(): BehaviorSubject<Ticket[]> {
  //   return this._tickets;
  // }

  // public get tickets(): Ticket[] {
  //   return this._tickets.value;
  // }

  // public get httpTicket(): ApiHttpResponse<Ticket> {
  //   return new ApiHttpResponse(200, null, this._ticket.value);
  // }

  private createTicketWithoutDisputes(): Ticket {
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
      disputant: null,
      offences: [],
      additional: null,
    };

    // --------------------------
    let offence: Offence = {
      offenceNumber: 2,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.soon().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
    };

    ticket.offences.push(offence);

    // --------------------------
    const soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

    offence = {
      offenceNumber: 3,
      ticketedAmount: 167,
      amountDue: 142,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: soonDate,
    };

    ticket.offences.push(offence);

    return ticket;
  }

  private createTicketWithDispute(): Ticket {
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
      disputant: null,
      offences: [],
      additional: null,
    };

    ticket.disputant = this.createDisputant();

    ticket.additional = {
      lawyerPresent: false,
      interpreterRequired: true,
      interpreterLanguage: 'Spanish',
      witnessPresent: false,
    };

    // --------------------------
    let offence: Offence = {
      offenceNumber: 1,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.soon().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
    };

    let offenceDispute: OffenceDispute = this.createOffenceDispute(
      offence.offenceNumber
    );
    offenceDispute.status = 1;
    offenceDispute.informationCertified = true;

    offence.offenceDispute = offenceDispute;

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 2,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
    };

    ticket.offences.push(offence);

    // --------------------------
    let soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

    offence = {
      offenceNumber: 3,
      ticketedAmount: 167,
      amountDue: 142,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: soonDate,
    };

    offence.notes = '';

    ticket.offences.push(offence);

    // --------------------------
    /*
    offence = {
      offenceNumber: 3,
      ticketedAmount: 126,
      amountDue: 0,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      offenceDispute: null,
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
    };

    offence.notes = '';

    ticket.offences.push(offence);
*/
    /*
    // --------------------------
    offence = {
      offenceNumber: 4,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Using Electronic Device While Driving',
      offenceDispute: null,
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

    ticket.offences.push(offence);*/

    return ticket;
  }

  private createDisputant(): Disputant {
    return {
      surname: faker.name.lastName(),
      givenNames: faker.name.firstName(),
      mailingAddress: faker.address.streetAddress(),
      city: faker.address.city(),
      province: faker.address.state(),
      postal: 'V8R3E3',
      birthdate: faker.date.past().toDateString(),
      emailAddress: faker.internet.email(),
      license: '234234',
      provLicense: 'BC',
      homePhone: faker.phone.phoneNumber(),
      workPhone: faker.phone.phoneNumber(),
    };
  }

  private createOffenceDispute(offenceNumber: number): OffenceDispute {
    return {
      status: 0,
      offenceNumber: offenceNumber,
      offenceAgreementStatus: '3',
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: null,
      moreTimeReason: null,
      informationCertified: false,
    };
  }
}
