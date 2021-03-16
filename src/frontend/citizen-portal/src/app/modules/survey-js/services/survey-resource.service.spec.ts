import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SharedModule } from '@shared/shared.module';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { SurveyResourceService } from './survey-resource.service';

describe('SurveyResourceService', () => {
  let service: SurveyResourceService;
  let mock: MockDisputeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, SharedModule],
      providers: [MockDisputeService],
    });
    service = TestBed.inject(SurveyResourceService);
    mock = TestBed.inject(MockDisputeService);
  });

  it('should create', () => {
    expect(service).toBeTruthy();
  });

  it('should get ticket', () => {
    const ticket = service.getTicket();
    expect(ticket).toBeDefined();
  });
});
