import { Component, Input  } from '@angular/core';
import { DisputeView } from '../../services/disputes.service';

@Component({
  selector: 'app-ticket-status',
  templateUrl: './ticket-status.component.html',
  styleUrls: ['./ticket-status.component.scss']
})
export class TicketStatusComponent {
  @Input() public dispute: DisputeView;
  
  constructor() { }

}
