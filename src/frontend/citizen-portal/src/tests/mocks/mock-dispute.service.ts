import { Ticket } from '@shared/models/ticket.model';
import * as faker from 'faker';

import { BehaviorSubject } from 'rxjs';

export class MockDisputeService {
  private _ticket: BehaviorSubject<Ticket>;

  constructor() {
    const ticketId = faker.random.number();

    this._ticket = new BehaviorSubject<Ticket>({
      id: ticketId,
      userId: faker.random.uuid(),
      violationTicketNumber: faker.finance.routingNumber(),
      courtLocation: faker.address.city(1),
      violationDate: faker.date.recent(),
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
      birthdate: null, // faker.date.past(1),
      lawyerPresent: faker.random.boolean(),
      interpreterRequired: true,
      interpreterLanguage: faker.random.word(),
      callWitness: faker.random.boolean(),
    });
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }
}
