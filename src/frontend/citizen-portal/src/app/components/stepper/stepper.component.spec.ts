import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { StepperComponent } from './stepper.component';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { CurrencyPipe } from '@angular/common';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';

describe('StepperComponent', () => {
  let component: StepperComponent;
  let fixture: ComponentFixture<StepperComponent>;
  let router: Router;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RouterModule.forRoot([]),
        RouterTestingModule,
        BrowserAnimationsModule,
        NgxMaterialModule,
        TranslateModule.forRoot(),
      ],
      declarations: [StepperComponent],
      providers: [FormatDatePipe, CurrencyPipe],
    }).compileComponents();
  });

  beforeEach(() => {
    router = TestBed.inject(Router);
    spyOn(router, 'getCurrentNavigation').and.returnValues({
      id: 1,
      initialUrl: '',
      extractedUrl: null,
      trigger: null,
      previousNavigation: null,
      extras: { state: { disputeOffenceNumber: 1 } },
    });
    fixture = TestBed.createComponent(StepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
