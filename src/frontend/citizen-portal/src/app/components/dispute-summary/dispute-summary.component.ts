import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { TranslateService } from '@ngx-translate/core';
import { TicketDisputeView } from '@shared/models/ticketDisputeView.model';
import { AppRoutes } from 'app/app.routes';
import { AppConfigService } from 'app/services/app-config.service';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';
import { Subscription } from 'rxjs';
import {ticketTypes} from '../../shared/enums/ticket-type.enum';
import {TicketNotFoundDialogComponent} from '@shared/dialogs/ticket-not-found-dialog/ticket-not-found-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';


@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit {

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private logger: LoggerService,
    private dialog: MatDialog,
    private translateService: TranslateService,
    private appConfigService: AppConfigService,
    private ticketTypePipe: TicketTypePipe
  ) {}
  public busy: Subscription;
  public ticket: TicketDisputeView;
  public defaultLanguage: string;
  public useMockServices: boolean;
  public ticketType: string;
  ticketTypeLocal = ticketTypes;

  public change;

  public ngOnInit(): void {
    this.defaultLanguage = this.translateService.getDefaultLang();
    this.useMockServices = this.appConfigService.useMockServices;

    this.route.queryParams.subscribe((params) => {
      this.logger.info('DisputeSummaryComponent::params', params);

      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      const ticketNumber = params.ticketNumber;
      const ticketTime = params.time;

      if (!ticketNumber || !ticketTime) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
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
    this.ticketType = this.ticketTypePipe.transform(
      this.ticket.violationTicketNumber.charAt(0)
    );
  }
  private performSearch(params): void {
    this.logger.log('DisputeSummaryComponent::performSearch');

    this.busy = this.disputeResource.getTicket(params).subscribe((response) => {
      this.logger.info(
        'DisputeSummaryComponent::performSearch response',
        response
      );

      if (!response) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }

      this.disputeService.ticket$.next(response);
      this.ticket = response;
    });
  }
  public onTicketNotFound(): void {
    this.dialog.open(TicketNotFoundDialogComponent, {
      width: '400px',
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
