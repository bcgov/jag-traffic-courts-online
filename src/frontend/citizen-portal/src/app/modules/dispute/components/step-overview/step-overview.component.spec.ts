import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DisputeFormStateService } from '@dispute/services/dispute-form-state.service';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { DefaultPipe } from '@shared/pipes/default.pipe';
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
      declarations: [StepOverviewComponent, DefaultPipe],
      providers: [MockDisputeService, DisputeFormStateService],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepOverviewComponent);
    component = fixture.componentInstance;

    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
