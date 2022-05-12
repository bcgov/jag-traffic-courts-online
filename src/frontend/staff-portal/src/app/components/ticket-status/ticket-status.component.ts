import { Component, Input, OnInit } from '@angular/core';
import { Dispute, DisputeStatus } from 'app/api';
import { DisputeView } from '../../services/disputes.service';

@Component({
  selector: 'app-ticket-status',
  templateUrl: './ticket-status.component.html',
  styleUrls: ['./ticket-status.component.scss']
})
export class TicketStatusComponent implements OnInit {
  @Input() public dispute: DisputeView;
  
  constructor() { console.log("ticket status", this.dispute); }

  ngOnInit(): void {
  }

}
