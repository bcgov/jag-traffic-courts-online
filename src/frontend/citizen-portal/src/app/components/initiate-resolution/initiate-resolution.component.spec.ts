import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';
import { CdkAccordionModule } from '@angular/cdk/accordion';
import { Component } from '@angular/core';

import { InitiateResolutionComponent } from './initiate-resolution.component';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { DatePipe } from '@angular/common';
import { of } from 'rxjs';

// Added the declaration of BlankComponent to be used for routing
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent { }

describe('InitiateResolutionComponent', () => {
  let component: InitiateResolutionComponent;
  let fixture: ComponentFixture<InitiateResolutionComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterModule.forRoot([]),

        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        BrowserAnimationsModule,
        NgxMaterialModule,
        CdkAccordionModule,
        TranslateModule.forRoot(),
      ],
      declarations: [InitiateResolutionComponent, BlankComponent],
      providers: [
        DatePipe,
        TicketTypePipe,
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({
              ticketNumber: 123,
              time: '12:00',
            }),
            snapshot: {
              queryParams: {
                ticketNumber: 123,
                time: '12:00',
              },
            }
          },
        },
      ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InitiateResolutionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
