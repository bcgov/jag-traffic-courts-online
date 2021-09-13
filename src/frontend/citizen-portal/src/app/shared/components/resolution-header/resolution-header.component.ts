import { Component, Input, OnInit } from '@angular/core';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';

@Component({
  selector: 'app-resolution-header',
  templateUrl: './resolution-header.component.html',
  styleUrls: ['./resolution-header.component.scss'],
})
export class ResolutionHeaderComponent implements OnInit {
  @Input() public ticket: TicketDisputeView;

  constructor() { }

  ngOnInit(): void { }
}
