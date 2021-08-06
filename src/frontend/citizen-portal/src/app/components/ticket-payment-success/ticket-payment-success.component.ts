import { AfterViewInit, Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LoggerService } from '@core/services/logger.service';
import { UtilsService } from '@core/services/utils.service';
import { TicketDispute } from '@shared/models/ticketDispute.model';
import { AppRoutes } from 'app/app.routes';
import { DisputeResourceService } from 'app/services/dispute-resource.service';
import { DisputeService } from 'app/services/dispute.service';

@Component({
  selector: 'app-ticket-payment-success',
  templateUrl: './ticket-payment-success.component.html',
  styleUrls: ['./ticket-payment-success.component.scss'],
})
export class TicketPaymentSuccessComponent implements OnInit, AfterViewInit {
  public ticket: TicketDispute;

  constructor(
    private router: Router,
    private disputeResource: DisputeResourceService,
    private disputeService: DisputeService,
    private utilsService: UtilsService,
    private logger: LoggerService
  ) {}

  public ngOnInit(): void {
    const formParams = { ticketNumber: 'EZ02000460', time: '09:54' };
    this.disputeResource.getTicket(formParams).subscribe((response) => {
      this.disputeService.ticket$.next(response);
    });

    this.disputeService.ticket$.subscribe((ticket) => {
      this.ticket = ticket;
      this.logger.info('DisputeSubmitComponent', ticket);

      if (!ticket) {
        this.router.navigate([AppRoutes.disputePath(AppRoutes.FIND)]);
      }
    });
  }

  public ngAfterViewInit(): void {
    this.utilsService.scrollTop();
  }
}
