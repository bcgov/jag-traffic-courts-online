import { Component, Input, OnInit } from '@angular/core';
import { DisputeStatus } from 'app/api';

@Component({
  selector: 'app-ticket-status',
  templateUrl: './ticket-status.component.html',
  styleUrls: ['./ticket-status.component.scss']
})
export class TicketStatusComponent implements OnInit {
  @Input() public dispute: any; // FIXME: this should be of type Dispute, not "any".
  
  constructor() { }

  ngOnInit(): void {
    // FIXME, remove these custom mappings and use the Dispute object directly
    this.dispute.citizenSubmittedDate = this.dispute.DateSubmitted;
    this.dispute.status = DisputeStatus.Processing; // No status field on the custom "any" object being passed in
  }

}
