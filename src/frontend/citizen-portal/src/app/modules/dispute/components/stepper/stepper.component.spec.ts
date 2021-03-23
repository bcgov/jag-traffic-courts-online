import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { StepperComponent } from './stepper.component';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { CurrencyPipe } from '@angular/common';

describe('StepperComponent', () => {
  let component: StepperComponent;
  let fixture: ComponentFixture<StepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RouterModule.forRoot([]),
        BrowserAnimationsModule,
        NgxMaterialModule,
      ],
      declarations: [StepperComponent],
      providers: [FormatDatePipe, CurrencyPipe],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(StepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
