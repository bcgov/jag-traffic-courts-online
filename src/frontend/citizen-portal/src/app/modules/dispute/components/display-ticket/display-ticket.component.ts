import { Component, OnInit } from '@angular/core';
import { LoggerService } from '@core/services/logger.service';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-display-ticket',
  templateUrl: './display-ticket.component.html',
  styleUrls: ['./display-ticket.component.scss'],
})
export class DisplayTicketComponent implements OnInit {
  public busy: Subscription;
  public ticket: any;

  constructor(
    private disputeResource: DisputeResourceService,
    private loggerService: LoggerService
  ) {}

  ngOnInit(): void {
    const queryParams = {
      ticketNumber: 'EZ02000460',
      time: '09:54',
    };

    this.busy = this.disputeResource
      .getRsiTicket(queryParams)
      .subscribe((response) => {
        this.ticket = response;
      });
  }
}
