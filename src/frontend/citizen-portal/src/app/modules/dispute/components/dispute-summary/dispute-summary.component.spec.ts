import { CurrencyPipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';

import { DisputeSummaryComponent } from './dispute-summary.component';

describe('DisputeSummaryComponent', () => {
  let component: DisputeSummaryComponent;
  let fixture: ComponentFixture<DisputeSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        ReactiveFormsModule,
        RouterModule.forRoot([]),
        RouterTestingModule,
        BrowserAnimationsModule,
        NgxMaterialModule,
      ],
      declarations: [DisputeSummaryComponent],
      providers: [FormatDatePipe, CurrencyPipe],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
