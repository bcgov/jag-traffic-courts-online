import { Component, Input, OnInit } from '@angular/core';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Component({
  selector: 'app-dispute-ticket-summary',
  templateUrl: './dispute-ticket-summary.component.html',
  styleUrls: ['./dispute-ticket-summary.component.scss'],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public ticket: TicketDispute;
  public defaultLanguage: string;

  constructor() {
    //
  }

  ngOnInit(): void {
    //
  }
}
