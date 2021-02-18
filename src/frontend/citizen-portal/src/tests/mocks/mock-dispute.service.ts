import { ApiHttpResponse } from '@core/models/api-http-response.model';
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
      violationTicketNumber:
        'AE' +
        faker.random
          .number({
            min: 10000000,
            max: 99999999,
          })
          .toString(),
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
      birthdate: faker.date.past(60, new Date(2001, 0, 1)),
      lawyerPresent: faker.random.boolean(),
      interpreterRequired: true,
      interpreterLanguage: faker.random.word(),
      callWitness: faker.random.boolean(),
      counts: [
        {
          countNo: 1,
          actCode: 'MVA',
          section: '146',
          subSection: '1',
          description: 'MVA 146 (1) - Speed in municipality',
        },
        {
          countNo: 2,
          actCode: 'MVA',
          section: '73',
          subSection: '1',
          description: 'MVA 73 (1) - Fail to stop for police',
        },
      ],
    });
  }

  public get ticket$(): BehaviorSubject<Ticket> {
    return this._ticket;
  }

  public get ticket(): Ticket {
    return this._ticket.value;
  }

  public get httpTicket(): ApiHttpResponse<Ticket> {
    return new ApiHttpResponse(200, null, this._ticket.value);
  }
}
