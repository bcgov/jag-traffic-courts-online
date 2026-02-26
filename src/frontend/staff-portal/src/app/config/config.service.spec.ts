import { inject, TestBed } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { ConfigService } from './config.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';

describe('ConfigService', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [],
      providers: [
        {
          provide: ConfigService,
          useClass: MockConfigService,
        },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    })
  );

  it('should create', inject([ConfigService], (service: ConfigService) => {
    expect(service).toBeTruthy();
  }));

  it('should get provinces code', inject(
    [ConfigService],
    (service: ConfigService) => {
      const code = service.provincesAndStates[0].provAbbreviationCd;
      expect(code).toBeDefined();
    }
  ));

  it('should get countries code', inject(
    [ConfigService],
    (service: ConfigService) => {
      const code = service.countries[0].ctryId;
      expect(code).toBeDefined();
    }
  ));

  it('should get statuses code', inject(
    [ConfigService],
    (service: ConfigService) => {
      const code = service.statuses[0].code;
      expect(code).toBeDefined();
    }
  ));
});
