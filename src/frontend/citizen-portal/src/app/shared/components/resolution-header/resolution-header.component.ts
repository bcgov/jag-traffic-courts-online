import { Component, Input, OnInit } from '@angular/core';
import { TicketDispute } from '@shared/models/ticketDispute.model';

@Component({
  selector: 'app-resolution-header',
  templateUrl: './resolution-header.component.html',
  styleUrls: ['./resolution-header.component.scss'],
})
export class ResolutionHeaderComponent implements OnInit {
  @Input() public ticket: TicketDispute;

  constructor() {}

  ngOnInit(): void {}
}
