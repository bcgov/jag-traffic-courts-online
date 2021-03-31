import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Offence, Ticket } from '@shared/models/ticket.model';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit {
  public busy: Subscription;
  public ticket: Ticket;

  private currentParams: Params;

  constructor(
    private route: Router,
    private activatedRoute: ActivatedRoute,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      this.currentParams = params;

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      const ticket = this.disputeService.ticket;
      if (
        ticket?.violationTicketNumber === ticketNumber &&
        ticket?.violationTime === ticketTime
      ) {
        this.ticket = ticket;
      } else {
        this.performSearch(params);
      }
    });
  }

  private performSearch(params): void {
    this.disputeResource.getTicket().subscribe((response) => {
      this.ticket = response;
    });
  }

  public onDispute(offence: Offence): void {
    this.logger.log('onDispute offence', offence);

    const ticketDispute = this.disputeService.getDisputeTicket(
      this.ticket,
      offence
    );
    this.disputeService.ticketDispute$.next(ticketDispute);

    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)], {
        queryParams: this.currentParams,
      });
    });
  }
}
