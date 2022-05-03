import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { AppRoutes } from 'app/app.routes';
import { Subscription } from 'rxjs';
import { ticketTypes } from '../../shared/enums/ticket-type.enum';
import { TicketTypePipe } from '@shared/pipes/ticket-type.pipe';
import { ViolationTicketService } from 'app/services/violation-ticket.service';
import { ViolationTicket } from 'app/api';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-dispute-summary',
  templateUrl: './dispute-summary.component.html',
  styleUrls: ['./dispute-summary.component.scss'],
})
export class DisputeSummaryComponent implements OnInit {
  public busy: Subscription;
  public ticket: ViolationTicket;
  public ticketType: string;
  private params: any;

  ticketTypeLocal = ticketTypes;

  constructor(
    protected route: ActivatedRoute,
    protected router: Router,
    private logger: LoggerService,
    private ticketTypePipe: TicketTypePipe,
    private datePipe: DatePipe,
    private violationTicketService: ViolationTicketService,
  ) {
    // always reconstruct current component
    this.router.routeReuseStrategy.shouldReuseRoute = () => { return false; };
    // check params
    this.route.queryParams.subscribe((params) => {
      this.logger.info('DisputeSummaryComponent::params', params);

      if (Object.keys(params).length === 0) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
        return;
      }
      this.params = params;
    });
  }

  public ngOnInit(): void {
    const ticketNumber = this.params.ticketNumber;
    const ticketTime = this.params.time;

    const ticket = this.violationTicketService.ticket;
    const storedTicketTime = this.datePipe.transform(ticket?.issued_date, "HH:mm");
    this.logger.info('DisputeSummaryComponent::ticket', ticket);

    if (ticket && ticket.ticket_number === ticketNumber && storedTicketTime === ticketTime) {
      this.logger.info('DisputeSummaryComponent:: Use existing ticket');
      this.ticket = ticket;
      this.ticketType = this.ticketTypePipe.transform(this.ticket?.ticket_number.charAt(0));
    } else {
      this.busy = this.violationTicketService.searchTicket(this.params).subscribe(res => res);
    }
  }

  public onDisputeTicket(): void {
    this.logger.info('DisputeSummaryComponent::onDisputeTicket', this.violationTicketService.ticket);
    this.router.navigate([AppRoutes.disputePath(AppRoutes.STEPPER)]);
  }
}
