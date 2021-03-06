import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';

import { ErrorHandlerInterceptor } from './error-handler.interceptor';

describe('ErrorHandlerInterceptor', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [RouterTestingModule, HttpClientTestingModule],
    })
  );

  it('should create', () => {
    const service: ErrorHandlerInterceptor = TestBed.inject(
      ErrorHandlerInterceptor
    );
    expect(service).toBeTruthy();
  });
});
