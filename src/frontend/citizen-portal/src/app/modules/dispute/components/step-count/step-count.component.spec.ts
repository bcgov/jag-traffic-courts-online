import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { CapitalizePipe } from '@shared/pipes/capitalize.pipe';
import { DefaultPipe } from '@shared/pipes/default.pipe';
import { ReplacePipe } from '@shared/pipes/replace.pipe';
import { SafeHtmlPipe } from '@shared/pipes/safe-html.pipe';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepCountComponent } from './step-count.component';

describe('StepCountComponent', () => {
  let component: StepCountComponent;
  let fixture: ComponentFixture<StepCountComponent>;

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
        StepCountComponent,
        DefaultPipe,
        ReplacePipe,
        CapitalizePipe,
        SafeHtmlPipe,
      ],
      providers: [MockDisputeService, DisputeFormStateService],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(inject(
    [DisputeFormStateService],
    (disputeFormStateService: DisputeFormStateService) => {
      fixture = TestBed.createComponent(StepCountComponent);
      component = fixture.componentInstance;
      // Add the bound FormGroup to the component
      component.form = disputeFormStateService.buildStepOffenceForm();
      fixture.detectChanges();
    }
  ));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
