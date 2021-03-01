import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { DefaultPipe } from '@shared/pipes/default.pipe';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepReviewTicketComponent } from './step-review-ticket.component';

describe('StepReviewTicketComponent', () => {
  let component: StepReviewTicketComponent;
  let fixture: ComponentFixture<StepReviewTicketComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        NgxMaterialModule,
      ],
      declarations: [
        StepReviewTicketComponent,
        DefaultPipe,
        FormatDatePipe,
        DatePipe,
      ],
      providers: [MockDisputeService, DisputeFormStateService],
    }).compileComponents();
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
