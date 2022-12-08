import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { MockNoticeOfDisputeService } from 'tests/mocks/mock-notice-of-dispute.service';

import { BackendHttpInterceptor } from './backend-http.interceptor';

describe('BackendHttpInterceptor', () => {
  beforeEach(() =>
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [BackendHttpInterceptor, MockNoticeOfDisputeService],
    })
  );

  it('should be created', () => {
    const interceptor: BackendHttpInterceptor = TestBed.inject(
      BackendHttpInterceptor
    );
    expect(interceptor).toBeTruthy();
  });
});
