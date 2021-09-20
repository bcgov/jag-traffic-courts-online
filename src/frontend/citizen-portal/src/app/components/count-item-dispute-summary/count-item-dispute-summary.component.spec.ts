import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountItemDisputeSummaryComponent } from './count-item-dispute-summary.component';

describe('CountItemDisputeSummaryComponent', () => {
  let component: CountItemDisputeSummaryComponent;
  let fixture: ComponentFixture<CountItemDisputeSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CountItemDisputeSummaryComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountItemDisputeSummaryComponent);
    component = fixture.componentInstance;
    component.count = {
      offenceNumber: 1,
      ticketedAmount: 100,
      amountDue: 100,
      violationDateTime: new Date().toDateString(),
      offenceDescription: 'Test',
      invoiceType: 'Traffic Violation Ticket',
      vehicleDescription: 'Toyota Prius',
      discountAmount: 0,
      status: 'New',
      offenceAgreementStatus: null,
      reductionAppearInCourt: false,
      requestReduction: false,
      requestMoreTime: false,

      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _within30days: false,
      _amountDue: 0,
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
