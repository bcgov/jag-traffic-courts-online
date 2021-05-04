import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepSingleCountComponent } from './step-single-count.component';

describe('StepSingleCountComponent', () => {
  let component: StepSingleCountComponent;
  let fixture: ComponentFixture<StepSingleCountComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        NgxMaterialModule,
        TranslateModule.forRoot(),
      ],
      declarations: [StepSingleCountComponent],
      providers: [MockDisputeService, DisputeFormStateService],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(inject(
    [DisputeFormStateService],
    (disputeFormStateService: DisputeFormStateService) => {
      fixture = TestBed.createComponent(StepSingleCountComponent);
      component = fixture.componentInstance;
      // Add the bound FormGroup to the component
      component.stepControl = disputeFormStateService.buildStepOffenceForm();
      fixture.detectChanges();
    }
  ));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
