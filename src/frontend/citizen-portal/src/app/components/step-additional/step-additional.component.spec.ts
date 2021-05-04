import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, inject, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from 'app/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { YesNoPipe } from '@shared/pipes/yes-no.pipe';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepAdditionalComponent } from './step-additional.component';
import { TranslateModule } from '@ngx-translate/core';
import { ConfigService } from '@config/config.service';
import { MockConfigService } from 'tests/mocks/mock-config.service';

describe('StepAdditionalComponent', () => {
  let component: StepAdditionalComponent;
  let fixture: ComponentFixture<StepAdditionalComponent>;

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
      declarations: [StepAdditionalComponent, YesNoPipe],
      providers: [
        MockDisputeService,
        DisputeFormStateService,
        {
          provide: ConfigService,
          useClass: MockConfigService,
        },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    }).compileComponents();
  });

  beforeEach(inject(
    [DisputeFormStateService],
    (disputeFormStateService: DisputeFormStateService) => {
      fixture = TestBed.createComponent(StepAdditionalComponent);
      component = fixture.componentInstance;
      // Add the bound FormGroup to the component
      component.form = disputeFormStateService.buildStepAdditionalForm();
      fixture.detectChanges();
    }
  ));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
