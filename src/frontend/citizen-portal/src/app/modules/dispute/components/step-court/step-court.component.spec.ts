import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { YesNoPipe } from '@shared/pipes/yes-no.pipe';
import { MockDisputeService } from 'tests/mocks/mock-dispute.service';

import { StepCourtComponent } from './step-court.component';

describe('StepCourtComponent', () => {
  let component: StepCourtComponent;
  let fixture: ComponentFixture<StepCourtComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        FormsModule,
        ReactiveFormsModule,
        HttpClientTestingModule,
        NgxMaterialModule,
      ],
      declarations: [StepCourtComponent, YesNoPipe],
      providers: [MockDisputeService, DisputeFormStateService],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepCourtComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
