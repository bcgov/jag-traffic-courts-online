import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { DefaultPipe } from '@shared/pipes/default.pipe';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { PhonePipe } from '@shared/pipes/phone.pipe';
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
        BrowserAnimationsModule,
      ],
      declarations: [
        StepReviewTicketComponent,
        DefaultPipe,
        FormatDatePipe,
        DatePipe,
        PhonePipe,
      ],
      providers: [MockDisputeService, DisputeFormStateService],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(inject(
    [DisputeFormStateService],
    (disputeFormStateService: DisputeFormStateService) => {
      fixture = TestBed.createComponent(StepReviewTicketComponent);
      component = fixture.componentInstance;
      // Add the bound FormGroup to the component
      component.form = disputeFormStateService.buildStepReviewForm();
      fixture.detectChanges();
    }
  ));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
