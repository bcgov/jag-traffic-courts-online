import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SharedModule } from '@shared/shared.module';
import { DisputeResourceService } from './dispute-resource.service';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

describe('DisputeResourceService', () => {
  let service: DisputeResourceService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, SharedModule],
      providers: [MockDisputeService],
    });
    service = TestBed.inject(DisputeResourceService);
  });

  it('should create', () => {
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
