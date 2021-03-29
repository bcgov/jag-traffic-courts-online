import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Params, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Dispute } from '@shared/models/dispute.model';
import { Subscription, timer } from 'rxjs';
import { filter, withLatestFrom } from 'rxjs/operators';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit {
  public busy: Subscription;
  public dispute: Dispute;

  constructor(
    private route: Router,
    private activatedRoute: ActivatedRoute,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService
  ) {}

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      const currentDispute = this.disputeService.dispute;
      const ticket = currentDispute?.ticket;
      if (
        ticket?.violationTicketNumber === ticketNumber &&
        ticket?.violationTime === ticketTime
      ) {
        this.dispute = currentDispute;
      } else {
        this.performSearch(params);
      }
    });
  }

  private performSearch(params): void {
    this.disputeResource.getDispute().subscribe((response) => {
      this.dispute = response;
    });
  }

  public onDispute(): void {
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      const ticket = this.dispute?.ticket;
      this.route.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)], {
        queryParams: {
          ticketNumber: ticket.violationTicketNumber,
          time: ticket.violationTime,
        },
      });
    });
  }
}
