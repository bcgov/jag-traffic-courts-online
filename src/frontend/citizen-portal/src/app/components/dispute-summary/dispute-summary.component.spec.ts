import { CurrencyPipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { FormatDatePipe } from '@shared/pipes/format-date.pipe';
import { of } from 'rxjs';

import { DisputeSummaryComponent } from './dispute-summary.component';

describe('DisputeSummaryComponent', () => {
  let component: DisputeSummaryComponent;
  let fixture: ComponentFixture<DisputeSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterModule.forRoot([]),
        RouterTestingModule,
        BrowserAnimationsModule,
        NgxMaterialModule,
        TranslateModule.forRoot(),
      ],
      declarations: [DisputeSummaryComponent],
      providers: [
        FormatDatePipe,
        CurrencyPipe,
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              ticketNumber: 123,
              time: '12:00',
            }),
          },
        },
      ],
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
