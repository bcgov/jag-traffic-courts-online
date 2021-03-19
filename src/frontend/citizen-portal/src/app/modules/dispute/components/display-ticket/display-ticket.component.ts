import { Component, OnInit } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { DisputeService } from '@dispute/services/dispute.service';

@Component({
  selector: 'app-display-ticket',
  templateUrl: './display-ticket.component.html',
  styleUrls: ['./display-ticket.component.scss'],
})
export class DisplayTicketComponent implements OnInit {
  public ticket: any;

  constructor(
    private disputeService: DisputeService,
    private loggerService: LoggerService
  ) {}

  ngOnInit(): void {
    this.disputeService.ticket$.subscribe((data) => {
      this.ticket = data;
    });
  }
}
