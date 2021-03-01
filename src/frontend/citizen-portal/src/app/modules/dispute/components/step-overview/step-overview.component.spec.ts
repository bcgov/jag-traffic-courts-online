import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { RouterModule } from '@angular/router';
import { DefaultPipe } from '@shared/pipes/default.pipe';

import { StepOverviewComponent } from './step-overview.component';

describe('StepOverviewComponent', () => {
  let component: StepOverviewComponent;
  let fixture: ComponentFixture<StepOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterModule.forRoot([]),
        ReactiveFormsModule,
        HttpClientTestingModule,
        MatDialogModule,
        MatSnackBarModule,
        MatCheckboxModule,
      ],
      declarations: [StepOverviewComponent, DefaultPipe],
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
