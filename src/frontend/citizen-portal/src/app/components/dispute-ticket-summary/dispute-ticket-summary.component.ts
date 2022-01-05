import { Component, Input, OnInit } from '@angular/core';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';

@Component({
  selector: 'app-dispute-ticket-summary',
  templateUrl: './dispute-ticket-summary.component.html',
  styleUrls: ['./dispute-ticket-summary.component.scss'],
})
export class DisputeTicketSummaryComponent implements OnInit {
  @Input() public ticket: TicketDisputeView;
  @Input() public countDataList:any;
  public defaultLanguage: string;

  constructor() {
    //
  }

  // eslint-disable-next-line @angular-eslint/no-empty-lifecycle-method
  ngOnInit(): void {
    //
  }
}
