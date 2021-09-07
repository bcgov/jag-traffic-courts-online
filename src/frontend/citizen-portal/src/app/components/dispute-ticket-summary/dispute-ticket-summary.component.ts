import { Component, Input, OnInit } from '@angular/core';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';

@Component({
  selector: 'app-dispute-ticket-summary',
  templateUrl: './dispute-ticket-summary.component.html',
  styleUrls: ['./dispute-ticket-summary.component.scss'],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public ticket: TicketDisputeView;
  public defaultLanguage: string;

  constructor() {
    //
  }

  ngOnInit(): void {
    //
  }
}
