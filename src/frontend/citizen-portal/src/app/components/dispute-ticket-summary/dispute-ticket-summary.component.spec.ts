import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DisputeTicketSummaryComponent } from './dispute-ticket-summary.component';

describe('DisputeTicketSummaryComponent', () => {
  let component: DisputeTicketSummaryComponent;
  let fixture: ComponentFixture<DisputeTicketSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DisputeTicketSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeTicketSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
