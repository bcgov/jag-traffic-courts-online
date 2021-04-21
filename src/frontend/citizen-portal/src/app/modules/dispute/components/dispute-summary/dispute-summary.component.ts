import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { DisputeRoutes } from '@dispute/dispute.routes';
import { DisputeResourceService } from '@dispute/services/dispute-resource.service';
import { DisputeService } from '@dispute/services/dispute.service';
import { Offence } from '@shared/models/offence.model';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription, timer } from 'rxjs';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: Ticket;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length === 0) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.FIND)]);
      }

      const ticket = this.disputeService.ticket;
      if (ticket) {
        this.ticket = ticket;
      } else {
        this.performSearch(params);
      }
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  private performSearch(params): void {
    this.busy = this.disputeResource.getTicket(params).subscribe((response) => {
      this.disputeService.ticket$.next(response);
      this.ticket = response;
    });
  }

  public onDisputeOffence(offence: Offence): void {
    this.logger.info('onDisputeOffence offence', offence);
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.router.navigate([DisputeRoutes.routePath(DisputeRoutes.STEPPER)], {
        state: { disputeOffenceNumber: offence.offenceNumber },
      });
    });
  }

  public onDisputeTicket(): void {
    this.logger.info('onDisputeTicket', this.disputeService.ticket);
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.router.navigate([
        DisputeRoutes.routePath(DisputeRoutes.ALL_STEPPER),
      ]);
    });
  }
}
