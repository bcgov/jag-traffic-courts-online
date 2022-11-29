import { Component, Input  } from '@angular/core';
import { Dispute } from 'app/services/dispute.service';

@Component({
  selector: 'app-ticket-status',
  templateUrl: './ticket-status.component.html',
  styleUrls: ['./ticket-status.component.scss']
})
export class TicketStatusComponent {
  @Input() public dispute: Dispute;

  constructor() { }

}
