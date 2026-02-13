import { TestBed, waitForAsync, inject } from '@angular/core/testing';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { ConfigCodePipe } from './config-code.pipe';
import { ConfigService } from './config.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';
import { RouterTestingModule } from '@angular/router/testing';
import { Component } from '@angular/core';
import {
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';

// Added the declaration of BlankComponent to be used for routing
@Component({
  selector: 'app-test-blank',
  template: ``,
  standalone: false,
})
class BlankComponent {}

describe('ConfigCodePipe', () => {
  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [BlankComponent],
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
      ],
      providers: [
        {
          provide: ConfigService,
          useClass: MockConfigService,
        },
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    });
  }));

  it('create an instance of Config Code Pipe', inject(
    [ConfigService],
    (configService: ConfigService) => {
      const pipe = new ConfigCodePipe(configService);
      expect(pipe).toBeTruthy();
    }
  ));
});
