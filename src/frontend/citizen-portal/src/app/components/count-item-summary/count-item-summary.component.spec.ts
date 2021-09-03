import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CountItemSummaryComponent } from './count-item-summary.component';

describe('CountItemSummaryComponent', () => {
  let component: CountItemSummaryComponent;
  let fixture: ComponentFixture<CountItemSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CountItemSummaryComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CountItemSummaryComponent);
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
      _within30days: false,
      _amountDue: 0,
      _applyToAllCounts: false,
      _allowApplyToAllCounts: false,
      _firstOffence: false,
      _offenceStatus: null,
      _offenceStatusDesc: null
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
