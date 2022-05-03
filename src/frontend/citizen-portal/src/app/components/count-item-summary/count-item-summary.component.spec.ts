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
      count: 1,
      ticketed_amount: 100,
      amount_due: 100,
      description: 'Test',
      act_or_regulation: "test",
      section: "test",
      is_regulation: false
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
