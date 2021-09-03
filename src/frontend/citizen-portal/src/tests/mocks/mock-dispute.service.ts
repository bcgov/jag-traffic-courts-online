import { Disputant } from '@shared/models/disputant.model';
import { Offence } from '@shared/models/offence.model';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import * as faker from 'faker';
import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<TicketDispute>;

  constructor() {
    const ticket = this.createTicketWithoutDisputes();
    // const ticket = this.createTicketWithDispute();

    this._ticket = new BehaviorSubject<TicketDispute>(ticket);
  }

  public get ticket$(): BehaviorSubject<TicketDispute> {
    return this._ticket;
  }

  public get ticket(): TicketDispute {
    return this._ticket.value;
  }

  private createTicketWithoutDisputes(): TicketDispute {
    const soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

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
      violationDate: '2021-09-18',
      discountDueDate: soonDate,
      discountAmount: 25,
      disputant: null,
      offences: [],
      additional: null,
      _within30days: true,
    };

    ticket.disputant = this.createDisputant();

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
      status: 'New',
      offenceAgreementStatus: '',
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: true,
      _offenceStatus: 'Owe',
      _offenceStatusDesc: 'Balance outstanding',
      _within30days: true,
      _amountDue: 126 - 25,
    };

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 2,
      ticketedAmount: 167,
      amountDue: 167,
      violationDateTime: faker.date.recent().toString(),
      offenceDescription: 'Operate Vehicle Without Seatbelts',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      status: 'New',
      offenceAgreementStatus: '',
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _offenceStatus: 'Owe',
      _offenceStatusDesc: 'Balance outstanding',
      _within30days: false,
      _amountDue: 167 - 25,
    };

    ticket.offences.push(offence);

    return ticket;
  }

  private createTicketWithDispute(): TicketDispute {
    const soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

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
      violationDate: '2021-09-18',
      discountDueDate: soonDate,
      discountAmount: 25,
      disputant: null,
      offences: [],
      additional: null,
      _within30days: true,
    };

    ticket.disputant = this.createDisputant();

    ticket.additional = {
      lawyerPresent: true,
      interpreterRequired: true,
      interpreterLanguage: 'SPA',
      witnessPresent: true,
      numberOfWitnesses: 3,
      requestReduction: false,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
      _isCourtRequired: false,
      _isReductionRequired: false,
      _isReductionNotInCourt: false
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
      discountAmount: 25,
      status: 'New',
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: true,
      _within30days: false,
      _amountDue: 126,
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
      discountAmount: 25,
      status: 'Submitted',
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _within30days: false,
      _amountDue: 126,
    };

    offence = Object.assign(offence, this.createOffenceMoreTime());

    ticket.offences.push(offence);

    // --------------------------
    offence = {
      offenceNumber: 3,
      ticketedAmount: 167,
      amountDue: 0,
      violationDateTime: offenceDate,
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      status: 'New',
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _within30days: false,
      _amountDue: 0,
    };

    offence = Object.assign(offence, this.createOffenceDispute());

    ticket.offences.push(offence);

    return ticket;
  }

  private createDisputant(): Disputant {
    return {
      lastName: faker.name.lastName(),
      givenNames: faker.name.firstName(),
      mailingAddress: faker.address.streetAddress(),
      city: faker.address.city(),
      province: faker.address.state(),
      postalCode: 'V8R3E3',
      birthdate: null, // faker.date.past().toDateString(),
      emailAddress: faker.internet.email(),
      driverLicenseNumber: '2342342',
      driverLicenseProvince: 'BC',
      phoneNumber: '2506653434', // faker.phone.phoneNumberFormat(10),
    };
  }

  private createOffenceDispute(): any {
    return {
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
