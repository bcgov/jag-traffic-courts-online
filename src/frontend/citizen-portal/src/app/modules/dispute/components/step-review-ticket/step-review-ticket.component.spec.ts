import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StepReviewTicketComponent } from './step-review-ticket.component';

describe('StepReviewTicketComponent', () => {
  let component: StepReviewTicketComponent;
  let fixture: ComponentFixture<StepReviewTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ StepReviewTicketComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepReviewTicketComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
