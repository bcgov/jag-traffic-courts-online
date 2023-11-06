import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { TranslateModule } from '@ngx-translate/core';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.legacy.module';
import { DisputeStepperComponent } from './dispute-stepper.component';
import { Component } from '@angular/core';
import { DatePipe } from '@angular/common';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { StoreModule } from '@ngrx/store';
import { provideMockStore } from '@ngrx/store/testing';

// Added the declaration of BlankComponent to be used for routing
@Component({ selector: 'app-test-blank', template: `` })
class BlankComponent {}

describe('DisputeStepperComponent', () => {
  let component: DisputeStepperComponent;
  let fixture: ComponentFixture<DisputeStepperComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterModule.forRoot([]),
        ReactiveFormsModule,

        RouterTestingModule.withRoutes([
          { path: 'ticket/find', component: BlankComponent },
        ]),
        BrowserAnimationsModule,
        NgxMaterialModule,
        TranslateModule.forRoot(),
        StoreModule.forRoot(provideMockStore),
      ],
      providers: [
        DatePipe,
        TicketTypePipe
      ],
      declarations: [DisputeStepperComponent, BlankComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeStepperComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
