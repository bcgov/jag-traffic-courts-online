import { AddressAutocompleteFindResponse, AddressAutocompleteRetrieveResponse } from '@shared/models/address-autocomplete.model';
import { Address } from '@shared/models/address.model';
import { DisputantView } from '@shared/models/disputantView.model';
import { OffenceView } from '@shared/models/offenceView.model';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import * as faker from 'faker';
import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<TicketDisputeView>;

  constructor() {
    const ticket = this.createTicketWithoutDisputes();
    // const ticket = this.createTicketWithDispute();

    this._ticket = new BehaviorSubject<TicketDisputeView>(ticket);
  }

  public get ticket$(): BehaviorSubject<TicketDisputeView> {
    return this._ticket;
  }

  public get ticket(): TicketDisputeView {
    return this._ticket.value;
  }

  public get addressAutocompleteFindResponse(): AddressAutocompleteFindResponse[] {
    const resp: AddressAutocompleteFindResponse[] = [
      {
        id: '001',
        text: '1235 Douglas Street',
        highlight: '0',
        cursor: 1,
        description: 'Victoria, BC, V8D 8E8',
        next: 'Retrieve'
      }, {
        id: '002',
        text: '1244 Douglas Street',
        highlight: '0',
        cursor: 1,
        description: 'Victoria, BC, V8D 8E8',
        next: 'Retrieve'
      }
    ];
    return resp;
  }

  public get addressAutocompleteRetrieveResponse(): AddressAutocompleteRetrieveResponse[] {
    const resp: AddressAutocompleteRetrieveResponse[] = [
      {
        id: '',
        domesticId: '',
        language: 'ENG',
        languageAlternatives: '',
        department: '',
        company: '',
        subBuilding: '',
        buildingNumber: '',
        buildingName: '',
        secondaryStreet: '',
        street: '',
        block: '',
        neighbourhood: '',
        district: '',
        city: faker.address.city(),
        line1: faker.address.streetAddress(),
        line2: faker.address.secondaryAddress(),
        line3: '',
        line4: '',
        line5: '',
        adminAreaName: '',
        adminAreaCode: '',
        province: '',
        provinceName: '',
        provinceCode: 'BC',
        postalCode: 'V9D 9S9',
        countryName: '',
        countryIso2: 'CA',
        countryIso3: '',
        countryIsoNumber: 0,
        sortingNumber1: '',
        sortingNumber2: '',
        barcode: '',
        poBoxNumber: '',
        label: '',
        dataLevel: '',
      }
    ];

    return resp;
  }

  private createTicketWithoutDisputes(): TicketDisputeView {
    const soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

    const ticket: TicketDisputeView = {
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
      _totalBalanceDue: 167,
      _outstandingBalanceDue: 167 - 25,
      _within30days: true,
    };

    ticket.disputant = this.createEmptyDisputant();

    // --------------------------
    let offence: OffenceView = {
      offenceNumber: 1,
      ticketedAmount: 126,
      amountDue: 0,
      violationDateTime: faker.date.soon().toString(),
      offenceDescription: 'Load Or Projection Over 1.2M In Rear', //  Without Required Lamp During Time Specified In Mr Section 4.01
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      status: 'New',
      offenceAgreementStatus: null,
      requestReduction: false,
      requestMoreTime: false,
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: true,
      _offenceStatus: 'Paid',
      _offenceStatusDesc: 'Paid',

      _within30days: false,
      _amountDue: 0,
    };

    offence = Object.assign(offence, this.createOffencePay());

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
      requestReduction: false,
      requestMoreTime: false,
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

  private createTicketWithDispute(): TicketDisputeView {
    const soonDate =
      faker.date.soon().getFullYear() +
      '-' +
      (faker.date.soon().getMonth() + 1) +
      '-' +
      faker.date.soon().getDate();

    const ticket: TicketDisputeView = {
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
      requestReduction: true,
      requestMoreTime: false,
      reductionReason: '',
      moreTimeReason: '',
      _isCourtRequired: false,
      _isReductionRequired: false,
      _isReductionNotInCourt: false
    };

    const offenceDate = faker.date.soon().toString();

    // --------------------------
    let offence: OffenceView = {
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
      requestReduction: false,
      requestMoreTime: false,
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
      requestReduction: false,
      requestMoreTime: false,
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
      amountDue: 200,
      violationDateTime: offenceDate,
      offenceDescription:
        'Load Or Projection Over 1.2M In Rear Without Required Red Flag Or Cloth',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 25,
      status: 'New',
      offenceAgreementStatus: null,
      requestReduction: false,
      requestMoreTime: false,
      reductionAppearInCourt: false,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _within30days: false,
      _amountDue: 200,
    };

    offence = Object.assign(offence, this.createOffenceDispute());

    ticket.offences.push(offence);

    return ticket;
  }

  private createDisputant(): DisputantView {
    const mailingAddress = new Address('CA', 'BC', faker.address.streetAddress(), faker.address.secondaryAddress(), faker.address.city(), 'V8R3E3', 0);
    return {
      lastName: faker.name.lastName(),
      givenNames: faker.name.firstName(),
      city: faker.address.city(),
      province: faker.address.state(),
      postalCode: 'V8R3E3',
      birthdate: null,
      emailAddress: faker.internet.email(),
      driverLicenseNumber: '2342342',
      driverLicenseProvince: 'BC',
      phoneNumber: '2506653434',
      mailingAddress
    };
  }

  private createEmptyDisputant(): DisputantView {
    return {
      lastName: null,
      givenNames: null,
      birthdate: null,
      emailAddress: null,
      driverLicenseNumber: null,
      driverLicenseProvince: null,
      phoneNumber: null,
      mailingAddress: null
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
