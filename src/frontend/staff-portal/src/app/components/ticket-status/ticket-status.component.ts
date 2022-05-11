import { Component, Input, OnInit } from '@angular/core';
import { Dispute, DisputeStatus } from 'app/api';

@Component({
  selector: 'app-ticket-status',
  templateUrl: './ticket-status.component.html',
  styleUrls: ['./ticket-status.component.scss']
})
export class TicketStatusComponent implements OnInit {
  @Input() public dispute: Dispute;
  
  constructor() { }

  ngOnInit(): void {
  }

}
