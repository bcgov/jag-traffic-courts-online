import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ViolationTicketCountIsAct, ViolationTicketCountIsRegulation } from 'app/api';
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
    component.ticketCount = {
      count_no: 0,
      description: null,
      act_or_regulation_name_code:null,
      section: null,
      subsection: null,
      paragraph: null,
      ticketed_amount: null,
      is_act: ViolationTicketCountIsAct.Unknown,
      is_regulation: ViolationTicketCountIsRegulation.Unknown,
    };
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
