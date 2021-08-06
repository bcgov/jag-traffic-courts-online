import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TicketPaymentSuccessComponent } from './ticket-payment-success.component';

describe('TicketPaymentSuccessComponent', () => {
  let component: TicketPaymentSuccessComponent;
  let fixture: ComponentFixture<TicketPaymentSuccessComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TicketPaymentSuccessComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TicketPaymentSuccessComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
