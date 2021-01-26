import { TestBed } from '@angular/core/testing';

import { DisputeService } from './dispute.service';

describe('DisputeService', () => {
  let service: DisputeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DisputeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
