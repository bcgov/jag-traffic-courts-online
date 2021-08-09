import { Disputant } from '@shared/models/disputant.model';
import { Offence } from '@shared/models/offence.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import * as faker from 'faker';
import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<TicketDispute>;
  private _tickets: BehaviorSubject<TicketDispute[]>;

  constructor() {
    // const ticket = this.createTicketWithoutDisputes();
    const ticket = this.createTicketWithDispute();

    this._ticket = new BehaviorSubject<TicketDispute>(ticket);
    // this._tickets = new BehaviorSubject<TicketDispute[]>([ticketA, ticketB]);
  }

  public get ticket$(): BehaviorSubject<TicketDispute> {
    return this._ticket;
  }

  public get ticket(): TicketDispute {
    return this._ticket.value;
  }

  // public get tickets$(): BehaviorSubject<TicketDispute[]> {
  //   return this._tickets;
  // }

  // public get tickets(): TicketDispute[] {
  //   return this._tickets.value;
  // }

  // public get httpTicket(): ApiHttpResponse<TicketDispute> {
  //   return new ApiHttpResponse(200, null, this._ticket.value);
  // }

  private createTicketWithoutDisputes(): TicketDispute {
    const ticket: TicketDispute = {
      violationTicketNumber:
        'EA' +
        faker.datatype
          .number({
            min: 10000000,
            max: 99999999,
          })
          .toString(),
      violationTime:
        faker.datatype
          .number({
            min: 10,
            max: 23,
          })
          .toString() +
        ':' +
        faker.datatype
          .number({
            min: 10,
            max: 59,
          })
          .toString(),
      violationDate: null,
      // informationCertified: false,
      disputant: null,
      offences: [],
      additional: null,
    };

    // --------------------------
    let offence: Offence = {
      offenceNumber: 1,
      ticketedAmount: 126,
      amountDue: 87.56,
      violationDateTime: faker.date.soon().toString(),
      offenceDescription: 'Load Or Projection Over 1.2M In Rear', //  Without Required Lamp During Time Specified In Mr Section 4.01
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
      status: null,
      offenceAgreementStatus: '',
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
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
      offenceNumber: 2,
      ticketedAmount: 167,
      amountDue: 142,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: soonDate,
      status: null,
      offenceAgreementStatus: '',
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
    };

    ticket.offences.push(offence);

    return ticket;
  }

  private createTicketWithDispute(): TicketDispute {
    const ticket: TicketDispute = {
      violationTicketNumber:
        'EA' +
        faker.datatype
          .number({
            min: 10000000,
            max: 99999999,
          })
          .toString(),
      violationTime:
        faker.datatype
          .number({
            min: 10,
            max: 23,
          })
          .toString() +
        ':' +
        faker.datatype
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
      lawyerPresent: true,
      interpreterRequired: true,
      interpreterLanguage: 'SPA',
      witnessPresent: true,
      numberOfWitnesses: 3,
    };

    const offenceDate = faker.date.soon().toString();

    // --------------------------
    let offence: Offence = {
      offenceNumber: 1,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: offenceDate,
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Lamp During Time Specified In Mr Section 4.01',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
      status: 1,
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
    };

    offence = Object.assign(offence, this.createOffencePay());

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 2,
      ticketedAmount: 126,
      amountDue: 126,
      violationDateTime: offenceDate,
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      discountDueDate: null,
      status: 1,
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
    };

    offence = Object.assign(offence, this.createOffenceReduction());

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
      violationDateTime: offenceDate,
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      discountDueDate: soonDate,
      status: 1,
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
    };

    offence = Object.assign(offence, this.createOffenceDispute());

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
      lastName: faker.name.lastName(),
      givenNames: faker.name.firstName(),
      mailingAddress: faker.address.streetAddress(),
      city: faker.address.city(),
      province: faker.address.state(),
      postal: 'V8R3E3',
      birthdate: null, // faker.date.past().toDateString(),
      emailAddress: faker.internet.email(),
      license: '2342342',
      provLicense: 'BC',
      phoneNumber: '2506653434', // faker.phone.phoneNumberFormat(10),
    };
  }

  private createOffenceDispute(): any {
    return {
      status: 0,
      offenceAgreementStatus: 'DISPUTE',
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: null,
      moreTimeReason: null,
    };
  }

  private createOffenceNothing(): any {
    return {
      status: 0,
      offenceAgreementStatus: 'NOTHING',
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: null,
      moreTimeReason: null,
    };
  }

  private createOffencePay(): any {
    return {
      status: 0,
      offenceAgreementStatus: 'PAY',
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: null,
      moreTimeReason: null,
    };
  }

  private createOffenceReduction(): any {
    return {
      status: 0,
      offenceAgreementStatus: 'REDUCTION',
      reductionAppearInCourt: true,
      requestReduction: true,
      requestMoreTime: false,
      reductionReason: 'I have been unable to work for the past 6 months.',
      moreTimeReason: null,
    };
  }

  private createOffenceMoreTime(): any {
    return {
      status: 0,
      offenceAgreementStatus: 'REDUCTION',
      reductionAppearInCourt: true,
      requestReduction: true,
      requestMoreTime: true,
      reductionReason:
        'I have been unable to work for the past 6 months and cannot pay my rent.',
      moreTimeReason: 'I have a new job starting next week.',
    };
  }
}
