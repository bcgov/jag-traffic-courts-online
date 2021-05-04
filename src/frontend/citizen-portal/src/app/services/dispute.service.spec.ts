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

  it('should get ticket', () => {
    const ticket = service.ticket;
    expect(ticket).toBeDefined();
  });

  it('should get ticket observable', () => {
    const ticket = service.ticket$;
    expect(ticket).toBeDefined();
  });
});
