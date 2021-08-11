import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TranslateService } from '@ngx-translate/core';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit, AfterViewInit {
  public busy: Subscription;
  public ticket: TicketDispute;
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
      this.logger.info('DisputeSummaryComponent::params', params);

      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }

      const ticket = this.disputeService.ticket;
      this.logger.info('DisputeSummaryComponent::ticket', ticket);
      if (
        ticket &&
        ticket.violationTicketNumber === ticketNumber &&
        ticket.violationTime === ticketTime
      ) {
        this.logger.info('DisputeSummaryComponent:: Use existing ticket');
        this.ticket = ticket;
      } else {
        this.logger.info('DisputeSummaryComponent:: Search for ticket');
        this.performSearch(params);
      }
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }

  private performSearch(params): void {
    this.logger.log('DisputeSummaryComponent::performSearch');

    this.busy = this.disputeResource.getTicket(params).subscribe((response) => {
      this.logger.info(
        'DisputeSummaryComponent::performSearch response',
        response
      );
      this.disputeService.ticket$.next(response);
      this.ticket = response;
    });
  }

  public onDisputeTicket(): void {
    this.logger.info(
      'DisputeSummaryComponent::onDisputeTicket',
      this.disputeService.ticket
    );
    this.router.navigate([AppRoutes.disputePath(AppRoutes.STEPPER)]);
  }

  public onPayTicket(): void {
    this.logger.info(
      'DisputeSummaryComponent::onPayTicket',
      this.disputeService.ticket
    );
    this.router.navigate([AppRoutes.disputePath(AppRoutes.PAYMENT)]);
  }
}
