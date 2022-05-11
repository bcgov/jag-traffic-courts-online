import { DatePipe } from '@angular/common';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterModule } from '@angular/router';
import { NgxMaterialModule } from '@shared/modules/ngx-material/ngx-material.module';

import { DisputeTicketSummaryComponent } from './dispute-ticket-summary.component';

describe('DisputeTicketSummaryComponent', () => {
  let component: DisputeTicketSummaryComponent;
  let fixture: ComponentFixture<DisputeTicketSummaryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        HttpClientTestingModule,
        RouterModule.forRoot([]),
        NgxMaterialModule,
      ],
      providers: [
        DatePipe
      ],
      declarations: [ DisputeTicketSummaryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DisputeTicketSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
