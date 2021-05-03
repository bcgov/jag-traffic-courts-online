import { HttpClientTestingModule } from '@angular/common/http/testing';
import { Inject, NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { DefaultPipe } from '@shared/pipes/default.pipe';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepOverviewComponent } from './step-overview.component';

describe('StepOverviewComponent', () => {
  let component: StepOverviewComponent;
  let fixture: ComponentFixture<StepOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        NgxMaterialModule,
      ],
      declarations: [StepOverviewComponent, DefaultPipe, FormatDatePipe],
      providers: [MockDisputeService, DisputeFormStateService],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(inject(
    [DisputeFormStateService],
    (disputeFormStateService: DisputeFormStateService) => {
      fixture = TestBed.createComponent(StepOverviewComponent);
      component = fixture.componentInstance;
      // Add the bound FormGroup to the component
      component.form = disputeFormStateService.buildStepOverviewForm();
      component.offence1Form = disputeFormStateService.buildStepOffenceForm();
      component.additionalForm = disputeFormStateService.buildStepAdditionalForm();
      component.disputantForm = disputeFormStateService.buildStepDisputantForm();
      fixture.detectChanges();
    }
  ));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
