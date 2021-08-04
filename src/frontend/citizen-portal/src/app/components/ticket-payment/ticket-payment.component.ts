import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-ticket-payment',
  templateUrl: './ticket-payment.component.html',
  styleUrls: ['./ticket-payment.component.scss'],
})
export class TicketPaymentComponent implements OnInit {
  public busy: Subscription;

  constructor() {}

  ngOnInit(): void {}
}
