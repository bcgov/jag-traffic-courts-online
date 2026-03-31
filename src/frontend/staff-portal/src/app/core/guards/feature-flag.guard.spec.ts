import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { FeatureFlagGuard } from './feature-flag.guard';
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

describe('FeatureFlagGuard', () => {
  let guard: FeatureFlagGuard;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [BlankComponent],
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
      ],
      providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
      ],
    });
    guard = TestBed.inject(FeatureFlagGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
