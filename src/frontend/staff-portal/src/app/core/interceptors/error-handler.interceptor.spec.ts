import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { ErrorHandlerInterceptor } from './error-handler.interceptor';
import { Component } from '@angular/core';

// Added the declaration of BlankComponent to be used for routing
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent {}

describe('ErrorHandlerInterceptor', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [
        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        HttpClientTestingModule,
      ],
      declarations: [BlankComponent],
    })
  );

  it('should create', () => {
    const service: ErrorHandlerInterceptor = TestBed.inject(
      ErrorHandlerInterceptor
    );
    expect(service).toBeTruthy();
  });
});
