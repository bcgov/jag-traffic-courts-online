import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { ErrorHandlerService } from './error-handler.service';

describe('ErrorHandlerService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
  });

  it('should create', () => {
    const service: ErrorHandlerService = TestBed.inject(ErrorHandlerService);
    expect(service).toBeTruthy();
  });
});
