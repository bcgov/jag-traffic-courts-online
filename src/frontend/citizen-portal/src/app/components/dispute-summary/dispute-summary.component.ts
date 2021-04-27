import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Offence } from '@shared/models/offence.model';
import { Ticket } from '@shared/models/ticket.model';
import { Subscription, timer } from 'rxjs';
import { AppRoutes } from 'app/app.routes';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: Ticket;
  public defaultLanguage: string;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService,
    private translateService: TranslateService
  ) {}

  public ngOnInit(): void {
    this.defaultLanguage = this.translateService.getDefaultLang();

    this.route.queryParams.subscribe((params) => {
      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
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
      this.router.navigate([AppRoutes.routePath(AppRoutes.STEPPER)], {
        state: { disputeOffenceNumber: offence.offenceNumber },
      });
    });
  }

  public onDisputeTicket(): void {
    this.logger.info('onDisputeTicket', this.disputeService.ticket);
    const source = timer(1000);
    this.busy = source.subscribe((val) => {
      this.router.navigate([AppRoutes.routePath(AppRoutes.ALL_STEPPER)]);
    });
  }
}
