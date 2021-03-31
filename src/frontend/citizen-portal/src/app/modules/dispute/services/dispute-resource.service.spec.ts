import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
} from '@angular/common/http/testing';
import { SharedModule } from '@shared/shared.module';
import { DisputeResourceService } from './dispute-resource.service';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';
import { HttpErrorResponse } from '@angular/common/http';

describe('DisputeResourceService', () => {
  let httpMock: HttpTestingController;
  let service: DisputeResourceService;
  let mock: MockDisputeService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, SharedModule],
      providers: [MockDisputeService],
    });

    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(DisputeResourceService);
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
